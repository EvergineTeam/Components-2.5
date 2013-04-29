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
        /// <param name="text">The text line.</param>
        /// <param name="size">The size.</param>
        /// <param name="offsetX">The offset X.</param>
        public LineInfo(string text, Vector2 size, float offsetX)
        {
            this.Text = text;
            this.Size = size;
            this.AlignmentOffsetX = offsetX;
        }

        #endregion
    }
}
