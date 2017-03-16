#region File Description
//-----------------------------------------------------------------------------
// ScaleTo3DGameAction
//
// Copyright © 2017 Wave Coorporation. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using WaveEngine.Common.Math;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
#endregion

namespace WaveEngine.Components.GameActions
{
    /// <summary>
    /// Game action that scales an entity 3D
    /// </summary>
    public class ScaleTo3DGameAction : Vector3AnimationGameAction
    {
        /// <summary>
        /// The transform 3D
        /// </summary>
        private Transform3D transform;

        /// <summary>
        /// If the animation is in local coordinates.
        /// </summary>
        private bool local;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScaleTo3DGameAction"/> class.
        /// </summary>
        /// <param name="entity">The target entity</param>
        /// <param name="to">The target scale</param>
        /// <param name="time">Animation duration</param>
        /// <param name="ease">The ease function</param>
        /// <param name="local">If the scale is in local coordinate</param>
        public ScaleTo3DGameAction(Entity entity, Vector3 to, TimeSpan time, EaseFunction ease = EaseFunction.None, bool local = false)
            : base(entity, Vector3.Zero, to, time, ease)
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

            this.transform = entity.FindComponent<Transform3D>();
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
        /// <param name="delta">The delta scale</param>
        private void ScaleAction(Vector3 delta)
        {
            this.transform.Scale = delta;
        }

        /// <summary>
        /// Scale action
        /// </summary>
        /// <param name="delta">The delta scale</param>
        private void LocalScaleAction(Vector3 delta)
        {
            this.transform.LocalScale = delta;
        }
    }
}
