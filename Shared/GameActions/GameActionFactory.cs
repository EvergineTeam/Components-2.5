#region File Description
//-----------------------------------------------------------------------------
// GameActionFactory
//
// Copyright © 2015 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using System.Text;
using WaveEngine.Components.Gestures;
using WaveEngine.Framework;
using WaveEngine.Framework.Animation;
using WaveEngine.Framework.Services;
#endregion

namespace WaveEngine.Components.GameActions
{
    /// <summary>
    /// Extension methods for GameAction class
    /// </summary>
    public static class GameActionFactory
    {
        /// <summary>
        /// Continue with another action.
        /// </summary>
        /// <param name="parent">The parent action.</param>        
        /// <param name="nextAction">The next action.</param>
        /// <returns>An action that continue with the next action when the parent is completed</returns>
        /// <exception cref="System.NotSupportedException">It is not possible to continue with, aborted or finised task. Defer the run command to a posterior stage.</exception>
        public static IGameAction ContinueWith(this IGameAction parent, IGameAction nextAction)
        {
            if (nextAction.State == WaveEngine.Framework.Services.TaskState.Finished
                || nextAction.State == WaveEngine.Framework.Services.TaskState.Aborted)
            {
                throw new NotSupportedException("It is not possible to continue with, aborted or finised task. Defer the run command to a posterior stage.");
            }

            return new GameActionNode(parent, nextAction);
        }

        /// <summary>
        /// Continue with another action.
        /// </summary>
        /// <param name="parent">The parent action.</param>        
        /// <param name="nextActionGenerator">The next action generator.</param>
        /// <returns>An action that continue with the next action when the parent is completed</returns>
        /// <exception cref="System.NotSupportedException">It is not possible to continue with, aborted or finised task. Defer the run command to a posterior stage.</exception>
        public static IGameAction ContinueWith(this IGameAction parent, Func<IGameAction> nextActionGenerator)
        {
            return new GameActionNode(parent, nextActionGenerator);
        }

        /// <summary>
        /// Continue with another an action function
        /// </summary>
        /// <param name="parent">The parent action.</param>        
        /// <param name="action">The next action.</param>
        /// <returns>An action that continue with the next action when the parent is completed</returns>
        /// <exception cref="System.NotSupportedException">It is not possible to continue with, aborted or finised task. Defer the run command to a posterior stage.</exception>
        public static IGameAction ContinueWithAction(this IGameAction parent, Action action)
        {
            return new GameActionNode(parent, CreateGameActionFromAction(parent.Scene, action));
        }

        /// <summary>
        /// Continue with another action.
        /// </summary>
        /// <param name="parent">The parent action.</param>        
        /// <param name="childTasks">The chhild tasks.</param>
        /// <returns>An action that continue with the next action when the parent is completed</returns>
        /// <exception cref="System.NotSupportedException">It is not possible to continue with, aborted or finised task. Defer the run command to a posterior stage.</exception>
        public static IGameActionSet ContinueWith(this IGameAction parent, params IGameAction[] childTasks)
        {
            return new GameActionSet(parent, childTasks);
        }

        /// <summary>
        /// Continue with another action.
        /// </summary>
        /// <param name="parent">The parent action.</param>        
        /// <param name="childTasks">The chhild tasks.</param>
        /// <returns>An action that continue with the next action when the parent is completed</returns>
        /// <exception cref="System.NotSupportedException">It is not possible to continue with, aborted or finised task. Defer the run command to a posterior stage.</exception>
        public static IGameActionSet ContinueWith(this IGameAction parent, IEnumerable<IGameAction> childTasks)
        {
            return new GameActionSet(parent, childTasks);
        }

        /// <summary>
        /// Continue with another action.
        /// </summary>
        /// <param name="parent">The parent action.</param>        
        /// <param name="childTaskGenerators">The chhild task generators.</param>
        /// <returns>An action that continue with the next action when the parent is completed</returns>
        /// <exception cref="System.NotSupportedException">It is not possible to continue with, aborted or finised task. Defer the run command to a posterior stage.</exception>
        public static IGameActionSet ContinueWith(this IGameAction parent, params Func<IGameAction>[] childTaskGenerators)
        {
            return new GameActionSet(parent, childTaskGenerators);
        }

        /// <summary>
        /// Add a delay action.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="time">The time.</param>
        /// <returns>The action</returns>
        public static IGameAction Delay(this IGameAction parent, TimeSpan time)
        {
            return new WaitGameAction(parent, time);
        }

        /// <summary>
        /// Ands wait a tap to the touch gesture.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="touchGestures">The touch gestures.</param>
        /// <returns>The action</returns>
        public static IGameAction AndWaitTap(this IGameAction parent, TouchGestures touchGestures)
        {
            return new TapGameAction(parent, touchGestures);
        }

        /// <summary>
        /// Creates an empty game action.
        /// </summary>
        /// <param name="scene">The scene.</param>
        /// <returns>The action</returns>
        public static IGameAction CreateEmptyGameAction(this Scene scene)
        {
            return new ActionGameAction(() => { }, scene);
        }

        /// <summary>
        /// Creates an empty game action.
        /// </summary>
        /// <param name="scene">The scene.</param>
        /// <param name="action"> simple code to be executed</param>
        /// <returns>The action</returns>
        public static IGameAction CreateGameActionFromAction(this Scene scene, Action action)
        {
            return new ActionGameAction(action, scene);
        }

        /// <summary>
        /// Creates the wait action.
        /// </summary>
        /// <param name="scene">The scene.</param>
        /// <param name="timeSpan">The time span.</param>
        /// <returns>The action</returns>
        public static IGameAction CreateWaitGameAction(this Scene scene, TimeSpan timeSpan)
        {
            return new WaitGameAction(timeSpan, scene);
        }

