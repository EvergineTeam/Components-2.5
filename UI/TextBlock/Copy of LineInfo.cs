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
    public struct LineInfo
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

        /// <summary>
        /// The offset alignment
        /// </summary>
        public float AlignmentOffsetX;

        /// <summary>
        /// Indicates if is line ending
        /// </summary>
        public bool EndLine;

        #region Initialize

        /// <summary>
        /// Initializes a new instance of the <see cref="LineInfo" /> struct.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="color">The color.</param>
        /// <param name="size">The size.</param>
        /// <param name="offsetX">offset X alignment.</param>
        /// <param name="endLine">end line flag.</param>
        public LineInfo(string text, Color color, Vector2 size, float offsetX, bool endLine = true)
        {
            this.Text = text;
            this.Color = color;
            this.Size = size;
            this.AlignmentOffsetX = offsetX;
            this.EndLine = endLine;
        }

        #endregion
    }
}
