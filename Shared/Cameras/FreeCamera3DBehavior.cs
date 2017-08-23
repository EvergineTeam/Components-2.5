// Copyright © 2017 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Usings Statements
using System;
using System.Linq;
using System.Runtime.Serialization;
using WaveEngine.Common.Input;
using WaveEngine.Common.Math;
using WaveEngine.Framework;
using WaveEngine.Framework.Diagnostic;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Services;
#endregion

namespace WaveEngine.Components.Cameras
{
    /// <summary>
    /// A behavior that allows a camera to move freely.
    /// </summary>
    [DataContract(Namespace = "WaveEngine.Components.Cameras")]
    public class FreeCamera3DBehavior : Behavior
    {
        /// <summary>
        /// Stick threshold
        /// </summary>
        private const float StickThreshold = 0.1f;

        /// <summary>
        /// Max pitch value of the camera
        /// </summary>
        private const float MaxPitch = MathHelper.PiOver2 * 0.95f;

        /// <summary>
        /// The transform.
        /// </summary>
        [RequiredComponent]
        public Transform3D Transform = null;

        /// <summary>
        /// The current mouse state.
        /// </summary>
        private MouseState currentMouseState;

        /// <summary>
        /// The current touch location.
        /// </summary>
        private TouchLocation currentTouchLocation;

        /// <summary>
        /// The current touch panel state.
        /// </summary>
        private TouchPanelState currentTouchPanelState;

        /// <summary>
        /// The input.
        /// </summary>
        [RequiredService]
        private Input input = null;

        /// <summary>
        /// The is dragging.
        /// </summary>
        private bool isDragging;

        /// <summary>
        /// The is mouse connected.
        /// </summary>
        private bool isMouseConnected;

        /// <summary>
        /// The is touch panel connected.
        /// </summary>
        private bool isTouchPanelConnected;

        /// <summary>
        /// The keyboard state.
        /// </summary>
        private KeyboardState keyboardState;

        /// <summary>
        /// The last mouse state.
        /// </summary>
        private MouseState lastMouseState;

        /// <summary>
        /// The last touch location.
        /// </summary>
        private TouchLocation lastTouchLocation;

        /// <summary>
        /// The move back.
        /// </summary>
        private bool moveBack;

        /// <summary>
        /// The move forward.
        /// </summary>
        private bool moveForward;

        /// <summary>
        /// The move left.
        /// </summary>
        private bool moveLeft;

        /// <summary>
        /// The move right.
        /// </summary>
        private bool moveRight;

        /// <summary>
        /// The move with touch panel.
        /// </summary>
        private bool moveWithTouchPanel;

        /// <summary>
        ///     Mouse speed movement
        /// </summary>
        private float rotationSpeed;

        /// <summary>
        /// GamePad speed movement
        /// </summary>
        private float gamepadRotationSpeed;

        /// <summary>
        ///     Speed of the movement
        /// </summary>
        private float speed;

        /// <summary>
        /// The time difference.
        /// </summary>
        private float timeDifference;

        /// <summary>
        /// The x difference.
        /// </summary>
        private float xDifference;

