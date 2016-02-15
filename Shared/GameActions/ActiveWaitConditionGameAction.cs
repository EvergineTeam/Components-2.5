#region File Description
//-----------------------------------------------------------------------------
// ActiveWaitConditionGameAction
//
// Copyright © 2016 Wave Engine S.L. All rights reserved.
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
    /// A game action to wait an event for a time
    /// </summary>
    public class ActiveWaitConditionGameAction : GameAction, IUpdatableGameAction
    {
        /// <summary>
        /// Number of instances
        /// </summary>
        private static int instances;

        /// <summary>
        /// Current count;
        /// </summary>
        private int count;

        #region Properties
        /// <summary>
        /// Gets the predicate.
        /// </summary>
        public Func<bool> Predicate { get; private set; }

        /// <summary>
        /// Gets the event count
        /// </summary>
        public int EventCount { get; private set; }
        #endregion

        #region Public Methods
        /// <summary>
        /// Initializes a new instance of the <see cref="ActiveWaitConditionGameAction" /> class.
        /// </summary>
        /// <param name="breakPredicate">The condition predicate</param>
        /// <param name="scene">The scene.</param>
        /// <param name="eventCount">The event count</param>
        public ActiveWaitConditionGameAction(Func<bool> breakPredicate, Scene scene = null, int eventCount = 1)
            : base("ActiveWaitConditionGameAction" + instances++, scene)
        {
            this.Predicate = breakPredicate;
            this.EventCount = eventCount;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ActiveWaitConditionGameAction" /> class.
        /// </summary>
        /// <param name="parent">The parent action</param>
        /// <param name="breakPredicate">The condition predicate</param>        
        /// <param name="eventCount">The event count</param>
        public ActiveWaitConditionGameAction(IGameAction parent, Func<bool> breakPredicate, int eventCount = 1)
            : base(parent, "ActiveWaitConditionGameAction" + instances++)
        {
            this.Predicate = breakPredicate;
            this.EventCount = eventCount;
        }
        #endregion
        
        #region Public Methods
        /// <summary>
        /// Update the game action
        /// </summary>
        /// <param name="gameTime">The current game time.</param>
        public void Update(TimeSpan gameTime)
        {
            if (this.Predicate())
            {
                this.count++;
            }

            if (this.count >= this.EventCount)
            {
                this.PerformCompleted();
            }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Perform Run actions
        /// </summary>
        protected override void PerformRun()
        {
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
                return false;
            }
        }
        #endregion
    }
}
