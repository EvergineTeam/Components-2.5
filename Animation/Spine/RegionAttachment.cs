#region File Description
//-----------------------------------------------------------------------------
// RegionAttachment
//
// Copyright (c) 2013, Esoteric Software
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions are met:
// 
// 1. Redistributions of source code must retain the above copyright notice, this
//    list of conditions and the following disclaimer.
// 2. Redistributions in binary form must reproduce the above copyright notice,
//    this list of conditions and the following disclaimer in the documentation
//    and/or other materials provided with the distribution.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR
// ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
// (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
// LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
// ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
// (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
// SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
#endregion

namespace WaveEngine.Components.Animation.Spine
{
    /// <summary>
    /// Attachment that displays a texture region.
    /// </summary>
    public class RegionAttachment : Attachment
    {
        /// <summary>
        /// The x1
        /// </summary>
        public const int X1 = 0;

        /// <summary>
        /// The y1
        /// </summary>
        public const int Y1 = 1;

        /// <summary>
        /// The x2
        /// </summary>
        public const int X2 = 2;

        /// <summary>
        /// The y2
        /// </summary>
        public const int Y2 = 3;

        /// <summary>
        /// The x3
        /// </summary>
        public const int X3 = 4;

        /// <summary>
        /// The y3
        /// </summary>
        public const int Y3 = 5;

        /// <summary>
        /// The x4
        /// </summary>
        public const int X4 = 6;

        /// <summary>
        /// The y4
        /// </summary>
        public const int Y4 = 7;

        #region Properties
        /// <summary>
        /// Gets or sets the X.
        /// </summary>
        /// <value>
        /// The X.
        /// </value>
        public float X { get; set; }

        /// <summary>
        /// Gets or sets the Y.
        /// </summary>
        /// <value>
        /// The Y.
        /// </value>
        public float Y { get; set; }

        /// <summary>
        /// Gets or sets the scale X.
        /// </summary>
        /// <value>
        /// The scale X.
        /// </value>
        public float ScaleX { get; set; }

        /// <summary>
        /// Gets or sets the scale Y.
        /// </summary>
        /// <value>
        /// The scale Y.
        /// </value>
        public float ScaleY { get; set; }

        /// <summary>
        /// Gets or sets the rotation.
        /// </summary>
        /// <value>
        /// The rotation.
        /// </value>
        public float Rotation { get; set; }

        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        /// <value>
        /// The width.
        /// </value>
        public float Width { get; set; }

        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        /// <value>
        /// The height.
        /// </value>
        public float Height { get; set; }

        /// <summary>
        /// Gets or sets the texture.
        /// </summary>
        /// <value>
        /// The texture.
        /// </value>
        public object Texture { get; set; }

        /// <summary>
        /// Gets or sets the region offset X.
        /// </summary>
        /// <value>
        /// The region offset X.
        /// </value>
        public float RegionOffsetX { get; set; }

        /// <summary>
        /// Gets or sets the region offset Y.
        /// </summary>
        /// <remarks>
        /// Pixels stripped from the bottom left, unrotated.
        /// </remarks>
        /// <value>
        /// The region offset Y.
        /// </value>
        public float RegionOffsetY { get; set; } 

        /// <summary>
        /// Gets or sets the width of the region.
        /// </summary>
        /// <value>
        /// The width of the region.
        /// </value>
        public float RegionWidth { get; set; }

        /// <summary>
        /// Gets or sets the height of the region.
        /// </summary>
        /// <remarks>Unrotated, stripped size.</remarks>
        /// <value>
        /// The height of the region.
        /// </value>
        public float RegionHeight { get; set; }

        /// <summary>
        /// Gets or sets the width of the region original.
        /// </summary>
        /// <value>
        /// The width of the region original.
        /// </value>
        public float RegionOriginalWidth { get; set; }

        /// <summary>
        /// Gets or sets the height of the region original.
        /// </summary>
        /// <remarks>Unrotated, unstripped size.</remarks>
        /// <value>
        /// The height of the region original.
        /// </value>
        public float RegionOriginalHeight { get; set; }

        /// <summary>
        /// Gets the offset.
        /// </summary>
        /// <value>
        /// The offset.
        /// </value>
        public float[] Offset { get; private set; }

        /// <summary>
        /// Gets the vertices.
        /// </summary>
        /// <value>
        /// The vertices.
        /// </value>
        public float[] Vertices { get; private set; }

