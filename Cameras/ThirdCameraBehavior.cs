#region File Description
//-----------------------------------------------------------------------------
// ThirdCameraBehavior
//
// Copyright © 2014 Wave Corporation
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using WaveEngine.Common.Math;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
#endregion

namespace WaveEngine.Components.Cameras
{
    /// <summary>
    /// A behavior that simulates a third person camera.
    /// </summary>
    public class ThirdCameraBehavior : Behavior
    {
        /// <summary>
        /// Camera movement delay.
        /// </summary>
        public float PivotDelay = 10f;

        /// <summary>
        /// The entity to follow.
        /// </summary>
        private readonly Entity entity;

        /// <summary>
        /// The target transform.
        /// </summary>
        private Transform3D targetTransform;

        /// <summary>
        /// The camera to move.
        /// </summary>
        [RequiredComponent]
        public Camera3D Camera;

        /// <summary>
        /// The current camera position.
        /// </summary>
        private Vector3 cameraPosition;

        /// <summary>
        /// The calculated offset vector.
        /// </summary>
        private Vector3 calculatedOffsetVector;

        /// <summary>
        /// The pivot position.
        /// </summary>
        private Vector3 pivotPlayer;

        /// <summary>
        /// The camera distance.
        /// </summary>
        private Vector3 cameraDistance;

        #region Properties

        /// <summary>
        ///     Gets or sets the distance from the target position.
        /// </summary>
        /// <value>
        ///     The camera distance from the target position.
        /// </value>
        public Vector3 CameraDistance
        {
            get
            {
                return this.cameraDistance;
            }

            set
            {
                this.cameraDistance = value;
            }
        }

        /// <summary>
        /// Gets or sets the position to follow.
        /// </summary>
        /// <value>
        /// The position to follow.
        /// </value>
        public Vector3 TargetPosition { get; set; }
        #endregion

        #region Initialize
        /// <summary>
        /// Initializes a new instance of the <see cref="ThirdCameraBehavior" /> class.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <exception cref="System.ArgumentNullException">If entity is null.</exception>
        public ThirdCameraBehavior(Entity entity)
            : base("ThirdCameraBehavior")
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity", "The entity cannot be null");
            }

            this.Camera = null;
            this.entity = entity;
            this.CameraDistance = new Vector3(0f, -5f, -15f);
            this.TargetPosition = Vector3.Zero;
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Resolves the dependencies needed for this instance to work.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">If the transform was not set.</exception>
        protected override void ResolveDependencies()
        {
            base.ResolveDependencies();

            if ((this.targetTransform = this.entity.FindComponent<Transform3D>()) == null)
            {
                throw new InvalidOperationException("Transform not found.");
            }
        }

        /// <summary>
        /// Performs further custom initialization for this instance.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();

            this.cameraPosition = this.Camera.Position;
        }

        /// <summary>
        /// Updates the camera position.
        /// </summary>
        /// <param name="gameTime">The elapsed game time.</param>
        protected override void Update(TimeSpan gameTime)
        {
            this.TargetPosition = this.targetTransform.Position;

            Matrix trasnform = this.targetTransform.WorldTransform;
            Vector3.TransformNormal(ref this.cameraDistance, ref trasnform, out this.calculatedOffsetVector);

            // Manual inline: pivotPlayer = PlayerPosition - calculatedOffsetVector;
            this.pivotPlayer.X = this.TargetPosition.X - this.calculatedOffsetVector.X;
            this.pivotPlayer.Y = this.TargetPosition.Y - this.calculatedOffsetVector.Y;
            this.pivotPlayer.Z = this.TargetPosition.Z - this.calculatedOffsetVector.Z;

            // Manual inline: cameraPosition -= (camera.Position - pivotPlayer) / PivotDelay;
            this.cameraPosition.X = this.Camera.Position.X - ((this.Camera.Position.X - this.pivotPlayer.X) / this.PivotDelay);
            this.cameraPosition.Y = this.Camera.Position.Y - ((this.Camera.Position.Y - this.pivotPlayer.Y) / this.PivotDelay);
            this.cameraPosition.Z = this.Camera.Position.Z - ((this.Camera.Position.Z - this.pivotPlayer.Z) / this.PivotDelay);

            // Manual inline: camera.Position = cameraPosition;
            this.Camera.Position = this.cameraPosition;

            // Manual inline: camera.LookAt = PlayerPosition;
            this.Camera.LookAt = this.TargetPosition;
        }
        #endregion
    }
}
