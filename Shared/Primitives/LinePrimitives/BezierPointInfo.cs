// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

using System.Runtime.Serialization;
using WaveEngine.Common.Attributes;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Math;

namespace WaveEngine.Components.Primitives
{
    /// <summary>
    /// A class that contains the information of each point of a <see cref="LineBezierMesh"/>.
    /// </summary>
    [DataContract]
    public class BezierPointInfo
    {
        /// <summary>
        /// A value indicating whether the point is the first one of the line.
        /// </summary>
        internal bool IsFirstpoint;

        /// <summary>
        /// A value indicating whether the point is the last one of the line.
        /// </summary>
        internal bool IsLastpoint;

        /// <summary>
        /// Indicates the bezier type
        /// </summary>
        internal BezierTypes BezierType;

        /// <summary>
        /// The position of the point.
        /// </summary>
        [DataMember]
        public Vector3 Position;

        /// <summary>
        /// The relative position of the inbound handle.
        /// </summary>
        [DataMember]
        [RenderProperty(ShowConditionFunction = "HasInboundHandleVisible")]
        public Vector3 InboundHandle;

        /// <summary>
        /// The relative position of the outbound handle.
        /// </summary>
        [DataMember]
        [RenderProperty(ShowConditionFunction = "HasOutboundHandleVisible")]
        public Vector3 OutboundHandle;

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

        /// <summary>
        /// Indicates whether this point has an inbound handle
        /// </summary>
        /// <returns><c>true</c> if this point has an inbound handle; otherwise <c>false</c></returns>
        internal bool HasInboundHandleVisible()
        {
            return !this.IsFirstpoint;
        }

        /// <summary>
        /// Indicates whether this point has an outbound handle
        /// </summary>
        /// <returns><c>true</c> if this point has an outbound handle; otherwise <c>false</c></returns>
        internal bool HasOutboundHandleVisible()
        {
            return !this.IsLastpoint && this.BezierType == BezierTypes.Cubic;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return "Position:" + this.Position.ToString() + " Thickness:" + this.Thickness + " Color:" + this.Color.ToHexColorCode();
        }
    }
}
