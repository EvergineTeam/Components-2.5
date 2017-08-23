// Copyright © 2017 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Using Statements
using System;
using System.Collections.Generic;
using WaveEngine.Framework;
#endregion

namespace WaveEngine.Components.GameActions
{
    /// <summary>
    /// Behavior that updates the game action
    /// </summary>
    public class GameActionUpdaterBehavior : Behavior
    {
        #region Fields

        /// <summary>
        /// Game actions list
        /// </summary>
        private List<UpdatableGameAction> gameActions;

        /// <summary>
        /// New game actions list
        /// </summary>
        private List<UpdatableGameAction> addList;

        /// <summary>
        /// Remove game actions list
        /// </summary>
        private List<UpdatableGameAction> deleteList;

        #endregion

        #region Properties
        #endregion

        #region Initialize

        /// <summary>
        /// Initializes a new instance of the <see cref="GameActionUpdaterBehavior"/> class.
        /// </summary>
        public GameActionUpdaterBehavior()
            : base()
        {
            this.gameActions = new List<UpdatableGameAction>();
            this.addList = new List<UpdatableGameAction>();
            this.deleteList = new List<UpdatableGameAction>();
        }
        #endregion

        #region Public Methods
        #endregion

        #region Private Methods

        /// <summary>
        /// Updates the instance of the behavior
        /// </summary>
        /// <param name="gameTime">Ellapsed time</param>
        protected override void Update(TimeSpan gameTime)
        {
            if (this.addList.Count > 0)
            {
                foreach (var toAdd in this.addList)
                {
                    this.gameActions.Add(toAdd);
                }

                this.addList.Clear();
            }

            if (this.deleteList.Count > 0)
            {
                foreach (var toDelete in this.deleteList)
                {
                    this.gameActions.Remove(toDelete);
                }

                this.deleteList.Clear();
            }

            foreach (var gameAction in this.gameActions)
            {
                gameAction.Update(gameTime);
            }
        }

        /// <summary>
        /// Begins the action
        /// </summary>
        /// <param name="gameAction">The game action</param>
        internal void BeginAction(UpdatableGameAction gameAction)
        {
            if (!this.gameActions.Contains(gameAction))
            {
                this.addList.Add(gameAction);
            }
        }

        /// <summary>
        /// Stops the game action
        /// </summary>
        /// <param name="gameAction">The game action</param>
        internal void StopAction(UpdatableGameAction gameAction)
        {
            if (this.gameActions.Contains(gameAction))
            {
                this.deleteList.Add(gameAction);
            }
        }
        #endregion
    }
}
