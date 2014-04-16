#region File Description
//-----------------------------------------------------------------------------
// ImageControlRenderer
//
// Copyright © 2014 Wave Corporation
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using WaveEngine.Common.Math;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Common.Graphics;
using System.Diagnostics;

#endregion

namespace WaveEngine.Components.UI
{
    /// <summary>
    /// The Image renderer.
    /// </summary>
    public class ImageControlRenderer : Drawable2D
    {
        /// <summary>
        /// Total number of instances.
        /// </summary>
        private static int instances;

        /// <summary>
        /// The transform 2 d.
        /// </summary>
        [RequiredComponent]
        public Transform2D Transform2D;

        /// <summary>
        /// The Image.
        /// </summary>
        [RequiredComponent(false)]
        public ImageControl Image;

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
        /// Initializes a new instance of the <see cref="ImageControlRenderer" /> class.
        /// </summary>
        public ImageControlRenderer()
            : this(DefaultLayers.GUI)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageControlRenderer" /> class.
        /// </summary>
        /// <param name="layerType">Type of the layer.</param>
        public ImageControlRenderer(Type layerType)
            : base("ImageRenderer" + instances++, layerType)
        {
            this.Transform2D = null;
            this.Image = null;
            this.scale = Vector2.Zero;
            this.position = Vector2.Zero;
            this.origin = Vector2.Zero;            
        }
        #endregion

        #region Public Methods
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

                this.scale.X = (this.Transform2D.Rectangle.Width / this.Image.Texture.Width) * this.Transform2D.XScale;
                this.scale.Y = (this.Transform2D.Rectangle.Height / this.Image.Texture.Height) * this.Transform2D.YScale;

                Vector2 transformOrigin = this.Transform2D.Origin;
                this.origin.X = transformOrigin.X * this.Image.Texture.Width;
                this.origin.Y = transformOrigin.Y * this.Image.Texture.Height;

                float opacity = this.RenderManager.DebugLines ? this.DebugAlpha : this.Transform2D.Opacity;
                Color color = this.Image.TintColor * opacity;

                this.spriteBatch.DrawVM(
                    this.Image.Texture,
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
            RenderManager.LineBatch2D.DrawRectangleVM(this.Transform2D.Rectangle, Color.Blue);

            // Origin
            RenderManager.LineBatch2D.DrawPointVM(this.Transform2D.Rectangle.Location + this.Transform2D.Origin, 10f, Color.Red);
        }
        #endregion
    }
}