        /// <summary>
        /// Gets the U vs.
        /// </summary>
        /// <value>
        /// The U vs.
        /// </value>
        public float[] UVs { get; private set; }
        #endregion

        #region Initialize
        /// <summary>
        /// Initializes a new instance of the <see cref="RegionAttachment" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public RegionAttachment(string name)
            : base(name)
        {
            this.Offset = new float[8];
            this.Vertices = new float[8];
            this.UVs = new float[8];
            this.ScaleX = 1;
            this.ScaleY = 1;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Sets the U vs.
        /// </summary>
        /// <param name="u">The u.</param>
        /// <param name="v">The v.</param>
        /// <param name="u2">The u2.</param>
        /// <param name="v2">The v2.</param>
        /// <param name="rotate">if set to <c>true</c> [rotate].</param>
        public void SetUVs(float u, float v, float u2, float v2, bool rotate)
        {
            float[] uvs = this.UVs;
            if (rotate)
            {
                uvs[X2] = u;
                uvs[Y2] = v2;
                uvs[X3] = u;
                uvs[Y3] = v;
                uvs[X4] = u2;
                uvs[Y4] = v;
                uvs[X1] = u2;
                uvs[Y1] = v2;
            }
            else
            {
                uvs[X1] = u;
                uvs[Y1] = v2;
                uvs[X2] = u;
                uvs[Y2] = v;
                uvs[X3] = u2;
                uvs[Y3] = v;
                uvs[X4] = u2;
                uvs[Y4] = v2;
            }
        }

        /// <summary>
        /// Updates the offset.
        /// </summary>
        public void UpdateOffset()
        {
            float width = this.Width;
            float height = this.Height;
            float scaleX = this.ScaleX;
            float scaleY = this.ScaleY;
            float regionScaleX = width / this.RegionOriginalWidth * scaleX;
            float regionScaleY = height / this.RegionOriginalHeight * scaleY;
            float localX = (-width / 2 * scaleX) + (this.RegionOffsetX * regionScaleX);
            float localY = (-height / 2 * scaleY) + (this.RegionOffsetY * regionScaleY);
            float localX2 = localX + (this.RegionWidth * regionScaleX);
            float localY2 = localY + (this.RegionHeight * regionScaleY);
            float radians = this.Rotation * (float)Math.PI / 180;
            float cos = (float)Math.Cos(radians);
            float sin = (float)Math.Sin(radians);
            float x = this.X;
            float y = this.Y;

            float localXCos = (localX * cos) + x;
            float localXSin = localX * sin;
            float localYCos = (localY * cos) + y;
            float localYSin = localY * sin;
            float localX2Cos = (localX2 * cos) + x;
            float localX2Sin = localX2 * sin;
            float localY2Cos = (localY2 * cos) + y;
            float localY2Sin = localY2 * sin;

            float[] offset = this.Offset;
            offset[X1] = localXCos - localYSin;
            offset[Y1] = localYCos + localXSin;
            offset[X2] = localXCos - localY2Sin;
            offset[Y2] = localY2Cos + localXSin;
            offset[X3] = localX2Cos - localY2Sin;
            offset[Y3] = localY2Cos + localX2Sin;
            offset[X4] = localX2Cos - localYSin;
            offset[Y4] = localYCos + localX2Sin;
        }

        /// <summary>
        /// Updates the vertices.
        /// </summary>
        /// <param name="bone">The bone.</param>
        public void UpdateVertices(Bone bone)
        {
            float x = bone.WorldX;
            float y = bone.WorldY;

            float m00 = bone.M00;
            float m01 = bone.M01;
            float m10 = bone.M10;
            float m11 = bone.M11;

            float[] vertices = this.Vertices;
            float[] offset = this.Offset;

            vertices[X1] = (offset[X1] * m00) + (offset[Y1] * m01) + x;
            vertices[Y1] = (offset[X1] * m10) + (offset[Y1] * m11) + y;
            vertices[X2] = (offset[X2] * m00) + (offset[Y2] * m01) + x;
            vertices[Y2] = (offset[X2] * m10) + (offset[Y2] * m11) + y;
            vertices[X3] = (offset[X3] * m00) + (offset[Y3] * m01) + x;
            vertices[Y3] = (offset[X3] * m10) + (offset[Y3] * m11) + y;
            vertices[X4] = (offset[X4] * m00) + (offset[Y4] * m01) + x;
            vertices[Y4] = (offset[X4] * m10) + (offset[Y4] * m11) + y;
        }
        #endregion
    }
}
