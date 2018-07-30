// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Using Statements
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Math;
#endregion

namespace WaveEngine.Components.Graphics2D
{
    /// <summary>
    /// Slice cache properties used by the renderer
    /// </summary>
    internal class SliceCacheItem
    {
        /// <summary>
        /// Source rectangle
        /// </summary>
        internal Rectangle SourceRectangle;

        /// <summary>
        /// World matrix
        /// </summary>
        internal Matrix WorldMatrix;

        /// <summary>
        /// Debug tint color
        /// </summary>
        internal Color DebugTintColor;
    }
}
