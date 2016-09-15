#region File Description
//-----------------------------------------------------------------------------
// UpdatableGameAction
//
// Copyright © 2016 Plain Concepts S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using WaveEngine.Framework;
#endregion

namespace WaveEngine.Components.GameActions
{
    /// <summary>
    /// Game action that is updated
    /// </summary>
    public abstract class UpdatableGameAction : GameAction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UpdatableGameAction"/> class.
        /// </summary>
        /// <param name="name">The name of the game action</param>
        /// <param name="scene">The scene</param>
        public UpdatableGameAction(string name, Scene scene = null)
            : base(name, scene)
        {
        }

        /// <summary>
        /// Updates the game action
        /// </summary>
        /// <param name="gameTime">The ellapsed time</param>
        public abstract void Update(TimeSpan gameTime);
    }
}
