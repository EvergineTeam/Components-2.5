#region File Description
//-----------------------------------------------------------------------------
// ColumnDefinition
//
// Copyright © 2010 - 2013 Wave Coorporation. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
#endregion

namespace WaveEngine.Components.UI
{
    /// <summary>
    /// This class represent a grid column.
    /// </summary>
    public sealed class ColumnDefinition
    {
        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        /// <value>
        /// The width.
        /// </value>
        public GridLength Width { get; set; }

        /// <summary>
        /// Gets the actual width.
        /// </summary>
        /// <value>
        /// The actual width.
        /// </value>
        public float ActualWidth { get; internal set; }
    }
}
