#region File Description
//-----------------------------------------------------------------------------
// GestureEventArgs
//
// Copyright © 2010 - 2013 Wave Coorporation. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
#endregion

namespace WaveEngine.Components.Gestures
{
    /// <summary>
    /// Information when a gesture event is raised.
    /// </summary>
    public class GestureEventArgs : EventArgs
    {
        /// <summary>
        /// The gesture that was performed.
        /// </summary>
        public GestureSample GestureSample;

        #region Initialize
        /// <summary>
        /// Initializes a new instance of the <see cref="GestureEventArgs"/> class.
        /// </summary>
        /// <param name="sample">The gesture that was performed.</param>
        public GestureEventArgs(GestureSample sample)
        {
            this.GestureSample = sample;
        }
        #endregion
    }
}
