#region File Description
//-----------------------------------------------------------------------------
// VRCameraRig
//
// Copyright © 2015 Wave Coorporation. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Runtime.Serialization;
using WaveEngine.Common.Attributes;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.VR;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Services;
using WaveEngine.Framework.Sound;
#endregion

namespace WaveEngine.Components.VR
{
    /// <summary>
    /// Oculus Rift manager
    /// </summary>
    [DataContract]
    public class VRCameraRig : Behavior
    {
        /// <summary>
        /// Tracking space name
        /// </summary>
        private static readonly string trackingSpaceName = "TrackingSpace";

        /// <summary>
        /// Tracker anchor
        /// </summary>
        private static readonly string trackerAnchorName = "TrackerAnchor";

        /// <summary>
        /// Eye anchor
        /// </summary>
        private static readonly string eyeAnchorName = "EyeAnchor";

        /// <summary>
        /// Occurs when the eye pose anchors have been set.
        /// </summary>
        public event System.Action<VRCameraRig> UpdatedAnchors;

        /// <summary>
        /// The VR info provider
        /// </summary>
        private VRProvider vrProvider = null;

        /// <summary>
        /// Platform service
        /// </summary>
        private Platform platform;

        /// <summary>
        /// Camera near plane
        /// </summary>
        [DataMember]
        private float nearPlane;

        /// <summary>
        /// Camera far plane
        /// </summary>
        [DataMember]
        private float farPlane;

        /// <summary>
        /// Camera background color
        /// </summary>
        [DataMember]
        private Color backgroundColor;

        /// <summary>
        /// Camera clear flags
        /// </summary>
        [DataMember]
        private ClearFlags clearFlags;

        /// <summary>
        /// The vr mode
        /// </summary>
        [DataMember]
        private VRMode vrMode;

