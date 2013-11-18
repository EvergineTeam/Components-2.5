#region File Description
//-----------------------------------------------------------------------------
// LineInfo
//
// Copyright © 2010 - 2013 Wave Coorporation. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Math;
#endregion

namespace WaveEngine.Components.UI
{
    /// <summary>
    /// Helper to TextControl component
    /// </summary>
    public struct SubTextInfo
    {
        /// <summary>
        /// The line text
        /// </summary>
        public string Text;

        /// <summary>
        /// The text
        /// </summary>
        public Color Color;

        /// <summary>
        /// The size
        /// </summary>
        public Vector2 Size;
    }

    /// <summary>
    /// Helper to TextControl component
    /// </summary>
    public struct LineInfo
    {
        /// <summary>
        /// Sub text list
        /// </summary>
        public List<SubTextInfo> SubTextList;

        /// <summary>
        /// The size
        /// </summary>
        public Vector2 Size;

        /// <summary>
        /// The offset alignment
        /// </summary>
        public float AlignmentOffsetX;

        #region Initialize
        /// <summary>
        /// Initializes a new instance of the <see cref="LineInfo" /> struct.
        /// </summary>
        /// <param name="offsetX">offset X alignment.</param>
        public LineInfo(float offsetX = 0)
        {
            this.SubTextList = new List<SubTextInfo>();
            this.Size = Vector2.Zero;
            this.AlignmentOffsetX = offsetX;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LineInfo" /> struct.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="color">The color.</param>
        /// <param name="size">The size.</param>
        /// <param name="offsetX">offset X alignment.</param>
        public LineInfo(string text, Color color, Vector2 size, float offsetX)
        {
            this.SubTextList = new List<SubTextInfo>();
            this.Size = Vector2.Zero;
            this.AlignmentOffsetX = offsetX;
            this.AddText(text, color, size);
        }
        #endregion

        /// <summary>
        /// Add text to the line
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="color">The color.</param>
        /// <param name="size">The size.</param>
        public void AddText(string text, Color color, Vector2 size)
        {
            this.SubTextList.Add(new SubTextInfo()
            {
                Text = text,
                Color = color,
                Size = size
            });

            this.Size.X += size.X;
            this.Size.Y = MathHelper.Max(this.Size.Y, size.Y);
        }
    }
}
