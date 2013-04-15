#region File Description
//-----------------------------------------------------------------------------
// FreeCamera
//
// Copyright © 2010 - 2013 Wave Coorporation. All rights reserved.
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
    /// FreeCamera decorate class
    /// </summary>
    public class FreeCamera : FixedCamera
    {
        #region Properties

        /// <summary>
        /// Gets or sets the rotation speed.
        /// </summary>
        /// <value>
        /// The rotation speed.
        /// </value>
        public float RotationSpeed
        {
            get
            {
                return this.entity.FindComponent<FreeCameraBehavior>().RotationSpeed;
            }

            set
            {
                this.entity.FindComponent<FreeCameraBehavior>().RotationSpeed = value;
            }
        }

        /// <summary>
        /// Gets or sets the speed.
        /// </summary>
        /// <value>
        /// The speed.
        /// </value>
        public float Speed
        {
            get
            {
                return this.entity.FindComponent<FreeCameraBehavior>().Speed;
            }

            set
            {
                this.entity.FindComponent<FreeCameraBehavior>().Speed = value;
            }
        }

        #endregion

        #region Initialize

        /// <summary>
        /// Initializes a new instance of the <see cref="FreeCamera" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="position">The position.</param>
        /// <param name="lookAt">The look at.</param>
        public FreeCamera(string name, Vector3 position, Vector3 lookAt)
            : base(name, position, lookAt)
        {
            this.entity.AddComponent(new FreeCameraBehavior());
        }

        #endregion
    }
}
