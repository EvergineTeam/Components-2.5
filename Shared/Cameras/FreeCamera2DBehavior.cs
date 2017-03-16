#region File Description
//-----------------------------------------------------------------------------
// FreeCamera2DBehavior
// Copyright © 2017 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Usings Statements
using System;
using System.Linq;
using System.Runtime.Serialization;
using WaveEngine.Common.Input;
using WaveEngine.Common.Math;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Services;
#endregion

namespace WaveEngine.Components.Cameras
{
    /// <summary>
    /// A behavior that allows a camera to move freely.
    /// </summary>
    [DataContract(Namespace = "WaveEngine.Components.Cameras")]
    public class FreeCamera2DBehavior : Behavior
    {
        /// <summary>
        /// Stick threshold
        /// </summary>
        private const float StickThreshold = 0.1f;

        /// <summary>
        /// Mouse conversion factor
        /// </summary>
        private const float MouseFactor = 4;

        /// <summary>
        /// The camera to move.
        /// </summary>
        [RequiredComponent]
        public Camera2D Camera = null;

        /// <summary>
        /// The transform 2D
        /// </summary>
        [RequiredComponent]
        private Transform2D transform2D = null;

        /// <summary>
        /// The input.
        /// </summary>
        [RequiredService]
        private Input input = null;

        /// <summary>   
        /// The mouse is dragging.
        /// </summary>
        private bool isDragging;

        /// <summary>
        /// Mouse speed movement
        /// </summary>
        private float rotationSpeed;

        /// <summary>
        /// Zoom speed with mouse wheel.
        /// </summary>
        private float wheelZoomSpeed;

        /// <summary>
        ///     Speed of the movement
        /// </summary>
        private float speed;

        /// <summary>
        /// The time difference.
        /// </summary>
        private float timeDifference;

        /// <summary>
        /// The up vector
        /// </summary>
        private Vector3 up;

        /// <summary>
        /// The right vector
        /// </summary>
        private Vector3 right;

        /// <summary>
        /// The position delta
        /// </summary>
        private Vector2 positionDelta;

        /// <summary>
        /// The previous mouse state
        /// </summary>
        private MouseState lastMouseState;

        /// <summary>
        /// Last drag position
        /// </summary>
        private Vector2 lastDragPosition;

        #region Properties
        /// <summary>
        /// Gets or sets the rotation speed.
        /// </summary>
        /// <value>
        /// The rotation speed.
        /// </value>
        [DataMember]
        public float RotationSpeed
        {
            get
            {
                return this.rotationSpeed;
            }

            set
            {
                this.rotationSpeed = value;
            }
        }

        /// <summary>
        /// Gets or sets the mouse wheel zoom speed.
        /// </summary>
        /// <value>
        /// The zoom speed.
        /// </value>
        [DataMember]
        public float WheelZoomSpeed
        {
            get
            {
                return this.wheelZoomSpeed;
            }

            set
            {
                this.wheelZoomSpeed = value;
            }
        }

        /// <summary>
        /// Gets or sets the speed of the camera movement.
        /// </summary>
        /// <value>
        /// The speed.
        /// </value>
        [DataMember]
        public float Speed
        {
            get
            {
                return this.speed;
            }

            set
            {
                this.speed = value;
            }
        }

        #endregion

        #region Initialize
        /// <summary>
        /// Initializes a new instance of the <see cref="FreeCamera2DBehavior" /> class.
        /// </summary>
        public FreeCamera2DBehavior()
            : base("FreeCamera2DBehavior")
        {
        }

