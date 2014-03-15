#region File Description
//-----------------------------------------------------------------------------
// Sprite
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
using WaveEngine.Framework.Services;
#endregion

namespace WaveEngine.Components.Graphics2D
{
    /// <summary>
    /// Represents a 2D image. Such image is loaded from a content file (.wpk),
    /// which is generated from a main PNG or JPEG file format.
    /// </summary>
    public class Sprite : Component
    {
        /// <summary>
        /// Sprite texture path
        /// </summary>
        protected string texturePath;

        /// <summary>
        /// The sprite texture
        /// </summary>
        protected Texture texture;

        /// <summary>
        /// Number of instances of this component created.
        /// </summary>
        private static int instances;

        /// <summary>
        /// The is global asset.
        /// </summary>
        private bool isGlobalAsset;        

        /// <summary>
        /// The disposed
        /// </summary>
        protected bool disposed;

        /// <summary>
        /// Required 2D transform.
        /// See <see cref="Transform2D"/> for more information.
        /// </summary>
        [RequiredComponent]
        public Transform2D Transform2D;

        /// <summary>
        /// Rectangle that represents this sprite in case it is part of a bigger image.
        /// Most of the cases this field will be null, which means the entire texture is used.
        /// On the other side, if a value is provided, will mean the rectangle (inside the original
        /// texture's rectangle) to be drawn.
        /// </summary>
        public Rectangle? SourceRectangle;

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether this asset is global.
        /// By "global" it is meant this asset will be consumed anywhere else. It implies 
        /// once this component is disposed, the asset it-self will not be unload from memory.
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

        /// <summary>
        /// Gets the texture path.
        /// Such path is platform agnostic, and will always start with "Content/".
        /// Example: "Content/Characters/Tim.wpk"
        /// </summary>
        /// <value>
        ///     The texture path.
        /// </value>
        public string TexturePath
        {
            get { return this.texturePath; } 
        }

        /// <summary>
        ///     Gets the texture.
        ///     Such is the in-memory representation for the given asset.
        ///     See <see cref="Texture"/> for more information.
        /// </summary>
        /// <value>
        ///     The texture.
        /// </value>
        public Texture Texture 
        {
            get { return this.texture; } 
        }

        /// <summary>
        /// Gets or sets the tint color.
        /// Each pixel of the sprite will be multiplied by such color during the drawing.
        /// By default, it is white.
        /// </summary>
        /// <value>
        /// The tint color.
        /// </value>
        public Color TintColor { get; set; }

        #endregion

        #region Initialize

        /// <summary>
        /// Initializes a new instance of the <see cref="Sprite" /> class
        /// based on a content file.
        /// </summary>
        /// <param name="texturePath">The texture path to the content file.</param>
        /// <exception cref="System.ArgumentException">TexturePath can not be null.</exception>
        public Sprite(string texturePath)
            : base("Sprite" + instances++)
        {
            if (string.IsNullOrEmpty(texturePath))
            {
                throw new ArgumentException("TexturePath can not be null.");
            }

            this.SourceRectangle = null;
            this.Transform2D = null;
            this.isGlobalAsset = false;
            this.texturePath = texturePath;
            this.TintColor = Color.White;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Sprite" /> class
        /// based on a texture.
        /// See <see cref="Texture"/> for more information.
        /// </summary>
        /// <param name="texture">The texture.</param>
        /// <exception cref="System.ArgumentException">Texture can not be null.</exception>
        public Sprite(Texture texture)
            : base("Sprite" + instances++)
        {
            if (texture == null)
            {
                throw new ArgumentException("Texture can not be null.");
            }

            this.SourceRectangle = null;
            this.Transform2D = null;
            this.isGlobalAsset = false;
            this.texture = texture;
            this.TintColor = Color.White;
        }

        #endregion

        #region Public Methods
        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
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
                throw new ObjectDisposedException("ImageAtlasRenderer");
            }

            if (this.texture == null)
            {
                if (this.isGlobalAsset)
                {
                    this.texture = WaveServices.Assets.Global.LoadAsset<Texture2D>(this.TexturePath);
                }
                else
                {
                    this.texture = this.Assets.LoadAsset<Texture2D>(this.TexturePath);
                }
            }

            this.Transform2D.Rectangle.Width = this.Texture.Width;
            this.Transform2D.Rectangle.Height = this.Texture.Height;
        }

        /// <summary>
        ///     Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing">
        ///     <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    if (!this.IsGlobalAsset && this.TexturePath != null)
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
