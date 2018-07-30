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
    /// FreeCamera2D decorate class
    /// </summary>
    [DataContract(Namespace = "WaveEngine.Components.Cameras")]
    public class FreeCamera2D : FixedCamera2D
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
                return this.entity.FindComponent<FreeCamera2DBehavior>().RotationSpeed;
            }

            set
            {
                this.entity.FindComponent<FreeCamera2DBehavior>().RotationSpeed = value;
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
                return this.entity.FindComponent<FreeCamera2DBehavior>().Speed;
            }

            set
            {
                this.entity.FindComponent<FreeCamera2DBehavior>().Speed = value;
            }
        }

        #endregion

        #region Initialize

        /// <summary>
        /// Initializes a new instance of the <see cref="FreeCamera2D" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public FreeCamera2D(string name)
            : base(name)
        {
            this.entity.AddComponent(new FreeCamera2DBehavior());
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FreeCamera2D" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="position">The position.</param>
        public FreeCamera2D(string name, Vector2 position)
            : base(name, position)
        {
            this.entity.AddComponent(new FreeCamera2DBehavior());
        }
        #endregion
    }
}
