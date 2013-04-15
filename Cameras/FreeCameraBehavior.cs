#region File Description
//-----------------------------------------------------------------------------
// FreeCameraBehavior
// Copyright © 2010 - 2013 Wave Coorporation. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Usings Statements
using System;
using System.Linq;

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
    public class FreeCameraBehavior : Behavior
    {
        /// <summary>
        /// The camera to move.
        /// </summary>
        [RequiredComponent]
        public Camera Camera;

        /// <summary>
        /// Camera rotation calculation.
        /// </summary>
        private Matrix cameraMatrixRotation;

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
        /// The forward.
        /// </summary>
        private Vector3 forward;

        /// <summary>
        /// The forward normalized vector.
        /// </summary>
        private Vector3 forwardNormalizedVector;

        /// <summary>
        /// The input.
        /// </summary>
        private Input input;

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
        /// The pitch.
        /// </summary>
        private float pitch;

        /// <summary>
        /// The position.
        /// </summary>
        private Vector3 position;

        /// <summary>
        /// The right.
        /// </summary>
        private Vector3 right;

        /// <summary>
        /// The right normalized vector.
        /// </summary>
        private Vector3 rightNormalizedVector;

        /// <summary>
        ///     Mouse speed movement
        /// </summary>
        private float rotationSpeed = .16f;

        /// <summary>
        ///     Speed of the movement
        /// </summary>
        private float speed = 20f;

        /// <summary>
        /// The temp rotation matrix.
        /// </summary>
        private Matrix tempRotationMatrix;

        /// <summary>
        /// The time difference.
        /// </summary>
        private float timeDifference;

        /// <summary>
        /// The up normalized vector.
        /// </summary>
        private Vector3 upNormalizedVector;

        /// <summary>
        /// The x difference.
        /// </summary>
        private float xDifference;

        /// <summary>
        /// The y difference.
        /// </summary>
        private float yDifference;

        /// <summary>
        /// The yaw.
        /// </summary>
        private float yaw;

        #region Properties

        /// <summary>
        /// Gets or sets the rotation speed.
        /// </summary>
        /// <value>
        /// The rotation speed.
        /// </value>
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
        /// Gets or sets the speed of the camera movement.
        /// </summary>
        /// <value>
        /// The speed.
        /// </value>
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
        ///     Initializes a new instance of the <see cref="FreeCameraBehavior" /> class.
        /// </summary>
        public FreeCameraBehavior()
            : base("FreeCameraBehavior")
        {
            this.isDragging = false;
            this.Camera = null;
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
            this.yaw = 0.0f;
            this.pitch = 0.0f;
            this.xDifference = 0f;
            this.yDifference = 0f;
            this.isDragging = false;

            // Preview of the view matrix
            this.cameraMatrixRotation = Matrix.Identity;

            // Calculate the camera view matrix.
            Matrix initialMatrix =
                Matrix.Invert(Matrix.CreateLookAt(this.Camera.Position, this.Camera.LookAt, this.Camera.UpVector));

            // Initialize the vectors
            this.right = initialMatrix.Right;
            this.forward = initialMatrix.Forward;
            Vector3 up = initialMatrix.Up;
            up.Normalize();
            this.upNormalizedVector = up;
            this.position = this.Camera.Position;
            this.cameraMatrixRotation = initialMatrix;
        }

        /// <summary>
        /// Updates the camera position.
        /// </summary>
        /// <param name="gameTime">
        /// The elapsed game time.
        /// </param>
        protected override void Update(TimeSpan gameTime)
        {
            this.timeDifference = (float)gameTime.TotalMilliseconds / 1000.0f;
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
            this.input = WaveServices.Input;
            this.isMouseConnected = this.input.MouseState.IsConnected;
            this.isTouchPanelConnected = this.input.TouchPanelState.IsConnected;

            if (this.isMouseConnected)
            {
                this.isTouchPanelConnected = false;
            }

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

                if (this.moveForward)
                {
                    // Manual inline: position += speed * forward;
                    this.position.X = this.position.X + (amount * this.speed * this.forward.X);
                    this.position.Y = this.position.Y + (amount * this.speed * this.forward.Y);
                    this.position.Z = this.position.Z + (amount * this.speed * this.forward.Z);
                    this.UpdateCameraPosition();
                }
                else if (this.moveBack)
                {
                    // Manual inline: position -= speed * forward;
                    this.position.X = this.position.X - (amount * this.speed * this.forward.X);
                    this.position.Y = this.position.Y - (amount * this.speed * this.forward.Y);
                    this.position.Z = this.position.Z - (amount * this.speed * this.forward.Z);
                    this.UpdateCameraPosition();
                }

                if (this.moveLeft)
                {
                    // Manual inline: position -= speed * right;
                    this.position.X = this.position.X - (amount * this.speed * this.right.X);
                    this.position.Y = this.position.Y - (amount * this.speed * this.right.Y);
                    this.position.Z = this.position.Z - (amount * this.speed * this.right.Z);
                    this.UpdateCameraPosition();
                }
                else if (this.moveRight)
                {
                    // Manual inline: position += speed * right;
                    this.position.X = this.position.X + (amount * this.speed * this.right.X);
                    this.position.Y = this.position.Y + (amount * this.speed * this.right.Y);
                    this.position.Z = this.position.Z + (amount * this.speed * this.right.Z);
                    this.UpdateCameraPosition();
                }

                this.moveWithTouchPanel = false;
            }

            if (this.isTouchPanelConnected || this.isMouseConnected)
            {
                if (this.isTouchPanelConnected)
                {
                    this.currentTouchPanelState = this.input.TouchPanelState;
                }

                if (this.isMouseConnected)
                {
                    this.currentMouseState = this.input.MouseState;
                }

                if ((this.isTouchPanelConnected && this.currentTouchPanelState.Count == 1)
                    || (this.isMouseConnected && this.currentMouseState.RightButton == ButtonState.Pressed))
                {
                    if (this.isTouchPanelConnected && this.currentTouchPanelState.Count == 1)
                    {
                        this.currentTouchLocation = this.currentTouchPanelState.First();
                    }

                    if ((this.isTouchPanelConnected
                         && (this.currentTouchLocation.State == TouchLocationState.Pressed
                             || this.currentTouchLocation.State == TouchLocationState.Moved))
                        || (this.isMouseConnected && this.currentMouseState.RightButton == ButtonState.Pressed))
                    {
                        if (this.isDragging == false)
                        {
                            this.isDragging = true;
                        }
                        else
                        {
                            // Get the current different between x and Y
                            // From touchpad
                            if (this.currentTouchPanelState.IsConnected)
                            {
                                this.xDifference = this.currentTouchLocation.Position.X
                                                   - this.lastTouchLocation.Position.X;
                                this.yDifference = this.currentTouchLocation.Position.Y
                                                   - this.lastTouchLocation.Position.Y;
                            }

                            // From mouse
                            if (this.isMouseConnected)
                            {
                                this.xDifference = this.currentMouseState.X - this.lastMouseState.X;
                                this.yDifference = this.currentMouseState.Y - this.lastMouseState.Y;
                            }

                            // Calculated yaw and pitch
                            this.yaw = this.yaw - (this.xDifference * amount * this.rotationSpeed);
                            this.pitch = this.pitch - (this.yDifference * amount * this.rotationSpeed);

                            // Manual inline: forwardNormalizedVector = cameraRotation.Forward;
                            this.forwardNormalizedVector.X = this.cameraMatrixRotation.Forward.X;
                            this.forwardNormalizedVector.Y = this.cameraMatrixRotation.Forward.Y;
                            this.forwardNormalizedVector.Z = this.cameraMatrixRotation.Forward.Z;
                            this.forwardNormalizedVector.Normalize();

                            // Manual inline: rightNormalizedVector = cameraRotation.Right;
                            this.rightNormalizedVector.X = this.cameraMatrixRotation.Right.X;
                            this.rightNormalizedVector.Y = this.cameraMatrixRotation.Right.Y;
                            this.rightNormalizedVector.Z = this.cameraMatrixRotation.Right.Z;
                            this.rightNormalizedVector.Normalize();

                            // Manual inline: upNormalizedVector = cameraMatrixRotation.Up;
                            this.upNormalizedVector.X = this.cameraMatrixRotation.Up.X;
                            this.upNormalizedVector.Y = this.cameraMatrixRotation.Up.Y;
                            this.upNormalizedVector.Z = this.cameraMatrixRotation.Up.Z;
                            this.upNormalizedVector.Normalize();

                            // Calculate the new camera matrix angle with the normalized vectors
                            Matrix.CreateFromAxisAngle(
                                ref this.rightNormalizedVector, this.pitch, out this.tempRotationMatrix);
                            Matrix.Multiply(
                                ref this.cameraMatrixRotation, 
                                ref this.tempRotationMatrix, 
                                out this.cameraMatrixRotation);

                            Matrix.CreateFromAxisAngle(
                                ref this.upNormalizedVector, this.yaw, out this.tempRotationMatrix);
                            Matrix.Multiply(
                                ref this.cameraMatrixRotation, 
                                ref this.tempRotationMatrix, 
                                out this.cameraMatrixRotation);

                            Matrix.CreateFromAxisAngle(
                                ref this.forwardNormalizedVector, 0f, out this.tempRotationMatrix);
                            Matrix.Multiply(
                                ref this.cameraMatrixRotation, 
                                ref this.tempRotationMatrix, 
                                out this.cameraMatrixRotation);

                            // Restore the yaw and pitch
                            this.yaw = 0.0f;
                            this.pitch = 0.0f;

                            // Manual inline: forward = cameraRotation.Forward;
                            this.forward.X = this.cameraMatrixRotation.Forward.X;
                            this.forward.Y = this.cameraMatrixRotation.Forward.Y;
                            this.forward.Z = this.cameraMatrixRotation.Forward.Z;

                            // Manual inline: right = cameraRotation.Right;
                            this.right.X = this.cameraMatrixRotation.Right.X;
                            this.right.Y = this.cameraMatrixRotation.Right.Y;
                            this.right.Z = this.cameraMatrixRotation.Right.Z;

                            // Update the current look at
                            this.UpdateLookAt();

                            // Restore the current matrix rotation
                            this.cameraMatrixRotation =
                                Matrix.Invert(
                                    Matrix.CreateLookAt(this.Camera.Position, this.Camera.LookAt, this.Camera.UpVector));
                        }
                    }

                    this.lastTouchLocation = this.currentTouchLocation;
                    this.lastMouseState = this.currentMouseState;
                }
                else
                {
                    this.isDragging = false;
                }
            }
        }

        /// <summary>
        /// The update camera position.
        /// </summary>
        private void UpdateCameraPosition()
        {
            this.UpdateLookAt();

            // Manual inline: camera.Position = position;
            this.Camera.Position.X = this.position.X;
            this.Camera.Position.Y = this.position.Y;
            this.Camera.Position.Z = this.position.Z;
        }

        /// <summary>
        /// The update look at.
        /// </summary>
        private void UpdateLookAt()
        {
            // Manual inline: camera.LookAt = target;
            this.Camera.LookAt.X = this.position.X + this.forward.X;
            this.Camera.LookAt.Y = this.position.Y + this.forward.Y;
            this.Camera.LookAt.Z = this.position.Z + this.forward.Z;

            // Manual inline: camera.UpVector = Vector3.Up;
            this.Camera.UpVector.X = Vector3.Up.X;
            this.Camera.UpVector.Y = Vector3.Up.Y;
            this.Camera.UpVector.Z = Vector3.Up.Z;
        }

        #endregion
    }
}