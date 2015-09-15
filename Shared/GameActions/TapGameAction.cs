#region File Description
//-----------------------------------------------------------------------------
// TapGameAction
//
// Copyright © 2015 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using WaveEngine.Common.Media;
using WaveEngine.Components.Gestures;
using WaveEngine.Framework;
using WaveEngine.Framework.Animation;
using WaveEngine.Framework.Services;
using WaveEngine.Framework.Sound;
#endregion

namespace WaveEngine.Components.GameActions
{
    /// <summary>
    /// A game action that execute an Action
    /// </summary>
    public class TapGameAction : GameAction
    {
        /// <summary>
        /// Number of instances
        /// </summary>
        private static int instances;
        
        /// <summary>
        /// The touch gestures to detect the tap
        /// </summary>
        private TouchGestures touchGestures;

        #region Properties
        #endregion

        #region Initialize
        /// <summary>
        /// Initializes a new instance of the <see cref="TapGameAction" /> class.
        /// </summary>
        /// <param name="touchGestures">The TouchGestures instances.</param>
        /// <param name="scene">The associated scene.</param>
        public TapGameAction(TouchGestures touchGestures, Scene scene = null)
            : base("TapGameAction" + instances++, scene)
        {
            this.touchGestures = touchGestures;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TapGameAction" /> class.
        /// </summary>
        /// <param name="parent">The parent task.</param>
        /// <param name="touchGestures">The TouchGestures instances.</param>
        public TapGameAction(IGameAction parent, TouchGestures touchGestures)
            : base(parent, "ActionGameAction" + instances++)
        {
            this.touchGestures = touchGestures;
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Perform run action
        /// </summary>
        protected override void PerformRun()
        {
            this.touchGestures.TouchTap += this.NotifyTap;
        }
        #endregion
        
        #region Private Methods
        /// <summary>
        /// Handles the tap event on the touch gestures
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event args.</param>
        private void NotifyTap(object sender, GestureEventArgs e)
        {
            this.PerformCompleted();
            this.touchGestures.TouchTap -= this.NotifyTap;
        }

        /// <summary>
        /// Perform cancelation event
        /// </summary>
        protected override void PerformCancel()
        {
            this.touchGestures.TouchTap -= this.NotifyTap;
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
