#region File Description
//-----------------------------------------------------------------------------
// Spinner
//
// Copyright © 2011 Weekend Game Studio. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Common.Math;

#endregion

namespace WaveEngine.Components.Graphics3D
{
    /// <summary>
    /// Spins a model around an imaginary axis.
    /// </summary>
    public class Spinner : Behavior
    {
        /// <summary>
        /// Total number of instances.
        /// </summary>
        private static int instances;

        /// <summary>
        /// The transform of the model to spin.
        /// </summary>
        [RequiredComponent]
        public Transform3D Transform;

        /// <summary>
        /// The angle
        /// </summary>
        private Vector3 angle;

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
        public Vector3 AxisTotalIncreases
        {
            get
            {
                return this.increase;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentException("Increment cannot be null.");
                }

                this.increase = value;
            }
        }

        /// <summary>
        /// Gets or sets the increase in X.
        /// </summary>
        /// <value>
        /// The increase in X.
        /// </value>
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
            this.increase = Vector3.Zero;
            this.Transform = null;
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

            // angle += increase;
            this.angle.X = this.angle.X + (this.increase.X * totalSeconds);
            this.angle.Y = this.angle.Y + (this.increase.Y * totalSeconds);
            this.angle.Z = this.angle.Z + (this.increase.Z * totalSeconds);

            // transform.Rotation = angle;
            this.Transform.Rotation.X = this.angle.X;
            this.Transform.Rotation.Y = this.angle.Y;
            this.Transform.Rotation.Z = this.angle.Z;
        }
        #endregion
    }
}
