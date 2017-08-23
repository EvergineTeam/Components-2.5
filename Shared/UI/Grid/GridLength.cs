// Copyright © 2017 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Using Statements
using System;
using System.Runtime.Serialization;
#endregion

namespace WaveEngine.Components.UI
{
    /// <summary>
    /// This class represent the grid size.
    /// </summary>
    public struct GridLength : IEquatable<GridLength>
    {
        /// <summary>
        /// The value
        /// </summary>
        private readonly float value;

        /// <summary>
        /// The type
        /// </summary>
        private GridUnitType type;

        #region Properties

        /// <summary>
        /// Gets a value indicating whether this instance is auto.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is auto; otherwise, <c>false</c>.
        /// </value>
        public bool IsAuto
        {
            get
            {
                return this.type == GridUnitType.Auto;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is pixel.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is pixel; otherwise, <c>false</c>.
        /// </value>
        public bool IsPixel
        {
            get
            {
                return this.type == GridUnitType.Pixel;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is proportional.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is proportional; otherwise, <c>false</c>.
        /// </value>
        public bool IsProportional
        {
            get
            {
                return this.type == GridUnitType.Proportional;
            }
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public float Value
        {
            get
            {
                if (this.type != GridUnitType.Auto)
                {
                    return this.value;
                }

                return 1f;
            }
        }
        #endregion

        #region Initialize

        /// <summary>
        /// Initializes a new instance of the <see cref="GridLength" /> struct.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="type">The type.</param>
        /// <exception cref="System.ArgumentException">Invalid Parameter No NaN</exception>
        public GridLength(float value, GridUnitType type)
        {
            if (float.IsNaN(value))
            {
                throw new ArgumentException("InvalidParameterNoNaN");
            }

            if (double.IsInfinity(value))
            {
                throw new ArgumentException("InvalidParameterNoInfinity");
            }

            this.value = (type == GridUnitType.Auto) ? 0f : value;
            this.type = type;
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// Iqual operator of GridLength struct.
        /// </summary>
        /// <param name="value1">The value1.</param>
        /// <param name="value2">The value2.</param>
        /// <returns>Operation result.</returns>
        public static bool operator ==(GridLength value1, GridLength value2)
        {
            return (value1.type == value2.type) && (value1.value == value2.value);
        }

        /// <summary>
        /// != operator of GridLength struct.
        /// </summary>
        /// <param name="value1">The value1.</param>
        /// <param name="value2">The value2.</param>
        /// <returns>Operation result.</returns>
        public static bool operator !=(GridLength value1, GridLength value2)
        {
            return !(value1 == value2);
        }

        /// <summary>
        /// Determines whether the specified <see cref="object" /> is equal to this instance.
        /// </summary>
        /// <param name="o">The <see cref="object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object o)
        {
            return this == (GridLength)o;
        }

        /// <summary>
        /// Equalses the specified other.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns>Operation result.</returns>
        public bool Equals(GridLength other)
        {
            return this == other;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        public override int GetHashCode()
        {
            return (int)this.value + (int)this.type;
        }
        #endregion
    }
}
