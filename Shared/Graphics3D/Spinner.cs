// Copyright © 2017 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Using Statements
using System;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Common.Math;
using System.Runtime.Serialization;
using WaveEngine.Common.Attributes;
using WaveEngine.Common.Attributes.Converters;
#endregion

namespace WaveEngine.Components.Graphics3D
{
    /// <summary>
    /// Spins a model around an imaginary axis.
    /// </summary>
    [DataContract(Namespace = "WaveEngine.Components.Graphics3D")]
    public class Spinner : Behavior
    {
        /// <summary>
        /// Total number of instances.
        /// </summary>
        private static int instances;

        /// <summary>
        /// The transform of the model to spin.
        /// </summary>
        [RequiredComponent(false)]
        private Transform3D transform = null;

        /// <summary>
        /// The increase
        /// </summary>
        private Vector3 increase;

        #region Properties

        /// <summary>
        /// Gets or sets the axis increase.
        /// </summary>
        /// <value>
        /// The axis increase.
        /// </value>
        [DataMember]
        [RenderProperty(typeof(Vector3RadianToDegreeConverter))]
        public Vector3 AxisTotalIncreases
        {
            get
            {
                return this.increase;
            }

            set
            {
                this.increase = value;
            }
        }

        /// <summary>
        /// Gets or sets the increase in X.
        /// </summary>
        /// <value>
        /// The increase in X.
        /// </value>
        [DontRenderProperty]
        public float IncreaseX
        {
            get
            {
                return this.increase.X;
            }

            set
            {
                this.increase.X = value;
            }
        }

        /// <summary>
        /// Gets or sets the increase in Y.
        /// </summary>
        /// <value>
        /// The increase in Y.
        /// </value>
        [DontRenderProperty]
        public float IncreaseY
        {
            get
            {
                return this.increase.Y;
            }

            set
            {
                this.increase.Y = value;
            }
        }

        /// <summary>
        /// Gets or sets the increase in Z.
        /// </summary>
        /// <value>
        /// The increase in Z.
        /// </value>
        [DontRenderProperty]
        public float IncreaseZ
        {
            get
            {
                return this.increase.Z;
            }

            set
            {
                this.increase.Z = value;
            }
        }
        #endregion

        #region Initialize

        /// <summary>
        /// Initializes a new instance of the <see cref="Spinner" /> class.
        /// </summary>
        public Spinner()
            : base("Spinner" + instances++)
        {
        }

        /// <summary>
        /// Default values
        /// </summary>
        protected override void DefaultValues()
        {
            base.DefaultValues();
            this.increase = Vector3.Zero;
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// Updates the transform of the model so it rotates along the defined axis.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        protected override void Update(TimeSpan gameTime)
        {
            float totalSeconds = (float)gameTime.TotalSeconds;

            var rotation = Quaternion.CreateFromAxisAngle(Vector3.UnitX, this.increase.X * totalSeconds) *
                           Quaternion.CreateFromAxisAngle(Vector3.UnitY, this.increase.Y * totalSeconds) *
                           Quaternion.CreateFromAxisAngle(Vector3.UnitZ, this.increase.Z * totalSeconds);

            this.transform.LocalOrientation *= rotation;
        }
        #endregion
    }
}
