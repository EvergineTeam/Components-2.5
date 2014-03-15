#region File Description
//-----------------------------------------------------------------------------
// SpriteSheetAnimationSequence
// Copyright © 2010 - 2013 Wave Coorporation. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#endregion

namespace WaveEngine.Components.Animation
{
    /// <summary>
    /// Specifies a sequence within the returning frames of a <see cref="ISpriteSheetLoader"/>.
    /// </summary>
    public class SpriteSheetAnimationSequence
    {
        /// <summary>
        /// Gets or sets the 1-based index of the first frame.
        /// </summary>
        public int First { get; set; }

        /// <summary>
        /// Gets or sets the sequence length.
        /// </summary>
        public int Length { get; set; }

        /// <summary>
        /// Gets or sets the dessired frames per second.
        /// </summary>
        public int FramesPerSecond { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SpriteSheetAnimationSequence" /> class.
        /// </summary>
        public SpriteSheetAnimationSequence()
        {
            this.FramesPerSecond = 30; // Default value
        }
    }
}
