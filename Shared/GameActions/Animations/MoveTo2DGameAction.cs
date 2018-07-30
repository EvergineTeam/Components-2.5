// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Using Statements
using System;
using WaveEngine.Common.Math;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
#endregion

namespace WaveEngine.Components.GameActions
{
    /// <summary>
    /// Game action which animates an 2D entity
    /// </summary>
    public class MoveTo2DGameAction : Vector2AnimationGameAction
    {
        /// <summary>
        /// The transform
        /// </summary>
        private Transform2D transform;

        /// <summary>
        /// If the animation is in local coordinates.
        /// </summary>
        private bool local;

        /// <summary>
        /// Initializes a new instance of the <see cref="MoveTo2DGameAction"/> class.
        /// </summary>
        /// <param name="entity">The target entity</param>
        /// <param name="to">The target position</param>
        /// <param name="time">Animation duration</param>
        /// <param name="ease">The ease function</param>
        /// <param name="local">If the position is in local coordinates.</param>
        public MoveTo2DGameAction(Entity entity, Vector2 to, TimeSpan time, EaseFunction ease = EaseFunction.None, bool local = false)
            : base(entity, Vector2.Zero, to, time, ease)
        {
            this.local = local;

            if (local)
            {
                this.updateAction = this.LocalMoveAction;
            }
            else
            {
                this.updateAction = this.MoveAction;
            }

            this.transform = entity.FindComponent<Transform2D>();
        }

        /// <summary>
        /// Performs the run operation
        /// </summary>
        protected override void PerformRun()
        {
            this.from = this.local ? this.transform.LocalPosition : this.transform.Position;
            base.PerformRun();
        }

        /// <summary>
        /// Move action
        /// </summary>
        /// <param name="delta">The delta movement</param>
        private void MoveAction(Vector2 delta)
        {
            this.transform.Position = delta;
        }

        /// <summary>
        /// Move action
        /// </summary>
        /// <param name="delta">The delta movement</param>
        private void LocalMoveAction(Vector2 delta)
        {
            this.transform.Position = delta;
        }
    }
}
