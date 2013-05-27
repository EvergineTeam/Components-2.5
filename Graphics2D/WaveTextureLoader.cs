#region File Description
//-----------------------------------------------------------------------------
// WaveTextureLoader
//
// Copyright © 2010 - 2013 Wave Coorporation. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System.IO;
using WaveEngine.Components.Animation.Spine;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Services;
#endregion

namespace WaveEngine.Components.Graphics2D
{
    /// <summary>
    /// Loader for wpk textures
    /// </summary>
    internal class WaveTextureLoader : ITextureLoader
    {
        /// <summary>
        /// The assets
        /// </summary>
        private AssetsContainer assets;

        #region Initialize
        /// <summary>
        /// Initializes a new instance of the <see cref="WaveTextureLoader" /> class.
        /// </summary>
        /// <param name="assets">The assets.</param>
        public WaveTextureLoader(AssetsContainer assets)
        {
            this.assets = assets;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Loads the specified page.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="path">The path.</param>
        public void Load(AtlasPage page, string path)
        {
            path = Path.ChangeExtension(path, ".wpk");
            Texture2D texture = this.assets.LoadAsset<Texture2D>(path);
            page.Texture = texture;
            page.Width = texture.Width;
            page.Height = texture.Height;
        }

        /// <summary>
        /// Unloads the specified texture.
        /// </summary>
        /// <param name="texture">The texture.</param>
        public void Unload(object texture)
        {
            this.assets.UnloadAsset(((Texture2D)texture).AssetPath);
        }
        #endregion
    }
}
