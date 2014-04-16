#region File Description
//-----------------------------------------------------------------------------
// ChangedEventArgs
//
// Copyright © 2014 Wave Corporation
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
#endregion

namespace WaveEngine.Components.UI
{
    /// <summary>
    /// Changed Event Handler delegate
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="ChangedEventArgs" /> instance containing the event data.</param>
    public delegate void ChangedEventHandler(object sender, ChangedEventArgs e);

    /// <summary>
    /// Changed events args class
    /// </summary>
    public class ChangedEventArgs : EventArgs
    {
        /// <summary>
        /// The old value
        /// </summary>
        public readonly int OldValue;

        /// <summary>
        /// The new value
        /// </summary>
        public readonly int NewValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChangedEventArgs" /> class.
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        public ChangedEventArgs(int oldValue, int newValue)
        {
            this.OldValue = oldValue;
            this.NewValue = newValue;
        }
    }
}
