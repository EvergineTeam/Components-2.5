// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

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
    public class CurtainsTransition : ScreenTransition
    {
        /// <summary>
        /// The sprite batch
        /// </summary>
        private SpriteBatch spriteBatch;

        /// <summary>
        /// Initializes a new instance of the <see cref="CurtainsTransition"/> class.
        /// </summary>
        /// <param name="duration">The duration.</param>
        public CurtainsTransition(TimeSpan duration)
            : base(duration)
        {
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
            var sourceRenderTarget = this.graphicsDevice.RenderTargets.GetTemporalRenderTarget(this.platform.ScreenWidth, this.platform.ScreenHeight);
            var targetRenderTarget = this.graphicsDevice.RenderTargets.GetTemporalRenderTarget(this.platform.ScreenWidth, this.platform.ScreenHeight);

            this.DrawSources(gameTime, sourceRenderTarget);
            this.DrawTarget(gameTime, targetRenderTarget);

            this.SetRenderState();
            this.graphicsDevice.RenderTargets.SetRenderTarget(null);
            this.graphicsDevice.Clear(ref this.BackgroundColor, ClearFlags.Target | ClearFlags.DepthAndStencil, 1);
            Vector2 center = new Vector2(sourceRenderTarget.Width / 2, sourceRenderTarget.Height / 2);

            this.spriteBatch.Draw(targetRenderTarget, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, Vector2.One, SpriteEffects.None, 0.5f);

            int wMiddle = (int)(sourceRenderTarget.Width / 2f);
            float inverse = 1 - this.Lerp;
            int w = (int)(wMiddle * inverse * inverse);

            this.spriteBatch.Draw(
                sourceRenderTarget,
                new Rectangle(0, 0, w, sourceRenderTarget.Height),
                new Rectangle(0, 0, wMiddle, sourceRenderTarget.Height),
                Color.White * inverse,
                0,
                Vector2.Zero,
                SpriteEffects.None,
                0);

            this.spriteBatch.Draw(
                sourceRenderTarget,
                new Rectangle(sourceRenderTarget.Width - w, 0, w, sourceRenderTarget.Height),
                new Rectangle(wMiddle, 0, wMiddle, sourceRenderTarget.Height),
                Color.White * inverse,
                0,
                Vector2.Zero,
                SpriteEffects.None,
                0);

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
