// Copyright © 2017 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Using Statements
using System;
using WaveEngine.Common.Math;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Common.Graphics;
using System.Runtime.Serialization;

#endregion

namespace WaveEngine.Components.UI
{
    /// <summary>
    /// The Image atlas renderer.
    /// </summary>
    public class ImageAtlasRenderer : DrawableGUI
    {
        /// <summary>
        /// Total number of instances.
        /// </summary>
        private static int instances;

        /// <summary>
        /// The transform 2d.
        /// </summary>
        [RequiredComponent]
        public Transform2D Transform2D;

        /// <summary>
        /// The image.
        /// </summary>
        [RequiredComponent(false)]
        public ImageAtlas Image;

        /// <summary>
        /// The position.
        /// </summary>
        private Vector2 position;

        /// <summary>
        /// The scale.
        /// </summary>
        private Vector2 scale;

        /// <summary>
        /// The origin.
        /// </summary>
        private Vector2 origin;

        #region Initialize

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageAtlasRenderer" /> class.
        /// </summary>
        public ImageAtlasRenderer()
            : this(DefaultLayers.GUI)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageAtlasRenderer" /> class.
        /// </summary>
        /// <param name="layerType">Type of the layer.</param>
        public ImageAtlasRenderer(Type layerType)
            : base("ImageAtlasRenderer" + instances++, layerType)
        {
            this.Transform2D = null;
            this.Image = null;
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
            if (this.Transform2D.GlobalOpacity > Drawable2D.Delta)
            {
                this.position.X = this.Transform2D.Rectangle.X + this.Transform2D.X;
                this.position.Y = this.Transform2D.Rectangle.Y + this.Transform2D.Y;

                this.scale.X = (this.Transform2D.Rectangle.Width / this.Image.SourceRectangle.Width)
                               * this.Transform2D.XScale;
                this.scale.Y = (this.Transform2D.Rectangle.Height / this.Image.SourceRectangle.Height)
                               * this.Transform2D.YScale;

                Vector2 transformOrigin = this.Transform2D.Origin;
                this.origin.X = transformOrigin.X * this.Transform2D.Rectangle.Width;
                this.origin.Y = transformOrigin.Y * this.Transform2D.Rectangle.Height;

                float opacity = this.RenderManager.DebugLines ? DebugAlpha : this.Transform2D.GlobalOpacity;
                Color color = this.Image.TintColor * opacity;

                this.layer.SpriteBatch.Draw(
                    this.Image.SpriteSheet.Texture,
                    this.position,
                    this.Image.SourceRectangle,
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
        }

        /// <summary>
        /// Helper method that draws debug lines.
        /// </summary>
        /// <remarks>
        /// This method will only work on debug mode and if RenderManager.DebugLines /&gt;
        /// is set to <c>true</c>.
        /// </remarks>
        protected override void DrawDebugLines()
        {
            base.DrawDebugLines();

            // Rectangle
            this.RenderManager.LineBatch2D.DrawRectangle(this.Transform2D.Rectangle, Color.Blue, this.Transform2D.DrawOrder);

            // Origin
            this.RenderManager.LineBatch2D.DrawPoint(this.Transform2D.Rectangle.Location + this.Transform2D.Origin, 10f, Color.Red, this.Transform2D.DrawOrder);
        }
        #endregion
    }
}
