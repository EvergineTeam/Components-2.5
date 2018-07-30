// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Using Statements
using System;
#endregion

namespace WaveEngine.Components.Gestures
{
    /// <summary>
    /// Provides information about a gesture event when such is raised.
    /// </summary>
    public class GestureEventArgs : EventArgs
    {
        /// <summary>
        /// Relevant information about the gesture performed.
        /// See <see cref="GestureSample"/> for more information.
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
