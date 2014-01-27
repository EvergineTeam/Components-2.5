#region File Description
//-----------------------------------------------------------------------------
// ViewCameraBehavior
//
// Copyright © 2010 - 2013 Wave Coorporation. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using WaveEngine.Common.Input;
using WaveEngine.Common.Math;
using WaveEngine.Components.UI;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Services;
#endregion

namespace WaveEngine.Components.Cameras
{
    /// <summary>
    /// A behavior that allows a camera to rotate around the lookAt
    /// </summary>
    public class ViewCameraBehavior : Behavior
    {
        /// <summary>
        /// The camera to move.
        /// </summary>
        [RequiredComponent]
        public Camera Camera;

        /// <summary>
        /// The speed.
        /// </summary>
        public float RotationSpeed;

        /// <summary>
        /// The touch state.
        /// </summary>
        private TouchPanelState touchState;

        /// <summary>
        /// The is dragging.
        /// </summary>
        private bool isDragging;

        /// <summary>
        /// The prev position.
        /// </summary>
        private Vector2 prevPosition;

        /// <summary>
        /// The current position.
        /// </summary>
        private Vector2 currentPosition;

        /// <summary>
        /// The delta.
        /// </summary>
        private Vector2 delta;

        /// <summary>
        /// The theta angle
        /// </summary>
        private float theta;

        /// <summary>
        /// The phi angle.
        /// </summary>
        private float phi;

        /// <summary>
        /// The initial position.
        /// </summary>
        private Vector3 initialPosition;

        #region Initialize
        /// <summary>
        /// Initializes a new instance of the <see cref="ViewCameraBehavior" /> class.
        /// </summary>
        public ViewCameraBehavior()
            : base("ViewCameraBehavior")
        {
            this.RotationSpeed = 1f;
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Performs further custom initialization for this instance.
        /// </summary>
        /// <remarks>
        /// By default this method does nothing.
        /// </remarks>
        protected override void Initialize()
        {
            base.Initialize();
            this.theta = 0;
            this.phi = 0;
            this.initialPosition = this.Camera.Position - this.Camera.LookAt;
        }

        /// <summary>
        /// Manage the touch state input when dragging to calculate delta, phi and theta angles
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        /// <remarks>
        /// This method will not be executed if the <see cref="Component" />, or the <see cref="Entity" />
        /// owning it are not <c>Active</c>.
        /// </remarks>
        protected override void Update(TimeSpan gameTime)
        {
            this.touchState = WaveServices.Input.TouchPanelState;
            if (this.touchState.Count > 0 && this.touchState[0].State == TouchLocationState.Pressed)
            {
                if (!this.isDragging)
                {
                    this.isDragging = true;
                    this.prevPosition = this.touchState[0].Position;
                }
                else
                {
                    this.currentPosition = this.touchState[0].Position;
                    this.delta = (this.currentPosition - this.prevPosition) * ((float)Math.PI / 180);
                    this.prevPosition = this.currentPosition;
                    this.phi -= this.delta.X * this.RotationSpeed;
                    this.theta -= this.delta.Y * this.RotationSpeed;

                    if (this.theta <= -MathHelper.TwoPi)
                    {
                        this.theta += MathHelper.TwoPi;
                    }

                    if (this.theta > MathHelper.TwoPi)
                    {
                        this.theta -= MathHelper.TwoPi;
                    }

                    if (this.phi <= -MathHelper.TwoPi)
                    {
                        this.phi += MathHelper.TwoPi;
                    }

                    if (this.phi > MathHelper.TwoPi)
                    {
                        this.phi -= MathHelper.TwoPi;
                    }

                    this.UpdateCameraPosition();
                }
            }
            else
            {
                this.isDragging = false;
            }
        }

        /// <summary>
        /// Calculates the new camera Position relative to the initial position and lookAt
        /// </summary>
        private void UpdateCameraPosition()
        {
            Matrix rotationMatrix = Matrix.CreateFromYawPitchRoll(this.phi, this.theta, 0);
            Vector3 transformedReference = Vector3.Transform(this.initialPosition, rotationMatrix);

            this.Camera.UpVector = Vector3.Transform(Vector3.Up, rotationMatrix);

            this.Camera.Position = transformedReference + this.Camera.LookAt;
        }
        
        #endregion
    }
}