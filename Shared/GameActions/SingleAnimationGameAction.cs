// Copyright © 2017 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Using Statements
using System;
using System.Collections.Generic;
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
    public class SingleAnimationGameAction : GameAction
    {
        /// <summary>
        /// Number of instances
        /// </summary>
        private static int instances;

        /// <summary>
        /// The single animation
        /// </summary>
        private SingleAnimation singleAnimation;

        /// <summary>
        /// The animation
        /// </summary>
        private AnimationUI animation;

        /// <summary>
        /// The dependency property
        /// </summary>
        private DependencyProperty dependencyProperty;

        #region Public Methods

        /// <summary>
        /// Initializes a new instance of the <see cref="SingleAnimationGameAction" /> class.
        /// </summary>
        /// <param name="singleAnimation">The single animation.</param>
        /// <param name="animation">The AnimationUI component.</param>
        /// <param name="dependencyProperty">The dependency property to animate.</param>
        /// <param name="scene">The associated scene.</param>
        public SingleAnimationGameAction(SingleAnimation singleAnimation, AnimationUI animation, DependencyProperty dependencyProperty, Scene scene = null)
            : base("SingleAnimationGameAction" + instances++, scene)
        {
            this.animation = animation;
            this.singleAnimation = singleAnimation;
            this.dependencyProperty = dependencyProperty;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SingleAnimationGameAction" /> class.
        /// </summary>
        /// <param name="parent">The parent task.</param>
        /// <param name="singleAnimation">The single animation.</param>
        /// <param name="animation">The AnimationUI component.</param>
        /// <param name="dependencyProperty">The dependency property to animate.</param>
        public SingleAnimationGameAction(IGameAction parent, SingleAnimation singleAnimation, AnimationUI animation, DependencyProperty dependencyProperty)
             : base(parent, "SingleAnimationGameAction" + instances++)
        {
            this.animation = animation;
            this.singleAnimation = singleAnimation;
            this.dependencyProperty = dependencyProperty;
        }
        #endregion

        #region Private Methods

        /// <summary>
        /// Perform Run actions
        /// </summary>
        protected override void PerformRun()
        {
            this.singleAnimation.Completed += this.OnAnimationCompleted;
            this.singleAnimation.Cancelled += this.OnAnimationCompleted;

            this.animation.BeginAnimation(this.dependencyProperty, this.singleAnimation);
        }

        /// <summary>
        /// Handles the Completed event of the singleAnimation control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void OnAnimationCompleted(object sender, EventArgs e)
        {
            this.PerformCompleted();
            this.singleAnimation.Completed -= this.OnAnimationCompleted;
            this.singleAnimation.Cancelled -= this.OnAnimationCompleted;
        }

        /// <summary>
        /// Notifies the cancelled.
        /// </summary>
        protected override void PerformCancel()
        {
            this.singleAnimation.Completed -= this.OnAnimationCompleted;
            this.singleAnimation.Cancelled -= this.OnAnimationCompleted;
            this.singleAnimation.CancelCompletedEvent();
            this.singleAnimation.Stop();
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
