#region File Description
//-----------------------------------------------------------------------------
// Animation3D
// Copyright © 2010 - 2013 Wave Coorporation. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;

using WaveEngine.Common.Helpers;
using WaveEngine.Framework;
using WaveEngine.Framework.Animation;
#endregion

namespace WaveEngine.Components.Animation
{
    /// <summary>
    ///     Behavior to controls the animations of a 3D model.
    /// </summary>
    /// <remarks>
    ///     Ideally this class should be used to hold all the animations related to a given 3D model.
    /// </remarks>
    public class Animation3D : Behavior
    {
<<<<<<< HEAD
        #region Static Fields

=======
>>>>>>> Added all files in Component library
        /// <summary>
        ///     Number of instances of this component created.
        /// </summary>
        private static int instances;

<<<<<<< HEAD
        #endregion

        #region Fields

=======
>>>>>>> Added all files in Component library
        /// <summary>
        /// The animation path.
        /// </summary>
        private readonly string animationPath;

        /// <summary>
        /// The key frame events.
        /// </summary>
        private readonly Dictionary<string, Dictionary<int, string>> keyFrameEvents;

        /// <summary>
        /// The current animation.
        /// </summary>
        private string currentAnimation;

        /// <summary>
        /// The frame.
        /// </summary>
        private int frame;

        /// <summary>
        /// The frames per second.
        /// </summary>
        private int framesPerSecond;

        /// <summary>
        /// The internal animation.
        /// </summary>
        private InternalAnimation internalAnimation;

        /// <summary>
        /// The internal frame.
        /// </summary>
        private int internalFrame;

        /// <summary>
        /// The last frame.
        /// </summary>
        private int lastFrame;

        /// <summary>
        /// The num frames.
        /// </summary>
        private int numFrames;

        /// <summary>
        /// The prev frame.
        /// </summary>
        private int prevFrame;

        /// <summary>
        /// The target frame.
        /// </summary>
        private int targetFrame;

        /// <summary>
        /// The time per frame.
        /// </summary>
        private TimeSpan timePerFrame;

        /// <summary>
        /// The total anim time.
        /// </summary>
        private TimeSpan totalAnimTime;

<<<<<<< HEAD
        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Animation3D"/> class.
        /// </summary>
        /// <param name="animationPath">
        /// The path to the animation data.
        /// </param>
        public Animation3D(string animationPath)
            : this("Animation3D" + instances, animationPath)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Animation3D"/> class.
        /// </summary>
        /// <param name="name">
        /// The name of this behavior.
        /// </param>
        /// <param name="animationPath">
        /// The path to the animation data.
        /// </param>
        public Animation3D(string name, string animationPath)
            : base(name)
        {
            if (string.IsNullOrEmpty(animationPath))
            {
                throw new NullReferenceException("AnimationPath cannot be null.");
            }

            this.keyFrameEvents = new Dictionary<string, Dictionary<int, string>>();
            this.animationPath = animationPath;
            this.FramesPerSecond = 30;
            this.frame = 0;
            this.lastFrame = 0;
            this.Loop = true;
            this.targetFrame = -1;
            instances++;
        }

        #endregion

        #region Public Events

=======
>>>>>>> Added all files in Component library
        /// <summary>
        ///     Raised when a certain frame of an animation is played.
        /// </summary>
        public event EventHandler<StringEventArgs> OnKeyFrameEvent;

<<<<<<< HEAD
        #endregion

        #region Public Properties
=======
        #region Properties
>>>>>>> Added all files in Component library

        /// <summary>
        /// Gets a value indicating whether the current active animation is played backwards.
        /// </summary>
        /// <value>
        ///   <c>true</c> if backwards; otherwise, <c>false</c>.
        /// </value>
        public bool Backwards { get; private set; }

        /// <summary>
        ///     Gets or sets a value indicating whether the bounding box was refreshed.
        /// </summary>
        /// <value>
        ///     <c>true</c> if the bounding box was refreshed; otherwise, <c>false</c>.
        /// </value>
        public bool BoundingBoxRefreshed { get; set; }

        /// <summary>
        ///     Gets or sets the current active animation.
        /// </summary>
        /// <value>
        ///     The current animation.
        /// </value>
        public string CurrentAnimation
        {
            get
            {
                return this.currentAnimation;
            }

            set
            {
                if (this.currentAnimation != value)
                {
                    this.currentAnimation = value;
                    this.frame = 0;
                    this.lastFrame = 0;
                    this.UpdateNumFrames();
                }
            }
        }

        /// <summary>
        ///     Gets the current frame of the animation.
        /// </summary>
        public int Frame
        {
            get
            {
                return this.frame;
            }
        }

        /// <summary>
        ///     Gets or sets speed of the animation.
        /// </summary>
        /// <value>
        ///     The speed of the animation.
        /// </value>
        public int FramesPerSecond
        {
            get
            {
                return this.framesPerSecond;
            }

            set
            {
                this.framesPerSecond = value;
                this.timePerFrame = TimeSpan.FromMilliseconds(1000 / this.framesPerSecond);
            }
        }

