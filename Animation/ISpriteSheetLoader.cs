#region File Description
//-----------------------------------------------------------------------------
// ISpriteSheetLoader
// Copyright © 2014 Wave Corporation
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WaveEngine.Common.Math;
#endregion

namespace WaveEngine.Components.Animation
{
    /// <summary>
    /// Contract for every sprite sheet loader.
    /// </summary>
    public interface ISpriteSheetLoader
    {
        /// <summary>
        /// Builds the collection of frames by parsing passed document.
        /// </summary>
        /// <param name="path">Content-relative path to the sprite sheet document.</param>
        /// <returns>Array of <see cref="Rectangle"/> containing each frame.</returns>
        Rectangle[] Parse(string path);
    }
}
