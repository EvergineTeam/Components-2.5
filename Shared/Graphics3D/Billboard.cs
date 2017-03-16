#region File Description
//-----------------------------------------------------------------------------
// Billboard
//
// Copyright © 2017 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
#endregion

using System;
using System.Runtime.Serialization;
using WaveEngine.Common.Attributes;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Math;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Services;

namespace WaveEngine.Components.Graphics2D
{
    /// <summary>
    /// Represents a 2D image. Such image is loaded from a content file (.wpk),
    /// which is generated from a main PNG or JPEG file format.
    /// </summary>
    [DataContract(Namespace = "WaveEngine.Components.Graphics2D")]
    public class Billboard : Component
    {
        /// <summary>
        /// Billboard texture path
        /// </summary>
        [DataMember]
        protected string texturePath;

        /// <summary>
        /// The billboard texture
        /// </summary>
        protected Texture texture;

        /// <summary>
        /// Number of instances of this component created.
        /// </summary>
        private static int instances;

        /// <summary>
        /// The is global asset.
        /// </summary>
        [DataMember]
        private bool isGlobalAsset;

        /// <summary>
        /// The current texture is global asset.
        /// </summary>        
        private bool currentIsGlobalAsset;

        /// <summary>
        /// The disposed
        /// </summary>
        protected bool disposed;

        /// <summary>
        /// The final size
        /// </summary>
        protected Vector2 size;

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
                this.isGlobalAsset = value;

                if (this.isInitialized
                    && (this.currentIsGlobalAsset != this.isGlobalAsset))
                {
                    this.RefreshTexture();
                }
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
        [DataMember]
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
        ///     Gets the texture.
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
        }

        /// <summary>
        /// Gets or sets the tint color.
        /// Each pixel of the Billboard will be multiplied by such color during the drawing.
        /// By default, it is white.
        /// </summary>
        /// <value>
        /// The tint color.
        /// </value>
        [DataMember]
        public Color TintColor { get; set; }

        /// <summary>
        /// Gets or sets the Billboard Origin. The origin (also known as pivot) from where the entity scales, rotates and translates.
        /// Its values are included in [0, 1] where (0, 0) indicates the top left corner.
        /// Such values are percentages where 1 means the 100% of the rectangle's width/height.
        /// </summary>
        [DataMember]
        public Vector2 Origin { get; set; }

        /// <summary>
        /// Gets or sets the Billboard rotation
        /// </summary>
        [DataMember]
        public float Rotation { get; set; }

        /// <summary>
        /// Gets or sets the Billboard look at camera type
        /// </summary>
        [DataMember]
        public BillboardType BillboardType { get; set; }

        /// <summary>
        /// Gets the size of the Billboard
        /// </summary>
        internal Vector2 Size
        {
            get { return this.size; }
        }
        #endregion

        #region Initialize
        /// <summary>
        /// Initializes a new instance of the <see cref="Billboard" /> class
        /// based on a content file.
        /// </summary>
        public Billboard()
            : base("Billboard" + instances++)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Billboard" /> class
        /// based on a content file.
        /// </summary>
        /// <param name="texturePath">The texture path to the content file.</param>
        /// <exception cref="System.ArgumentException">TexturePath can not be null.</exception>
        public Billboard(string texturePath)
            : base("Billboard" + instances++)
        {
            if (string.IsNullOrEmpty(texturePath))
            {
                throw new ArgumentException("TexturePath can not be null.");
            }

            this.texturePath = texturePath;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Billboard" /> class
        /// based on a texture.
        /// See <see cref="Texture"/> for more information.
        /// </summary>
        /// <param name="texture">The texture.</param>        
        /// <exception cref="System.ArgumentException">Texture can not be null.</exception>
        public Billboard(Texture texture)
            : base("Billboard" + instances++)
        {
            if (texture == null)
            {
                throw new ArgumentException("Texture can not be null.");
            }

            this.texture = texture;
        }

        /// <summary>
        /// The default values
        /// </summary>
        protected override void DefaultValues()
        {
            base.DefaultValues();
            this.isGlobalAsset = false;
            this.TintColor = Color.White;
            this.Origin = Vector2.Center;
            this.BillboardType = BillboardType.PointOrientation;
            this.Rotation = 0;
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
                throw new ObjectDisposedException("Billboard");
            }

            this.RefreshTexture();
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

        /// <summary>
        /// Reload the billboard texture
        /// </summary>
        private void RefreshTexture()
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

            this.UpdateSize();
        }

        /// <summary>
        /// The update source rectangle.
        /// </summary>
        /// <exception cref="System.ObjectDisposedException">Billboard has been disposed.</exception>
        private void UpdateSize()
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException("Billboard");
            }

            if (this.texture != null)
            {
                float aspectRatio = this.texture.Width / (float)this.texture.Height;

                if (aspectRatio > 1)
                {
                    this.size.X = 1;
                    this.size.Y = 1 / aspectRatio;
                }
                else
                {
                    this.size.X = 1 / aspectRatio;
                    this.size.Y = 1;
                }
            }
        }
        #endregion
    }
}