        /// <summary>
        ///     Gets the animation data.
        /// </summary>
        public InternalAnimation InternalAnimation
        {
            get
            {
                return this.internalAnimation;
            }
        }

        /// <summary>
        ///     Gets a value indicating whether current active animation is looping.
        /// </summary>
        /// <value>
        ///     <c>true</c> if the animation is looping; otherwise, <c>false</c>.
        /// </value>
        public bool Loop { get; private set; }

        /// <summary>
        ///     Gets the state of the current active animation.
        /// </summary>
        public AnimationState State { get; private set; }

        #endregion

<<<<<<< HEAD
        #region Public Methods and Operators
=======
        #region Initialize

        /// <summary>
        /// Initializes a new instance of the <see cref="Animation3D"/> class.
        /// </summary>
        /// <param name="animationPath">
        /// The path to the animation data.
        /// </param>
        public Animation3D(string animationPath)
            : this("Animation3D" + instances, animationPath)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Animation3D"/> class.
        /// </summary>
        /// <param name="name">
        /// The name of this behavior.
        /// </param>
        /// <param name="animationPath">
        /// The path to the animation data.
        /// </param>
        public Animation3D(string name, string animationPath)
            : base(name)
        {
            if (string.IsNullOrEmpty(animationPath))
            {
                throw new NullReferenceException("AnimationPath cannot be null.");
            }

            this.keyFrameEvents = new Dictionary<string, Dictionary<int, string>>();
            this.animationPath = animationPath;
            this.FramesPerSecond = 30;
            this.frame = 0;
            this.lastFrame = 0;
            this.Loop = true;
            this.targetFrame = -1;
            instances++;
        }

        #endregion

        #region Public Methods
>>>>>>> Added all files in Component library

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
        public Animation3D AddKeyFrameEvent(string animation, int keyFrame)
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
        public Animation3D AddKeyFrameEvent(string animation, int keyFrame, string tag)
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
        public Animation3D ClearKeyFrameEvents(string animation)
        {
            this.keyFrameEvents.Remove(animation);

            return this;
        }

        /// <summary>
        /// Gets the duration of an animation.
        /// </summary>
        /// <param name="animation">
        /// The animation name.
        /// </param>
        /// <returns>
        /// The duration of the animation
        /// </returns>
        public float GetDuration(string animation)
        {
            if (!this.internalAnimation.Animations.ContainsKey(animation))
            {
                return 0.0f;
            }

            return (float)this.internalAnimation.Animations[animation].Duration.TotalSeconds;
        }

        /// <summary>
        /// Plays the animation.
        /// </summary>
        /// <param name="name">
        /// The name of the animation.
        /// </param>
        public void PlayAnimation(string name)
        {
            this.PlayAnimation(name, false, false);
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
            this.PlayAnimation(name, loop, false);
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
            this.totalAnimTime = TimeSpan.Zero;

            this.CurrentAnimation = name;
            this.BoundingBoxRefreshed = true;
            this.Loop = loop;
            this.State = AnimationState.Playing;
            this.targetFrame = -1;
            this.Backwards = backwards;

            if (backwards)
            {
                this.prevFrame = 0;
                this.frame = this.numFrames - 1;
            }
            else
            {
                this.prevFrame = this.numFrames - 1;
                this.frame = 0;
            }
        }

        /// <summary>
        /// Plays the animation up to a given frame.
        /// </summary>
        /// <param name="name">
        /// The name of the animation.
        /// </param>
        /// <param name="destFrame">
        /// The last frame of the animation to play.
        /// </param>
        public void PlayToFrame(string name, int destFrame)
        {
            this.PlayToFrame(name, null, destFrame);
        }

        /// <summary>
        /// Plays the animation up to a given frame.
        /// </summary>
        /// <param name="name">
        /// The name of the animation.
        /// </param>
        /// <param name="from">
        /// The frame where the animation starts playing.
        /// </param>
        /// <param name="destFrame">
        /// The last frame of the animation to play.
        /// </param>
        public void PlayToFrame(string name, int? from, int destFrame)
        {
            this.CurrentAnimation = name;

            if (from.HasValue)
            {
                this.lastFrame = from.Value;
            }
            else if (this.State == AnimationState.Playing)
            {
                this.totalAnimTime = TimeSpan.Zero;
                this.lastFrame = this.frame;
            }

            this.BoundingBoxRefreshed = true;
            this.Loop = false;
            this.State = AnimationState.Playing;
            this.targetFrame = destFrame;

            if (destFrame < this.lastFrame)
            {
                this.Backwards = true;
            }
            else
            {
                this.Backwards = false;
            }
        }

        /// <summary>
        /// Removes a key frame event.
        /// </summary>
        /// <param name="animation">
        /// The animation name.
        /// </param>
        /// <param name="keyFrame">
        /// The key frame when the event was raised.
        /// </param>
        /// <returns>
        /// This instance.
        /// </returns>
        public Animation3D RemoveKeyFrameEvent(string animation, int keyFrame)
        {
            if (this.keyFrameEvents.ContainsKey(animation) && this.keyFrameEvents[animation].ContainsKey(keyFrame))
            {
                this.keyFrameEvents[animation].Remove(keyFrame);
            }

            return this;
        }

