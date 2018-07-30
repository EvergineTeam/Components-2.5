﻿// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Using Statements
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
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
    [DataContract(Namespace = "WaveEngine.Components.Cameras")]
    public class ViewCamera3DBehavior : Behavior
    {
        /// <summary>
        /// The camera to move.
        /// </summary>
        [RequiredComponent]
        public Camera3D Camera;

        /// <summary>
        /// The speed.
        /// </summary>
        [DataMember]
        public float RotationSpeed;

        /// <summary>
        /// Input service.
        /// </summary>
        [RequiredService]
        private Input input = null;

        /// <summary>
        /// The touch state.
        /// </summary>
        private TouchPanelState touchState;

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

        /// <summary>
        /// The initial lookAt.
        /// </summary>
        private Vector3 initialLookAt;

        #region Initialize

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewCamera3DBehavior" /> class.
        /// </summary>
        public ViewCamera3DBehavior()
            : this(Vector3.Zero)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewCamera3DBehavior" /> class.
        /// </summary>
        /// <param name="lookAt">The look at</param>
        public ViewCamera3DBehavior(Vector3 lookAt)
            : base("ViewCameraBehavior")
        {
            this.initialLookAt = lookAt;
        }

        /// <summary>
        /// Sets default values for this instance.
        /// </summary>
        protected override void DefaultValues()
        {
            base.DefaultValues();

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
            this.initialPosition = this.Camera.Position - this.initialLookAt;

            this.UpdateCameraPosition();
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
            this.touchState = this.input.TouchPanelState;
            if (this.touchState.Count > 0)
            {
                var currentState = this.touchState[0].State;

                if (currentState == TouchLocationState.Pressed)
                {
                    this.prevPosition = this.touchState[0].Position;
                }
                else if (currentState == TouchLocationState.Moved)
                {
                    this.currentPosition = this.touchState[0].Position;
                    this.delta = (this.currentPosition - this.prevPosition) * ((float)Math.PI / 180);
                    this.prevPosition = this.currentPosition;
                    this.phi -= this.delta.X * this.RotationSpeed;
                    this.theta += this.delta.Y * this.RotationSpeed;

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
        }

        /// <summary>
        /// Calculates the new camera Position relative to the initial position and lookAt
        /// </summary>
        private void UpdateCameraPosition()
        {
            Matrix rotationMatrix = Matrix.CreateFromYawPitchRoll(this.phi, this.theta, 0);
            Vector3 transformedReference = Vector3.Transform(this.initialPosition, rotationMatrix);
            this.Camera.Transform.Position = transformedReference + this.initialLookAt;
            this.Camera.Transform.LookAt(this.initialLookAt);
        }

        #endregion
    }
}