        /// <summary>
        /// Creates a game action.
        /// </summary>
        /// <param name="scene">The scene.</param>
        /// <param name="actionGenerator">The action generator method.</param>
        /// <returns>The action</returns>
        public static IGameAction CreateGameAction(this Scene scene, Func<IGameAction> actionGenerator)
        {
            return new GameActionNode(actionGenerator, scene);
        }

        /// <summary>
        /// Creates a game action.
        /// </summary>
        /// <param name="scene">The scene.</param>
        /// <param name="action">The action.</param>
        /// <returns>The action</returns>
        public static IGameAction CreateGameAction(this Scene scene, IGameAction action)
        {
            return new GameActionNode(action, scene);
        }

        /// <summary>
        /// Ands the wait condition.
        /// </summary>
        /// <param name="scene">The scene.</param>
        /// <param name="breakPredicate">The predicate.</param>
        /// <param name="eventCount">The event count.</param>
        /// <returns>The action</returns>
        public static IGameAction CreateWaitConditionGameAction(this Scene scene, Func<bool> breakPredicate, int eventCount = 1)
        {
            return new ActiveWaitConditionGameAction(breakPredicate, scene, eventCount);
        }

        /// <summary>
        /// Creates the single animation action.
        /// </summary>
        /// <param name="scene">The scene.</param>
        /// <param name="singleAnimation">The single animation.</param>
        /// <param name="animationUI">The animation UI.</param>
        /// <param name="dependencyProperty">The dependency property.</param>
        /// <returns>The action</returns>
        public static IGameAction CreateSingleAnimationGameAction(this Scene scene, SingleAnimation singleAnimation, AnimationUI animationUI, DependencyProperty dependencyProperty)
        {
            return new SingleAnimationGameAction(singleAnimation, animationUI, dependencyProperty, scene);
        }

        /// <summary>
        /// Creates the wait tap task.
        /// </summary>
        /// <param name="scene">The scene.</param>
        /// <param name="touchGestures">The touch gestures.</param>
        /// <returns>The action</returns>
        public static IGameAction CreateWaitTapGameAction(this Scene scene, TouchGestures touchGestures)
        {
            return new TapGameAction(touchGestures, scene);
        }

        /// <summary>
        /// Create parallel actions.
        /// </summary>
        /// <param name="scene">The scene.</param>
        /// <param name="actionGenerators">The action generators.</param>
        /// <returns>The action</returns>
        public static IGameActionSet CreateParallelGameActions(this Scene scene, IEnumerable<Func<IGameAction>> actionGenerators)
        {
            return new GameActionSet(actionGenerators, scene);
        }

        /// <summary>
        /// Create parallel actions.
        /// </summary>
        /// <param name="scene">The scene.</param>
        /// <param name="actions">The actions.</param>
        /// <returns>The action</returns>
        public static IGameActionSet CreateParallelGameActions(this Scene scene, IEnumerable<IGameAction> actions)
        {
            return new GameActionSet(actions, scene);
        }

        /// <summary>
        /// Create parallel actions.
        /// </summary>
        /// <param name="scene">The scene.</param>
        /// <param name="actionGenerators">The action generators.</param>
        /// <returns>The action</returns>
        public static IGameActionSet CreateParallelGameActions(this Scene scene, params Func<IGameAction>[] actionGenerators)
        {
            return new GameActionSet(actionGenerators, scene);
        }

        /// <summary>
        /// Create parallel actions.
        /// </summary>
        /// <param name="scene">The scene.</param>
        /// <param name="actions">The actions.</param>
        /// <returns>The action</returns>
        public static IGameActionSet CreateParallelGameActions(this Scene scene, params IGameAction[] actions)
        {
            return new GameActionSet(actions, scene);
        }

        /// <summary>
        /// Continue with another action.
        /// </summary>
        /// <param name="parent">The parent action.</param>        
        /// <param name="childTasks">The chhild tasks.</param>
        /// <returns>An action that continue with the next action when the parent is completed</returns>
        /// <exception cref="System.NotSupportedException">It is not possible to continue with, aborted or finised task. Defer the run command to a posterior stage.</exception>
        public static IGameActionSet CreateParallelGameActions(this IGameAction parent, IEnumerable<IGameAction> childTasks)
        {
            return new GameActionSet(parent, childTasks);
        }

        /// <summary>
        /// Creates the repeat task until.
        /// </summary>
        /// <param name="scene">The scene.</param>
        /// <param name="actionGenerator">The action generator.</param>
        /// <param name="stopCondition">The until predicate.</param>
        /// <returns>The action</returns>
        public static IGameAction CreateLoopGameActionUntil(this Scene scene, Func<IGameAction> actionGenerator, Func<bool> stopCondition)
        {
            return scene.CreateGameAction(() =>
            {
                if (stopCondition())
                {
                    return scene.CreateEmptyGameAction();
                }
                else
                {
                    return actionGenerator().ContinueWith(scene.CreateLoopGameActionUntil(actionGenerator, stopCondition));
                }
            });
        }

        /// <summary>
        /// Ases the skipable task.
        /// </summary>
        /// <param name="actionToSkip">The action to skip.</param>
        /// <returns>The action</returns>
        public static IGameAction AsSkippableGameAction(this IGameAction actionToSkip)
        {
            var action = actionToSkip as GameAction;
            if (action != null)
            {
                action.IsSkippable = true;
            }

            return actionToSkip;
        }
    }
}