        #region Properties
        /// <summary>
        /// Gets or sets a value indicating whether the the eyes see the same image, which is rendered only by the left camera.
        /// </summary>
        [DataMember]
        public bool Monoscopic
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets th VR rendering mode.
        /// </summary>
        public VRMode VRMode
        {
            get
            {
                return this.vrMode;
            }

            set
            {
                this.vrMode = value;
                if (this.isInitialized)
                {
                    this.RefreshVRMode();
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the the eyes see the same image, which is rendered only by the left camera.
        /// </summary>
        [DataMember]
        public bool Disable
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the left eye camera.
        /// </summary>
        [DontRenderProperty]
        public Camera3D LeftEyeCamera
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the right eye camera.
        /// </summary>
        [DontRenderProperty]
        public Camera3D RightEyeCamera
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the attached camera. This camera is mounted in the Central Eye anchor
        /// </summary>
        [DontRenderProperty]
        public Camera3D AttachedCamera
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the root entity for all anchors in tracking space.
        /// </summary>
        [DontRenderProperty]
        public Entity TrackingSpace
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the root entity that always coincides with the pose of the left eye.
        /// </summary>
        [DontRenderProperty]
        public Entity LeftEyeAnchor
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the root entity that always coincides with the pose of the right eye.
        /// </summary>
        [DontRenderProperty]
        public Entity RightEyeAnchor
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the root entity that always coincides with average of the left and right eye poses.
        /// </summary>
        [DontRenderProperty]
        public Entity CenterEyeAnchor
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the root entity that always coincides with the pose of the tracker.
        /// </summary>
        [DontRenderProperty]
        public Entity TrackerAnchor
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the root transform for all anchors in tracking space.
        /// </summary>
        [DontRenderProperty]
        public Transform3D TrackingSpaceTransform
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the root transform that always coincides with the pose of the left eye.
        /// </summary>
        [DontRenderProperty]
        public Transform3D LeftEyeAnchorTransform
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the root transform that always coincides with the pose of the right eye.
        /// </summary>
        [DontRenderProperty]
        public Transform3D RightEyeAnchorTransform
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the root transform that always coincides with average of the left and right eye poses.
        /// </summary>
        [DontRenderProperty]
        public Transform3D CenterEyeAnchorTransform
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the root transform that always coincides with the pose of the tracker.
        /// </summary>
        [DontRenderProperty]
        public Transform3D TrackerAnchorTransform
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the Near plane of the camera
        /// </summary>
        public float NearPlane
        {
            get
            {
                return this.nearPlane;
            }

            set
            {
                this.nearPlane = value;
                this.RefreshCameraProperties();
            }
        }

        /// <summary>
        /// Gets or sets the Far plane of the camera
        /// </summary>
        public float FarPlane
        {
            get
            {
                return this.farPlane;
            }

            set
            {
                this.farPlane = value;
                this.RefreshCameraProperties();
            }
        }

        /// <summary>
        /// Gets or sets the background color of the camera
        /// </summary>
        public Color BackgroundColor
        {
            get
            {
                return this.backgroundColor;
            }

            set
            {
                this.backgroundColor = value;
                this.RefreshCameraProperties();
            }
        }

        /// <summary>
        /// Gets or sets the clear flags of the camera
        /// </summary>
        public ClearFlags ClearFlags
        {
            get
            {
                return this.clearFlags;
            }

            set
            {
                this.clearFlags = value;
                this.RefreshCameraProperties();
            }
        }
        #endregion

        #region Initialize
        /// <summary>
        /// Default values
        /// </summary>
        protected override void DefaultValues()
        {
            base.DefaultValues();
            this.Monoscopic = false;
            this.platform = WaveServices.Platform;
            this.nearPlane = 0.05f;
            this.farPlane = 500f;
            this.clearFlags = ClearFlags.All;
            this.backgroundColor = Color.Transparent;
            this.VRMode = VRMode.HmdMode;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Initializes the OVR entities
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
            this.InstantiateOVRHierarchy();
        }

        /// <summary>
        /// Update the behavior
        /// </summary>
        /// <param name="gameTime">The current game time.</param>
        protected override void Update(TimeSpan gameTime)
        {
            if (this.vrProvider == null)
            {
                this.vrProvider = this.Owner.FindComponent<VRProvider>(false);
            }

            if (this.vrProvider == null || !this.vrProvider.IsConnected)
            {
                return;
            }

            var eyePoses = this.vrProvider.EyePoses;
            var trackerCameraPose = this.vrProvider.TrackerCameraPose;

            // Left eye camera
            this.UpdateCamera(this.LeftEyeCamera, (int)VREyeType.LeftEye);
            this.UpdateCamera(this.RightEyeCamera, (int)VREyeType.RightEye);

            // Camera tracker
            this.TrackerAnchorTransform.LocalPosition = trackerCameraPose.Position;
            this.TrackerAnchorTransform.LocalOrientation = trackerCameraPose.Orientation;

            // Center
            this.CenterEyeAnchorTransform.LocalPosition = eyePoses[(int)VREyeType.CenterEye].Position;
            this.CenterEyeAnchorTransform.LocalOrientation = eyePoses[(int)VREyeType.CenterEye].Orientation;

            if (this.LeftEyeCamera.RenderTarget == this.RightEyeCamera.RenderTarget)
            {
                this.RightEyeCamera.ClearFlags = ClearFlags.DepthAndStencil;
            }
            else
            {
                this.RightEyeCamera.ClearFlags = ClearFlags.All;
            }

            if (this.UpdatedAnchors != null)
            {
                this.UpdatedAnchors(this);
            }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Instantiate the ovr hierarchy
        /// </summary>
        private void InstantiateOVRHierarchy()
        {
            if (this.TrackingSpace == null)
            {
                this.TrackingSpace = this.ConfigureRootAnchor(trackingSpaceName);
                this.TrackingSpaceTransform = this.TrackingSpace.FindComponent<Transform3D>();
            }

            if (this.LeftEyeAnchor == null)
            {
                this.LeftEyeAnchor = this.ConfigureEyeAnchor(this.TrackingSpace, VREyeType.LeftEye);
                this.LeftEyeAnchorTransform = this.LeftEyeAnchor.FindComponent<Transform3D>();
            }

            if (this.CenterEyeAnchor == null)
            {
                this.CenterEyeAnchor = this.ConfigureEyeAnchor(this.TrackingSpace, VREyeType.CenterEye);
                this.CenterEyeAnchorTransform = this.CenterEyeAnchor.FindComponent<Transform3D>();
            }

            if (this.RightEyeAnchor == null)
            {
                this.RightEyeAnchor = this.ConfigureEyeAnchor(this.TrackingSpace, VREyeType.RightEye);
                this.RightEyeAnchorTransform = this.RightEyeAnchor.FindComponent<Transform3D>();
            }

            if (this.TrackerAnchor == null)
            {
                this.TrackerAnchor = this.ConfigureTrackerAnchor(this.TrackingSpace);
                this.TrackerAnchorTransform = this.TrackerAnchor.FindComponent<Transform3D>();
            }

            bool needsCamera = this.LeftEyeCamera == null || this.RightEyeCamera == null || this.AttachedCamera == null;

            if (needsCamera)
            {
                this.LeftEyeCamera = this.LeftEyeAnchor.FindComponent<Camera3D>();
                if (this.LeftEyeCamera == null)
                {
                    this.LeftEyeCamera = new Camera3D() { CameraOrder = 0 };
                    this.LeftEyeAnchor.AddComponent(this.LeftEyeCamera);
                }

                this.RightEyeCamera = this.RightEyeAnchor.FindComponent<Camera3D>();
                if (this.RightEyeCamera == null)
                {
                    this.RightEyeCamera = new Camera3D() { CameraOrder = 0.1f };
                    this.RightEyeAnchor.AddComponent(this.RightEyeCamera);
                }

                this.AttachedCamera = this.CenterEyeAnchor.FindComponent<Camera3D>();
                if (this.AttachedCamera == null)
                {
                    this.AttachedCamera = new Camera3D();
                    this.CenterEyeAnchor.AddComponent(this.AttachedCamera);
                }

                this.RefreshCameraProperties();
            }

            this.RefreshVRMode();
        }

        /// <summary>
        /// Refresh the VRMode
        /// </summary>
        private void RefreshVRMode()
        {
            this.AttachedCamera.IsActive = this.VRMode.HasFlag(VRMode.AttachedMode);
            this.LeftEyeCamera.IsActive = this.RightEyeCamera.IsActive = this.VRMode.HasFlag(VRMode.HmdMode);
        }

        /// <summary>
        /// Refresh camera draw properties (Clear flags, background, etc..)
        /// </summary>
        private void RefreshCameraProperties()
        {
            if (this.LeftEyeCamera == null || this.RightEyeCamera == null)
            {
                return;
            }

            this.AttachedCamera.BackgroundColor = this.LeftEyeCamera.BackgroundColor = this.RightEyeCamera.BackgroundColor = this.backgroundColor;
            this.AttachedCamera.NearPlane = this.LeftEyeCamera.NearPlane = this.RightEyeCamera.NearPlane = this.nearPlane;
            this.AttachedCamera.FarPlane = this.LeftEyeCamera.FarPlane = this.RightEyeCamera.FarPlane = this.farPlane;
            this.AttachedCamera.ClearFlags = this.LeftEyeCamera.ClearFlags = this.RightEyeCamera.ClearFlags = this.clearFlags;
        }

        /// <summary>
        /// Update camera using vr provider data
        /// </summary>
        /// <param name="camera">The camera</param>
        /// <param name="eyeIndex">The eye index</param>
        private void UpdateCamera(Camera3D camera, int eyeIndex)
        {
            var eyeTexture = this.vrProvider.EyeTextures[eyeIndex];
            var eyePose = this.vrProvider.EyePoses[eyeIndex];
            var rt = eyeTexture.RenderTarget;

            camera.RenderTarget = rt;
            camera.Viewport = eyeTexture.Viewport;

            camera.Transform.LocalPosition = this.Monoscopic ? this.CenterEyeAnchorTransform.LocalPosition : eyePose.Position;
            camera.Transform.LocalOrientation = this.Monoscopic ? this.CenterEyeAnchorTransform.LocalOrientation : eyePose.Orientation;
        }

        /// <summary>
        /// Create the root anchor
        /// </summary>
        /// <param name="name">The name</param>
        /// <returns>The anchor entity</returns>
        private Entity ConfigureRootAnchor(string name)
        {
            Entity root = this.Owner.FindChild(name);

            if (root == null)
            {
                root = new Entity(name)
                    .AddComponent(new Transform3D());

                this.Owner.AddChild(root);
            }

            return root;
        }

        /// <summary>
        /// Creates an eye anchor
        /// </summary>
        /// <param name="root">The root entity</param>
        /// <param name="eye">The eye type</param>
        /// <returns>The eye anchor</returns>
        private Entity ConfigureEyeAnchor(Entity root, VREyeType eye)
        {
            string eyeName = (eye == VREyeType.CenterEye) ? "Center" : (eye == VREyeType.LeftEye) ? "Left" : "Right";
            string name = eyeName + eyeAnchorName;

            Entity anchor = root.FindChild(name);

            if (anchor == null)
            {
                anchor = new Entity(name)
                .AddComponent(new Transform3D());

                if (eye == VREyeType.CenterEye)
                {
                    anchor.AddComponent(new SoundListener3D());
                }

                root.AddChild(anchor);
            }

            return anchor;
        }

        /// <summary>
        /// Creates the OVR position tracker position
        /// </summary>
        /// <param name="root">The root entity</param>
        /// <returns>The tracker anchor</returns>
        private Entity ConfigureTrackerAnchor(Entity root)
        {
            Entity anchor = root.FindChild(trackerAnchorName);

            if (anchor == null)
            {
                anchor = new Entity(trackerAnchorName)
                .AddComponent(new Transform3D());

                root.AddChild(anchor);
            }

            return anchor;
        }
        #endregion
    }
}