        /// <summary>
        /// Default values
        /// </summary>
        protected override void DefaultValues()
        {
            base.DefaultValues();

            this.rotationSpeed = .004f;
            this.wheelZoomSpeed = 1 / 4000f;
            this.speed = 200.0f;
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Updates the camera position.
        /// </summary>
        /// <param name="gameTime">
        /// The elapsed game time.
        /// </param>
        protected override void Update(TimeSpan gameTime)
        {
            this.timeDifference = (float)gameTime.TotalSeconds;
            this.HandleInput(this.timeDifference);
        }

        /// <summary>
        /// The handle input.
        /// </summary>
        /// <param name="amount">
        /// The amount.
        /// </param>
        private void HandleInput(float amount)
        {
            this.up = this.Camera.UpVector;
            this.right.X = this.up.Y;
            this.right.Y = -this.up.X;
            this.right.Z = this.up.Z;

            this.positionDelta = Vector2.Zero;

            if (this.input.KeyboardState.IsConnected)
            {
                this.HandleKeyboard(amount);
            }

            if (this.input.MouseState.IsConnected)
            {
                this.HandleMouse(amount);
            }
            else if (this.input.TouchPanelState.IsConnected)
            {
                this.HandleTouch(amount);
            }

            if (this.input.GamePadState.IsConnected)
            {
                this.HandleGamePad(amount);
            }

            // Update position
            this.Camera.Transform.Position += this.positionDelta;
        }

        /// <summary>
        /// Move the camera using the Keyboard
        /// </summary>
        /// <param name="amount">The amount of time</param>
        private void HandleKeyboard(float amount)
        {
            KeyboardState keyboardState = this.input.KeyboardState;

            if (keyboardState.S == ButtonState.Pressed)
            {
                // Manual inline: position += speed * forward;
                this.positionDelta.X = this.positionDelta.X + (amount * this.speed * this.up.X);
                this.positionDelta.Y = this.positionDelta.Y + (amount * this.speed * this.up.Y);
            }
            else if (keyboardState.W == ButtonState.Pressed)
            {
                // Manual inline: position -= speed * forward;                          
                this.positionDelta.X = this.positionDelta.X - (amount * this.speed * this.up.X);
                this.positionDelta.Y = this.positionDelta.Y - (amount * this.speed * this.up.Y);
            }

            if (keyboardState.A == ButtonState.Pressed)
            {
                // Manual inline: position -= speed * right;
                this.positionDelta.X = this.positionDelta.X - (amount * this.speed * this.right.X);
                this.positionDelta.Y = this.positionDelta.Y - (amount * this.speed * this.right.Y);
            }
            else if (keyboardState.D == ButtonState.Pressed)
            {
                // Manual inline: position += speed * right;
                this.positionDelta.X = this.positionDelta.X + (amount * this.speed * this.right.X);
                this.positionDelta.Y = this.positionDelta.Y + (amount * this.speed * this.right.Y);
            }
        }

        /// <summary>
        /// Move the camera using the mouse
        /// </summary>
        /// <param name="amount">The amount of time</param>
        private void HandleMouse(float amount)
        {
            MouseState mouseState = this.input.MouseState;

            if (mouseState.Wheel != 0)
            {
                this.transform2D.Scale *= 1 + (mouseState.Wheel * this.wheelZoomSpeed);
            }

            if (mouseState.RightButton == ButtonState.Pressed || mouseState.MiddleButton == ButtonState.Pressed)
            {
                if (this.isDragging)
                {
                    float deltaX = mouseState.X - this.lastMouseState.X;
                    float deltaY = mouseState.Y - this.lastMouseState.Y;

                    Vector2 zoom = this.transform2D.Scale;

                    // Up
                    this.positionDelta.X = this.positionDelta.X - (amount * MouseFactor * this.speed * deltaY * zoom.X * this.up.X);
                    this.positionDelta.Y = this.positionDelta.Y - (amount * MouseFactor * this.speed * deltaY * zoom.Y * this.up.Y);

                    // Right
                    this.positionDelta.X = this.positionDelta.X - (amount * MouseFactor * this.speed * deltaX * zoom.X * this.right.X);
                    this.positionDelta.Y = this.positionDelta.Y - (amount * MouseFactor * this.speed * deltaX * zoom.Y * this.right.Y);
                }

                this.lastMouseState = mouseState;
                this.isDragging = true;
            }
            else
            {
                this.isDragging = false;
            }
        }

        /// <summary>
        /// Move the camera using the touch panel
        /// </summary>
        /// <param name="amount">The amount of time</param>
        private void HandleTouch(float amount)
        {
            TouchPanelState touchState = this.input.TouchPanelState;

            if (touchState.Count >= 2)
            {
                Vector2 touch1 = touchState[0].Position;
                Vector2 touch2 = touchState[1].Position;
                Vector2 dragPosition;
                Vector2.Lerp(ref touch1, ref touch2, 0.5f, out dragPosition);

                if (this.isDragging)
                {
                    float deltaX = dragPosition.X - this.lastDragPosition.X;
                    float deltaY = dragPosition.Y - this.lastDragPosition.Y;

                    Vector2 zoom = this.transform2D.Scale;

                    // Up
                    this.positionDelta.X = this.positionDelta.X - (deltaY * zoom.X * this.up.X);
                    this.positionDelta.Y = this.positionDelta.Y - (deltaY * zoom.Y * this.up.Y);

                    // Right
                    this.positionDelta.X = this.positionDelta.X - (deltaX * zoom.X * this.right.X);
                    this.positionDelta.Y = this.positionDelta.Y - (deltaX * zoom.Y * this.right.Y);
                }

                this.lastDragPosition = dragPosition;
                this.isDragging = true;
            }
            else
            {
                this.isDragging = false;
            }
        }

        /// <summary>
        /// Move the camera using the gamepad
        /// </summary>
        /// <param name="amount">The amount of time</param>
        private void HandleGamePad(float amount)
        {
            GamePadState gamePadState = this.input.GamePadState;

            Vector2 leftStick = gamePadState.ThumbStricks.Left;

            if (leftStick.Y > StickThreshold)
            {
                // Manual inline: position += speed * forward;
                this.positionDelta.X = this.positionDelta.X - (amount * this.speed * this.up.X * leftStick.Y);
                this.positionDelta.Y = this.positionDelta.Y - (amount * this.speed * this.up.Y * leftStick.Y);
            }
            else if (leftStick.Y < -StickThreshold)
            {
                // Manual inline: position -= speed * forward;
                this.positionDelta.X = this.positionDelta.X - (amount * this.speed * this.up.X * leftStick.Y);
                this.positionDelta.Y = this.positionDelta.Y - (amount * this.speed * this.up.Y * leftStick.Y);
            }

            if (leftStick.X > StickThreshold)
            {
                // Manual inline: position -= speed * right;
                this.positionDelta.X = this.positionDelta.X + (amount * this.speed * this.right.X * leftStick.X);
                this.positionDelta.Y = this.positionDelta.Y + (amount * this.speed * this.right.Y * leftStick.X);
            }
            else if (leftStick.X < -StickThreshold)
            {
                // Manual inline: position += speed * right;
                this.positionDelta.X = this.positionDelta.X + (amount * this.speed * this.right.X * leftStick.X);
                this.positionDelta.Y = this.positionDelta.Y + (amount * this.speed * this.right.Y * leftStick.X);
            }
        }
        #endregion
    }
}