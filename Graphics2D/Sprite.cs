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
    /// Represents a 2D Image.
    /// </summary>
    public class Sprite : Component
    {
        /// <summary>
        ///     Number of instances of this component created.
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
        /// Transform of the Image.
        /// </summary>
        [RequiredComponent]
        public Transform2D Transform2D;

        /// <summary>
        /// Rectangle that represents this Sprite in case it is part of a bigger Image.
        /// </summary>
        public Rectangle? SourceRectangle;

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether this asset is global.
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
        /// Gets or sets the texture path.
        /// </summary>
        /// <value>
        ///     The texture path.
        /// </value>
        public string TexturePath { get; protected set; }

        /// <summary>
        ///     Gets or sets the texture.
        /// </summary>
        /// <value>
        ///     The texture.
        /// </value>
        public Texture2D Texture { get; protected set; }

        /// <summary>
        /// Gets or sets the color of the tint.
        /// </summary>
        /// <value>
        /// The color of the tint.
        /// </value>
        public Color TintColor { get; set; }

        #endregion

        #region Initialize

        /// <summary>
        /// Initializes a new instance of the <see cref="Sprite" /> class.
        /// </summary>
        /// <param name="texturePath">The texture path.</param>
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
            this.TexturePath = texturePath;
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
                this.Texture = WaveServices.Assets.Global.LoadAsset<Texture2D>(this.TexturePath);
            }
            else
            {
                this.Texture = this.Assets.LoadAsset<Texture2D>(this.TexturePath);
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
                    if (!this.IsGlobalAsset)
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
