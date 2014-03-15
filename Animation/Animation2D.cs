#region File Description
//-----------------------------------------------------------------------------
// Animation2D
// Copyright © 2010 - 2013 Wave Coorporation. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using WaveEngine.Common.Math;
using WaveEngine.Components.Graphics2D;
using WaveEngine.Framework;
using WaveEngine.Framework.Animation;
using WaveEngine.Framework.Graphics;
#endregion

namespace WaveEngine.Components.Animation
{
    /// <summary>
    ///     Behavior to control the animations of a Sprite.
    /// </summary>
    /// <remarks>
    ///     Ideally this class should be used to hold all the animations related to a given Sprite.
    /// </remarks>
    public class Animation2D : Behavior
    {
        /// <summary>
        /// Number of instances of this component created.
        /// </summary>
        private static int instances;

        /// <summary>
        ///     Transform of the <see cref="Sprite" />.
        /// </summary>
        [RequiredComponent]
        public Transform2D Transform2D;

        /// <summary>
        /// Gets the Rectangle of the current active animation.
        /// </summary>
        public Rectangle CurrentRectangle
        {
            get;
            internal set;
        }

        /// <summary>
        ///     The animations
        /// </summary>
        private readonly Dictionary<string, StripAnimation> animations;

        /// <summary>
        ///     The current animation
        /// </summary>
        private string currentAnimation;

        /// <summary>
        ///     The state
        /// </summary>
        private AnimationState state;

        /// <summary>
        ///     The target frame
        /// </summary>
        private int? targetFrame;
        
        /// <summary>
        /// Whether to loop current animation
        /// </summary>
        private bool loop;

        /// <summary>
        /// Sprite sheet loader.
        /// </summary>
        private ISpriteSheetLoader spriteSheetLoader;
        
        /// <summary>
        /// Internal frames calc for further dispatching on Add() calls.
        /// </summary>
        private Rectangle[] frames;

        #region Properties

        /// <summary>
        ///     Gets the names of the different animations.
        /// </summary>
        public IEnumerable<string> AnimationNames
        {
            get
            {
                return this.animations.Keys;
            }
        }

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
                this.currentAnimation = value;
                this.animations[this.currentAnimation].CurrentFrameIndex = 0;

