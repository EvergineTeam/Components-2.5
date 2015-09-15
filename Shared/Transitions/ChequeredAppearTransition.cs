#region File Description
//-----------------------------------------------------------------------------
// ChequeredAppearTransition
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

            this.sourceRenderTarget = this.graphicsDevice.RenderTargets.GetTemporalRenderTarget(this.platform.ScreenWidth, this.platform.ScreenHeight);
            this.targetRenderTarget = this.graphicsDevice.RenderTargets.GetTemporalRenderTarget(this.platform.ScreenWidth, this.platform.ScreenHeight);

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

            this.spriteBatch.DrawVM(this.targetRenderTarget, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, Vector2.One, SpriteEffects.None, 0.5f);

            int width = this.targetRenderTarget.Width;
            int height = this.targetRenderTarget.Height;

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

                        this.spriteBatch.DrawVM(this.sourceRenderTarget, rect, rect, Color.White, 0, Vector2.Zero, SpriteEffects.None, 0);
                    }
                }
            }

            this.spriteBatch.Render();

            this.graphicsDevice.RenderTargets.DestroyRenderTarget(this.sourceRenderTarget);
            this.graphicsDevice.RenderTargets.DestroyRenderTarget(this.targetRenderTarget);
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
