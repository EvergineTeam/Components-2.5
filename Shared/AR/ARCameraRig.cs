// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

using System;
using System.Runtime.Serialization;
using WaveEngine.Common.Attributes;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Math;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Services;

namespace WaveEngine.Components.AR
{
    /// <summary>
    /// The AR Camera rig
    /// </summary>
    [DataContract]
    public class ARCameraRig : Camera3D
    {
        /// <summary>
        /// The transform
        /// </summary>
        [RequiredComponent]
        protected Transform3D transform;

        /// <summary>
        /// The AR provider
        /// </summary>
        private ARProvider arProvider;

        /// <summary>
        /// Gets or sets a value indicating whether to display a point cloud
        /// showing intermediate results of the scene analysis used to track device position.
        /// <see cref="ARProvider.PointCloud"/> property must be available.
        /// </summary>
        [DataMember]
        [RenderProperty(Tooltip = "Display a point cloud showing intermediate results of the scene analysis used to track device position")]
        public bool ShowPointCloud { get; set; }

        /// <inheritdoc />
        protected override void Initialize()
        {
            base.Initialize();

            if (WaveServices.Platform.IsEditor)
            {
                return;
            }

            this.arProvider = this.Owner.FindComponent<ARProvider>(false);

            if (this.arProvider != null)
            {
                this.arProvider.ActiveCamera = this;
            }
        }

        /// <summary>
        /// Renders the background image.
        /// </summary>
        protected void RenderBackgroundImage()
        {
            if (this.arProvider.BackgroundCameraMesh == null ||
                this.arProvider.BackgroundCameraMaterial == null)
            {
                return;
            }

            this.RenderManager.DrawMesh(
                this.arProvider.BackgroundCameraMesh,
                this.arProvider.BackgroundCameraMaterial,
                this.ViewProjectionInverse,
                false);
        }

        /// <summary>
        /// Renders the point clound
        /// </summary>
        protected void RenderPointCloud()
        {
            var pointCloud = this.arProvider.PointCloud;

            if (pointCloud != null)
            {
                for (int i = 0; i < pointCloud.Length; i++)
                {
                    this.RenderManager.LineBatch3D.DrawPoint(pointCloud[i], 0.01f, Color.Red);
                }
            }
        }

        /// <inheritdoc />
        protected override void RefreshDimensions()
        {
            base.RefreshDimensions();

            if (this.arProvider != null)
            {
                this.aspectRatio = (float)platformService.ScreenWidth / (float)platformService.ScreenHeight;
                this.width = this.viewportWidth = platformService.ScreenWidth;
                this.height = this.viewportHeight = platformService.ScreenHeight;
            }
        }

        /// <inheritdoc />
        protected override void Render(TimeSpan gameTime)
        {
            if (this.arProvider == null ||
               !this.arProvider.IsSupported)
            {
                base.Render(gameTime);
            }
            else
            {
                this.RefreshTransform();
                this.SetCustomProjection(this.arProvider.CameraProjection);
                this.RenderBackgroundImage();

                if (this.ShowPointCloud)
                {
                    this.RenderPointCloud();
                }

                base.Render(gameTime);
            }
        }

        /// <summary>
        /// Updates the camera transform
        /// </summary>
        private void RefreshTransform()
        {
            var transform = this.arProvider.CameraTransform;

            if (transform.HasValue)
            {
                this.transform.LocalPosition = transform.Value.Translation;
                this.transform.LocalOrientation = transform.Value.Orientation;
            }
        }
    }
}
