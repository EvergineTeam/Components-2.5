// Copyright © 2017 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Using Statements
using System;
using System.Runtime.Serialization;
using WaveEngine.Common.Attributes;
using WaveEngine.Common.Attributes.Converters;
using WaveEngine.Common.Math;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
#endregion

namespace WaveEngine.Components.Toolkit
{
    /// <summary>
    /// Behavior for an entity that follows the camera
    /// </summary>
    [DataContract(Namespace = "WaveEngine.Components.Toolkit")]
    public class Tagalong : Behavior
    {
        /// <summary>
        /// The transform component
        /// </summary>
        [RequiredComponent]
        protected Transform3D transform = null;

        /// <summary>
        /// The desired position
        /// </summary>
        private Vector3 desiredPosition;

        /// <summary>
        /// The desired distance
        /// </summary>
        private float desiredDistance;

        /// <summary>
        /// Gets or sets the max angle of the panel
        /// </summary>
        [DataMember]
        [RenderProperty(typeof(FloatRadianToDegreeConverter))]
        public float MaxAngle { get; set; }

        /// <summary>
        /// Gets or sets the minimum distance
        /// </summary>
        [DataMember]
        public float MinDistance { get; set; }

        /// <summary>
        /// Gets or sets the maximum distance
        /// </summary>
        [DataMember]
        public float MaxDistance { get; set; }

        /// <summary>
        /// Gets or sets the smooth factor for the position
        /// </summary>
        [DataMember]
        public float SmoothPositionFactor { get; set; }

        /// <summary>
        /// Gets or sets the smooth factor for the distance
        /// </summary>
        [DataMember]
        public float SmoothDistanceFactor { get; set; }

        /// <summary>
        /// The default values of the behavior
        /// </summary>
        protected override void DefaultValues()
        {
            base.DefaultValues();
            this.MaxAngle = MathHelper.ToRadians(13);

            this.MinDistance = 0.4f;
            this.MaxDistance = 1;

            this.SmoothPositionFactor = 0.1f;
            this.SmoothDistanceFactor = 0.5f;
        }

        /// <summary>
        /// Initializes the behavior
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();

            this.desiredPosition = this.transform.Position;
        }

        /// <summary>
        /// Updates the behavior
        /// </summary>
        /// <param name="gameTime">The ellapsed game time.</param>
        protected override void Update(TimeSpan gameTime)
        {
            // Gets the camera properties
            Transform3D cameraTransform = this.RenderManager.ActiveCamera3D.Transform;
            Vector3 cameraPosition = cameraTransform.Position;
            Vector3 cameraForward = cameraTransform.WorldTransform.Forward;

            // Gets the panel properties
            Vector3 panelPosition = this.transform.Position;
            Vector3 panelForward = this.transform.WorldTransform.Forward;

            Vector3 panelDirection = panelPosition - cameraPosition;
            float panelDistance = panelDirection.Length();
            panelDirection.Normalize();

            // Compute angle to the camera
            float panelAngle = Vector3.Angle(cameraForward, panelDirection);
            if (panelAngle > this.MaxAngle)
            {
                panelDirection = Vector3.Lerp(cameraForward, panelDirection, this.MaxAngle / panelAngle);
                panelDirection.Normalize();
            }

            // Compute distance
            this.desiredDistance = MathHelper.Lerp(panelDistance, this.MaxDistance, this.SmoothDistanceFactor);
            this.desiredPosition = cameraPosition + (panelDirection * this.desiredDistance);

            // Sets final values
            this.transform.Position = Vector3.Lerp(this.transform.Position, this.desiredPosition, this.SmoothPositionFactor);
            this.transform.LookAt(panelPosition + panelDirection);
        }
    }
}
