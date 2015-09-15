#region File Description
//-----------------------------------------------------------------------------
// CrossFadeTransition
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
    /// This class make an effect between two <see cref="ColorFadeTransition"/> pasing first to a specified color (white for example)
    /// </summary>
    public class CrossFadeTransition : ScreenTransition
    {
        /// <summary>
        /// The sprite batch
        /// </summary>
        private SpriteBatch spriteBatch;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="CrossFadeTransition" /> class.
        /// </summary>
        /// <param name="duration">The transition duration.</param>
        public CrossFadeTransition(TimeSpan duration)
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
            RenderTarget sourceRenderTarget = this.graphicsDevice.RenderTargets.GetTemporalRenderTarget(this.platform.ScreenWidth, this.platform.ScreenHeight);
            RenderTarget targetRenderTarget = this.graphicsDevice.RenderTargets.GetTemporalRenderTarget(this.platform.ScreenWidth, this.platform.ScreenHeight);

            if (this.Sources != null)
            {
                for (int i = 0; i < this.Sources.Length; i++)
                {
                    this.Sources[i].TakeSnapshot(sourceRenderTarget, gameTime);
                }
            }

            if (this.Target != null)
            {
                this.Target.TakeSnapshot(targetRenderTarget, gameTime);
            }

            Color blendColor = Color.White * this.Lerp;

            this.graphicsDevice.RenderTargets.SetRenderTarget(null);
            this.graphicsDevice.Clear(ref this.BackgroundColor, ClearFlags.Target | ClearFlags.DepthAndStencil, 1);

            this.SetRenderState();
            this.spriteBatch.DrawVM(sourceRenderTarget, new Rectangle(0, 0, sourceRenderTarget.Width, sourceRenderTarget.Height), null, Color.White, 0, Vector2.Zero, SpriteEffects.None, 0.5f);
            this.spriteBatch.DrawVM(targetRenderTarget, new Rectangle(0, 0, sourceRenderTarget.Width, sourceRenderTarget.Height), null, blendColor, 0, Vector2.Zero, SpriteEffects.None, 0);
            this.spriteBatch.Render();

            this.graphicsDevice.RenderTargets.DestroyRenderTarget(sourceRenderTarget);
            this.graphicsDevice.RenderTargets.DestroyRenderTarget(targetRenderTarget);
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
