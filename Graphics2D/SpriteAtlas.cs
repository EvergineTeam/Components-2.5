#region File Description
//-----------------------------------------------------------------------------
// SpriteAtlas
// Copyright © 2014 Wave Corporation
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Linq;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Math;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Services;
#endregion

namespace WaveEngine.Components.Graphics2D
{
    /// <summary>
    ///     A collection of images.
    /// </summary>
    public class SpriteAtlas : Component, IDisposable
    {
        /// <summary>
        ///     Number of instances of this component created.
        /// </summary>
        private static int instances;

        /// <summary>
        ///     Rectangle for the Image of the atlas that will be rendered.
        /// </summary>
        public Rectangle SourceRectangle;

        /// <summary>
        ///     Transform for the Image of the atlas that will be rendered.
        /// </summary>
        [RequiredComponent]
        public Transform2D Transform2D;

        /// <summary>
        /// The disposed.
        /// </summary>
        protected bool disposed;

        /// <summary>
        /// The is global asset.
        /// </summary>
        private bool isGlobalAsset;

        /// <summary>
        /// The texture name.
        /// </summary>
        private string textureName;

        #region Properties

        /// <summary>
        ///     Gets or sets the path to the atlas.
        /// </summary>
        /// <value>
        ///     The atlas path.
        /// </value>
        public string AtlasPath { get; protected set; }

        /// <summary>
        ///     Gets or sets a value indicating whether this asset is global.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this asset is global; otherwise, <c>false</c>.
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

        // REVIEW: esto para que se usa?

        /// <summary>
        ///     Gets or sets the texture atlas.
        /// </summary>
        /// <value>
        ///     The texture atlas.
        /// </value>
        public TextureAtlas TextureAtlas { get; protected set; }

        /// <summary>
        /// Gets or sets the color of the tint.
        /// </summary>
        /// <value>
        /// The color of the tint.
        /// </value>
        public Color TintColor { get; set; }

        /// <summary>
        ///     Gets or sets the name of the texture from where this atlas is loaded.
        /// </summary>
        /// <value>
        ///     The name of the texture.
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

        #endregion

        #region Initialize

        /// <summary>
        /// Initializes a new instance of the <see cref="SpriteAtlas" /> class.
        /// </summary>
        /// <param name="atlasPath">The path to the atlas.</param>
        /// <param name="textureName">Name of the texture from where this atlas is loaded.</param>
        /// <exception cref="System.ArgumentException">TexturePath can not be null.</exception>
        public SpriteAtlas(string atlasPath, string textureName)
            : base("SpriteAtlas" + instances++)
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

        #region Public Methods
        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing">
        /// <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    if (!this.IsGlobalAsset && this.isInitialized)
                    {
                        Assets.UnloadAsset(this.AtlasPath);
                    }

                    this.disposed = true;
                }
            }
        }

        /// <summary>
        ///     Performs further custom initialization for this instance.
        /// </summary>
        protected override void Initialize()
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException("ImageAtlasRenderer");
            }

            if (this.isGlobalAsset)
            {
                this.TextureAtlas = WaveServices.Assets.Global.LoadAsset<TextureAtlas>(this.AtlasPath);
            }
            else
            {
                this.TextureAtlas = Assets.LoadAsset<TextureAtlas>(this.AtlasPath);
            }

            this.UpdateSourceRectangle();
        }

        /// <summary>
        /// The update source rectangle.
        /// </summary>
        /// <exception cref="System.ObjectDisposedException">ImageAtlasRenderer has been disposed.</exception>
        protected void UpdateSourceRectangle()
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException("ImageAtlasRenderer");
            }

            if (this.TextureAtlas != null)
            {
                if (string.IsNullOrEmpty(this.TextureName))
                {
                    this.TextureName = this.TextureAtlas.SpriteRectangles.First().Key;
                }

                this.SourceRectangle = this.TextureAtlas.SpriteRectangles[this.textureName];
                this.Transform2D.Rectangle.Width = this.SourceRectangle.Width;
                this.Transform2D.Rectangle.Height = this.SourceRectangle.Height;
            }
        }

        #endregion
    }
}