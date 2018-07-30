// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Using statements
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using WaveEngine.Common;
using WaveEngine.Common.IO;
using WaveEngine.Framework;
using WaveEngine.Framework.Services;
#endregion

namespace WaveEngine.Components.GameActions
{
    /// <summary>
    /// Represent a Game Action to perform something
    /// </summary>
    public abstract class GameAction : SerializableObject, IGameAction
    {
        /// <summary>
        /// The action Scheduler
        /// </summary>
        private static GameActionScheduler actionScheduler = WaveServices.GameActionScheduler;

        /// <summary>
        /// Gets the name of the task.
        /// </summary>
        /// <value>
        /// The name of the task.
        /// </value>
        public string Name { get; private set; }

        /// <summary>
        /// Occurs when [completed].
        /// </summary>
        public event Action<IGameAction> Completed;

        /// <summary>
        /// Occurs when [cancelled].
        /// </summary>
        public event Action<IGameAction> Cancelled;

        /// <summary>
        /// The skippable
        /// </summary>
        public bool IsSkippable;

        /// <summary>
        /// Occurs when [skipped].
        /// </summary>
        public event Action<IGameAction> Skipped;

        /// <summary>
        /// The parent action
        /// </summary>
        private IGameAction parent;

        /// <summary>
        /// The gameaction state
        /// </summary>
        private GameActionState state;

        /// <summary>
        /// Gets the parent.
        /// </summary>
        /// <value>
        /// The parent.
        /// </value>
        internal IGameAction Parent
        {
            get
            {
                return this.parent;
            }

            private set
            {
                this.parent = value;
            }
        }

        /// <summary>
        /// Gets the state of the task.
        /// </summary>
        /// <value>
        /// The state of the task.
        /// </value>
        public GameActionState State
        {
            get
            {
                return this.state;
            }

            internal set
            {
                this.state = value;
            }
        }

        /// <summary>
        /// Gets the scene.
        /// </summary>
        /// <value>
        /// The scene.
        /// </value>
        public Scene Scene { get; internal set; }

        /// <summary>
        /// Gets the child tasks.
        /// </summary>
        /// <value>
        /// The child tasks.
        /// </value>
        public virtual IEnumerable<IGameAction> ChildActions
        {
            get
            {
                return System.Linq.Enumerable.Empty<IGameAction>();
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GameAction" /> class.
        /// </summary>
        /// <param name="name">Name of the game action.</param>
        /// <param name="scene">The scene</param>
        protected GameAction(string name, Scene scene = null)
        {
            this.IsSkippable = true;
            this.State = GameActionState.None;
            this.Name = name;
            this.Scene = scene;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GameAction" /> class.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="taskName">Name of the task.</param>
        /// <exception cref="System.ArgumentException">parent task cannot be null. if is a root task, use the other constructor</exception>
        protected GameAction(IGameAction parent, string taskName)
        {
            this.IsSkippable = true;
            if (parent == null)
            {
                throw new ArgumentException("parent task cannot be null. if is a root task, use the other constructor");
            }

            this.Parent = parent;

            parent.Completed += this.OnParentComplete;

            this.State = GameActionState.Waiting;
            this.Scene = parent.Scene;
            this.Name = taskName;
        }

        #region Public Methods

        /// <summary>
        /// Ons the run.
        /// </summary>
        protected abstract void PerformRun();

        /// <summary>
        /// Runs this instance.
        /// </summary>
        /// <exception cref="System.NotSupportedException">When the action state is not correct</exception>
        public virtual void Run()
        {
            // ignore the run if the scene is already disposed
            if (this.Scene != null && this.Scene.IsDisposed)
            {
                return;
            }

            if (this.State == GameActionState.Finished
                || this.State == GameActionState.Aborted
                || this.State == GameActionState.Running)
            {
                throw new NotSupportedException(string.Format("The GameAction cannot be runned because its state ({0}) does not allow it", this.State.ToString()));
            }

            if (this.Parent == null || this.Parent.State == GameActionState.Finished)
            {
                this.State = GameActionState.Running;
                actionScheduler.RegisterGameAction(this);

                this.PerformRun();
            }
            else if (this.Parent.State == GameActionState.None || this.Parent.State == GameActionState.Waiting)
            {
                this.Parent.Run();
            }
            else
            {
#if DEBUG
                throw new NotSupportedException("Incoherent State");
#endif
            }
        }

        /// <summary>
        /// Skips this instance.
        /// </summary>
        /// <exception cref="System.NotImplementedException">When the action state is not correct</exception>
        /// <returns>If the action is skipped susscessfully</returns>
        public bool TrySkip()
        {
            if (this.State == GameActionState.Running)
            {
                // SKIP THIS
                return this.PerformSkip();
            }
            else if (this.Parent != null && (this.Parent.State == GameActionState.Waiting || this.Parent.State == GameActionState.Running))
            {
                // SKIP UP
                return this.Parent.TrySkip();
            }
            else if (this.State == GameActionState.None || this.State == GameActionState.Aborted || this.State == GameActionState.Finished)
            {
#if DEBUG
                throw new NotSupportedException("It cannot be skipped because it has not been started or already has been cancelled");
#endif
            }

            return true;
        }

        /// <summary>
        /// Cancels this instance.
        /// </summary>
        public void Cancel()
        {
            if (this.State == GameActionState.Running)
            {
                // CANCEL THIS
                this.PerformCancel();
            }
            else if (this.Parent != null && this.State == GameActionState.Waiting)
            {
                // CANCEL UP
                this.Parent.Cancel();
            }
            else if (
                this.State == GameActionState.None ||
                this.State == GameActionState.Aborted ||
                this.State == GameActionState.Finished)
            {
#if DEBUG
                throw new NotSupportedException("It cannot be canncelled because it has not been started or already has been cancelled");
#endif
            }
        }

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return this.Name;
        }
        #endregion

        #region Private Methods

        /// <summary>
        /// Notifies the completed.
        /// </summary>
        protected void PerformCompleted()
        {
            if (this.State == GameActionState.Running)
            {
                this.State = GameActionState.Finished;
                actionScheduler.UnregisterGameAction(this);

                if (this.Completed != null)
                {
                    this.Completed(this);
                }
            }
        }

        /// <summary>
        /// Notifies the cancelled.
        /// </summary>
        protected virtual void PerformCancel()
        {
            this.State = GameActionState.Aborted;
            actionScheduler.UnregisterGameAction(this);

            if (this.Cancelled != null)
            {
                this.Cancelled(this);
            }
        }

        /// <summary>
        /// Notifies the skip.
        /// </summary>
        /// <returns>If the action is skipped susscessfully</returns>
        protected virtual bool PerformSkip()
        {
            if (this.IsSkippable)
            {
                this.State = GameActionState.Finished;
                actionScheduler.UnregisterGameAction(this);

                if (this.Skipped != null)
                {
                    this.Skipped(this);
                }

                if (this.Completed != null)
                {
                    this.Completed(this);
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Parent action is completed
        /// </summary>
        /// <param name="parent">The parent action</param>
        private void OnParentComplete(IGameAction parent)
        {
            this.Run();
        }
        #endregion
    }
}
