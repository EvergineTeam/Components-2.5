// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Using Statements
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using WaveEngine.Common.Math;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
#endregion

namespace WaveEngine.Components.Cameras
{
    /// <summary>
    /// FreeCamera decorate class
    /// </summary>
    [DataContract(Namespace = "WaveEngine.Components.Cameras")]
    public class FreeCamera3D : FixedCamera3D
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
                return this.entity.FindComponent<FreeCamera3DBehavior>().RotationSpeed;
            }

            set
            {
                this.entity.FindComponent<FreeCamera3DBehavior>().RotationSpeed = value;
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
                return this.entity.FindComponent<FreeCamera3DBehavior>().Speed;
            }

            set
            {
                this.entity.FindComponent<FreeCamera3DBehavior>().Speed = value;
            }
        }

        #endregion

        #region Initialize

        /// <summary>
        /// Initializes a new instance of the <see cref="FreeCamera3D" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="position">The position.</param>
        /// <param name="lookAt">The look at.</param>
        public FreeCamera3D(string name, Vector3 position, Vector3 lookAt)
            : base(name, position, lookAt)
        {
            this.entity.AddComponent(new FreeCamera3DBehavior());
        }

        #endregion
    }
}
