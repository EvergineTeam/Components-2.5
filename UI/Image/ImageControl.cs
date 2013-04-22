#region File Description
//-----------------------------------------------------------------------------
// ImageControl
//
// Copyright © $year$ Weekend Game Studio. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Services;
using WaveEngine.Common.Math;
using WaveEngine.Framework.UI;
using WaveEngine.Common.Graphics;
#endregion

namespace WaveEngine.Components.UI
{
    /// <summary>
    /// Image control.
    /// </summary>
    public class ImageControl : Control
    {
        /// <summary>
        /// The instances
        /// </summary>
        private static int instances;

        /// <summary>
        /// The white pixel
        /// </summary>
        private static Texture2D whitePixel;

        /// <summary>
        /// The is global asset
        /// </summary>
        protected bool isGlobalAsset;

        /// <summary>
        /// The disposed
        /// </summary>
        protected bool disposed;

        /// <summary>
        /// The source rectangle
        /// </summary>
        public Rectangle? SourceRectangle;

        /// <summary>
        /// The texture color
        /// </summary>
        private bool isTextureColor;

        #region Properties
        /// <summary>
        /// Gets or sets a value indicating whether [perssistent asset].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [perssistent asset]; otherwise, <c>false</c>.
        /// </value>
        public bool IsGlobalAsset
        {
            get
            {
                return this.isGlobalAsset;
            }

            set
            {
                if (this.isInitialized)
                {
                    throw new InvalidOperationException("Asset has already initialized.");
                }

                this.isGlobalAsset = value;
            }
        }

        /// <summary>
        /// Gets or sets the texture path.
        /// </summary>
        /// <value>
        /// The texture path.
        /// </value>
        public string TexturePath { get; protected set; }

        /// <summary>
        /// Gets or sets the texture.
        /// </summary>
        /// <value>
        /// The texture.
        /// </value>
        public Texture Texture { get; protected set; }

        /// <summary>
        /// Gets or sets the color of the tint.
        /// </summary>
        /// <value>
        /// The color of the tint.
        /// </value>
        public Color TintColor { get; set; }     

        /// <summary>
        /// Gets or sets the stretch.
        /// </summary>
        /// <value>
        /// The stretch.
        /// </value>
        public Stretch Stretch { get; set; }

        #endregion

