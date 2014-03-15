#region File Description
//-----------------------------------------------------------------------------
// MaterialsMap
//
// Copyright © 2010 - 2013 Wave Coorporation. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using WaveEngine.Common.Graphics;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Materials;
#endregion

namespace WaveEngine.Components.Graphics3D
{
    /// <summary>
    /// A list of materials.
    /// </summary>
    public class MaterialsMap : Component
    {
        /// <summary>   
        /// Number of instances of this component created.
        /// </summary>
        private static int instances;

        /// <summary>
        /// Internal path for the default texture resource.
        /// </summary>
#if WINDOWS
        private readonly string defaultTextureResourcePath = "WaveEngine.Components.Resources.DefaultTexture.wpk";
#endif
#if WINDOWS_PHONE
        private readonly string defaultTextureResourcePath = "WaveEngineWP.Components.Resources.DefaultTexture.wpk";
#endif
#if METRO
        private readonly string defaultTextureResourcePath = "WaveEngineMetro.Components.Resources.DefaultTexture.wpk";
#endif
#if OUYA
        private readonly string defaultTextureResourcePath = "WaveEngineOUYA.Components.Resources.DefaultTexture.wpk";
#elif ANDROID
        private readonly string defaultTextureResourcePath = "WaveEngineAndroid.Components.Resources.DefaultTexture.wpk";
#elif IOS
        private readonly string defaultTextureResourcePath = "WaveEngineiOS.Components.Resources.DefaultTexture.wpk";
#elif MAC
		private readonly string defaultTextureResourcePath = "WaveEngineMac.Components.Resources.DefaultTexture.wpk";
#elif LINUX
		private readonly string defaultTextureResourcePath = "WaveEngineLinux.Components.Resources.DefaultTexture.wpk";
#endif

        /// <summary>
        /// Default texture in case we don't specify any materials.
        /// </summary>
        private readonly Texture2D defaultTexture;

        #region Properties
        /// <summary>
        /// Gets or sets the default material.
        /// </summary>
        /// <value>
        /// The default material.
        /// </value>
        public Material DefaultMaterial { get; set; }

        /// <summary>
        /// Gets the materials.
        /// </summary>
        public Dictionary<string, Material> Materials { get; private set; }

        #endregion

        #region Initialize
        /// <summary>
        /// Initializes a new instance of the <see cref="MaterialsMap" /> class.
        /// </summary>
        public MaterialsMap()
            : this("MaterialMap" + instances, new Dictionary<string, Material>())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MaterialsMap" /> class.
        /// </summary>
        /// <param name="materials">The materials.</param>
        public MaterialsMap(Dictionary<string, Material> materials)
            : this("MaterialMap" + instances, materials)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MaterialsMap" /> class.
        /// </summary>
        /// <param name="material">Material applied to all meshes.</param>
        public MaterialsMap(Material material)
            : base("MaterialMap" + instances)
        {
            instances++;
            this.DefaultMaterial = material;
            this.Materials = new Dictionary<string, Material>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MaterialsMap"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="materials">The materials.</param>
        public MaterialsMap(string name, Dictionary<string, Material> materials)
            : base(name)
        {
            instances++;
            this.Materials = materials;

            Stream textureStream;
#if METRO || PSSuite
            Assembly assembly = this.GetType().GetTypeInfo().Assembly;
            textureStream = assembly.GetManifestResourceStream(defaultTextureResourcePath);
#else
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            textureStream = assembly.GetManifestResourceStream(this.defaultTextureResourcePath);
#endif
            this.defaultTexture = Framework.Services.WaveServices.Assets.Global.LoadAsset<Texture2D>(this.defaultTextureResourcePath, textureStream);

            this.DefaultMaterial = new BasicMaterial(this.defaultTexture);
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Performs further custom initialization for this instance.
        /// </summary>
        protected override void Initialize()
        {
            foreach (KeyValuePair<string, Material> kv in this.Materials)
            {
                kv.Value.Initialize(this.Assets);
            }

            if (this.DefaultMaterial != null)
            {
                this.DefaultMaterial.Initialize(this.Assets);
            }
        }
        #endregion
    }
}