        /// <summary>
        ///     Stops the animation.
        /// </summary>
        public void StopAnimation()
        {
            this.State = AnimationState.Stopped;
        }

        #endregion

<<<<<<< HEAD
        #region Methods
=======
        #region Private Methods
>>>>>>> Added all files in Component library

        /// <summary>
        ///     Performs further custom initialization for this instance.
        /// </summary>
        protected override void Initialize()
        {
            this.internalAnimation = this.Assets.LoadAsset<InternalAnimation>(this.animationPath);
            if (string.IsNullOrEmpty(this.CurrentAnimation))
            {
                this.CurrentAnimation = this.internalAnimation.Animations.Keys.ToArray()[0];
            }

            this.UpdateNumFrames();
        }

        /// <summary>
        /// Updates the animation.
        /// </summary>
        /// <param name="gameTime">
        /// The game time.
        /// </param>
        protected override void Update(TimeSpan gameTime)
        {
            if (this.State == AnimationState.Playing)
            {
                this.totalAnimTime += gameTime;

                this.internalFrame = (int)(this.totalAnimTime.TotalMilliseconds / this.timePerFrame.TotalMilliseconds);

                if (this.Backwards)
                {
                    // Is backwards
                    if (this.targetFrame >= 0)
                    {
                        // TargetFrame enabled
                        this.frame = this.lastFrame - this.internalFrame; // Frame Increment

                        if (this.frame <= this.targetFrame)
                        {
                            // Active Target frame)
                            this.totalAnimTime = TimeSpan.Zero;
                            this.lastFrame = this.frame = this.targetFrame;
                            this.targetFrame = -1;
                            this.StopAnimation();
                        }
                    }
                    else
                    {
                        // Normal backwards
                        this.frame = (this.numFrames - 1) - this.internalFrame; // Frame Increment

                        if (this.frame < 0)
                        {
                            // If animation completed
                            this.totalAnimTime = TimeSpan.Zero;

                            if (this.Loop)
                            {
                                // Is looped
                                this.frame = 0;
                            }
                            else
                            {
                                this.frame = this.lastFrame = 0;
                                this.StopAnimation();
                            }
                        }
                    }

                    // Throw KeyFrameEvents
                    if (this.OnKeyFrameEvent != null && this.keyFrameEvents.ContainsKey(this.currentAnimation))
                    {
                        Dictionary<int, string> listKeys = this.keyFrameEvents[this.currentAnimation];
                        int i = this.prevFrame;

                        while (i != this.frame)
                        {
                            i = i - 1;

                            if (i < 0)
                            {
                                i = this.numFrames - 1;
                            }

                            if (listKeys.ContainsKey(i))
                            {
                                this.OnKeyFrameEvent(this, new StringEventArgs(listKeys[i]));
                            }
                        }
                    }
                }
                else
                {
                    // Isn't backwards
                    if (this.targetFrame >= 0)
                    {
                        // TargetFrame enabled
                        this.frame = this.lastFrame + this.internalFrame; // Frame Increment

                        if (this.frame >= this.targetFrame)
                        {
                            // Active Target frame
                            this.totalAnimTime = TimeSpan.Zero;
                            this.lastFrame = this.frame = this.targetFrame;
                            this.targetFrame = -1;
                            this.StopAnimation();
                        }
                    }
                    else
                    {
                        // Normal
                        this.frame = this.internalFrame; // Frame Increment

                        if (this.frame >= this.numFrames)
                        {
                            // If animation completed
                            this.totalAnimTime = TimeSpan.Zero;

                            if (this.Loop)
                            {
                                // Is looped
                                this.frame = this.numFrames - 1;
                            }
                            else
                            {
                                this.frame = this.lastFrame = this.numFrames - 1;
                                this.StopAnimation();
                            }
                        }
                    }

                    // Throw KeyFrameEvents
                    if (this.OnKeyFrameEvent != null && this.keyFrameEvents.ContainsKey(this.currentAnimation))
                    {
                        Dictionary<int, string> listKeys = this.keyFrameEvents[this.currentAnimation];
                        int i = this.prevFrame;
                        while (i != this.frame)
                        {
                            i = (i + 1) % this.numFrames;

                            if (listKeys.ContainsKey(i))
                            {
                                this.OnKeyFrameEvent(this, new StringEventArgs(listKeys[i]));
                            }
                        }
                    }
                }

                this.prevFrame = this.frame;
            }
        }

        /// <summary>
        /// The update num frames.
        /// </summary>
        private void UpdateNumFrames()
        {
            if (this.internalAnimation != null && this.internalAnimation.Animations.Count > 0)
            {
                this.numFrames = this.internalAnimation.Animations[this.CurrentAnimation].Frames.Count;
            }
        }

        #endregion
    }
}