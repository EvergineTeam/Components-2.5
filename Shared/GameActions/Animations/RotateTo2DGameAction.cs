// Copyright © 2017 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Using Statements
using System;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
#endregion

namespace WaveEngine.Components.GameActions
{
    /// <summary>
    /// Game action that performs a rotating animation to a 2D entity
    /// </summary>
    public class RotateTo2DGameAction : FloatAnimationGameAction
    {
        /// <summary>
        /// The trasform 2D
        /// </summary>
        private Transform2D transform;

        /// <summary>
        /// If the animation is in local coordinates.
        /// </summary>
        private bool local;

        /// <summary>
        /// Initializes a new instance of the <see cref="RotateTo2DGameAction"/> class.
        /// </summary>
        /// <param name="entity">The target entity</param>
        /// <param name="to">The target angle</param>
        /// <param name="time">Animation duration</param>
        /// <param name="ease">The ease function</param>
        /// <param name="local">If the rotation is local</param>
        public RotateTo2DGameAction(Entity entity, float to, TimeSpan time, EaseFunction ease = EaseFunction.None, bool local = false)
            : base(entity, 0, to, time, ease)
        {
            this.local = local;

            if (local)
            {
                this.updateAction = this.LocalRotateAction;
            }
            else
            {
                this.updateAction = this.RotateAction;
            }

            this.transform = entity.FindComponent<Transform2D>();
        }

        /// <summary>
        /// Performs the run operation
        /// </summary>
        protected override void PerformRun()
        {
            this.from = this.local ? this.transform.LocalRotation : this.transform.Rotation;
            base.PerformRun();
        }

        /// <summary>
        /// The rotate method
        /// </summary>
        /// <param name="delta">Delta angle</param>
        private void RotateAction(float delta)
        {
            this.transform.Rotation = delta;
        }

        /// <summary>
        /// The local rotate method
        /// </summary>
        /// <param name="delta">Delta angle</param>
        private void LocalRotateAction(float delta)
        {
            this.transform.LocalRotation = delta;
        }
    }
}
