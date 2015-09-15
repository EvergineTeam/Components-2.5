#region File Description
//-----------------------------------------------------------------------------
// ColumnDefinition
//
// Copyright © 2015 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System.Runtime.Serialization;
#endregion

namespace WaveEngine.Components.UI
{
    /// <summary>
    /// This class represent a grid column.
    /// </summary>
    [DataContract(Namespace = "WaveEngine.Components.UI")]
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
