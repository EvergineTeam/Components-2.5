// Copyright © 2017 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Using Statements
using System;
using WaveEngine.Common.Math;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
#endregion

namespace WaveEngine.Components.GameActions
{
    /// <summary>
    /// Game action that scales a 2D entity
    /// </summary>
    public class ScaleTo2DGameAction : Vector2AnimationGameAction
    {
        /// <summary>
        /// The transform 2D
        /// </summary>
        private Transform2D transform;

        /// <summary>
        /// If the animation is in local coordinates.
        /// </summary>
        private bool local;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScaleTo2DGameAction"/> class.
        /// </summary>
        /// <param name="entity">The target entity</param>
        /// <param name="to">The target scale</param>
        /// <param name="time">Animation duration</param>
        /// <param name="ease">The ease function</param>
        /// <param name="local">If the scale is in local coordinate</param>
        public ScaleTo2DGameAction(Entity entity, Vector2 to, TimeSpan time, EaseFunction ease = EaseFunction.None, bool local = false)
            : base(entity, Vector2.Zero, to, time, ease)
        {
            this.local = local;

            if (local)
            {
                this.updateAction = this.LocalScaleAction;
            }
            else
            {
                this.updateAction = this.ScaleAction;
            }

            this.transform = entity.FindComponent<Transform2D>();
        }

        /// <summary>
        /// Performs the run operation
        /// </summary>
        protected override void PerformRun()
        {
            this.from = this.local ? this.transform.LocalScale : this.transform.Scale;
            base.PerformRun();
        }

        /// <summary>
        /// Scale action
        /// </summary>
        /// <param name="delta">Delta scale</param>
        private void ScaleAction(Vector2 delta)
        {
            this.transform.Scale = delta;
        }

        /// <summary>
        /// Scale action
        /// </summary>
        /// <param name="delta">The delta scale</param>
        private void LocalScaleAction(Vector2 delta)
        {
            this.transform.LocalScale = delta;
        }
    }
}
