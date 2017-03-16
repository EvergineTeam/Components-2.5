#region File Description
//-----------------------------------------------------------------------------
// ColorFadeTransition
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
using WaveEngine.Framework.Animation;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Resources;
using WaveEngine.Framework.Services;
#endregion

namespace WaveEngine.Components.Transitions
{
    /// <summary>
    /// This class make an effect between two <see cref="ColorFadeTransition"/> pasing first to a specified color (white for example)
    /// </summary>
    public class ColorFadeTransition : ScreenTransition
    {
        /// <summary>
        /// Source transition renderTarget
        /// </summary>
        private RenderTarget sourceRenderTarget;

        /// <summary>
        /// Target transition renderTarget
        /// </summary>
        private RenderTarget targetRenderTarget;

        /// <summary>
        /// The transition color
        /// </summary>
        private Color transitionColor;

        /// <summary>
        /// The sprite batch
        /// </summary>
        private SpriteBatch spriteBatch;

        /// <summary>
        /// Initializes a new instance of the <see cref="ColorFadeTransition" /> class.
        /// </summary>
        /// <param name="transitionColor">The transition color.</param>
        /// <param name="duration">The transition duration.</param>
        public ColorFadeTransition(Color transitionColor, TimeSpan duration)
            : base(duration)
        {
            this.transitionColor = transitionColor;
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
            this.sourceRenderTarget = this.graphicsDevice.RenderTargets.GetTemporalRenderTarget(this.platform.ScreenWidth, this.platform.ScreenHeight);
            this.targetRenderTarget = this.graphicsDevice.RenderTargets.GetTemporalRenderTarget(this.platform.ScreenWidth, this.platform.ScreenHeight);

            this.DrawSources(gameTime, this.sourceRenderTarget);
            this.DrawTarget(gameTime, this.targetRenderTarget);

            float factor = (this.Lerp <= 0.5f) ? 2 * this.Lerp : 1 - (2 * (this.Lerp - 0.5f));
            Color blendColor = this.transitionColor * factor;

            this.SetRenderState();
            this.graphicsDevice.RenderTargets.SetRenderTarget(null);
            this.graphicsDevice.Clear(ref this.BackgroundColor, ClearFlags.Target | ClearFlags.DepthAndStencil, 1);

            if (this.Lerp <= 0.5f)
            {
                this.spriteBatch.Draw(this.sourceRenderTarget, new Rectangle(0, 0, this.sourceRenderTarget.Width, this.sourceRenderTarget.Height), null, Color.White, 0, Vector2.Zero, SpriteEffects.None, 0.5f);
            }
            else
            {
                this.spriteBatch.Draw(this.targetRenderTarget, new Rectangle(0, 0, this.sourceRenderTarget.Width, this.sourceRenderTarget.Height), null, Color.White, 0, Vector2.Zero, SpriteEffects.None, 0.5f);
            }

            this.spriteBatch.Draw(StaticResources.WhitePixel, new Rectangle(0, 0, this.platform.ScreenWidth, this.platform.ScreenHeight), null, blendColor, 0, Vector2.Zero, SpriteEffects.None, 0);
            this.spriteBatch.Render();

            this.graphicsDevice.RenderTargets.ReleaseTemporalRenderTarget(this.sourceRenderTarget);
            this.graphicsDevice.RenderTargets.ReleaseTemporalRenderTarget(this.targetRenderTarget);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
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