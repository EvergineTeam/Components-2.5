#region File Description
//-----------------------------------------------------------------------------
// FixedCamera
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
    /// FixedCamera decorate class
    /// </summary>
    public class FixedCamera : BaseDecorator
    {
        #region Properties

        /// <summary>
        /// Gets or sets the filed of view.
        /// </summary>
        /// <value>
        /// The filed of view.
        /// </value>
        public float FieldOfView
        {
            get
            {
                return this.entity.FindComponent<Camera>().FieldOfView;
            }

            set
            {
                this.entity.FindComponent<Camera>().FieldOfView = value;
            }
        }

        /// <summary>
        /// Gets or sets the aspect ratio.
        /// </summary>
        /// <value>
        /// The aspect ratio.
        /// </value>
        public float AspectRatio
        {
            get
            {
                return this.entity.FindComponent<Camera>().AspectRatio;
            }

            set
            {
                this.entity.FindComponent<Camera>().AspectRatio = value;
            }
        }

        /// <summary>
        /// Gets or sets the far plane.
        /// </summary>
        /// <value>
        /// The far plane.
        /// </value>
        public float FarPlane
        {
            get
            {
                return this.entity.FindComponent<Camera>().FarPlane;
            }

            set
            {
                this.entity.FindComponent<Camera>().FarPlane = value;
            }
        }

        /// <summary>
        /// Gets or sets the near plane.
        /// </summary>
        /// <value>
        /// The near plane.
        /// </value>
        public float NearPlane
        {
            get
            {
                return this.entity.FindComponent<Camera>().NearPlane;
            }

            set
            {
                this.entity.FindComponent<Camera>().NearPlane = value;
            }
        }

        #endregion

        #region Initialize

         /// <summary>
        /// Initializes a new instance of the <see cref="FixedCamera" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="position">The position.</param>
        /// <param name="lookAt">The look at.</param>
        public FixedCamera(string name, Vector3 position, Vector3 lookAt)
        {
            this.entity = new Entity(name)
                        .AddComponent(new Camera()
                        {
                            Position = position,
                            LookAt = lookAt,
                        });
        }
      
        #endregion
    }
}
