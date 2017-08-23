// Copyright © 2017 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#endregion

namespace WaveEngine.Components.UI
{
    /// <summary>
    /// Describes the kind of value that a GridLength object is holding.
    /// </summary>
    public enum GridUnitType
    {
        /// <summary>
        /// The size is determined by the size properties of the content object.
        /// </summary>
        Auto,

        /// <summary>
        /// The value is expressed as a pixel.
        /// </summary>
        Pixel,

        /// <summary>
        /// The value is expressed as a weighted proportion of available space.
        /// </summary>
        Proportional
    }
}
