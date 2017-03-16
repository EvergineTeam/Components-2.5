#region File Description
//-----------------------------------------------------------------------------
// ActionGameAction
//
// Copyright © 2017 Wave Engine S.L. All rights reserved.
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
    /// A game action that execute an Action
    /// </summary>
    public class ActionGameAction : GameAction
    {
        /// <summary>
        /// The action to execute
        /// </summary>
        private Action action;

        /// <summary>
        /// Number of instances
        /// </summary>
        private static int instances;

        #region Properties
        #endregion

        #region Initialize
        /// <summary>
        /// Initializes a new instance of the <see cref="ActionGameAction" /> class.
        /// </summary>
        /// <param name="action">The action to execute</param>
        /// <param name="scene">The associated scene.</param>
        public ActionGameAction(Action action, Scene scene = null)
            : base("ActionGameAction" + instances++, scene)
        {
            this.action = action;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ActionGameAction" /> class.
        /// </summary>
        /// <param name="parent">The parent task.</param>
        /// <param name="action">The action to execute</param>
        public ActionGameAction(IGameAction parent, Action action)
            : base(parent, "ActionGameAction" + instances++)
        {
            this.action = action;
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Perform run action
        /// </summary>
        protected override void PerformRun()
        {            
            this.action();
            this.PerformCompleted();
        }
        #endregion
        
        #region Private Methods
        #endregion
    }
}
