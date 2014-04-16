#region File Description
//-----------------------------------------------------------------------------
// ViewCamera
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
    /// ViewCamera decorate class
    /// </summary>
    public class ViewCamera : FixedCamera
    {
        #region Properties

        /// <summary>
        /// Gets or sets the rotation speed.
        /// </summary>
        /// <value>
        /// The speed.
        /// </value>
        public float RotationSpeed
        {
            get
            {
                return this.entity.FindComponent<ViewCameraBehavior>().RotationSpeed;
            }

            set
            {
                this.entity.FindComponent<ViewCameraBehavior>().RotationSpeed = value;
            }
        }
        #endregion

        #region Initialize

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewCamera" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="position">The position.</param>
        /// <param name="lookAt">The look at.</param>
        public ViewCamera(string name, Vector3 position, Vector3 lookAt)
            : base(name, position, lookAt)
        {
            this.entity.AddComponent(new ViewCameraBehavior());
        }
        
        #endregion
    }
}
