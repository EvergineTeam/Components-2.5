#region File Description
//-----------------------------------------------------------------------------
// Animation3D
// Copyright © 2015 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using WaveEngine.Common.Attributes;
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
    [DataContract(Namespace = "WaveEngine.Components.Animation")]
    public class Animation3D : AnimationBase
    {
        /// <summary>
        ///     Number of instances of this component created.
        /// </summary>
        private static int instances;

        /// <summary>
        /// The animation path.
        /// </summary>
        [DataMember]
        private string animationPath;

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

        /// <summary>
        /// Current animation sequence
        /// </summary>
        private AnimationSequence currentAnimationSequence;

        /// <summary>
        ///     Raised when a certain frame of an animation is played.
        /// </summary>
        public override event EventHandler<StringEventArgs> OnKeyFrameEvent;

        #region Properties
        /// <summary>
        /// Gets or sets the animation file path
        /// </summary>
        [DontRenderProperty]
        [RenderPropertyAsAsset(AssetType.SkinnedModel)]
        public string AnimationPath
        {
            get
            {
                return this.animationPath;
            }

            set
            {
                this.animationPath = value;

                if (this.isInitialized)
                {
                    this.RefreshAnimationAsset();
                }
            }
        }

        /// <summary>
        /// Gets the animation names.
        /// </summary>
        /// <value>
        /// The animation names.
        /// </value>
        [DontRenderProperty]
        public override IEnumerable<string> AnimationNames
        {
            get
            {
                if (this.internalAnimation != null)
                {
                    return this.internalAnimation.Animations.Keys;
                }

                return new List<string>();
            }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether the bounding box was refreshed.
        /// </summary>
        /// <value>
        ///     <c>true</c> if the bounding box was refreshed; otherwise, <c>false</c>.
        /// </value>
        [DontRenderProperty]
        public bool BoundingBoxRefreshed { get; set; }

        /// <summary>
        ///     Gets or sets the current active animation.
        /// </summary>
        /// <value>
        ///     The current animation.
        /// </value>
        [DataMember]
        [RenderPropertyAsSelector("AnimationNames")]
        public override string CurrentAnimation
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
        /// Gets or sets the current frame of the animation.
        /// </summary>
        [DontRenderProperty]
        public override int Frame
        {
            get
            {
                return this.frame;
            }

            set
            {
                if (this.currentAnimationSequence != null
                    && value >= 0
                    && this.currentAnimationSequence.Frames.Count > value)
                {
                    this.frame = value;
                    this.totalAnimTime = TimeSpan.FromSeconds(this.frame * this.timePerFrame.TotalMilliseconds);
                }
            }
        }

        /// <summary>
        ///     Gets the current animation time.
        /// </summary>
        [DontRenderProperty]
        public TimeSpan TotalAnimTime
        {
            get
            {
                return this.totalAnimTime;
            }
        }

        /// <summary>
        ///     Gets or sets speed of the animation.
        /// </summary>
        /// <value>
        ///     The speed of the animation.
        /// </value>
        [DataMember]
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
        [DontRenderProperty]
        public InternalAnimation InternalAnimation
        {
            get
            {
                return this.internalAnimation;
            }
        }
        #endregion

        #region Initialize
        /// <summary>
        /// Initializes a new instance of the <see cref="Animation3D"/> class.
        /// </summary>
        public Animation3D()
            : base("Animation3D" + instances)
        {
        }

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

            this.animationPath = animationPath;
        }

        /// <summary>
        /// The default values
        /// </summary>
        protected override void DefaultValues()
        {
            base.DefaultValues();

            this.keyFrameEvents = new Dictionary<string, Dictionary<int, string>>();
            this.FramesPerSecond = 30;
            this.frame = 0;
            this.lastFrame = 0;
            this.Loop = true;
            this.targetFrame = -1;
            instances++;
        }
        #endregion

        #region Public Methods

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

        /////// <summary>
        /////// Plays the animation.
        /////// </summary>
        /////// <param name="name">
        /////// The name of the animation.
        /////// </param>
        /////// <param name="loop">
        /////// if set to <c>true</c> loop the animation.
        /////// </param>
        /////// <param name="backwards">
        /////// if set to <c>true</c> play the animation backwards.
        /////// </param>
        ////public override void PlayAnimation(string name, bool loop, bool backwards)
        ////{
        ////    this.totalAnimTime = TimeSpan.Zero;

        ////    this.CurrentAnimation = name;
        ////    this.BoundingBoxRefreshed = true;
        ////    this.Loop = loop;
        ////    this.State = AnimationState.Playing;
        ////    this.targetFrame = -1;
        ////    this.Backwards = backwards;
        ////    this.totalAnimTime = TimeSpan.Zero;

        ////    if (backwards)
        ////    {
        ////        this.prevFrame = 0;
        ////        this.frame = this.numFrames - 1;
        ////    }
        ////    else
        ////    {
        ////        this.prevFrame = this.numFrames - 1;
        ////        this.frame = 0;
        ////    }
        ////}

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
        public override void PlayAnimation(string name, int? startFrame, int? endFrame, bool loop = true, bool backwards = false)
        {
            this.CurrentAnimation = name;
            this.BoundingBoxRefreshed = true;
            this.Loop = loop;
            this.State = AnimationState.Playing;
            this.Backwards = backwards;
            this.totalAnimTime = TimeSpan.Zero;

            int start = this.lastFrame = startFrame.HasValue ? startFrame.Value : 0;
            int end = this.targetFrame = endFrame.HasValue ? endFrame.Value : this.numFrames - 1;

            if (backwards)
            {
                this.prevFrame = end;
                this.frame = start;
            }
            else
            {
                this.prevFrame = start;
                this.frame = end;
            }
        }

        /////// <summary>
        /////// Plays the animation up to a given frame.
        /////// </summary>
        /////// <param name="name">
        /////// The name of the animation.
        /////// </param>
        /////// <param name="from">
        /////// The frame where the animation starts playing.
        /////// </param>
        /////// <param name="destFrame">
        /////// The last frame of the animation to play.
        /////// </param>
        /////// <param name="loop">
        /////// if set to <c>true</c> loop the animation.
        /////// </param>
        ////public override void PlayToFrame(string name, int? from, int? destFrame, bool loop = true)
        ////{
        ////    this.CurrentAnimation = name;

        ////    if (from.HasValue)
        ////    {
        ////        this.lastFrame = from.Value;
        ////    }
        ////    else if (this.State == AnimationState.Playing)
        ////    {
        ////        this.totalAnimTime = TimeSpan.Zero;
        ////        this.lastFrame = this.frame;
        ////    }

        ////    this.BoundingBoxRefreshed = true;
        ////    this.Loop = loop;
        ////    this.State = AnimationState.Playing;
        ////    this.targetFrame = destFrame.Value;

        ////    if (destFrame < this.lastFrame)
        ////    {
        ////        this.Backwards = true;
        ////    }
        ////    else
        ////    {
        ////        this.Backwards = false;
        ////    }
        ////}

        /// <summary>
        /// Resume the animation.
        /// </summary>
        public override void ResumeAnimation()
        {
            this.State = AnimationState.Playing;
        }

        /// <summary>
        /// Stops the animation.
        /// </summary>
        public override void StopAnimation()
        {
            this.State = AnimationState.Stopped;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Performs further custom initialization for this instance.
        /// </summary>
        protected override void Initialize()
        {
            this.RefreshAnimationAsset();
        }

        /// <summary>
        /// Refresh animation asset
        /// </summary>
        private void RefreshAnimationAsset()
        {
            if (this.internalAnimation != null && string.IsNullOrEmpty(this.internalAnimation.AssetPath))
            {
                this.Assets.UnloadAsset(this.internalAnimation.AssetPath);
                this.internalAnimation = null;
            }

            if (!string.IsNullOrEmpty(this.animationPath))
            {
                this.internalAnimation = this.Assets.LoadAsset<InternalAnimation>(this.animationPath);
                if (string.IsNullOrEmpty(this.CurrentAnimation))
                {
                    this.CurrentAnimation = this.internalAnimation.Animations.Keys.ToArray()[0];
                }

                this.UpdateNumFrames();
            }

            if (this.PlayAutomatically && !string.IsNullOrEmpty(this.CurrentAnimation))
            {
                this.PlayAnimation(this.CurrentAnimation, this.Loop);
            }
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

                if (!this.Backwards)
                {
                    // Is forward 
                    if (this.targetFrame >= 0)
                    {
                        // TargetFrame enabled
                        this.frame = this.lastFrame + this.internalFrame; // Frame Increment

                        if (this.frame >= this.targetFrame)
                        {
                            // If animation completed
                            this.totalAnimTime = TimeSpan.Zero;

                            if (this.Loop)
                            {
                                // Is looped
                                this.frame = this.numFrames - 1;
                                this.frame = this.lastFrame;
                            }
                            else
                            {
                                this.lastFrame = this.frame = this.targetFrame;
                                this.targetFrame = -1;
                                this.StopAnimation();
                            }
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
                else
                {
                    // Is backwards
                    if (this.targetFrame >= 0)
                    {
                        // TargetFrame enabled
                        this.frame = this.lastFrame - this.internalFrame; // Frame Increment

                        if (this.frame <= this.targetFrame)
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

                this.prevFrame = this.frame;
            }
        }

        /// <summary>
        /// The update num frames.
        /// </summary>
        private void UpdateNumFrames()
        {
            if (this.internalAnimation != null && this.internalAnimation.Animations.Count > 0 && this.internalAnimation.Animations.ContainsKey(this.CurrentAnimation))
            {
                this.currentAnimationSequence = this.internalAnimation.Animations[this.CurrentAnimation];
                this.numFrames = this.currentAnimationSequence.Frames.Count;
            }
        }
        #endregion
    }
}