        #region Initialize
        /// <summary>
        /// Initializes static members of the <see cref="ImageControl" /> class.
        /// </summary>
        static ImageControl()
        {
            whitePixel = new Texture2D()
            {
                Width = 1,
                Height = 1,
                Levels = 1,
                Data = new byte[1][][] { new byte[1][] { new byte[] { 255, 255, 255, 255 } } },
            };
            WaveServices.GraphicsDevice.Textures.UploadTexture(whitePixel);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageControl"/> class.
        /// </summary>
        /// <param name="texturePath">The texture path.</param>
        public ImageControl(string texturePath)
            : base("Image" + instances++)
        {
            if (string.IsNullOrEmpty(texturePath))
            {
                throw new ArgumentException("TexturePath can not be null.");
            }

            this.SourceRectangle = null;
            this.Transform2D = null;
            this.isGlobalAsset = false;
            this.TexturePath = texturePath;
            this.isTextureColor = false;
            this.TintColor = Color.White;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageControl"/> class.
        /// </summary>
        /// <param name="texture">Texture instance.</param>
        public ImageControl(Texture texture)
            : base("Image" + instances++)
        {
            if (texture == null)
            {
                throw new ArgumentException("Texture can not be null.");
            }

            this.SourceRectangle = null;
            this.Transform2D = null;
            this.isGlobalAsset = false;
            this.Texture = texture;
            this.isTextureColor = false;
            this.TintColor = Color.White;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageControl" /> class.
        /// </summary>
        /// <param name="color">The color.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        public ImageControl(Color color, int width, int height)
            : base("Image" + instances++)
        {
            this.TintColor = color;
            this.Width = width;
            this.Height = height;
            this.isTextureColor = true;
        }

        #endregion

        #region Public Methods
        /// <summary>
        /// Arranges the specified final size.
        /// </summary>
        /// <param name="finalSize">The final size.</param>
        public override void Arrange(RectangleF finalSize)
        {
            if (this.Transform2D.Rectangle.Width <= 0)
            {
                this.Transform2D.Rectangle.Width = this.desiredSize.X;
            }

            if (this.Transform2D.Rectangle.Height <= 0)
            {
                this.Transform2D.Rectangle.Height = this.desiredSize.Y;
            }                              

            switch (this.Stretch)
            {
                case Stretch.None:
                    break;
                case Stretch.Fill:
                    this.Transform2D.Rectangle.Width = finalSize.Width - this.Margin.Left - this.Margin.Right;
                    this.Transform2D.Rectangle.Height = finalSize.Height - this.Margin.Top - this.Margin.Bottom;
                    break;
                case Stretch.Uniform:

                    if (this.width >= this.height)
                    {                        
                        float finalProportionalHeight = (this.height * finalSize.Width) / this.width;

                        if (finalSize.Height < finalProportionalHeight)
                        {
                            this.Transform2D.Rectangle.Width = ((this.width * finalSize.Height) / this.height) - this.Margin.Left - this.Margin.Right;
                            this.Transform2D.Rectangle.Height = finalSize.Height - this.Margin.Top - this.Margin.Bottom;
                        }
                        else
                        {
                            this.Transform2D.Rectangle.Width = finalSize.Width - this.Margin.Left - this.Margin.Right;
                            this.Transform2D.Rectangle.Height = finalProportionalHeight - this.Margin.Top - this.Margin.Bottom;
                        }
                    }
                    else
                    {
                        float finalProportionalWidth = (this.width * finalSize.Height) / this.height;

                        if (finalSize.Width < finalProportionalWidth)
                        {
                            this.Transform2D.Rectangle.Width = finalSize.Width - this.Margin.Left - this.Margin.Right;
                            this.Transform2D.Rectangle.Height = ((this.height * finalSize.Width) / this.width) - this.Margin.Top - this.Margin.Bottom;
                        }
                        else
                        {
                            this.Transform2D.Rectangle.Width = finalProportionalWidth - this.Margin.Left - this.Margin.Right;
                            this.Transform2D.Rectangle.Height = finalSize.Height - this.Margin.Top - this.Margin.Bottom;
                        }
                    }

                    break;
                case Stretch.UniformToFill:

                    if (this.width >= this.height)
                    {
                        float finalProportionalWidth = (this.width * finalSize.Height) / this.height;

                        if (finalSize.Width > finalProportionalWidth)
                        {
                            this.Transform2D.Rectangle.Width = finalSize.Width - this.Margin.Left - this.Margin.Right;
                            this.Transform2D.Rectangle.Height = ((this.height * finalSize.Width) / this.width) - this.Margin.Top - this.Margin.Bottom;
                        }
                        else
                        {
                            this.Transform2D.Rectangle.Width = finalProportionalWidth - this.Margin.Left - this.Margin.Right;
                            this.Transform2D.Rectangle.Height = finalSize.Height - this.Margin.Top - this.Margin.Bottom;
                        }
                    }
                    else
                    {
                        float finalProportionalHeight = (this.height * finalSize.Width) / this.width;

                        if (finalSize.Height > finalProportionalHeight)
                        {
                            this.Transform2D.Rectangle.Width = ((this.width * finalSize.Height) / this.height) - this.Margin.Left - this.Margin.Right;
                            this.Transform2D.Rectangle.Height = finalSize.Height - this.Margin.Top - this.Margin.Bottom;
                        }
                        else
                        {
                            this.Transform2D.Rectangle.Width = finalSize.Width - this.Margin.Left - this.Margin.Right;
                            this.Transform2D.Rectangle.Height = finalProportionalHeight - this.Margin.Top - this.Margin.Bottom;
                        }
                    }

                    break;
            }

            RectangleF rect = this.Transform2D.Rectangle;
            rect.Width *= this.Transform2D.XScale;
            rect.Height *= this.Transform2D.YScale;  

            switch (this.HorizontalAlignment)
            {
                case HorizontalAlignment.Left:
                    this.Transform2D.Rectangle.X = finalSize.X + this.Margin.Left;
                    break;
                case HorizontalAlignment.Center:
                    this.Transform2D.Rectangle.X = finalSize.X + (finalSize.Width / 2) - (rect.Width / 2) - this.Margin.Right + this.Margin.Left;
                    break;
                case HorizontalAlignment.Right:
                    this.Transform2D.Rectangle.X = finalSize.X + finalSize.Width - this.Margin.Right - rect.Width;
                    break;
                case HorizontalAlignment.Stretch:                  
                    break;
            }

            switch (this.VerticalAlignment)
            {
                case VerticalAlignment.Top:
                    this.Transform2D.Rectangle.Y = finalSize.Y + this.Margin.Top;
                    break;
                case VerticalAlignment.Center:
                    this.Transform2D.Rectangle.Y = finalSize.Y + (finalSize.Height / 2) - (rect.Height / 2) - this.Margin.Bottom + this.Margin.Top;
                    break;
                case VerticalAlignment.Bottom:
                    this.Transform2D.Rectangle.Y = finalSize.Y + finalSize.Height - this.Margin.Bottom - rect.Height;
                    break;
                case VerticalAlignment.Stretch:
                    break;
            }

            this.Transform2D.Rectangle.X += this.Transform2D.Origin.X * rect.Width;
            this.Transform2D.Rectangle.Y += this.Transform2D.Origin.Y * rect.Height;
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Performs further custom initialization for this instance.
        /// </summary>
        protected override void Initialize()
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException("ImageRenderer");
            }

            if (!string.IsNullOrEmpty(this.TexturePath))
            {
                if (this.isGlobalAsset)
                {
                    this.Texture = WaveServices.Assets.Global.LoadAsset<Texture2D>(this.TexturePath);
                }
                else
                {
                    this.Texture = this.Assets.LoadAsset<Texture2D>(this.TexturePath);
                }

                this.Width = this.Texture.Width;
                this.Height = this.Texture.Height;
            }
            else if (this.isTextureColor)
            {
                this.Texture = whitePixel;
            }
        }

        /// <summary>
        ///     Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing">
        ///     <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.
        /// </param>
        protected override void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    if (!this.IsGlobalAsset && !string.IsNullOrEmpty(this.TexturePath))
                    {
                        this.Assets.UnloadAsset(this.TexturePath);
                    }

                    this.disposed = true;
                }
            }
        }
        #endregion
    }
}
