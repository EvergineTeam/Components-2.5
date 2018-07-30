// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

using System.Runtime.Serialization;
using WaveEngine.Common.Attributes;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Math;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;

namespace WaveEngine.Components.AR
{
    /// <summary>
    /// The Augmented Reality provider class
    /// </summary>
    [DataContract]
    public abstract class ARProvider : Component
    {
        /// <summary>
        /// Gets the material for the background camera
        /// </summary>
        public abstract Material BackgroundCameraMaterial { get; }

        /// <summary>
        /// Gets the mesh for the background camera
        /// </summary>
        public abstract Mesh BackgroundCameraMesh { get; }

        /// <summary>
        /// Gets the camera projection matrix
        /// </summary>
        public abstract Matrix CameraProjection { get; }

        /// <summary>
        /// Gets the camera transform matrix
        /// </summary>
        public abstract Matrix? CameraTransform { get; }

        /// <summary>
        /// Gets a value indicating whether the AR provider is supported
        /// </summary>
        public abstract bool IsSupported { get; }

        /// <summary>
        /// Gets the tracking state
        /// </summary>
        public abstract ARTrackingState TrackingState { get; }

        /// <summary>
        /// Gets an array with the current intermediate results of the scene analysis that is used to perform world tracking
        /// </summary>
        public abstract Vector3[] PointCloud { get; }

        /// <summary>
        /// Gets or sets the active camera
        /// </summary>
        [DontRenderProperty]
        public abstract Camera3D ActiveCamera { get;  set; }
    }
}
