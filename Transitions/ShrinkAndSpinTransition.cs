#region File Description
//-----------------------------------------------------------------------------
// ShrinkAndSpinTransition
//
// Copyright © 2014 Wave Corporation
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
    public class ShrinkAndSpinTransition : ScreenTransition
    {
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
        /// Initializes a new instance of the <see cref="ShrinkAndSpinTransition"/> class.
        /// </summary>
        /// <param name="duration">The duration.</param>
        public ShrinkAndSpinTransition(TimeSpan duration)
            : base(duration)
        {
            this.spriteBatch = new SpriteBatch(this.graphicsDevice);
            this.sourceRenderTarget = this.graphicsDevice.RenderTargets.CreateRenderTarget(
                WaveServices.Platform.ScreenWidth,
                WaveServices.Platform.ScreenHeight);
            this.targetRenderTarget = this.graphicsDevice.RenderTargets.CreateRenderTarget(
                WaveServices.Platform.ScreenWidth,
                WaveServices.Platform.ScreenHeight);
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
            this.UpdateSources(gameTime, this.sourceRenderTarget);
            this.UpdateTarget(gameTime, this.targetRenderTarget);
        }

        /// <summary>
        /// Draws the specified game time.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        protected override void Draw(TimeSpan gameTime)
        {
            System.Random random = new System.Random(23);

            this.DrawSources(gameTime, this.sourceRenderTarget);
            this.DrawTarget(gameTime, this.targetRenderTarget);

            this.graphicsDevice.RenderTargets.SetRenderTarget(null);
            this.graphicsDevice.Clear(ref this.BackgroundColor, ClearFlags.Target | ClearFlags.DepthAndStencil, 1);
            Vector2 center = new Vector2(this.sourceRenderTarget.Width / 2, this.sourceRenderTarget.Height / 2);

            this.spriteBatch.Begin(BlendMode.AlphaBlend, DepthMode.None);
            this.spriteBatch.Draw(this.targetRenderTarget, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, Vector2.One, SpriteEffects.None, 0.5f);

            float inverse = 1 - this.Lerp;
            Vector2 translate = (new Vector2(32, this.sourceRenderTarget.Height - 32) - center) * this.Lerp;

            float rotation = this.Lerp * -2;
            Vector2 scale = new Vector2(inverse);

            Color tint = Color.White * (float)Math.Sqrt(inverse);

            this.spriteBatch.Draw(this.sourceRenderTarget, center + translate, null, tint, rotation, center, scale, SpriteEffects.None, 0);
            
            this.spriteBatch.End();
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
                    this.graphicsDevice.RenderTargets.DestroyRenderTarget(this.sourceRenderTarget);
                    this.graphicsDevice.RenderTargets.DestroyRenderTarget(this.targetRenderTarget);
                }

                this.disposed = true;
            }
        }
    }
}
