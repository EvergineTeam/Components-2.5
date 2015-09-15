#region File Description
//-----------------------------------------------------------------------------
// SupportedGesture
//
// Copyright © 2015 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
#endregion

namespace WaveEngine.Components.Gestures
{
    /// <summary>
    /// Supported gestures by a control.
    /// </summary>
    [Flags]
    public enum SupportedGesture
    {
        /// <summary>
        /// No gestures supported.
        /// </summary>
        None = 0,

        /// <summary>
        /// Gestures that involve translation are supported.
        /// </summary>
        Translation = 2,

        /// <summary>
        /// Gestures that involve rotation are supported.
        /// </summary>
        Rotation = 4,

        /// <summary>
        /// Gestures that involve scaling are supported.
        /// </summary>
        Scale = 8,
    }
}
