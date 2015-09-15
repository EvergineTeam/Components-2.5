#region File Description
//-----------------------------------------------------------------------------
// Animation2D
// Copyright © 2015 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using WaveEngine.Common.Math;
using WaveEngine.Components.Graphics2D;
using WaveEngine.Framework;
using WaveEngine.Framework.Animation;
using WaveEngine.Framework.Graphics;
using System.Reflection;
using WaveEngine.Common.Attributes;
using WaveEngine.Common.Helpers;
using System.Text;
#endregion

namespace WaveEngine.Components.Animation
{
    /// <summary>
    ///     Behavior to control the animations of a Sprite.
    /// </summary>
    /// <remarks>
    ///     Ideally this class should be used to hold all the animations related to a given Sprite.
    /// </remarks>
    [DataContract(Namespace = "WaveEngine.Components.Animation")]
    public class Animation2D : AnimationBase
    {
        /// <summary>
        /// Number of instances of this component created.
        /// </summary>
        private static int instances;

        /// <summary>
        /// Current animation key frame events
        /// </summary>
        private Dictionary<int, string> currentKeyFrameEvents;

        /// <summary>
        /// The frames per second
        /// </summary>
        private int framesPerSecond;

        /// <summary>
        ///     Transform of the <see cref="Sprite" />.
        /// </summary>
        [RequiredComponent]
        public Transform2D Transform2D;

        /// <summary>
        ///     The SpriteSheet component
        /// </summary>
        [RequiredComponent]
        public SpriteAtlas SpriteAtlas;

        /// <summary>
        ///     The current animation
        /// </summary>
        [DataMember]
        private string currentAnimation;

        /// <summary>
        /// Current animation frame
        /// </summary>
        private int currentAnimationFrame;

        /// <summary>
        /// The current animation
        /// </summary>
        private SpriteSheetAnimation currentSpriteSheetAnimation;

        /// <summary>
        /// Current animation time
        /// </summary>
        private double currentAnimationTime;

        /// <summary>
        /// Current animation endtime
        /// </summary>
        private double currentAnimationEndTime;

        /// <summary>
        /// Previous frame
        /// </summary>
        private int previousAnimationFrame;

        /// <summary>
        /// Current animation start frame
        /// </summary>
        private int startFrame;

        /// <summary>
        /// Current animation end frame
        /// </summary>
        private int endFrame;

        /// <summary>
        /// Current animation length in frames
        /// </summary>
        private int frameLength;

        /// <summary>
        /// Time animation factor
        /// </summary>
        private float timeFactor;

        /// <summary>
        /// Frame increment
        /// </summary>
        private int frameIncrement;

        /// <summary>
        /// flags that indicate that an animation must be played in the next update cicle
        /// </summary>
        private bool playAnimation;

        /// <summary>
        ///     Raised when a certain frame of an animation is played.
        /// </summary>
        public override event EventHandler<StringEventArgs> OnKeyFrameEvent;

        #region Properties
        /// <summary>
        /// Gets or sets the current frame of the animation.
        /// </summary>
        [DontRenderProperty]
        public override int Frame
        {
            get
            {
                return this.currentAnimationFrame;
            }

            set
            {
                if (this.currentSpriteSheetAnimation != null
                    && value >= 0
                    && this.currentSpriteSheetAnimation.Length > value)
                {
                    this.SetFrame(value);
                }
            }
        }

        /// <summary>
        /// Gets or sets the Animation speed factor
        /// </summary>
        public float SpeedFactor
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the names of the different animations.
        /// </summary>
        public override IEnumerable<string> AnimationNames
        {
            get
            {
                if (this.SpriteSheetAnimations != null)
                {
                    return this.SpriteSheetAnimations.Keys;
                }

                return null;
            }
        }

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
                this.currentAnimation = value;

                var animations = this.SpriteSheetAnimations;
                if (animations != null)
                {
                    if (animations.TryGetValue(this.currentAnimation, out this.currentSpriteSheetAnimation))
                    {
                        this.currentAnimationFrame = 0;
                        this.SpriteAtlas.TextureIndex = this.currentSpriteSheetAnimation.First;
                    }
                }

