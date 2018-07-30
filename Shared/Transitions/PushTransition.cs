// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

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
    /// Transition effect where the next screenContext cover the current screenContext
    /// </summary>
    public class PushTransition : ScreenTransition
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
        private Vector2 position1;

        /// <summary>
        /// The position
        /// </summary>
        private Vector2 position2;

        /// <summary>
        /// The initial position
        /// </summary>
        private Vector2 initialPosition;

        /// <summary>
        /// The target direction
        /// </summary>
        private Vector2 targetPosition;

        /// <summary>
        /// Initializes a new instance of the <see cref="PushTransition" /> class.
        /// </summary>
        /// <param name="duration">The duration.</param>
        /// <param name="effect">The effect.</param>
        public PushTransition(TimeSpan duration, EffectOptions effect)
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
                    this.initialPosition = new Vector2(this.platform.ScreenWidth, 0);
                    break;
                case EffectOptions.FromTop:
                    this.initialPosition = new Vector2(0, -this.platform.ScreenHeight);
                    break;
                case EffectOptions.FromLeft:
                    this.initialPosition = new Vector2(-this.platform.ScreenWidth, 0);
                    break;
                case EffectOptions.FromBotton:
                    this.initialPosition = new Vector2(0, this.platform.ScreenHeight);
                    break;
            }

            this.position1 = this.initialPosition;
            this.targetPosition = new Vector2(0, 0);
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

           Vector2 pos = ((this.targetPosition - this.initialPosition) * this.Lerp) + this.initialPosition;
            this.position2 += pos - this.position1;
            this.position1 = pos;

            this.spriteBatch.Draw(sourceRenderTarget, this.position2, null, Color.White, 0f, Vector2.Zero, Vector2.One, SpriteEffects.None, 0.5f);
            this.spriteBatch.Draw(targetRenderTarget, this.position1, null, Color.White, 0f, Vector2.Zero, Vector2.One, SpriteEffects.None, 0f);
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
