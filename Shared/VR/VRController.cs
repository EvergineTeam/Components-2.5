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
    /// VR Controller
    /// </summary>
    [DataContract]
    public class VRController : Component
    {
        #region Properties

        /// <summary>
        /// Gets the associated transform
        /// </summary>
        [RequiredComponent]
        public Transform3D Transform { get; private set; }

        /// <summary>
        /// Gets or sets how this controller behave after tracking lost
        /// </summary>
        [DataMember]
        public VRTrackingLostMode TrackingLostMode
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the controller role that we want to track
        /// </summary>
        [RenderProperty(
            Tag = 1,
                        CustomPropertyName = "Controller Role",
                        Tooltip = "The controller role that we want to track")]
        [DataMember]
        public VRControllerRole Role
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the controller index in case that we want to track a controller by index
        /// </summary>
        [RenderProperty(
            AttatchToTag = 1,
                        AttachToValue = VRControllerRole.Undefined,
                        CustomPropertyName = "Controller Index",
                        Tooltip = "Controller index in case that we want to track a controller by index")]
        [DataMember]
        public int ControllerIndex
        {
            get; set;
        }

        /// <summary>
        /// Gets the tracker camera pose
        /// </summary>
        [DontRenderProperty]
        public VRGenericControllerState State
        {
            get; internal set;
        }

        /// <summary>
        /// Gets a value indicating whether the vr provider is connected
        /// </summary>
        [DontRenderProperty]
        public bool IsConnected
        {
            get
            {
                return this.State.IsConnected;
            }
        }
        #endregion

        #region Initialization

        /// <summary>
        /// Sets the default values
        /// </summary>
        protected override void DefaultValues()
        {
            base.DefaultValues();
            this.TrackingLostMode = VRTrackingLostMode.KeepLastPose;
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// Update the state of the controller
        /// </summary>
        /// <param name="newState">The new state</param>
        internal void UpdateState(VRGenericControllerState newState)
        {
            this.State = newState;

            if (this.State.IsConnected)
            {
                this.Transform.LocalPosition = this.State.Pose.Position;
                this.Transform.LocalOrientation = this.State.Pose.Orientation;
            }

            if (this.TrackingLostMode == VRTrackingLostMode.DisableEntity
                && (this.Owner.Enabled != this.State.IsConnected))
            {
                this.Owner.Enabled = this.State.IsConnected;
            }
        }
        #endregion

        #region Private Methods
        #endregion
    }
}
