// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

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
    /// Describe how a controller will behave if the tracking is lost
    /// </summary>
    public enum VRTrackingLostMode
    {
        /// <summary>
        /// Keep the last pose detected
        /// </summary>
        KeepLastPose,

        /// <summary>
        /// Disable the entity
        /// </summary>
        DisableEntity
    }
}