        /// <summary>
        /// The y difference.
        /// </summary>
        private float yDifference;

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
        /// Gets or sets the gamepad rotation speed.
        /// </summary>
        /// <value>
        /// The rotation speed.
        /// </value>
        [DataMember]
        public float GamepadRotationSpeed
        {
            get
            {
                return this.gamepadRotationSpeed;
            }

            set
            {
                this.gamepadRotationSpeed = value;
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
        ///     Initializes a new instance of the <see cref="FreeCamera3DBehavior" /> class.
        /// </summary>
        public FreeCamera3DBehavior()
            : base("FreeCameraBehavior")
        {
        }

        /// <summary>
        /// Default values
        /// </summary>
        protected override void DefaultValues()
        {
            base.DefaultValues();

            this.rotationSpeed = .004f;
            this.gamepadRotationSpeed = .75f;
            this.speed = 20;
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///     Performs further custom initialization for this instance.
        /// </summary>
        /// <remarks>
        ///     By default this method does nothing.
        /// </remarks>
        protected override void Initialize()
        {
            this.xDifference = 0f;
            this.yDifference = 0f;
            this.isDragging = false;
        }

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
            this.isMouseConnected = this.input.MouseState.IsConnected;
            this.isTouchPanelConnected = this.input.TouchPanelState.IsConnected;

            if (this.input.KeyboardState.IsConnected || this.isTouchPanelConnected)
            {
                this.keyboardState = this.input.KeyboardState;

                // If the touch panel is connected and it has two points, we can go forward
                if (this.isTouchPanelConnected)
                {
                    if (this.input.TouchPanelState.Count == 2)
                    {
                        this.moveWithTouchPanel = true;
                    }
                }

                this.moveForward = this.keyboardState.W == ButtonState.Pressed || this.moveWithTouchPanel;
                this.moveBack = this.keyboardState.S == ButtonState.Pressed;
                this.moveLeft = this.keyboardState.A == ButtonState.Pressed;
                this.moveRight = this.keyboardState.D == ButtonState.Pressed;
                this.UpdateCameraPosition(amount);

                this.moveWithTouchPanel = false;
            }

            if (this.isMouseConnected)
            {
                this.currentMouseState = this.input.MouseState;

                // If there's a touch or right mouse button is pressed...
                if (this.currentMouseState.RightButton == ButtonState.Pressed)
                {
                    // If pressed or moved
                    if (this.currentMouseState.RightButton == ButtonState.Pressed)
                    {
                        if (this.isDragging == false)
                        {
                            // Set drag flag to true
                            this.isDragging = true;
                        }
                        else
                        {
                            this.xDifference = this.currentMouseState.X - this.lastMouseState.X;
                            this.yDifference = this.currentMouseState.Y - this.lastMouseState.Y;

                            // Calculated yaw and pitch
                            float yaw = -this.xDifference * this.rotationSpeed;
                            float pitch = -this.yDifference * this.rotationSpeed;

                            this.UpdateOrientation(yaw, pitch);
                        }
                    }

                    this.lastMouseState = this.currentMouseState;
                }
                else
                {
                    this.isDragging = false;
                }
            }
            else if (this.isTouchPanelConnected)
            {
                this.currentTouchPanelState = this.input.TouchPanelState;

                // If there's a touch, capture its touch properties
                if (this.currentTouchPanelState.Count == 1)
                {
                    this.currentTouchLocation = this.currentTouchPanelState.First();

                    // If current touch is pressed or moved
                    if (this.currentTouchLocation.State == TouchLocationState.Pressed
                        || this.currentTouchLocation.State == TouchLocationState.Moved)
                    {
                        if (this.isDragging == false)
                        {
                            // Set drag flag to true
                            this.isDragging = true;
                        }
                        else
                        {
                            // Get the current different between x and Y
                            // From touchpad
                            this.xDifference = this.currentTouchLocation.Position.X - this.lastTouchLocation.Position.X;
                            this.yDifference = this.currentTouchLocation.Position.Y - this.lastTouchLocation.Position.Y;

                            // Calculated yaw and pitch
                            float yaw = -this.xDifference * this.rotationSpeed;
                            float pitch = -this.yDifference * this.rotationSpeed;

                            this.UpdateOrientation(yaw, pitch);
                        }
                    }

                    this.lastTouchLocation = this.currentTouchLocation;
                }
                else
                {
                    this.isDragging = false;
                }
            }

            if (this.input.GamePadState.IsConnected)
            {
                /////////////////////////////////////////////////
                ////// Position Camera
                /////////////////////////////////////////////////
                GamePadState gamePadState = this.input.GamePadState;

                Vector2 leftStick = gamePadState.ThumbSticks.Left;

                float threshold = 0.2f;

                this.moveForward = leftStick.Y > threshold;
                this.moveBack = leftStick.Y < -threshold;
                this.moveRight = leftStick.X > threshold;
                this.moveLeft = leftStick.X < -threshold;

                this.UpdateCameraPosition(amount);

                /////////////////////////////////////////////////
                ////// LookAT
                /////////////////////////////////////////////////
                Vector2 rightStick = gamePadState.ThumbSticks.Right;
                this.xDifference = rightStick.X;
                this.yDifference = -rightStick.Y;

                ////// Calculated yaw and pitch
                float yaw = this.xDifference * amount * this.gamepadRotationSpeed;
                float pitch = this.yDifference * amount * this.gamepadRotationSpeed;

                this.UpdateOrientation(yaw, pitch);
            }
        }

        /// <summary>
        /// The update camera position.
        /// </summary>
        /// <param name="amount">The amount of movement</param>
        private void UpdateCameraPosition(float amount)
        {
            Vector3 displacement = Vector3.Zero;
            if (this.moveForward)
            {
                Vector3 forward = this.Transform.WorldTransform.Forward;

                // Manual inline: position += speed * forward;
                displacement.X = displacement.X + (amount * this.speed * forward.X);
                displacement.Y = displacement.Y + (amount * this.speed * forward.Y);
                displacement.Z = displacement.Z + (amount * this.speed * forward.Z);
            }
            else if (this.moveBack)
            {
                Vector3 backward = this.Transform.WorldTransform.Backward;

                // Manual inline: position -= speed * forward;
                displacement.X = displacement.X + (amount * this.speed * backward.X);
                displacement.Y = displacement.Y + (amount * this.speed * backward.Y);
                displacement.Z = displacement.Z + (amount * this.speed * backward.Z);
            }

            if (this.moveLeft)
            {
                Vector3 left = this.Transform.WorldTransform.Left;

                // Manual inline: position -= speed * right;
                displacement.X = displacement.X + (amount * this.speed * left.X);
                displacement.Y = displacement.Y + (amount * this.speed * left.Y);
                displacement.Z = displacement.Z + (amount * this.speed * left.Z);
            }
            else if (this.moveRight)
            {
                Vector3 right = this.Transform.WorldTransform.Right;

                // Manual inline: position += speed * right;
                displacement.X = displacement.X + (amount * this.speed * right.X);
                displacement.Y = displacement.Y + (amount * this.speed * right.Y);
                displacement.Z = displacement.Z + (amount * this.speed * right.Z);
            }

            // Manual inline: camera.Position = position;
            this.Transform.Position += displacement;
        }

        /// <summary>
        /// Update camera orientation
        /// </summary>
        /// <param name="yaw">The yaw</param>
        /// <param name="pitch">The pitch</param>
        private void UpdateOrientation(float yaw, float pitch)
        {
            var rotation = this.Transform.Rotation;

            rotation.Y += yaw;

            rotation.X += pitch;
            rotation.X = Math.Max(rotation.X, -MaxPitch);
            rotation.X = Math.Min(rotation.X, MaxPitch);

            this.Transform.Rotation = rotation;
        }
        #endregion
    }
}
