// Copyright © 2017 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Using statements
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using WaveEngine.Common;
using WaveEngine.Common.IO;
using WaveEngine.Framework.Services;
using WaveEngine.Framework;
#endregion

namespace WaveEngine.Components.GameActions
{
    /// <summary>
    /// Represent a Set of Game Actions
    /// </summary>
    public class GameActionSet : IGameActionSet
    {
        /// <summary>
        /// The game action collection
        /// </summary>
        private IEnumerable<IGameAction> actions;

        /// <summary>
        /// The game action collection generator
        /// </summary>
        private IEnumerable<Func<IGameAction>> actionGenerators;

        /// <summary>
        /// The parent action
        /// </summary>
        private IGameAction parent;

        /// <summary>
        /// The associated scene
        /// </summary>
        private Scene scene;

        #region Initialize

        /// <summary>
        /// Initializes a new instance of the <see cref="GameActionSet" /> class.
        /// </summary>
        /// <param name="actions">The action list.</param>
        /// <param name="scene">The associated scene.</param>
        public GameActionSet(IEnumerable<IGameAction> actions, Scene scene = null)
        {
            this.actions = actions;
            this.scene = scene;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GameActionSet" /> class.
        /// </summary>
        /// <param name="actionGenerators">The action list.</param>
        /// <param name="scene">The associated scene.</param>
        public GameActionSet(IEnumerable<Func<IGameAction>> actionGenerators, Scene scene = null)
        {
            this.actionGenerators = actionGenerators;
            this.scene = scene;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GameActionSet" /> class.
        /// </summary>
        /// <param name="parent">The parent action.</param>
        /// <param name="actions">The action list.</param>
        public GameActionSet(IGameAction parent, IEnumerable<IGameAction> actions)
        {
            this.actions = actions;
            this.parent = parent;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GameActionSet" /> class.
        /// </summary>
        /// <param name="parent">The parent action.</param>
        /// <param name="actionGenerators">The action list.</param>
        public GameActionSet(IGameAction parent, IEnumerable<Func<IGameAction>> actionGenerators)
        {
            this.actionGenerators = actionGenerators;
            this.parent = parent;
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// Return an action that will be completed when all actions has been completed.
        /// </summary>
        /// <returns>The task.</returns>
        public IGameAction WaitAll()
        {
            int count = this.GetActionCount();
            return this.WaitCount(count);
        }

        /// <summary>
        /// Return an action that will be completed when any actions has been completed.
        /// </summary>
        /// <returns>The task.</returns>
        public IGameAction WaitAny()
        {
            return this.WaitCount(1);
        }

        /// <summary>
        /// Return an action that will be completed when all actions has been completed.
        /// </summary>
        /// <param name="count">The count limit</param>
        /// <returns>The task.</returns>
        public IGameAction WaitCount(int count)
        {
            if (this.actions != null)
            {
                if (this.parent != null)
                {
                    return new WaitCountGameAction(this.parent, count, this.actions.ToArray());
                }
                else
                {
                    return new WaitCountGameAction(this.scene, count, this.actions.ToArray());
                }
            }
            else if (this.actionGenerators != null)
            {
                if (this.parent != null)
                {
                    return new WaitCountGameAction(this.parent, count, this.actionGenerators.ToArray());
                }
                else
                {
                    return new WaitCountGameAction(this.scene, count, this.actionGenerators.ToArray());
                }
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Waits the predicate.
        /// </summary>
        /// <param name="waitingTaskPredicate">The waiting task predicate.</param>
        /// <returns>The task</returns>
        public IGameAction WaitPredicate(Func<bool, IGameAction[]> waitingTaskPredicate)
        {
            throw new NotImplementedException();
        }
        #endregion

        /// <summary>
        /// Return action count
        /// </summary>
        /// <returns>The action count</returns>
        private int GetActionCount()
        {
            int count;
            if (this.actions != null)
            {
                var collection = this.actions as ICollection;
                if (collection != null)
                {
                    count = collection.Count;
                }
                else
                {
                    count = this.actions.Count();
                }
            }
            else if (this.actionGenerators != null)
            {
                var collection = this.actionGenerators as ICollection;
                if (collection != null)
                {
                    count = collection.Count;
                }
                else
                {
                    count = this.actionGenerators.Count();
                }
            }
            else
            {
                count = 0;
            }

            return count;
        }
    }
}
