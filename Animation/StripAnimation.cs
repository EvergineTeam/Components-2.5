#region File Description
//-----------------------------------------------------------------------------
// StripAnimation
//
// Copyright © 2014 Wave Corporation
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using WaveEngine.Common.Math;
using WaveEngine.Framework;
#endregion

namespace WaveEngine.Components.Animation
{
    /// <summary>
    /// A Sprite animation where each sequence of the animation is a different Image.
    /// </summary>
    public class StripAnimation
    {
        /// <summary>
        /// The num frames
        /// </summary>
        private int numFrames;

        /// <summary>
        /// The frames
        /// </summary>
        private readonly Rectangle[] frames;

        /// <summary>
        /// The frame length
        /// </summary>
        private float frameLength;

        /// <summary>
        /// The timer
        /// </summary>
        private TimeSpan timer;

        /// <summary>
        /// The current frame
        /// </summary>
        private int currentFrame;

        /// <summary>
        /// The frame width
        /// </summary>
        private readonly int frameWidth;

        /// <summary>
        /// The frame height
        /// </summary>
        private readonly int frameHeight;

        /// <summary>
        /// Whether the animation goes backwards.
        /// </summary>
        private bool backwards;

        #region Properties
        /// <summary>
        /// Gets or sets the number of frames of the animation.
        /// </summary>
        /// <value>
        /// The num frames.
        /// </value>
        public int NumFrames
        {
            get { return this.numFrames; }
            set { this.numFrames = value; }
        }

        /// <summary>
        /// Gets the current frame of the animation.
        /// </summary>
        public Rectangle CurrentFrame
        {
            get { return this.frames[this.currentFrame]; }
        }

        /// <summary>
        /// Gets or sets the current frame.
        /// </summary>
        /// <value>
        /// The current frame.
        /// </value>
        /// <remarks>Invalid values are clamped to a valid one.</remarks>
        public int CurrentFrameIndex
        {
            get
            {
                return this.currentFrame;
            }

            set
            {
                int frame = value;
                if (frame < 0)
                {
                    frame = 0;
                }

                if (frame > this.frames.Length - 1)
                {
                    frame = this.frames.Length - 1;
                }

                this.currentFrame = frame;
            }
        }

        /// <summary>
        /// Gets or sets the speed of the animation.
        /// </summary>
        /// <value>
        /// The speed of the animation.
        /// </value>
        public int FramesPerSecond
        {
            get
            {
                return (int)(1f / this.frameLength);
            }

            set
            {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException("FramePerSecond out of range (> 0)");
                }

                this.frameLength = 1f / (float)value;
            }
        }

        /// <summary>
        /// Gets or sets the height of the animation.
        /// </summary>
        /// <value>
        /// The height.
        /// </value>
        public int Height { get; set; }

        /// <summary>
        /// Gets or sets the width of the animation.
        /// </summary>
        /// <value>
        /// The width.
        /// </value>
        public int Width { get; set; }

        /// <summary>
        /// Gets the width of each frame.
        /// </summary>
        /// <value>
        /// The width of each frame.
        /// </value>
        public int FrameWidth
        {
            get { return this.frameWidth; }
        }

        /// <summary>
        /// Gets the height of each frame.
        /// </summary>
        /// <value>
        /// The height of each frame.
        /// </value>
        public int FrameHeight
        {
            get { return this.frameHeight; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the animation goes backwards.
        /// </summary>
        /// <value>Whether the animation goes backwards.</value>
        public bool Backwards
        {
            get { return this.backwards; }
            set { this.backwards = value; }
        }
        #endregion

        #region Initialize
        /// <summary>
        /// Initializes a new instance of the <see cref="StripAnimation"/> class.
        /// </summary>
        /// <param name="width">The width of the animation.</param>
        /// <param name="height">The height of the animation.</param>
        /// <param name="frameWidth">Width of each frame.</param>
        /// <param name="frameHeight">Height of each frame.</param>
        /// <param name="numFrames">The number of frames.</param>
        public StripAnimation(int width, int height, int frameWidth, int frameHeight, int numFrames) :
            this(width, height, frameWidth, frameHeight, numFrames, 0, 0)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StripAnimation"/> class.
        /// </summary>
        /// <param name="width">The width of the animation.</param>
        /// <param name="height">The height of the animation.</param>
        /// <param name="frameWidth">Width of each frame.</param>
        /// <param name="frameHeight">Height of each frame.</param>
        /// <param name="numFrames">The number frames.</param>
        /// <param name="xOffset">The x offset of each frame inside the Image.</param>
        /// <param name="yOffset">The y offset of each frame inside the Image.</param>
        public StripAnimation(int width, int height, int frameWidth, int frameHeight, int numFrames, int xOffset, int yOffset)
        {
            this.frameLength = 1f / 12f;
            this.timer = TimeSpan.FromSeconds(this.frameLength);
            this.numFrames = numFrames;
            this.frames = new Rectangle[numFrames];

            this.Width = width;
            this.Height = height;
            this.frameWidth = frameWidth;
            this.frameHeight = frameHeight;

            int xLenght, j = 0;
            int tempXOffset = xOffset;
            int tempYOffset = yOffset;
            for (int i = 0; i < numFrames; i++, j++)
            {
                xLenght = tempXOffset + (frameWidth * j);
                if ((xLenght + frameWidth) > width)
                {
                    tempXOffset = 0;
                    tempYOffset += frameHeight;
                    j = 0;
                    xLenght = tempXOffset + (frameWidth * j);
                }

                if ((tempYOffset + frameHeight) > height)
                {
                    throw new InvalidOperationException("The animation's frames are incorrect.");
                }

                this.frames[i] = new Rectangle(xLenght, tempYOffset, frameWidth, frameHeight);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StripAnimation"/> class starting from a collection of frames.
        /// </summary>
        /// <param name="frames">Array of <see cref="Rectangle"/> concerning every frames sequence.</param>
        /// <param name="framesPerSecond">Frames per second.</param>
        public StripAnimation(Rectangle[] frames, int framesPerSecond)
        {
            // TODO: Complete member initialization
            this.frames = frames;
            this.numFrames = frames.Length;
            this.FramesPerSecond = framesPerSecond;
            if ((frames != null) && (frames.Length > 0))
            {
                this.frameWidth = frames[0].Width;
                this.frameHeight = frames[0].Height;
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Updates the animation.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        public void Update(TimeSpan gameTime)
        {
            this.timer -= gameTime;

            if (this.timer < TimeSpan.Zero)
            {
                this.timer = TimeSpan.FromSeconds(this.frameLength);
                this.currentFrame = this.backwards ? 
                    this.currentFrame >= 1 ? 
                        this.currentFrame - 1 
                        : this.frames.Length - 1
                    : (this.currentFrame + 1) % this.frames.Length;
            }
        }

        /// <summary>
        /// Resets the animation.
        /// </summary>
        public void Reset()
        {
            this.currentFrame = 0;
            this.timer = TimeSpan.FromSeconds(this.frameLength);
        }
        #endregion
    }
}
