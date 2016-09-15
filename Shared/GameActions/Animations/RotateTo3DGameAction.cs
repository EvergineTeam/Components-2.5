#region File Description
//-----------------------------------------------------------------------------
// RotateTo3DGameAction
//
// Copyright © 2016 Wave Coorporation. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using WaveEngine.Common.Math;
using WaveEngine.Components.GameActions;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
#endregion

namespace WaveEngine.Components.GameActions
{
    /// <summary>
    /// Game action that rotates an entity 3D.
    /// </summary>
    public class RotateTo3DGameAction : Vector3AnimationGameAction
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
        /// Initializes a new instance of the <see cref="RotateTo3DGameAction"/> class.
        /// </summary>
        /// <param name="entity">The target entity</param>
        /// <param name="to">The target rotation</param>
        /// <param name="time">Animation duration</param>
        /// <param name="ease">The ease function</param>
        /// <param name="local">If the rotation is in local coordinates</param>
        public RotateTo3DGameAction(Entity entity, Vector3 to, TimeSpan time, EaseFunction ease = EaseFunction.None, bool local = false)
            : base(entity, Vector3.Zero, to, time, ease)
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

            this.transform = entity.FindComponent<Transform3D>();
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
        /// Rotate action
        /// </summary>
        /// <param name="delta">The delta rotation</param>
        private void RotateAction(Vector3 delta)
        {
            this.transform.Rotation = delta;
        }

        /// <summary>
        /// Local Rotate action
        /// </summary>
        /// <param name="delta">The delta rotation</param>
        private void LocalRotateAction(Vector3 delta)
        {
            this.transform.LocalRotation = delta;
        }
    }
}
