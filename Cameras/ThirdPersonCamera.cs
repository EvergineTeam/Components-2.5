#region File Description
//-----------------------------------------------------------------------------
// ThirdPersonCamera
//
// Copyright © 2014 Wave Corporation
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using WaveEngine.Common.Math;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
#endregion

namespace WaveEngine.Components.Cameras
{
    /// <summary>
    /// Third person camera decorate class
    /// </summary>
    public class ThirdPersonCamera : FixedCamera
    {
        #region Properties

        /// <summary>
        /// Gets or sets the camera distance.
        /// </summary>
        /// <value>
        /// The camera distance.
        /// </value>
        public Vector3 CameraDistance
        {
            get
            {
                return this.entity.FindComponent<ThirdCameraBehavior>().CameraDistance;
            }

            set
            {
                this.entity.FindComponent<ThirdCameraBehavior>().CameraDistance = value;
            }
        }

        /// <summary>
        /// Gets or sets the target position.
        /// </summary>
        /// <value>
        /// The target position.
        /// </value>
        public Vector3 TargetPosition
        {
            get
            {
                return this.entity.FindComponent<ThirdCameraBehavior>().TargetPosition;
            }

            set
            {
                this.entity.FindComponent<ThirdCameraBehavior>().TargetPosition = value;
            }
        }

        #endregion

        #region Initialize

        /// <summary>
        /// Initializes a new instance of the <see cref="ThirdPersonCamera" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="entityToFollow">The entity to follow.</param>
        public ThirdPersonCamera(string name, Entity entityToFollow)
            : base(name, Vector3.Zero, Vector3.Zero)
        {
            this.entity.AddComponent(new ThirdCameraBehavior(entityToFollow));
        }

        #endregion
    }
}
