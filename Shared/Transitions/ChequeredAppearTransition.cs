// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaveEngine.Common.Attributes;
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
    public class ChequeredAppearTransition : ScreenTransition
    {
        /// <summary>
        /// Gets or sets the segments.
        /// </summary>
        private int segments;

        /// <summary>
        /// The sprite batch
        /// </summary>
        private SpriteBatch spriteBatch;

        /// <summary>
        /// Gets or sets the segments.
        /// </summary>
        /// <exception cref="System.ArgumentException">Out of range, segments >= 4</exception>
        public int Segments
        {
            get
            {
                return this.segments;
            }

            set
            {
                if (value < 4)
                {
                    throw new ArgumentException("Out of range, segments >= 4");
                }

                this.segments = value;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChequeredAppearTransition"/> class.
        /// </summary>
        /// <param name="duration">The duration.</param>
        public ChequeredAppearTransition(TimeSpan duration)
            : base(duration)
        {
            this.segments = 8;
            this.spriteBatch = new SpriteBatch(this.graphicsDevice);
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        protected override void Initialize()
        {
        }

        /// <summary>
        /// Updates the specified game time.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        protected override void Update(TimeSpan gameTime)
        {
            this.UpdateSources(gameTime);
            this.UpdateTarget(gameTime);
        }

        /// <summary>
        /// Draws the specified game time.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        protected override void Draw(TimeSpan gameTime)
        {
            System.Random random = new System.Random(23);

            var sourceRenderTarget = this.graphicsDevice.RenderTargets.GetTemporalRenderTarget(this.platform.ScreenWidth, this.platform.ScreenHeight);
            var targetRenderTarget = this.graphicsDevice.RenderTargets.GetTemporalRenderTarget(this.platform.ScreenWidth, this.platform.ScreenHeight);

            this.DrawSources(gameTime, sourceRenderTarget);
            this.DrawTarget(gameTime, targetRenderTarget);

            this.SetRenderState();
            this.graphicsDevice.RenderTargets.SetRenderTarget(null);
            this.graphicsDevice.Clear(ref this.BackgroundColor, ClearFlags.Target | ClearFlags.DepthAndStencil, 1);

            this.spriteBatch.Draw(targetRenderTarget, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, Vector2.One, SpriteEffects.None, 0.5f);

            int width = targetRenderTarget.Width;
            int height = targetRenderTarget.Height;

            for (int x = 0; x < this.segments; x++)
            {
                for (int y = 0; y < this.segments; y++)
                {
                    if (random.NextDouble() > this.Lerp * this.Lerp)
                    {
                        Rectangle rect = new Rectangle(
                            width * x / this.segments,
                            height * y / this.segments,
                            width / this.segments,
                            height / this.segments);

                        this.spriteBatch.Draw(sourceRenderTarget, rect, rect, Color.White, 0, Vector2.Zero, SpriteEffects.None, 0);
                    }
                }
            }

            this.spriteBatch.Render();

            this.graphicsDevice.RenderTargets.ReleaseTemporalRenderTarget(sourceRenderTarget);
            this.graphicsDevice.RenderTargets.ReleaseTemporalRenderTarget(targetRenderTarget);
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
