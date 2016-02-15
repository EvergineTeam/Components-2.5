#region File Description
//-----------------------------------------------------------------------------
// BasicGameAction
//
// Copyright © 2016 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using WaveEngine.Common.Media;
using WaveEngine.Framework;
using WaveEngine.Framework.Animation;
using WaveEngine.Framework.Services;
using WaveEngine.Framework.Sound;
#endregion

namespace WaveEngine.Components.GameActions
{
    /// <summary>
    /// A game action to play a sound
    /// </summary>
    public class BasicGameAction : GameAction
    {
        /// <summary>
        /// Number of instances
        /// </summary>
        private static int instances;

        #region Properties
        /// <summary>
        /// Occurs when the action is running.
        /// </summary>
        public event Action OnRun;
        #endregion

        #region Initialize
        /// <summary>
        /// Initializes a new instance of the <see cref="BasicGameAction" /> class.
        /// </summary>
        /// <param name="scene">The associated scene.</param>
        public BasicGameAction(Scene scene = null)
            : base("BasicGameAction" + instances++, scene)
        {
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Notifies that the action is completed.
        /// </summary>
        [DebuggerStepThrough]
        public void NotifyActionCompleted()
        {
            this.PerformCompleted();
        }
        #endregion
        
        #region Private Methods
        /// <summary>
        /// Perform Run actions
        /// </summary>
        protected override void PerformRun()
        {
            if (this.OnRun != null)
            {
                this.OnRun();
            }
        } 
        #endregion
    }
}
