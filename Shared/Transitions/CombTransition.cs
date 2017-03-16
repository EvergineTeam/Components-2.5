#region File Description
//-----------------------------------------------------------------------------
// CombTransition
//
// Copyright © 2017 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Math;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Services;
#endregion

namespace WaveEngine.Components.Transitions
{
    /// <summary>
    /// Transition effect where each square of the image appears at a different time.
    /// </summary>
    public class CombTransition : ScreenTransition
    {
        /// <summary>
        /// The direction of this effect.
        /// </summary>
        public enum EffectOptions
        {
            /// <summary>
            /// The horizontal
            /// </summary>
            Horizontal,

            /// <summary>
            /// The vertical
            /// </summary>
            Vertical,
        }

        /// <summary>
        /// Gets or sets the segments.
        /// </summary>
        /// <value>
        /// The segments.
        /// </value>
        private int segments;

        /// <summary>
        /// The sprite batch
        /// </summary>
        private SpriteBatch spriteBatch;

        /// <summary>
        /// Source transition renderTarget
        /// </summary>
        private RenderTarget sourceRenderTarget;

        /// <summary>
        /// Target transition renderTarget
        /// </summary>
        private RenderTarget targetRenderTarget;

        /// <summary>
        /// The transition options
        /// </summary>
        private EffectOptions effectOption;

        /// <summary>
        /// The position
        /// </summary>
        private Vector2 position1;

        /// <summary>
        /// The position
        /// </summary>
        private Vector2 position2;

        /// <summary>
        /// The initial position
        /// </summary>
        private Vector2 initialPosition;

        /// <summary>
        /// The target direction
        /// </summary>
        private Vector2 targetPosition;

        /// <summary>
        /// Gets or sets the segments.
        /// </summary>
        /// <exception cref="System.ArgumentException">Out of range, segments >= 3</exception>
        public int Segments
        {
            get
            {
                return this.segments;
            }

            set
            {
                if (value < 3)
                {
                    throw new ArgumentException("Out of range, segments >= 3");
                }

                this.segments = value;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CombTransition" /> class.
        /// </summary>
        /// <param name="duration">The duration.</param>
        /// <param name="effect">The effect.</param>
        public CombTransition(TimeSpan duration, EffectOptions effect)
            : base(duration)
        {
            this.segments = 5;
            this.spriteBatch = new SpriteBatch(this.graphicsDevice);
            this.effectOption = effect;
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        protected override void Initialize()
        {
            switch (this.effectOption)
            {
                case EffectOptions.Horizontal:
                    this.initialPosition = new Vector2(this.platform.ScreenWidth, 0);
                    break;
                case EffectOptions.Vertical:
                    this.initialPosition = new Vector2(0, -this.platform.ScreenHeight);
                    break;
            }

            this.targetPosition = new Vector2(0, 0);

            this.position1 = this.initialPosition;
            this.position2 = this.targetPosition;
        }

        /// <summary>
        /// Updates the specified game time.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        protected override void Update(TimeSpan gameTime)
        {
            this.UpdateSources(gameTime);
            this.UpdateTarget(gameTime);

            this.position1 = (this.targetPosition - this.initialPosition) * this.Lerp;
            this.position2 = (this.initialPosition - this.targetPosition) * this.Lerp;
        }

        /// <summary>
        /// Draws the specified game time.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        protected override void Draw(TimeSpan gameTime)
        {
            this.sourceRenderTarget = this.graphicsDevice.RenderTargets.GetTemporalRenderTarget(
                this.platform.ScreenWidth,
                this.platform.ScreenHeight);
            this.targetRenderTarget = this.graphicsDevice.RenderTargets.GetTemporalRenderTarget(
                this.platform.ScreenWidth,
                this.platform.ScreenHeight);

            if (this.Sources != null)
            {
                for (int i = 0; i < this.Sources.Length; i++)
                {
                    this.Sources[i].TakeSnapshot(this.sourceRenderTarget, gameTime);
                }
            }

            this.Target.TakeSnapshot(this.targetRenderTarget, gameTime);

            this.SetRenderState();
            this.graphicsDevice.RenderTargets.SetRenderTarget(null);
            this.graphicsDevice.Clear(ref this.BackgroundColor, ClearFlags.Target | ClearFlags.DepthAndStencil, 1);

            int wSegment = this.targetRenderTarget.Width / this.segments;
            int hSegment = this.targetRenderTarget.Height / this.segments;

            for (int i = 0; i < this.segments; i++)
            {
                Rectangle rect;
                if (this.effectOption == EffectOptions.Horizontal)
                {
                    rect = new Rectangle(0, hSegment * i, this.targetRenderTarget.Width, hSegment);

                    Rectangle destination = rect;

                    if ((i % 2) == 0)
                    {
                        destination.X = (int)this.position1.X;
                        this.spriteBatch.Draw(this.sourceRenderTarget, destination, rect, Color.White, 0, Vector2.Zero, SpriteEffects.None, 0.5f);

                        destination.X = (int)this.position1.X + (int)this.initialPosition.X;
                        this.spriteBatch.Draw(this.targetRenderTarget, destination, rect, Color.White, 0, Vector2.Zero, SpriteEffects.None, 0);
                    }
                    else
                    {
                        destination.X = (int)this.position2.X;
                        this.spriteBatch.Draw(this.sourceRenderTarget, destination, rect, Color.White, 0, Vector2.Zero, SpriteEffects.None, 0.5f);

                        destination.X = (int)this.position2.X - (int)this.initialPosition.X;
                        this.spriteBatch.Draw(this.targetRenderTarget, destination, rect, Color.White, 0, Vector2.Zero, SpriteEffects.None, 0);
                    }
                }
                else
                {
                    rect = new Rectangle(wSegment * i, 0, wSegment, this.targetRenderTarget.Height);

                    Rectangle destination = rect;

                    if ((i % 2) == 0)
                    {
                        destination.Y = (int)this.position1.Y;
                        this.spriteBatch.Draw(this.sourceRenderTarget, destination, rect, Color.White, 0, Vector2.Zero, SpriteEffects.None, 0.5f);

                        destination.Y = (int)this.position1.Y + (int)this.initialPosition.Y;
                        this.spriteBatch.Draw(this.targetRenderTarget, destination, rect, Color.White, 0, Vector2.Zero, SpriteEffects.None, 0);
                    }
                    else
                    {
                        destination.Y = (int)this.position2.Y;
                        this.spriteBatch.Draw(this.sourceRenderTarget, destination, rect, Color.White, 0, Vector2.Zero, SpriteEffects.None, 0.5f);

                        destination.Y = (int)this.position2.Y - (int)this.initialPosition.Y;
                        this.spriteBatch.Draw(this.targetRenderTarget, destination, rect, Color.White, 0, Vector2.Zero, SpriteEffects.None, 0);
                    }
                }
            }

            this.spriteBatch.Render();

            this.graphicsDevice.RenderTargets.ReleaseTemporalRenderTarget(this.sourceRenderTarget);
            this.graphicsDevice.RenderTargets.ReleaseTemporalRenderTarget(this.targetRenderTarget);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    this.spriteBatch.Dispose();
                }

                this.disposed = true;
            }
        }
    }
}
