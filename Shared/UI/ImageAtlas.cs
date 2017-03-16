#region File Description
//-----------------------------------------------------------------------------
// ImageAtlas
//
// Copyright © 2017 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using WaveEngine.Framework.Services;
using WaveEngine.Framework.Graphics;
using WaveEngine.Common.Math;
using System.Linq;
using WaveEngine.Framework.UI;
using WaveEngine.Common.Graphics;
using System.Runtime.Serialization;
#endregion

namespace WaveEngine.Components.UI
{
    /// <summary>
    /// The image atlas.
    /// </summary>
    public class ImageAtlas : Control
    {
        /// <summary>
        /// Total number of instances.
        /// </summary>
        private static int instances;

        /// <summary>
        /// The is global asset.
        /// </summary>
        protected bool isGlobalAsset;

        /// <summary>
        /// The disposed
        /// </summary>
        protected bool disposed;

        /// <summary>
        /// The source rectangle.
        /// </summary>
        public Rectangle SourceRectangle;

        /// <summary>
        /// The texture name.
        /// </summary>
        private string textureName;

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
        /// Gets or sets the atlas path.
        /// </summary>
        /// <value>
        /// The atlas path.
        /// </value>
        public string AtlasPath { get; protected set; }

        /// <summary>
        /// Gets or sets the name of the texture.
        /// </summary>
        /// <value>
        /// The name of the texture.
        /// </value>
        public string TextureName
        {
            get
            {
                return this.textureName;
            }

            set
            {
                this.textureName = value;
                this.UpdateSourceRectangle();
            }
        }

        /// <summary>
        /// Gets or sets the texture atlas.
        /// </summary>
        /// <value>
        /// The texture atlas.
        /// </value>
        public SpriteSheet SpriteSheet { get; protected set; }

        /// <summary>
        /// Gets or sets the color of the tint.
        /// </summary>
        /// <value>
        /// The color of the tint.
        /// </value>
        public Color TintColor { get; set; }

        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        /// <value>
        /// The width.
        /// </value>
        public new float Width
        {
            get
            {
                return base.Width;
            }

            protected set
            {
                base.Width = value;
            }
        }

        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        /// <value>
        /// The height.
        /// </value>
        public new float Height
        {
            get
            {
                return base.Height;
            }

            protected set
            {
                base.Height = value;
            }
        }

        #endregion

        #region Initialize
        /// <summary>
        /// Initializes a new instance of the <see cref="ImageAtlas" /> class.
        /// </summary>
        /// <param name="atlasPath">The atlas path.</param>
        /// <param name="textureName">Name of the texture.</param>
        /// <exception cref="System.ArgumentException">TexturePath can not be null.</exception>
        public ImageAtlas(string atlasPath, string textureName)
            : base("ImageAtlas" + instances++)
        {
            if (string.IsNullOrEmpty(atlasPath))
            {
                throw new ArgumentException("TexturePath can not be null.");
            }

            this.TextureName = textureName;
            this.Transform2D = null;
            this.isGlobalAsset = false;
            this.AtlasPath = atlasPath;
            this.TintColor = Color.White;
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
                throw new ObjectDisposedException("ImageAtlas");
            }

            if (this.isGlobalAsset)
            {
                this.SpriteSheet = WaveServices.Assets.Global.LoadAsset<SpriteSheet>(this.AtlasPath);
            }
            else
            {
                this.SpriteSheet = Assets.LoadAsset<SpriteSheet>(this.AtlasPath);
            }

            this.UpdateSourceRectangle();
        }

        /// <summary>
        /// Updates the source rectangle.
        /// </summary>
        /// <exception cref="System.ObjectDisposedException">ImageAtlas has been disposed.</exception>
        protected void UpdateSourceRectangle()
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException("ImageAtlas");
            }

            if (this.SpriteSheet != null)
            {
                if (string.IsNullOrEmpty(this.TextureName))
                {
                    this.TextureName = this.SpriteSheet.Sprites[0].Name;
                }

                this.SourceRectangle = this.SpriteSheet.SpriteDictionary[this.textureName].Rectangle;

                RectangleF rectangle = this.Transform2D.Rectangle;
                rectangle.Width = this.SourceRectangle.Width;
                rectangle.Height = this.SourceRectangle.Height;
                this.Transform2D.Rectangle = rectangle;
            }
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
                    if (!this.IsGlobalAsset)
                    {
                        Assets.UnloadAsset(this.AtlasPath);
                    }

                    this.disposed = true;
                }
            }
        }
        #endregion
    }
}