                if (this.Transform2D != null)
                {
                    this.Transform2D.Rectangle = new RectangleF();
                    this.Transform2D.Rectangle.Width = this.animations[this.currentAnimation].FrameWidth;
                    this.Transform2D.Rectangle.Height = this.animations[this.currentAnimation].FrameHeight;
                }
            }
        }

        /// <summary>
        ///     Gets the state of the current active animation.
        /// </summary>
        public AnimationState State
        {
            get
            {
                return this.state;
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
            this.animations = new Dictionary<string, StripAnimation>();
            this.loop = true;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Animation2D" /> class.
        /// </summary>
        /// <param name="path">Content-relative path to sprite sheet data.</param>
        /// <param name="loader"><see cref="ISpriteSheetLoader"/> strategy</param>
        private Animation2D(string path, ISpriteSheetLoader loader)
            : this()
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException("Path cannot be null.", "path");
            }

            if (loader == null)
            {
                throw new ArgumentException("Sprite sheet loader cannot be null.", "loader");
            }

            this.spriteSheetLoader = loader;
            this.frames = this.spriteSheetLoader.Parse(path);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Adds an animation to this behavior.
        /// </summary>
        /// <param name="animationName">
        /// Name of the animation to add.
        /// </param>
        /// <param name="animation">
        /// The animation to add.
        /// </param>
        /// <remarks>
        /// If there is already an animation with the same animation name, it will be overwritten.
        /// </remarks>
        /// <returns>
        /// The <see cref="Animation2D"/> it-self.
        /// </returns>
        public Animation2D Add(string animationName, StripAnimation animation)
        {
            this.animations.Add(animationName, animation);

            if (this.currentAnimation == null)
            {
                this.CurrentAnimation = this.AnimationNames.First();
                this.CurrentRectangle = this.animations[this.currentAnimation].CurrentFrame;
            }

            return this;
        }

        /// <summary>
        /// Adds a new animation specified within a sprite sheet document.
        /// </summary>
        /// <param name="name">Animation name.</param>
        /// <param name="sequence">Animation sequence.</param>
        /// <returns>The <see cref="Animation2D"/> it-self.</returns>
        /// <remarks>An <see cref="ISpriteSheetLoader"/> must be specified prior to this call.</remarks>
        public Animation2D Add(string name, SpriteSheetAnimationSequence sequence)
        { 
            if (this.spriteSheetLoader == null)
            {
                // TODO: Maybe it could fall on a default loader (Texture Packer's Generic XML, for instance)
                throw new InvalidOperationException("A sprite sheet loader must be provided in advance.");
            }
            
            if (sequence == null)
            {
                throw new ArgumentException("Sequence cannot be null.", "sequence");
            }

            this.Add(name, 
                new StripAnimation(this.frames.Skip(sequence.First - 1).Take(sequence.Length).ToArray(),
                    sequence.FramesPerSecond));

            return this;
        }

        /// <summary>
        /// Plays the active animation just once.
        /// </summary>
        public void Play()
        {
            this.Play(false);
        }

        /// <summary>
        /// Plays the active animation.
        /// </summary>
        /// <param name="loop">Whether to loop the animation once it ends.</param>
        public void Play(bool loop)
        {
            this.Play(loop, false);
        }

        /// <summary>
        /// Plays the active animation.
        /// </summary>
        /// <param name="loop">Whether to loop the animation once it ends.</param>
        /// <param name="backwards">Whether the animation goes backwards.</param>
        public void Play(bool loop, bool backwards)
        {
            this.SetFrame(0);
            this.loop = loop;
            this.state = AnimationState.Playing;
            this.animations[this.currentAnimation].Backwards = backwards;
        }

        /// <summary>
        /// Plays the active animation starting from a given frame.
        /// </summary>
        /// <param name="targetFrame">
        /// The target frame.
        /// </param>
        public void PlayToFrame(int targetFrame)
        {
            this.SetFrame(0);
            this.targetFrame = targetFrame;
            this.state = AnimationState.Playing;
        }

        /// <summary>
        /// Removes an animation.
        /// </summary>
        /// <param name="animationName">
        /// Name of the animation to remove.
        /// </param>
        public void Remove(string animationName)
        {
            this.animations.Remove(animationName);
        }

        /// <summary>
        /// Sets the frame for the current active animation.
        /// </summary>
        /// <param name="frame">
        /// The frame index.
        /// </param>
        public void SetFrame(int frame)
        {
            StripAnimation stripAnimation = this.animations[this.currentAnimation];
            stripAnimation.CurrentFrameIndex = frame;
            this.CurrentRectangle = this.animations[this.currentAnimation].CurrentFrame;
        }

        /// <summary>
        ///     Stops the current active animation.
        /// </summary>
        public void Stop()
        {
            this.state = AnimationState.Stopped;
        }

        /// <summary>
        /// Creates a new instance of <see cref="Animation2D"/> based on the <see cref="ISpriteSheetLoader"/>.
        /// </summary>
        /// <typeparam name="T">Sprite sheet loader strategy.</typeparam>
        /// <param name="path">Content-relative path to sprite sheet data.</param>
        /// <returns>A new instance of <see cref="Animation2D"/>.</returns>
        public static Animation2D Create<T>(string path) where T : class, ISpriteSheetLoader
        {
            return new Animation2D(path, Activator.CreateInstance<T>());
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///     Performs further custom initialization for this instance.
        /// </summary>
        protected override void 
            Initialize()
        {
            this.Transform2D.Rectangle = new RectangleF();
            this.Transform2D.Rectangle.Width = this.CurrentRectangle.Width;
            this.Transform2D.Rectangle.Height = this.CurrentRectangle.Height;
        }

        /// <summary>
        /// Updates the animation.
        /// </summary>
        /// <param name="gameTime">
        /// The game time.
        /// </param>
        protected override void Update(TimeSpan gameTime)
        {
            if (this.state == AnimationState.Playing && !string.IsNullOrEmpty(this.currentAnimation))
            {
                StripAnimation stripAnimation = this.animations[this.currentAnimation];
                if (this.targetFrame.HasValue && (this.targetFrame.Value == stripAnimation.CurrentFrameIndex))
                {
                    this.Stop();
                    this.targetFrame = null;
                    return;
                }

                if (!this.loop && stripAnimation.CurrentFrameIndex + 1 >= stripAnimation.NumFrames)
                {
                    this.Stop();
                    return;
                }

                stripAnimation.Update(gameTime);
                this.CurrentRectangle = stripAnimation.CurrentFrame;
            }
        }

        #endregion
    }
}