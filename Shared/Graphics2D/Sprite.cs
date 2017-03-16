#region File Description
//-----------------------------------------------------------------------------
// Sprite
//
// Copyright © 2017 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Runtime.Serialization;
using WaveEngine.Common.Attributes;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Math;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Models;
using WaveEngine.Framework.Services;
#endregion

namespace WaveEngine.Components.Graphics2D
{
    /// <summary>
    /// Represents a 2D image. Such image is loaded from a content file (.wpk),
    /// which is generated from a main PNG or JPEG file format.
    /// </summary>
    [DataContract(Namespace = "WaveEngine.Components.Graphics2D")]
    public class Sprite : Component
    {
        /// <summary>
        /// Sprite texture path
        /// </summary>
        [DataMember]
        protected string texturePath;

        /// <summary>
        /// The current isglobalasset value
        /// </summary>
        private bool currentIsGlobalAsset;

        /// <summary>
        /// The sprite texture
        /// </summary>
        protected Texture texture;

        /// <summary>
        /// Number of instances of this component created.
        /// </summary>
        private static int instances;

        /// <summary>
        /// The texture is loaded in global asset manager.
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
        private Rectangle? sourceRectangle;

        /// <summary>
        /// Sprite material
        /// </summary>
        private Material material;

        /// <summary>
        /// Sprite material path
        /// </summary>
        [DataMember]
        private string materialPath;

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether this asset is global.
        /// By "global" it is meant this asset will be consumed anywhere else. It implies 
        /// once this component is disposed, the asset it-self will not be unload from memory.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this asset is global; otherwise, <c>false</c>.
        /// </value>
        [DataMember]
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
                    this.RefreshTexture();
                }
            }
        }

        /// <summary>
        /// Gets or sets the Rectangle that represents this sprite in case it is part of a bigger image.
        /// Most of the cases this field will be null, which means the entire texture is used.
        /// On the other side, if a value is provided, will mean the rectangle (inside the original
        /// texture's rectangle) to be drawn.
        /// </summary>
        [DataMember]
        public Rectangle? SourceRectangle
        {
            get
            {
                return this.sourceRectangle;
            }

            set
            {
                this.sourceRectangle = value;
                this.UpdateSourceRectangle();
            }
        }

        /// <summary>
        /// Gets or sets the texture path.
        /// Such path is platform agnostic, and will always start with "Content/".
        /// Example: "Content/Characters/Tim.wpk"
        /// </summary>
        /// <value>
        ///     The texture path.
        /// </value>
        [RenderPropertyAsAsset(AssetType.Texture)]
        public string TexturePath
        {
            get
            {
                return this.texturePath;
            }

            set
            {
                this.texturePath = value;

                if (this.isInitialized)
                {
                    this.RefreshTexture();
                }
            }
        }

        /// <summary>
        ///     Gets or sets the texture.
        ///     Such is the in-memory representation for the given asset.
        ///     See <see cref="Texture"/> for more information.
        /// </summary>
        /// <value>
        ///     The texture.
        /// </value>
        [DontRenderProperty]
        public Texture Texture
        {
            get
            {
                return this.texture;
            }

            set
            {
                this.UnloadTexture();
                this.texture = value;

                if (this.texture != null)
                {
                    this.UpdateSourceRectangle();
                }
            }
        }

        /// <summary>
        /// Gets or sets the tint color.
        /// Each pixel of the sprite will be multiplied by such color during the drawing.
        /// By default, it is white.
        /// </summary>
        /// <value>
        /// The tint color.
        /// </value>
        [DataMember]
        public Color TintColor { get; set; }

        /// <summary>
        /// Gets or sets the material used to render the sprite
        /// </summary>
        [DontRenderProperty]
        public Material Material
        {
            get
            {
                return this.material;
            }

            set
            {
                this.material = value;

                if (this.isInitialized)
                {
                    this.InitMaterial();
                }
            }
        }

        /// <summary>
        /// Gets or sets the material path used to render the sprite
        /// </summary>
        [RenderPropertyAsAsset(AssetType.Material)]
        public string MaterialPath
        {
            get
            {
                return this.materialPath;
            }

            set
            {
                this.materialPath = value;

                if (this.isInitialized)
                {
                    this.RefreshMaterialFromPath();
                }
            }
        }

        #endregion

        #region Initialize
        /// <summary>
        /// Initializes a new instance of the <see cref="Sprite" /> class
        /// based on a content file.
        /// </summary>
        public Sprite()
            : base("Sprite" + instances++)
        {
        }

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

            this.texturePath = texturePath;
        }

        /// <summary>
        /// The default values
        /// </summary>
        protected override void DefaultValues()
        {
            base.DefaultValues();

            this.sourceRectangle = null;
            this.Transform2D = null;
            this.isGlobalAsset = false;
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

            this.texture = texture;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Sprite" /> class
        /// based on a content file.
        /// </summary>
        /// <param name="material">The material used to render the sprite.</param>
        /// <exception cref="System.ArgumentException">Material can not be null.</exception>
        public Sprite(Material material)
            : base("Sprite" + instances++)
        {
            if (material == null)
            {
                throw new ArgumentException("material can not be null.");
            }

            this.material = material;
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
                throw new ObjectDisposedException("Sprite");
            }

            if (this.material != null)
            {
                this.InitMaterial();
            }
            else
            {
                this.RefreshMaterialFromPath();
            }

            this.LoadTexture();
        }

        /// <summary>
        /// Refresh the sprite texture
        /// </summary>
        private void RefreshTexture()
        {
            this.UnloadTexture();
            this.LoadTexture();
        }

        /// <summary>
        /// Load texture
        /// </summary>
        private void LoadTexture()
        {
            if (this.texture == null && !string.IsNullOrEmpty(this.TexturePath))
            {
                if (this.isGlobalAsset)
                {
                    this.texture = WaveServices.Assets.Global.LoadAsset<Texture2D>(this.TexturePath);
                }
                else
                {
                    this.texture = this.Assets.LoadAsset<Texture2D>(this.TexturePath);
                }

                this.currentIsGlobalAsset = this.isGlobalAsset;
            }

            if (this.texture != null)
            {
                this.UpdateSourceRectangle();
            }
        }

        /// <summary>
        /// Init material
        /// </summary>
        private void InitMaterial()
        {
            if (this.material != null)
            {
                AssetsContainer assets;

                if (this.IsGlobalAsset)
                {
                    assets = WaveServices.Assets.Global;
                }
                else
                {
                    assets = this.Assets;
                }

                this.material.Initialize(assets);
            }
        }

        /// <summary>
        /// Init material
        /// </summary>
        private void RefreshMaterialFromPath()
        {
            this.material = null;

            if (!string.IsNullOrEmpty(this.materialPath))
            {
                AssetsContainer assets;

                if (this.IsGlobalAsset)
                {
                    assets = WaveServices.Assets.Global;
                }
                else
                {
                    assets = this.Assets;
                }

                if (assets != null)
                {
                    var materialModel = this.Assets.LoadModel<MaterialModel>(this.materialPath);
                    this.material = materialModel.Material;
                    this.material.Initialize(assets);
                }
            }
        }

        /// <summary>
        /// Unload texture
        /// </summary>
        private void UnloadTexture()
        {
            if (this.texture != null && !string.IsNullOrEmpty(this.texture.AssetPath))
            {
                if (this.currentIsGlobalAsset)
                {
                    WaveServices.Assets.Global.UnloadAsset(this.texture.AssetPath);
                }
                else
                {
                    this.Assets.UnloadAsset(this.texture.AssetPath);
                }

                this.texture = null;
            }
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

        #region Private Methods
        /// <summary>
        /// The update source rectangle.
        /// </summary>
        /// <exception cref="System.ObjectDisposedException">Sprite has been disposed.</exception>
        protected void UpdateSourceRectangle()
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException("Sprite");
            }

            if (this.texture != null)
            {
                RectangleF rectangle = this.Transform2D.Rectangle;

                if (this.sourceRectangle.HasValue)
                {
                    Rectangle rect = this.sourceRectangle.Value;
                    rectangle.Width = rect.Width;
                    rectangle.Height = rect.Height;
                }
                else
                {
                    rectangle.Width = this.texture.Width;
                    rectangle.Height = this.texture.Height;
                }

                this.Transform2D.Rectangle = rectangle;
            }
        }
        #endregion
    }
}
