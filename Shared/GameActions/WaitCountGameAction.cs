// Copyright © 2017 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Using Statements
using System;
using System.Collections.Generic;
using System.Text;
using WaveEngine.Common.Media;
using WaveEngine.Framework;
using WaveEngine.Framework.Services;
#endregion

namespace WaveEngine.Components.GameActions
{
    /// <summary>
    /// A game action to continue with other action
    /// </summary>
    public class WaitCountGameAction : GameAction
    {
        /// <summary>
        /// Number of instances
        /// </summary>
        private static int instances;

        /// <summary>
        /// Gets the number of completed actions.
        /// </summary>
        /// <value>
        /// The completed.
        /// </value>
        protected int CompletedCount { get; private set; }

        /// <summary>
        /// Gets the cancelled.
        /// </summary>
        /// <value>
        /// The cancelled.
        /// </value>
        protected int CancelledCount { get; private set; }

        /// <summary>
        /// The function list that generate a game action
        /// </summary>
        private IGameAction[] childActions;

        /// <summary>
        /// The function list that generate a game action
        /// </summary>
        private Func<IGameAction>[] childActionGenerators;

        #region Properties

        /// <summary>
        /// Gets the count limit.
        /// </summary>
        /// <value>
        /// The count limit.
        /// </value>
        protected int CountLimit { get; private set; }

        /// <summary>
        /// Gets the child actions
        /// </summary>
        public override IEnumerable<IGameAction> ChildActions
        {
            get
            {
                return this.childActions;
            }
        }
        #endregion

        #region Initialize

        /// <summary>
        /// Initializes a new instance of the <see cref="WaitCountGameAction" /> class.
        /// </summary>
        /// <param name="parent">The parent action.</param>
        /// <param name="countLimit">The count limit.</param>
        /// <param name="childActions">The child action list</param>
        public WaitCountGameAction(IGameAction parent, int countLimit, params IGameAction[] childActions)
            : base(parent, "WaitCountGameAction" + instances++)
        {
            if (countLimit < 0 || countLimit > childActions.Length)
            {
                throw new ArgumentOutOfRangeException("countLimit");
            }

            if (childActions == null)
            {
                throw new ArgumentNullException("childActions");
            }

            this.childActions = childActions;
            this.CountLimit = countLimit;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WaitCountGameAction" /> class.
        /// </summary>
        /// <param name="parent">The parent action.</param>
        /// <param name="countLimit">The count limit.</param>
        /// <param name="childActionGenerators">The child generator action list</param>
        public WaitCountGameAction(IGameAction parent, int countLimit, params Func<IGameAction>[] childActionGenerators)
            : base(parent, "WaitCountGameAction" + instances++)
        {
            if (countLimit < 0 || countLimit > childActionGenerators.Length)
            {
                throw new ArgumentOutOfRangeException("countLimit");
            }

            if (childActionGenerators == null)
            {
                throw new ArgumentNullException("childActionGenerators");
            }

            this.childActionGenerators = childActionGenerators;
            this.CountLimit = countLimit;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WaitCountGameAction" /> class.
        /// </summary>
        /// <param name="scene">The scene.</param>
        /// <param name="countLimit">The count limit.</param>
        /// <param name="childActions">The child action list</param>
        public WaitCountGameAction(Scene scene, int countLimit, params IGameAction[] childActions)
            : base("WaitCountGameAction" + instances++, scene)
        {
            this.childActions = childActions;
            this.CountLimit = countLimit;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WaitCountGameAction" /> class.
        /// </summary>
        /// <param name="scene">The scene.</param>
        /// <param name="countLimit">The count limit.</param>
        /// <param name="childActionGenerators">The child generator action list</param>
        public WaitCountGameAction(Scene scene, int countLimit, params Func<IGameAction>[] childActionGenerators)
            : base("WaitCountGameAction" + instances++, scene)
        {
            this.childActionGenerators = childActionGenerators;
            this.CountLimit = countLimit;
        }
        #endregion

        #region Private Methods

        /// <summary>
        /// Checks the end.
        /// </summary>
        protected void CheckEnd()
        {
            if (this.CompletedCount == this.CountLimit)
            {
                this.PerformCompleted();
            }
        }

        /// <summary>
        /// Perform Run actions
        /// </summary>
        protected override void PerformRun()
        {
            this.CheckEnd();

            if (this.childActions == null)
            {
                if (this.childActionGenerators == null)
                {
                    throw new NullReferenceException("There are no action generators");
                }

                for (int i = 0; i < this.childActionGenerators.Length; i++)
                {
                    this.childActions[i] = this.childActionGenerators[i]();
                }
            }

            for (int i = 0; i < this.childActions.Length; i++)
            {
                IGameAction action = this.childActions[i];

                if (action.State == GameActionState.Finished)
                {
                    this.ActionCompleted(action);
                }
                else if (action.State == GameActionState.Aborted)
                {
                    this.ActionCancelled(action);
                }
                else if (action.State != GameActionState.Running)
                {
                    action.Completed += this.ActionCompleted;
                    action.Cancelled += this.ActionCancelled;

                    action.Run();
                }
            }
        }

        /// <summary>
        /// Perform cancel action
        /// </summary>
        protected override void PerformCancel()
        {
            if (this.childActions != null)
            {
                for (int i = 0; i < this.childActions.Length; i++)
                {
                    IGameAction action = this.childActions[i];

                    if (action.State == GameActionState.Running || action.State == GameActionState.Waiting)
                    {
                        action.Cancel();
                    }
                }
            }

            base.PerformCancel();
        }

        /// <summary>
        /// Method invoqued when an action is cancelled
        /// </summary>
        /// <param name="action">The game action.</param>
        protected void ActionCompleted(IGameAction action)
        {
            if (this.State == GameActionState.Running)
            {
                action.Completed -= this.ActionCompleted;
                action.Cancelled -= this.ActionCancelled;

                this.CompletedCount++;
                this.CheckEnd();
            }
        }

        /// <summary>
        /// Method invoqued when an action is cancelled
        /// </summary>
        /// <param name="action">The game action.</param>
        protected void ActionCancelled(IGameAction action)
        {
            if (this.State == GameActionState.Running)
            {
                action.Completed -= this.ActionCompleted;
                action.Cancelled -= this.ActionCancelled;

                this.CancelledCount++;
                this.CheckEnd();
            }
        }
        #endregion
    }
}
