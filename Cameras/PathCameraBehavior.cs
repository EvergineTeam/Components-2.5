#region File Description
//-----------------------------------------------------------------------------
// PathCameraBehavior
//
// Copyright © 2010 - 2013 Wave Coorporation. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
#endregion

namespace WaveEngine.Components.Cameras
{
    /// <summary>
    /// A behavior that moves a camera along a defined path of points.
    /// </summary>
    public class PathCameraBehavior : Behavior
    {
        /// <summary>
        ///     State of the behavior
        /// </summary>
        public enum State
        {
            /// <summary>
            ///     The behavior is stopped.
            /// </summary>
            Stop,

            /// <summary>
            ///     The behavior is playing.
            /// </summary>
            Play,

            /// <summary>
            ///     The behavior is playing in a loop.
            /// </summary>
            PlayAndRepeat,

            /// <summary>
            ///     The behavior is paused.
            /// </summary>
            Pause
        }

        /// <summary>
        ///     Speed of the camera.
        /// </summary>
        private float speed;

        /// <summary>
        ///     The camera to move.
        /// </summary>
        [RequiredComponent]
        public Camera Camera;

        /// <summary>
        ///     The path component.
        /// </summary>
        private readonly Path path;

        /// <summary>
        ///     The current camera point.
        /// </summary>
        private CameraPoint currentCameraPoint;

        /// <summary>
        ///     The current state of the path behavior.
        /// </summary>
        private State currentState;

        #region Properties

        /// <summary>
        /// Gets or sets the state of the current state of the behavior.
        /// </summary>
        /// <value>
        /// The state of the current.
        /// </value>
        public State CurrentState
        {
            get
            {
                return this.currentState;
            }

            set
            {
                this.ChangeState(value);
                this.currentState = value;
            }
        }

        /// <summary>
        /// Gets or sets the speed.
        /// </summary>
        /// <value>
        /// The speed.
        /// </value>
        public float Speed
        {
            get { return this.speed; }

            set { this.speed = value; }
        }
        #endregion

        #region Initialize

        /// <summary>
        ///     Initializes a new instance of the <see cref="PathCameraBehavior" /> class.
        /// </summary>
        /// <param name="cameraPoints">The camera points.</param>
        public PathCameraBehavior(List<CameraPoint> cameraPoints)
            : base("PathCameraBehavior")
        {
            this.path = new Path(cameraPoints);
            this.CurrentState = State.PlayAndRepeat;
            this.speed = 1f;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="PathCameraBehavior" /> class.
        /// </summary>
        /// <param name="cameraPoints">The camera points.</param>
        /// <param name="pathFrames">The frames between each camera point.</param>
        public PathCameraBehavior(List<CameraPoint> cameraPoints, int pathFrames)
            : base("PathCameraBehavior")
        {
            this.path = new Path(cameraPoints, pathFrames);
            this.CurrentState = State.PlayAndRepeat;
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///     Updates the camera movement.
        /// </summary>
        /// <param name="gameTime">The elapsed game time.</param>
        protected override void Update(TimeSpan gameTime)
        {
            if (this.currentState == State.Play || this.currentState == State.PlayAndRepeat)
            {
                bool isNext = this.path.Evaluate((int)(gameTime.Milliseconds * this.speed));

                if (isNext)
                {
                    this.currentCameraPoint = this.path.CurrentPoint;

                    // Manual inline: camera.Position = currentPoint.Position;
                    this.Camera.Position.X = this.currentCameraPoint.Position.X;
                    this.Camera.Position.Y = this.currentCameraPoint.Position.Y;
                    this.Camera.Position.Z = this.currentCameraPoint.Position.Z;

                    // Manual inline: camera.LookAt = currentPoint.LookAt;
                    this.Camera.LookAt.X = this.currentCameraPoint.LookAt.X;
                    this.Camera.LookAt.Y = this.currentCameraPoint.LookAt.Y;
                    this.Camera.LookAt.Z = this.currentCameraPoint.LookAt.Z;

                    // Manual inline: camera.UpVector = currentPoint.Up;
                    this.Camera.UpVector.X = this.currentCameraPoint.Up.X;
                    this.Camera.UpVector.Y = this.currentCameraPoint.Up.Y;
                    this.Camera.UpVector.Z = this.currentCameraPoint.Up.Z;
                }
                else
                {
                    this.currentState = State.Stop;
                }
            }
        }

        /// <summary>
        ///     Update the state of the behavior
        /// </summary>
        /// <param name="newState">The new state to set</param>
        private void ChangeState(State newState)
        {
            switch (newState)
            {
                case State.Play:
                    {
                        if (this.currentState != State.Pause)
                        {
                            this.path.ResetIndex();
                        }

                        this.path.LoopEnabled = false;
                        break;
                    }

                case State.PlayAndRepeat:
                    {
                        this.path.LoopEnabled = true;
                        break;
                    }

                case State.Stop:
                    {
                        this.path.ResetIndex();
                        break;
                    }

                case State.Pause:
                    {
                        break;
                    }
            }
        }

        #endregion
    }
}
