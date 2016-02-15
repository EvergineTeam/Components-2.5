#region File Description
//-----------------------------------------------------------------------------
// FanTransition
//
// Copyright © 2016 Wave Engine S.L. All rights reserved.
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
    public class FanTransition : ScreenTransition
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
        /// Initializes a new instance of the <see cref="FanTransition"/> class.
        /// </summary>
        /// <param name="duration">The duration.</param>
        public FanTransition(TimeSpan duration)
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

            float rotation = MathHelper.PiOver2 * this.Lerp;

            this.spriteBatch.Draw(this.sourceRenderTarget,
                                    Vector2.Zero,
                                    null,
                                    Color.White, 
                                    rotation,
                                    Vector2.Zero,
                                    Vector2.One, 
                                    SpriteEffects.None, 
                                    0.5f);

            rotation -= MathHelper.PiOver2;

            this.spriteBatch.Draw(this.targetRenderTarget,
                                    Vector2.Zero,
                                    null,
                                    Color.White,
                                    rotation,
                                    Vector2.Zero,
                                    Vector2.One,
                                    SpriteEffects.None,
                                    0.5f);

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