                if (!string.IsNullOrEmpty(this.currentAnimation) && this.PlayAutomatically)
                {
                    this.playAnimation = true;
                }
            }
        }

        /// <summary>
        /// Gets the sprite sheet animation dictionary
        /// </summary>
        private Dictionary<string, SpriteSheetAnimation> SpriteSheetAnimations
        {
            get
            {
                if (this.SpriteAtlas != null && this.SpriteAtlas.SpriteSheet != null)
                {
                    return this.SpriteAtlas.SpriteSheet.SpriteAnimations;
                }

                return null;
            }
        }
        #endregion

        #region Initialize

        /// <summary>
        ///     Initializes a new instance of the <see cref="Animation2D" /> class.
        /// </summary>
        public Animation2D()
            : base("SpriteSheet" + instances++)
        {
            this.Family = FamilyType.DefaultBehavior;
        }

        /// <summary>
        /// Sets the default values
        /// </summary>
        protected override void DefaultValues()
        {
            base.DefaultValues();
            this.Loop = true;
            this.SpeedFactor = 1;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Initialzie the component
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();

            if (!string.IsNullOrEmpty(this.currentAnimation) && this.PlayAutomatically)
            {
                this.playAnimation = true;
            }
        }

        /// <summary>
        /// Plays the animation between the specified frames.
        /// </summary>
        /// <param name="name">The name of the animation.</param>
        /// <param name="startFrame">The frame where the animation starts playing.</param>
        /// <param name="endFrame">The last frame of the animation to play.</param>
        /// <param name="loop">if set to <c>true</c> loop the animation.</param>
        /// <param name="backwards">if set to <c>true</c> play the animation backwards.</param>
        public override void PlayAnimation(string name, int? startFrame, int? endFrame, bool loop = true, bool backwards = false)
        {
            this.PlayAnimation(name, startFrame, endFrame, loop, backwards, null);
        }

        /// <summary>
        /// Plays the animation between the specified frames.
        /// </summary>
        /// <param name="name">The name of the animation.</param>
        /// <param name="startFrame">The frame where the animation starts playing.</param>
        /// <param name="endFrame">The last frame of the animation to play.</param>
        /// <param name="loop">if set to <c>true</c> loop the animation.</param>
        /// <param name="backwards">if set to <c>true</c> play the animation backwards.</param>
        /// <param name="framesPerSecond">The frames per second.</param>
        public void PlayAnimation(string name, int? startFrame, int? endFrame, bool loop = true, bool backwards = false, int? framesPerSecond = null)
        {
            this.CurrentAnimation = name;

            if (this.currentSpriteSheetAnimation != null)
            {
                this.keyFrameEvents.TryGetValue(name, out this.currentKeyFrameEvents);

                // Check start frame
                if (startFrame.HasValue)
                {
                    if (startFrame.Value < 0)
                    {
                        this.startFrame = 0;
                    }
                    else if (startFrame.Value >= this.currentSpriteSheetAnimation.Length - 1)
                    {
                        this.startFrame = this.currentSpriteSheetAnimation.Length - 2;
                    }
                    else
                    {
                        this.startFrame = startFrame.Value;
                    }
                }
                else
                {
                    this.startFrame = 0;
                }

                // Check end frame
                if (endFrame.HasValue)
                {
                    if (endFrame.Value < this.startFrame)
                    {
                        this.endFrame = this.startFrame + 1;
                    }
                    else if (endFrame.Value >= this.currentSpriteSheetAnimation.Length)
                    {
                        this.endFrame = this.currentSpriteSheetAnimation.Length - 1;
                    }
                    else
                    {
                        this.endFrame = endFrame.Value;
                    }
                }
                else
                {
                    this.endFrame = this.currentSpriteSheetAnimation.Length - 1;
                }

                if (framesPerSecond.HasValue)
                {
                    this.framesPerSecond = framesPerSecond.Value;
                }
                else
                {
                    this.framesPerSecond = this.currentSpriteSheetAnimation.FramesPerSecond;
                }

                if (this.startFrame < 0)
                {
                    this.startFrame = 0;
                }

                if (this.endFrame < 0)
                {
                    this.endFrame = 0;
                }

                this.frameLength = this.endFrame - this.startFrame + 1;

                this.SetFrame(0);
                this.Loop = loop;
                this.State = AnimationState.Playing;
                this.Backwards = backwards;

                this.currentAnimationTime = 0;
                this.currentAnimationEndTime = this.frameLength / (double)this.framesPerSecond;
            }
        }

        /// <summary>
        /// Sets the frame for the current active animation.
        /// </summary>
        /// <param name="frame">
        /// The frame index.
        /// </param>
        public void SetFrame(int frame)
        {
            if (this.currentSpriteSheetAnimation != null)
            {
                this.currentAnimationFrame = frame;
                this.SpriteAtlas.TextureIndex = this.currentSpriteSheetAnimation.First + this.startFrame + frame;
                this.currentAnimationTime = frame / (double)this.framesPerSecond;
                this.previousAnimationFrame = this.currentAnimationFrame;
            }
        }

        /// <summary>
        /// Resume the animation.
        /// </summary>
        public override void ResumeAnimation()
        {
            this.State = AnimationState.Playing;
        }

        /// <summary>
        ///     Stops the animation.
        /// </summary>
        public override void StopAnimation()
        {
            this.State = AnimationState.Stopped;
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Updates the animation.
        /// </summary>
        /// <param name="gameTime">
        /// The game time.
        /// </param>
        protected override void Update(TimeSpan gameTime)
        {
            // Detect if there are pending animations
            if (this.playAnimation && this.SpriteSheetAnimations != null)
            {
                this.PlayAnimation(this.currentAnimation, this.Loop, this.Backwards);
                this.playAnimation = false;
            }

            if (this.State == AnimationState.Playing)
            {
                this.frameIncrement = (!this.Backwards) ? 1 : -1;
                this.timeFactor = this.SpeedFactor * this.frameIncrement;

                this.currentAnimationTime += gameTime.TotalSeconds * this.timeFactor;

                // Handle end animation
                if (!this.Backwards)
                {
                    if (this.currentAnimationTime > this.currentAnimationEndTime)
                    {
                        this.HandleEndAnimation();
                    }
                }
                else if (this.currentAnimationTime < 0)
                {
                    this.HandleEndAnimation();
                }

                // Calc the new frame
                this.currentAnimationFrame = this.startFrame + (((int)(this.currentAnimationTime * this.framesPerSecond) + this.frameLength) % this.frameLength);
                this.SpriteAtlas.TextureIndex = this.currentSpriteSheetAnimation.First + this.currentAnimationFrame;

                this.HandleKeyFrameEvents();

                this.previousAnimationFrame = this.currentAnimationFrame;
            }
        }

        /// <summary>
        /// Handle End animation
        /// </summary>
        private void HandleEndAnimation()
        {
            if (!this.Loop)
            {
                this.currentAnimationFrame = this.frameLength - 1;
                this.StopAnimation();
                return;
            }
            else
            {
                this.currentAnimationTime -= this.currentAnimationEndTime * this.timeFactor;
            }
        }

        /// <summary>
        /// Handle key frame events
        /// </summary>
        private void HandleKeyFrameEvents()
        {
            if (this.OnKeyFrameEvent != null && this.currentKeyFrameEvents != null)
            {
                if (this.currentAnimationFrame != this.previousAnimationFrame)
                {
                    int increment = this.Backwards ? -1 : 1;
                    int i = this.previousAnimationFrame;
                    int counter = this.currentAnimationFrame - this.startFrame;

                    while (i != this.currentAnimationFrame)
                    {
                        i = this.startFrame + (counter % this.frameLength);
                        counter += increment;

                        string text;
                        if (this.currentKeyFrameEvents.TryGetValue(i, out text))
                        {
                            this.OnKeyFrameEvent(this, new StringEventArgs(text));
                        }
                    }
                }
            }
        }
        #endregion
    }
}