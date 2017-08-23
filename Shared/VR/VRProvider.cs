// Copyright © 2017 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Using Statements
using System;
using System.Runtime.Serialization;
using WaveEngine.Common.Attributes;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Input;
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
        /// Gets the eye properties
        /// </summary>
        [DontRenderProperty]
        public abstract VREye[] EyesProperties
        {
            get;
        }

        /// <summary>
        /// Gets the tracker camera pose
        /// </summary>
        [DontRenderProperty]
        public abstract VRPose TrackerCameraPose
        {
            get;
        }

        /// <summary>
        /// Gets the left controller index (-1 in other case)
        /// </summary>
        [DontRenderProperty]
        public abstract int LeftControllerIndex
        {
            get;
        }

        /// <summary>
        /// Gets the right controller index (-1 in other case)
        /// </summary>
        [DontRenderProperty]
        public abstract int RightControllerIndex
        {
            get;
        }

        /// <summary>
        /// Gets the left controller pose
        /// </summary>
        [DontRenderProperty]
        public abstract VRGenericControllerState[] ControllerStates
        {
            get;
        }

        /// <summary>
        /// Gets the state of the generic controller.
        /// </summary>
        /// <value>
        /// The state of the generic controller.
        /// </value>
        [DontRenderProperty]
        public abstract GamePadState GamepadState
        {
            get;
        }

        /// <summary>
        /// Gets a value indicating whether the vr provider is connected
        /// </summary>
        [DontRenderProperty]
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
                var leftEyeTexture = this.EyesProperties[(int)VREyeType.LeftEye].Texture;
                var rightEyeTexture = this.EyesProperties[(int)VREyeType.RightEye].Texture;

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
