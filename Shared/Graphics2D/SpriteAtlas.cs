#region File Description
//-----------------------------------------------------------------------------
// SpriteAtlas
// Copyright © 2017 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using WaveEngine.Common.Attributes;
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
    [DataContract(Namespace = "WaveEngine.Components.Graphics2D")]
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
        [DataMember]
        private bool isGlobalAsset;

        /// <summary>
        /// The current is global asset value
        /// </summary>
        private bool currentIsGlobalAsset;

        /// <summary>
        /// The texture name.
        /// </summary>
        [DataMember]
        private string textureName;

        /// <summary>
        /// The texture index.
        /// </summary>
        private int textureIndex;

        /// <summary>
        /// The path of the atlas
        /// </summary>
        [DataMember]
        private string spriteSheetPath;

        #region Properties

        /// <summary>
        ///     Gets or sets the path to the atlas.
        /// </summary>
        /// <value>
        ///     The atlas path.
        /// </value>
        [RenderPropertyAsAsset(AssetType.Spritesheet)]
        public string SpriteSheetPath
        {
            get
            {
                return this.spriteSheetPath;
            }

            set
            {
                this.spriteSheetPath = value;

                if (this.isInitialized)
                {
                    this.RefreshAtlasTexture();
                }
            }
        }

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
                this.isGlobalAsset = value;

                if (this.isInitialized)
                {
                    this.RefreshAtlasTexture();
                }
            }
        }

        /// <summary>
        ///     Gets or sets the texture atlas.
        /// </summary>
        /// <value>
        ///     The texture atlas.
        /// </value>
        [DontRenderProperty]
        public SpriteSheet SpriteSheet { get; protected set; }

        /// <summary>
        /// Gets or sets the color of the tint.
        /// </summary>
        /// <value>
        /// The color of the tint.
        /// </value>
        [DataMember]
        public Color TintColor { get; set; }

        /// <summary>
        ///     Gets or sets the name of the texture from where this atlas is loaded.
        /// </summary>
        /// <value>
        ///     The name of the texture.
        /// </value>
        [RenderPropertyAsSelector("TextureNames")]
        public string TextureName
        {
            get
            {
                return this.textureName;
            }

            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    this.textureName = value;

                    if (this.isInitialized)
                    {
                        this.RefreshTextureIndex();
                    }
                }
            }
        }

        /// <summary>
        ///     Gets or sets the index of the texture from where this atlas is loaded.
        /// </summary>
        /// <value>
        ///     The index of the texture inside the sprite array.
        /// </value>
        [DontRenderProperty]
        public int TextureIndex
        {
            get
            {
                return this.textureIndex;
            }

            set
            {
                if (this.SpriteSheet != null)
                {
                    this.textureIndex = value;

                    if (this.textureIndex < 0 && this.textureIndex >= this.SpriteSheet.Sprites.Length)
                    {
                        this.textureIndex = 0;
                    }

                    this.textureName = this.SpriteSheet.Sprites[value].Name;
                    this.RefreshSourceRectangle();
                }
            }
        }

        /// <summary>
        /// Gets the texture names.
        /// </summary>
        /// <value>
        /// The texture names.
        /// </value>
        [DontRenderProperty]
        public IEnumerable<string> TextureNames
        {
            get
            {
                if (this.SpriteSheet != null)
                {
                    return this.SpriteSheet.SpriteDictionary
                                            .Keys
                                            .AsEnumerable<string>();
                }

                return null;
            }
        }

        #endregion

        #region Initialize
        /// <summary>
        /// Initializes a new instance of the <see cref="SpriteAtlas" /> class.
        /// </summary>
        public SpriteAtlas()
            : base("SpriteAtlas" + instances++)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SpriteAtlas" /> class.
        /// </summary>
        /// <param name="atlasPath">The path to the atlas.</param>
        /// <exception cref="System.ArgumentException">TexturePath can not be null.</exception>
        public SpriteAtlas(string atlasPath)
            : base("SpriteAtlas" + instances++)
        {
            if (string.IsNullOrEmpty(atlasPath))
            {
                throw new ArgumentException("AtlasPath can not be null.");
            }

            this.SpriteSheetPath = atlasPath;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SpriteAtlas" /> class.
        /// </summary>
        /// <param name="atlasPath">The path to the atlas.</param>
        /// <param name="textureName">Name of the texture from where this atlas is loaded.</param>
        /// <exception cref="System.ArgumentException">TexturePath can not be null.</exception>
        public SpriteAtlas(string atlasPath, string textureName)
            : this(atlasPath)
        {
            if (string.IsNullOrEmpty(textureName))
            {
                throw new ArgumentException("TextureName can not be null.");
            }

            this.TextureName = textureName;
        }

        /// <summary>
        /// Sets default values
        /// </summary>
        protected override void DefaultValues()
        {
            base.DefaultValues();

            this.Transform2D = null;
            this.isGlobalAsset = false;
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
                    if (!this.IsGlobalAsset && this.isInitialized && !string.IsNullOrEmpty(this.spriteSheetPath))
                    {
                        Assets.UnloadAsset(this.SpriteSheetPath);
                        this.SpriteSheet = null;
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
                throw new ObjectDisposedException("SpriteAtlas");
            }

            base.Initialize();

            this.RefreshAtlasTexture();
            this.RefreshTextureIndex();
        }

        /// <summary>
        /// Refresh the atlas texture
        /// </summary>
        private void RefreshAtlasTexture()
        {
            if (this.SpriteSheet != null && !string.IsNullOrEmpty(this.SpriteSheet.AssetPath))
            {
                if (this.currentIsGlobalAsset)
                {
                    WaveServices.Assets.Global.UnloadAsset(this.SpriteSheet.AssetPath);
                }
                else
                {
                    this.Assets.UnloadAsset(this.SpriteSheet.AssetPath);
                }

                this.SpriteSheet = null;
            }

            if (this.SpriteSheet == null && !string.IsNullOrEmpty(this.SpriteSheetPath))
            {
                if (this.isGlobalAsset)
                {
                    this.SpriteSheet = WaveServices.Assets.Global.LoadAsset<SpriteSheet>(this.SpriteSheetPath);
                }
                else
                {
                    this.SpriteSheet = Assets.LoadAsset<SpriteSheet>(this.SpriteSheetPath);
                }

                if (this.TextureName == null)
                {
                    this.TextureName = this.TextureNames.FirstOrDefault();
                }

                this.currentIsGlobalAsset = true;

                this.RefreshSourceRectangle();
            }
        }

        /// <summary>
        /// Refresh the source rectangle of the sprite transform 2D.
        /// </summary>
        /// <exception cref="System.ObjectDisposedException">SpriteAtlas has been disposed.</exception>
        protected void RefreshSourceRectangle()
        {
            if (this.SpriteSheet != null && this.SpriteSheet.Sprites.Count() > 0)
            {
                if (this.textureIndex >= this.SpriteSheet.Sprites.Length)
                {
                    this.TextureIndex = this.SpriteSheet.Sprites.Length - 1;
                }

                this.SourceRectangle = this.SpriteSheet.Sprites[this.textureIndex].Rectangle;
                RectangleF rectangle = this.Transform2D.Rectangle;
                rectangle.Width = this.SourceRectangle.Width;
                rectangle.Height = this.SourceRectangle.Height;
                this.Transform2D.Rectangle = rectangle;
            }
        }

        /// <summary>
        /// Refresh texture index from its name
        /// </summary>
        private void RefreshTextureIndex()
        {
            if (this.SpriteSheet != null)
            {
                SpriteSheetResource resource;
                this.SpriteSheet.SpriteDictionary.TryGetValue(this.textureName, out resource);

                if (resource != null)
                {
                    this.textureIndex = resource.Index;
                }
                else
                {
                    this.textureName = this.SpriteSheet.Sprites[0].Name;
                    this.textureIndex = 0;
                }

                this.RefreshSourceRectangle();
            }
        }
        #endregion
    }
}