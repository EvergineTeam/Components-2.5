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
    /// Transition effect where the next screenContext cover the current screenContext
    /// </summary>
    public class UncoverTransition : ScreenTransition
    {
        /// <summary>
        /// The direction of this effect.
        /// </summary>
        public enum EffectOptions
        {
            /// <summary>
            /// From right
            /// </summary>
            FromRight,

            /// <summary>
            /// From top
            /// </summary>
            FromTop,

            /// <summary>
            /// From left
            /// </summary>
            FromLeft,

            /// <summary>
            /// From botton
            /// </summary>
            FromBotton,

            /// <summary>
            /// From top right
            /// </summary>
            FromTopRight,

            /// <summary>
            /// From bottom right
            /// </summary>
            FromBottomRight,

            /// <summary>
            /// From top left
            /// </summary>
            FromTopLeft,

            /// <summary>
            /// From botton left
            /// </summary>
            FromBottonLeft
        }

        /// <summary>
        /// The sprite batch
        /// </summary>
        private SpriteBatch spriteBatch;

        /// <summary>
        /// The transition options
        /// </summary>
        private EffectOptions effectOption;

        /// <summary>
        /// The position
        /// </summary>
        private Vector2 position;

        /// <summary>
        /// The initial position
        /// </summary>
        private Vector2 initialPosition;

        /// <summary>
        /// The target direction
        /// </summary>
        private Vector2 targetPosition;

        /// <summary>
        /// Initializes a new instance of the <see cref="UncoverTransition" /> class.
        /// </summary>
        /// <param name="duration">The duration.</param>
        /// <param name="effect">The effect.</param>
        public UncoverTransition(TimeSpan duration, EffectOptions effect)
            : base(duration)
        {
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
                case EffectOptions.FromRight:
                    this.targetPosition = new Vector2(-this.platform.ScreenWidth, 0);
                    break;
                case EffectOptions.FromTop:
                    this.targetPosition = new Vector2(0, this.platform.ScreenHeight);
                    break;
                case EffectOptions.FromLeft:
                    this.targetPosition = new Vector2(this.platform.ScreenWidth, 0);
                    break;
                case EffectOptions.FromBotton:
                    this.targetPosition = new Vector2(0, -this.platform.ScreenHeight);
                    break;
                case EffectOptions.FromTopRight:
                    this.targetPosition = new Vector2(-this.platform.ScreenWidth, this.platform.ScreenHeight);
                    break;
                case EffectOptions.FromBottomRight:
                    this.targetPosition = new Vector2(-this.platform.ScreenWidth, -this.platform.ScreenHeight);
                    break;
                case EffectOptions.FromTopLeft:
                    this.targetPosition = new Vector2(this.platform.ScreenWidth, this.platform.ScreenHeight);
                    break;
                case EffectOptions.FromBottonLeft:
                    this.targetPosition = new Vector2(this.platform.ScreenWidth, -this.platform.ScreenHeight);
                    break;
            }

            this.initialPosition = new Vector2(0, 0);
        }

        /// <summary>
        /// Updates the specified game time.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        protected override void Update(TimeSpan gameTime)
        {
            this.UpdateSources(gameTime);
            this.UpdateTarget(gameTime);

            this.position = ((this.targetPosition - this.initialPosition) * this.Lerp) + this.initialPosition;
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

            this.spriteBatch.Draw(targetRenderTarget, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, Vector2.One, SpriteEffects.None, 0.5f);
            this.spriteBatch.Draw(sourceRenderTarget, this.position, null, Color.White, 0f, Vector2.Zero, Vector2.One, SpriteEffects.None, 0f);
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
