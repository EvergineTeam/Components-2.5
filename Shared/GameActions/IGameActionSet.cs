#region File Description
//-----------------------------------------------------------------------------
// IGameActionSet
//
// Copyright © 2015 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Using statements
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using WaveEngine.Common;
using WaveEngine.Common.IO;
using WaveEngine.Framework.Services;
#endregion

namespace WaveEngine.Components.GameActions
{
    /// <summary>
    /// Represent a Set of Game Actions
    /// </summary>
    public interface IGameActionSet
    {
        /// <summary>
        /// Return an action that will be completed when all actions has been completed.
        /// </summary>
        /// <returns>The task.</returns>
        IGameAction WaitAll();

        /// <summary>
        /// Return an action that will be completed when any actions has been completed.
        /// </summary>
        /// <returns>The task.</returns>
        IGameAction WaitAny();

        /// <summary>
        /// Return an action that will be completed when all actions has been completed.
        /// </summary>
        /// <param name="count">The Count value.</param>
        /// <returns>The task.</returns>
        IGameAction WaitCount(int count);

        /// <summary>
        /// Waits the predicate.
        /// </summary>
        /// <param name="waitingTaskPredicate">The waiting task predicate.</param>
        /// <returns>The task.</returns>
        IGameAction WaitPredicate(Func<bool, IGameAction[]> waitingTaskPredicate);
    }
}