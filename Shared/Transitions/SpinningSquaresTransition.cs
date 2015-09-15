#region File Description
//-----------------------------------------------------------------------------
// SpinningSquaresTransition
//
// Copyright © 2015 Wave Engine S.L. All rights reserved.
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
    public class SpinningSquaresTransition : ScreenTransition
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
        /// Source transition renderTarget
        /// </summary>
        private RenderTarget sourceRenderTarget;

        /// <summary>
        /// Target transition renderTarget
        /// </summary>
        private RenderTarget targetRenderTarget;

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
        /// Initializes a new instance of the <see cref="SpinningSquaresTransition"/> class.
        /// </summary>
        /// <param name="duration">The duration.</param>
        public SpinningSquaresTransition(TimeSpan duration)
            : base(duration)
        {
            this.spriteBatch = new SpriteBatch(this.graphicsDevice);
            this.segments = 8;
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
            this.sourceRenderTarget = this.graphicsDevice.RenderTargets.GetTemporalRenderTarget(
                this.platform.ScreenWidth,
                this.platform.ScreenHeight);
            this.targetRenderTarget = this.graphicsDevice.RenderTargets.GetTemporalRenderTarget(
                this.platform.ScreenWidth,
                this.platform.ScreenHeight);

            this.DrawSources(gameTime, this.sourceRenderTarget);
            this.DrawTarget(gameTime, this.targetRenderTarget);

            this.SetRenderState();
            this.graphicsDevice.RenderTargets.SetRenderTarget(null);
            this.graphicsDevice.Clear(ref this.BackgroundColor, ClearFlags.Target | ClearFlags.DepthAndStencil, 1);
            Vector2 center = new Vector2(this.sourceRenderTarget.Width / 2, this.sourceRenderTarget.Height / 2);

            this.spriteBatch.DrawVM(this.targetRenderTarget, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, Vector2.One, SpriteEffects.None, 0.5f);

            System.Random random = new System.Random(23);

            for (int x = 0; x < this.segments; x++)
            {
                for (int y = 0; y < this.segments; y++)
                {
                    Rectangle rect = new Rectangle(
                        this.targetRenderTarget.Width * x / this.segments,
                        this.targetRenderTarget.Height * y / this.segments,
                        this.targetRenderTarget.Width / this.segments,
                        this.targetRenderTarget.Height / this.segments);

                    Vector2 origin = new Vector2(rect.Width, rect.Height) / 2;
                    float inverse = 1 - this.Lerp;

                    float rotation = (float)(random.NextDouble() - 0.5) * this.Lerp * 2;
                    Vector2 scale = new Vector2(1 + (float)((random.NextDouble() - 0.5f) * this.Lerp));

                    Vector2 pos = new Vector2(rect.Center.X, rect.Center.Y);

                    pos.X += (float)(random.NextDouble() - 0.5) * this.Lerp * (this.targetRenderTarget.Width / 2);
                    pos.Y += (float)(random.NextDouble() - 0.5) * this.Lerp * (this.targetRenderTarget.Height / 2);

                    this.spriteBatch.DrawVM(this.sourceRenderTarget, pos, rect, Color.White * inverse, rotation, origin, scale, 0, 0);
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
