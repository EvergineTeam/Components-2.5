#region File Description
//-----------------------------------------------------------------------------
// SpriteRenderer
//
// Copyright © 2014 Wave Corporation
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
    /// Renders a <see cref="Sprite"/> on the screen.
    /// The owner <see cref="Entity"/> must contain the <see cref="Sprite"/> to be drawn, plus a <see cref="Transform2D"/>.
    /// </summary>
    public class SpriteRenderer : Drawable2D
    {
        /// <summary>
        /// Number of instances of this component created.
        /// </summary>
        private static int instances;

        /// <summary>
        /// Required <see cref="Transform2D"/>.
        /// It provides where to draw the <see cref="Sprite"/>, which rotation to apply and which scale.
        /// </summary>
        [RequiredComponent]
        public Transform2D Transform2D;

        /// <summary>
        /// Required <see cref="Sprite"/>.
        /// It provides the in memory representation for a visual asset.
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
        /// <param name="layerType">
        /// Layer type (available at <see cref="DefaultLayers"/>).
        /// Example: new SpriteRenderer(DefaultLayers.Alpha)
        /// </param>
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

        #region Public Methods
        /// <summary>
        /// Allows to perform custom drawing.
        /// </summary>
        /// <param name="gameTime">The elapsed game time.</param>
        /// <remarks>
        /// This method will only be called if all the following points are true:
        /// <list type="bullet">
        /// <item>
        /// <description>The parent of the owner <see cref="Entity" /> of the <see cref="Drawable" /> cascades its visibility to its children and it is visible.</description>
        /// </item>
        /// <item>
        /// <description>The <see cref="Drawable" /> is active.</description>
        /// </item>
        /// <item>
        /// <description>The owner <see cref="Entity" /> of the <see cref="Drawable" /> is active and visible.</description>
        /// </item>
        /// </list>
        /// </remarks>
        public override void Draw(TimeSpan gameTime)
        {
            if (this.Transform2D.Opacity > this.Delta)
            {
                this.position.X = this.Transform2D.Rectangle.X + this.Transform2D.X;
                this.position.Y = this.Transform2D.Rectangle.Y + this.Transform2D.Y;

                if (this.Sprite.SourceRectangle.HasValue)
                {
                    var rect = this.Sprite.SourceRectangle.Value;
                    this.scale.X = (this.Transform2D.Rectangle.Width / rect.Width) * this.Transform2D.XScale;
                    this.scale.Y = (this.Transform2D.Rectangle.Height / rect.Height) * this.Transform2D.YScale;
                }
                else
                {
                    this.scale.X = (this.Transform2D.Rectangle.Width / this.Sprite.Texture.Width) * this.Transform2D.XScale;
                    this.scale.Y = (this.Transform2D.Rectangle.Height / this.Sprite.Texture.Height) * this.Transform2D.YScale;
                }

                Vector2 transformOrigin = this.Transform2D.Origin;
                this.origin.X = transformOrigin.X * this.Transform2D.Rectangle.Width;
                this.origin.Y = transformOrigin.Y * this.Transform2D.Rectangle.Height;

                float opacity = this.RenderManager.DebugLines ? this.DebugAlpha : this.Transform2D.Opacity;
                Color color = this.Sprite.TintColor * opacity;

                this.layer.SpriteBatch.DrawVM(
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

        #region Private Methods
        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && this.Sprite != null)
            {
                this.Sprite.Dispose();
            }
        }

        #endregion
    }
}
