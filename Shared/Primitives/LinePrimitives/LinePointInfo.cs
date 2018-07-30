// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Using Statements
using System.Runtime.Serialization;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Math;
#endregion

namespace WaveEngine.Components.Primitives
{
    /// <summary>
    /// A class that contains the information of each point of a <see cref="LineMeshBase"/>.
    /// </summary>
    [DataContract]
    public class LinePointInfo
    {
        /// <summary>
        /// The position of the point.
        /// </summary>
        [DataMember]
        public Vector3 Position;

        /// <summary>
        /// The thickness of the line at this point.
        /// </summary>
        [DataMember]
        public float Thickness;

        /// <summary>
        /// The color of the line at this point.
        /// </summary>
        [DataMember]
        public Color Color;

        /// <inheritdoc/>
        public override string ToString()
        {
            return "Position:" + this.Position.ToString() + " Thickness:" + this.Thickness + " Color:" + this.Color.ToHexColorCode();
        }
    }
}
