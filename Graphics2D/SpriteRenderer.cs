#region File Description
//-----------------------------------------------------------------------------
// SpriteRenderer
//
// Copyright © 2010 - 2013 Wave Coorporation. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Math;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
#endregion

namespace WaveEngine.Components.Graphics2D
{
    /// <summary>
    /// Renders a Sprite on the screen.
    /// </summary>
    public class SpriteRenderer : Drawable2D
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

        #region Initialize
        /// <summary>
        /// Initializes a new instance of the <see cref="SpriteRenderer" /> class.
        /// </summary>
        /// <param name="layerType">Type of the layer.</param>
        public SpriteRenderer(Type layerType)
            : base("SpriteRenderer" + instances++, layerType)
        {
            this.Transform2D = null;
            this.Sprite = null;
            this.scale = Vector2.Zero;
            this.position = Vector2.Zero;
            this.origin = Vector2.Zero;
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
        }

        /// <summary>
        /// Draws the basic unit.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        protected override void DrawBasicUnit(int parameter)
        {
            if (this.Transform2D.Opacity > this.Delta)
            {
                this.position.X = this.Transform2D.Rectangle.X + this.Transform2D.X;
                this.position.Y = this.Transform2D.Rectangle.Y + this.Transform2D.Y;

                this.scale.X = (this.Transform2D.Rectangle.Width / this.Sprite.Texture.Width) * this.Transform2D.XScale;
                this.scale.Y = (this.Transform2D.Rectangle.Height / this.Sprite.Texture.Height)
                               * this.Transform2D.YScale;

                Vector2 transformOrigin = this.Transform2D.Origin;
                if (this.Sprite.SourceRectangle.HasValue)
                {
                    this.origin.X = transformOrigin.X * this.Sprite.SourceRectangle.Value.Width;
                    this.origin.Y = transformOrigin.Y * this.Sprite.SourceRectangle.Value.Height;
                }
                else
                {
                    this.origin.X = transformOrigin.X * this.Sprite.Texture.Width;
                    this.origin.Y = transformOrigin.Y * this.Sprite.Texture.Height;
                }

                float opacity = this.RenderManager.DebugLines ? this.DebugAlpha : this.Transform2D.Opacity;
                Color color = this.Sprite.TintColor * opacity;

                this.spriteBatch.DrawVM(
                    this.Sprite.Texture,
                    this.position,
                    this.Sprite.SourceRectangle,
                    color,
                    this.Transform2D.Rotation,
                    this.origin,
                    this.scale,
                    this.Transform2D.Effect,
                    this.Transform2D.DrawOrder);
            }
        }
        #endregion
    }
}
