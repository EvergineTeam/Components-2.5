#region File Description
//-----------------------------------------------------------------------------
// VRProvider
//
// Copyright © 2017 Wave Coorporation. All rights reserved.
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
    /// VR HMD provider
    /// </summary>
    [DataContract]
    public abstract class VRProvider : Behavior
    {
        /// <summary>
        /// Camera rig
        /// </summary>
        [RequiredComponent]
        protected VRCameraRig cameraRig;

        #region Properties
        /// <summary>
        /// Gets the eye textures information.
        /// </summary>
        public abstract VREyeTexture[] EyeTextures
        {
            get;
        }

        /// <summary>
        /// Gets the eye poses
        /// </summary>
        public abstract VREyePose[] EyePoses
        {
            get;
        }

        /// <summary>
        /// Gets the tracker camera pose
        /// </summary>
        public abstract VREyePose TrackerCameraPose
        {
            get;
        }

        /// <summary>
        /// Gets the left controller pose
        /// </summary>
        public abstract VREyePose LeftControllerPose
        {
            get;
        }

        /// <summary>
        /// Gets the right controller pose
        /// </summary>
        public abstract VREyePose RightControllerPose
        {
            get;
        }

        /// <summary>
        /// Gets a value indicating whether the vr provider is connected
        /// </summary>
        public abstract bool IsConnected
        {
            get;
        }
        #endregion

        #region Public Methods
        #endregion

        #region Private Methods
        /// <summary>
        /// Update method
        /// </summary>
        /// <param name="gameTime">The game time</param>
        protected override void Update(TimeSpan gameTime)
        {
            if (this.IsConnected)
            {
                var leftEyeTexture = this.EyeTextures[0];
                var rightEyeTexture = this.EyeTextures[1];

                if (this.cameraRig.LeftEyeCamera != null)
                {
                    leftEyeTexture.NearPlane = this.cameraRig.LeftEyeCamera.NearPlane;
                    leftEyeTexture.FarPlane = this.cameraRig.LeftEyeCamera.FarPlane;
                }

                if (this.cameraRig.RightEyeCamera != null)
                {
                    rightEyeTexture.NearPlane = this.cameraRig.RightEyeCamera.NearPlane;
                    rightEyeTexture.FarPlane = this.cameraRig.RightEyeCamera.FarPlane;
                }
            }
        }
        #endregion
    }
}
