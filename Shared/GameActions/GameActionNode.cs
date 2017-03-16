#region File Description
//-----------------------------------------------------------------------------
// GameActionNode
//
// Copyright © 2017 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

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
    public class GameActionNode : GameAction
    {
        /// <summary>
        /// Number of instances
        /// </summary>
        private static int instances;

        /// <summary>
        /// The wrapped Action
        /// </summary>
        protected IGameAction wrappedAction;

        /// <summary>
        /// The function that generate a game action
        /// </summary>
        private Func<IGameAction> actionFunction;

        #region Properties
        /// <summary>
        /// Gets the child tasks.
        /// </summary>
        /// <value>
        /// The child tasks.
        /// </value>
        public override IEnumerable<IGameAction> ChildActions
        {
            get
            {
                yield return this.wrappedAction;
            }
        }
        #endregion

        #region Initialize
        /// <summary>
        /// Initializes a new instance of the <see cref="GameActionNode" /> class.
        /// </summary>
        /// <param name="wrappedAction">The wrapped game action</param>
        /// <param name="scene">The scene.</param>
        public GameActionNode(IGameAction wrappedAction, Scene scene = null)
            : base("GameActionNode" + instances++, scene)
        {
            this.wrappedAction = wrappedAction;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GameActionNode" /> class.
        /// </summary>
        /// <param name="actionFunction">The wrapped game action</param>
        /// <param name="scene">The scene.</param>
        public GameActionNode(Func<IGameAction> actionFunction, Scene scene = null)
            : base("GameActionNode" + instances++, scene)
        {
            this.actionFunction = actionFunction;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GameActionNode" /> class.
        /// </summary>
        /// <param name="parent">The parent action.</param>
        /// <param name="wrappedAction">The wrapped game action</param>        
        public GameActionNode(IGameAction parent, IGameAction wrappedAction)
            : base(parent, "GameActionNode" + instances++)
        {
            this.wrappedAction = wrappedAction;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GameActionNode" /> class.
        /// </summary>
        /// <param name="parent">The parent action.</param>
        /// <param name="actionFunction">The wrapped game action</param>        
        public GameActionNode(IGameAction parent, Func<IGameAction> actionFunction)
            : base(parent, "GameActionNode" + instances++)
        {
            this.actionFunction = actionFunction;
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Perform Run actions
        /// </summary>
        protected override void PerformRun()
        {
            if (this.wrappedAction == null)
            {
                this.wrappedAction = this.actionFunction();
                if (this.wrappedAction == null)
                {
                    throw new NullReferenceException("Game action function reurn a null value");
                }
            }

            this.wrappedAction.Completed += this.WrappedActionCompleted;
            this.wrappedAction.Cancelled += this.WrappedActionCancelled;

            if (this.wrappedAction.State != TaskState.Running)
            {
                this.wrappedAction.Run();
            }
        }

        /// <summary>
        /// The wrapped action is completed
        /// </summary>
        /// <param name="action">The action</param>
        private void WrappedActionCompleted(IGameAction action)
        {
            action.Completed -= this.WrappedActionCompleted;

            this.PerformCompleted();
        }

        /// <summary>
        /// The wrapped action is cancelled
        /// </summary>
        /// <param name="action">The action</param>
        private void WrappedActionCancelled(IGameAction action)
        {
            action.Cancelled -= this.WrappedActionCancelled;

            if (this.State == TaskState.Running || this.State == TaskState.Waiting)
            {
                this.PerformCancel();
            }
        }

        /// <summary>
        /// Perform the game action cancelation
        /// </summary>
        protected override void PerformCancel()
        {
            if (this.State == TaskState.Running)
            {
                this.wrappedAction.Cancelled -= this.WrappedActionCancelled;
                this.wrappedAction.Cancel();
            }

            base.PerformCancel();
        }

        /// <summary>
        /// Skip the action
        /// </summary>
        /// <returns>A value indicating it the game action is susscessfully skipped</returns>
        protected override bool PerformSkip()
        {
            if (this.IsSkippable)
            {
                this.Cancel();
                return base.PerformSkip();
            }
            else
            {
                return this.wrappedAction.TrySkip();
            }
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return "[" + base.ToString() + " -> " + this.wrappedAction.ToString() + "]";
        }
        #endregion
    }
}
