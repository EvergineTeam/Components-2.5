#region File Description
//-----------------------------------------------------------------------------
// ColorFadeTransition
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
using WaveEngine.Framework.Animation;
using WaveEngine.Framework.Graphics;
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
        /// The transition color
        /// </summary>
        private Color transitionColor;

        /// <summary>
        /// The sprite batch
        /// </summary>
        private SpriteBatch spriteBatch;

        /// <summary>
        /// The transition color texture
        /// </summary>
        private Texture2D transitionColorTexture;

        /// <summary>
        /// Target transition renderTarget
        /// </summary>
        private RenderTarget renderTarget;

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

            this.transitionColorTexture = new Texture2D()
            {
                Width = 1,
                Height = 1,
                Levels = 1,
                Data = new byte[1][][] { new byte[1][] { new byte[] { 255, 255, 255, 255 } } },
            };
            this.graphicsDevice.Textures.UploadTexture(this.transitionColorTexture);
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
            if (this.Lerp <= 0.5f)
            {
                this.UpdateSources(gameTime);
            }
            else
            {
                this.UpdateTarget(gameTime);
            }
        }

        /// <summary>
        /// Draws the specified game time.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        protected override void Draw(TimeSpan gameTime)
        {
            this.renderTarget = this.graphicsDevice.RenderTargets.GetTemporalRenderTarget(this.platform.ScreenWidth, this.platform.ScreenHeight);

            if (this.Lerp <= 0.5f)
            {
                if (this.Sources != null)
                {
                    for (int i = 0; i < this.Sources.Length; i++)
                    {
                        this.Sources[i].TakeSnapshot(this.renderTarget, gameTime);
                    }
                }
            }
            else
            {
                this.Target.TakeSnapshot(this.renderTarget, gameTime);
            }
        
            float factor = (this.Lerp <= 0.5f) ? 2 * this.Lerp : 1 - (2 * (this.Lerp - 0.5f));
            Color blendColor = this.transitionColor * factor;

            this.SetRenderState();
            this.graphicsDevice.RenderTargets.SetRenderTarget(null);

            this.spriteBatch.DrawVM(this.renderTarget, new Rectangle(0, 0, this.renderTarget.Width, this.renderTarget.Height), null, Color.White, 0, Vector2.Zero, SpriteEffects.None, 0.5f);
            this.spriteBatch.DrawVM(this.transitionColorTexture, new Rectangle(0, 0, this.renderTarget.Width, this.renderTarget.Height), null, blendColor, 0, Vector2.Zero, SpriteEffects.None, 0);
            this.spriteBatch.Render();

            this.graphicsDevice.RenderTargets.ReleaseTemporalRenderTarget(this.renderTarget);
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
                    this.graphicsDevice.Textures.DestroyTexture(this.transitionColorTexture);
                }

                this.disposed = true;
            }
        }
    }
}
