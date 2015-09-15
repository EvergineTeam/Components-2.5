#region File Description
//-----------------------------------------------------------------------------
// WaitGameAction
//
// Copyright © 2015 Wave Engine S.L. All rights reserved.
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
    /// A game action to wait for a time
    /// </summary>
    public class WaitGameAction : GameAction
    {
        /// <summary>
        /// Number of instances
        /// </summary>
        private static int instances;

        /// <summary>
        /// Cached timer factory
        /// </summary>
        private static TimerFactory timerFactory = WaveServices.TimerFactory;

        /// <summary>
        /// The timer
        /// </summary>
        private Timer timer;

        #region Properties
        /// <summary>
        /// Gets or sets the duration of the game action
        /// </summary>
        public TimeSpan Duration { get; set; }
        #endregion

        #region Public Methods
        /// <summary>
        /// Initializes a new instance of the <see cref="WaitGameAction" /> class.
        /// </summary>
        /// <param name="duration">The duration of the action</param>
        /// <param name="scene">The scene.</param>
        public WaitGameAction(TimeSpan duration, Scene scene = null)
            : base("WaitGameAction" + instances++, scene)
        {
            this.Duration = duration;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WaitGameAction" /> class.
        /// </summary>
        /// <param name="parent">The parent action</param>
        /// <param name="duration">The duration of the action</param>        
        public WaitGameAction(IGameAction parent, TimeSpan duration)
            : base(parent, "WaitGameAction" + instances++)
        {
            this.Duration = duration;
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Perform Run actions
        /// </summary>
        protected override void PerformRun()
        {
            // Play sound
            this.timer = timerFactory.CreateTimer(this.Duration, this.TimerCompleted, false, this.Scene);
        }

        /// <summary>
        /// The timer has been fired
        /// </summary>
        private void TimerCompleted()
        {
            if (this.State == WaveEngine.Framework.Services.TaskState.Running)
            {
                this.PerformCompleted();
            }
            else
            {
#if DEBUG
                throw new NotSupportedException("This point should be not executed because if it has been skiped or aborted the timer (of the TimerTask)should have been removed");
#endif
            }

            this.timer = null;
        }

        /// <summary>
        /// Perform cancel
        /// </summary>
        protected override void PerformCancel()
        {
            timerFactory.RemoveTimer(this.timer);
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
                return false;
            }
        }
        #endregion
    }
}
