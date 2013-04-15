#region File Description
//-----------------------------------------------------------------------------
// AnimatedSpriteRenderer
//
// Copyright © 2010 - 2013 Wave Coorporation. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Math;
using WaveEngine.Components.Animation;
using WaveEngine.Components.Graphics2D;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
#endregion

namespace WaveEngine.Components.Graphics2D
{
    /// <summary>
    /// Custom sprite renderer to support 2D animations
    /// </summary>
    public class AnimatedSpriteRenderer : Drawable2D
    {
        /// <summary>
        /// Number of instances of this component created.
        /// </summary>
        private static int instances;

        /// <summary>
        /// Transform of the <see cref="Graphics2D.Sprite"/>.
        /// </summary>
        [RequiredComponent]
        public Transform2D Transform2D;

        /// <summary>
        /// <see cref="Graphics2D.Sprite"/> to render.
        /// </summary>
        [RequiredComponent(false)]
        public Sprite Sprite;

        /// <summary>
        /// The position
        /// </summary>
        private Vector2 position;

        /// <summary>
        /// The scale
        /// </summary>
        private Vector2 scale;

        /// <summary>
        /// The origin
        /// </summary>
        private Vector2 origin;

        /// <summary>
        /// The animation behavior
        /// </summary>
        [RequiredComponent]
        public Animation2D Animation2D;

        #region Properties
        #endregion

        #region Initialize
        /// <summary>
        /// Initializes a new instance of the <see cref="AnimatedSpriteRenderer" /> class.
        /// </summary>
        /// <param name="layer">Layer type.</param>
        public AnimatedSpriteRenderer(Type layer)
            : base("AnimatedSpriteRenderer" + instances++, layer)
        {
            this.Transform2D = null;
            this.Sprite = null;
            this.scale = Vector2.Zero;
            this.position = Vector2.Zero;
            this.origin = Vector2.Zero;
            this.Animation2D = null;
        }
        #endregion

        #region Public Methods
        #endregion

        #region Private Methods
        /// <summary>
        /// Override to set the source rectangle from <see cref="Animation2D"/>.
        /// </summary>
        /// <param name="parameter">The parameter</param>
        protected override void DrawBasicUnit(int parameter)
        {
            if (this.Transform2D.Opacity > this.Delta)
            {
                this.position.X = this.Transform2D.Rectangle.X + this.Transform2D.X;
                this.position.Y = this.Transform2D.Rectangle.Y + this.Transform2D.Y;

                this.scale.X = this.Transform2D.XScale;
                this.scale.Y = this.Transform2D.YScale;

                this.origin.X = this.Transform2D.Origin.X * this.Animation2D.CurrentRectangle.Width;
                this.origin.Y = this.Transform2D.Origin.Y * this.Animation2D.CurrentRectangle.Height;

                float opacity = this.RenderManager.DebugLines ? this.DebugAlpha : 
                    this.Transform2D.Opacity;
                Color color = this.Sprite.TintColor * opacity;

                this.spriteBatch.Draw(
                    this.Sprite.Texture,
                    this.position,
                    this.Animation2D.CurrentRectangle,
                    color,
                    this.Transform2D.Rotation,
                    this.origin,
                    this.scale,
                    this.Transform2D.Effect,
                    this.Transform2D.DrawOrder);
            }
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
        }
        #endregion
    }
}
