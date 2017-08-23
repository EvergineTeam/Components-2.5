// Copyright © 2017 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using WaveEngine.Common.Attributes;
using WaveEngine.Common.Helpers;
using WaveEngine.Common.Math;
using WaveEngine.Components.Graphics2D;
using WaveEngine.Framework;
using WaveEngine.Framework.Animation;
using WaveEngine.Framework.Graphics;
#endregion

namespace WaveEngine.Components.Animation
{
    /// <summary>
    /// Base class of a behavior that controls the animations.
    /// </summary>
    /// <remarks>
    /// Ideally this class should be used to hold all the animations related to an entity.
    /// </remarks>
    [DataContract(Namespace = "WaveEngine.Components.Animation")]
    public abstract class AnimationBase : Behavior
    {
        /// <summary>
        ///     Number of instances of this component created.
        /// </summary>
        private static int instances;

        /// <summary>
        /// The key frame events.
        /// </summary>
        protected Dictionary<string, Dictionary<int, string>> keyFrameEvents;

        /// <summary>
        ///     Raised when a certain frame of an animation is played.
        /// </summary>
        public abstract event EventHandler<StringEventArgs> OnKeyFrameEvent;

        #region Initialization

        /// <summary>
        /// Initializes a new instance of the <see cref="AnimationBase"/> class.
        /// </summary>
        /// <param name="name">Name of this instance.</param>
        public AnimationBase(string name)
            : base("AnimationBase" + instances)
        {
        }

        /// <summary>
        /// The default values
        /// </summary>
        protected override void DefaultValues()
        {
            base.DefaultValues();

            this.keyFrameEvents = new Dictionary<string, Dictionary<int, string>>();
            instances++;
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// Gets the animation names.
        /// </summary>
        /// <value>
        /// The animation names.
        /// </value>
        [DontRenderProperty]
        public abstract IEnumerable<string> AnimationNames { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the current active animation is played backwards.
        /// </summary>
        /// <value>
        ///   <c>true</c> if backwards; otherwise, <c>false</c>.
        /// </value>
        [DontRenderProperty]
        public bool Backwards { get; protected set; }

        /// <summary>
        /// Gets or sets a value indicating whether the animation is played automatically when the CurrentAnimation
        /// </summary>
        /// <value>
        ///   <c>true</c> if [play automatically]; otherwise, <c>false</c>.
        /// </value>
        [DataMember]
        public bool PlayAutomatically { get; set; }

        /// <summary>
        ///     Gets or sets the current active animation.
        /// </summary>
        /// <value>
        ///     The current animation.
        /// </value>
        [DataMember]
        [RenderPropertyAsSelector("AnimationNames")]
        public abstract string CurrentAnimation { get; set; }

        /// <summary>
        /// Gets or sets the current frame of the animation.
        /// </summary>
        [DontRenderProperty]
        public abstract int Frame { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether current active animation is looping.
        /// </summary>
        /// <value>
        ///     <c>true</c> if the animation is looping; otherwise, <c>false</c>.
        /// </value>
        [DontRenderProperty]
        public bool Loop { get; protected set; }

        /// <summary>
        /// Gets or sets the state of the current active animation.
        /// </summary>
        [DontRenderProperty]
        public AnimationState State { get; protected set; }
        #endregion

        #region Public Methods

        /// <summary>
        /// Adds a key frame event to a given animation.
        /// </summary>
        /// <param name="animation">
        /// The animation name.
        /// </param>
        /// <param name="keyFrame">
        /// The key frame when the event will be raised.
        /// </param>
        /// <returns>
        /// This instance.
        /// </returns>
        public AnimationBase AddKeyFrameEvent(string animation, int keyFrame)
        {
            return this.AddKeyFrameEvent(animation, keyFrame, animation);
        }

        /// <summary>
        /// Adds a key frame event to a given animation.
        /// </summary>
        /// <param name="animation">
        /// The animation name.
        /// </param>
        /// <param name="keyFrame">
        /// The key frame when the event will be raised.
        /// </param>
        /// <param name="tag">
        /// The tag associated with the event.
        /// </param>
        /// <returns>
        /// This instance.
        /// </returns>
        public AnimationBase AddKeyFrameEvent(string animation, int keyFrame, string tag)
        {
            if (this.keyFrameEvents.ContainsKey(animation))
            {
                if (this.keyFrameEvents[animation].ContainsKey(keyFrame))
                {
                    throw new InvalidOperationException("Already exists a event for this keyframe.");
                }

                this.keyFrameEvents[animation].Add(keyFrame, tag);
            }
            else
            {
                this.keyFrameEvents.Add(animation, new Dictionary<int, string>());
                this.keyFrameEvents[animation].Add(keyFrame, tag);
            }

            return this;
        }

        /// <summary>
        /// Clears all the key frame events of a given animation.
        /// </summary>
        /// <param name="animation">
        /// The animation name.
        /// </param>
        /// <returns>
        /// This instance.
        /// </returns>
        public AnimationBase ClearKeyFrameEvents(string animation)
        {
            this.keyFrameEvents.Remove(animation);

            return this;
        }

        /// <summary>
        /// Plays the animation.
        /// </summary>
        /// <param name="name">
        /// The name of the animation.
        /// </param>
        public void PlayAnimation(string name)
        {
            this.PlayAnimation(name, this.Loop, this.Backwards);
        }

        /// <summary>
        /// Plays the animation.
        /// </summary>
        /// <param name="name">
        /// The name of the animation.
        /// </param>
        /// <param name="loop">
        /// if set to <c>true</c> loop the animation.
        /// </param>
        public void PlayAnimation(string name, bool loop)
        {
            this.PlayAnimation(name, loop, this.Backwards);
        }

        /// <summary>
        /// Plays the animation.
        /// </summary>
        /// <param name="name">
        /// The name of the animation.
        /// </param>
        /// <param name="loop">
        /// if set to <c>true</c> loop the animation.
        /// </param>
        /// <param name="backwards">
        /// if set to <c>true</c> play the animation backwards.
        /// </param>
        public void PlayAnimation(string name, bool loop, bool backwards)
        {
            this.PlayAnimation(name, null, null, loop, backwards);
        }

        /// <summary>
        /// Plays the animation between the specified frames.
        /// </summary>
        /// <param name="name">
        /// The name of the animation.
        /// </param>
        /// <param name="startFrame">
        /// The frame where the animation starts playing.
        /// </param>
        /// <param name="endFrame">
        /// The last frame of the animation to play.
        /// </param>
        public void PlayAnimation(string name, int? startFrame, int? endFrame)
        {
            this.PlayAnimation(name, startFrame, endFrame, this.Loop, this.Backwards);
        }

        /// <summary>
        /// Plays the animation between the specified frames.
        /// </summary>
        /// <param name="name">
        /// The name of the animation.
        /// </param>
        /// <param name="startFrame">
        /// The frame where the animation starts playing.
        /// </param>
        /// <param name="endFrame">
        /// The last frame of the animation to play.
        /// </param>
        /// <param name="loop">
        /// if set to <c>true</c> loop the animation.
        /// </param>
        /// <param name="backwards">
        /// if set to <c>true</c> play the animation backwards.
        /// </param>
        public abstract void PlayAnimation(string name, int? startFrame, int? endFrame, bool loop, bool backwards);

        /// <summary>
        /// Resume the animation.
        /// </summary>
        public abstract void ResumeAnimation();

        /// <summary>
        /// Stops the animation.
        /// </summary>
        public abstract void StopAnimation();
        #endregion
    }
}
