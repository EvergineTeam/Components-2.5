#region File Description
//-----------------------------------------------------------------------------
// MaterialsMap
//
// Copyright © 2016 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using WaveEngine.Common.Attributes;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.IO;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Models;
using WaveEngine.Framework.Resources;
using WaveEngine.Materials;
#endregion

namespace WaveEngine.Components.Graphics3D
{
    /// <summary>
    /// A list of materials.
    /// </summary>
    [DataContract(Namespace = "WaveEngine.Components.Graphics3D")]
    public class MaterialsMap : Component
    {
        /// <summary>   
        /// Number of instances of this component created.
        /// </summary>
        private static int instances;

        /// <summary>
        /// The use material copy
        /// </summary>
        private bool useMaterialCopy;

        /// <summary>
        /// Default Material
        /// </summary>
        private Material defaultMaterial;

        /// <summary>
        /// Default Material Path
        /// </summary>
        private string defaultMaterialPath;

        /// <summary>
        /// The materials path
        /// </summary>
        private Dictionary<string, string> materialsPath;

        /// <summary>
        /// Default material is used (only serialization information)
        /// </summary>
        [DataMember]
        private bool useDefaultMaterial;

        /// <summary>
        /// Dummy material is used (only serialization information)
        /// </summary>
        [DataMember]
        private bool useDummyMaterial;

        #region Properties
        /// <summary>
        /// Gets or sets the default material.
        /// </summary>
        /// <value>
        /// The default material.
        /// </value>
        [DontRenderProperty]
        public Material DefaultMaterial
        {
            get
            {
                return this.defaultMaterial;
            }

            set
            {
                this.defaultMaterial = value;
                this.useDefaultMaterial = false;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the component will use an individual copy of the material file, instead of sharing the material instance.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the material file will be copied otherwise, <c>false</c>.
        /// </value>
        [DataMember]
        public bool UseMaterialCopy
        {
            get
            {
                return this.useMaterialCopy;
            }

            set
            {
                this.useMaterialCopy = value;
            }
        }

        /// <summary>
        /// Gets or sets the default material path.
        /// </summary>
        /// <value>
        /// The default material path.
        /// </value>
        [RenderPropertyAsAsset(AssetType.Material)]
        [DataMember]
        public string DefaultMaterialPath
        {
            get
            {
                return this.defaultMaterialPath;
            }

            set
            {
                this.defaultMaterialPath = value;

                this.useDefaultMaterial = string.IsNullOrEmpty(value);

                this.RefreshDefaultMaterialPath();
            }
        }

        /// <summary>
        /// Gets or sets the materials.
        /// </summary>
        [DontRenderProperty]
        public Dictionary<string, Material> Materials { get; set; }

        /// <summary>
        /// Gets or sets the materials of paths.
        /// </summary>
        [DataMember]
        [RenderPropertyAsAsset(AssetType.Material)]
        public Dictionary<string, string> MaterialsPath
        {
            get
            {
                return this.materialsPath;
            }

            set
            {
                this.materialsPath = value;
                if (this.isInitialized)
                {
                    this.RefreshMaterialsPath();
                }
            }
        }
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
            this.defaultMaterial = material;
            this.useDefaultMaterial = false;
            this.useDummyMaterial = false;
            this.defaultMaterialPath = null;
            this.Materials = new Dictionary<string, Material>();
            this.MaterialsPath = new Dictionary<string, string>();
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

            this.MaterialsPath = new Dictionary<string, string>();
            this.useDefaultMaterial = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MaterialsMap"/> class.
        /// Deserializing process contructor
        /// </summary>
        /// <param name="context">Streaming Context</param>
        [OnDeserialized]
        protected void OnDeserialized(StreamingContext context)
        {
            if (this.useDummyMaterial)
            {
                this.defaultMaterial = null;
            }
        }
        #endregion

        #region Private Methods

        /// <summary>
        /// Performs further custom initialization for this instance.
        /// </summary>
        protected override void ResolveDependencies()
        {
            this.useDefaultMaterial = (this.defaultMaterial == null) && string.IsNullOrEmpty(this.defaultMaterialPath);
            this.RefreshDefaultMaterialPath();

            if (this.Materials != null && this.Materials.Count > 0)
            {
                this.RefreshMaterials();
            }
            else
            {
                this.RefreshMaterialsPath();
            }
        }

        /// <summary>
        /// Refreshes the materials.
        /// </summary>
        private void RefreshMaterials()
        {
            if (this.Materials == null)
            {
                this.Materials = new Dictionary<string, Material>();
            }

            foreach (KeyValuePair<string, Material> kv in this.Materials)
            {
                kv.Value.Initialize(this.Assets);
            }
        }

        /// <summary>
        /// Refreshes the default material.
        /// </summary>
        private void RefreshDefaultMaterial()
        {
            if (this.DefaultMaterial != null)
            {
                this.DefaultMaterial.Initialize(this.Assets);
            }
        }

        /// <summary>
        /// Refreshes the materials path.
        /// </summary>
        private void RefreshMaterialsPath()
        {
            if (this.Assets != null)
            {
                if (this.Materials != null)
                {
                    this.Materials.Clear();
                }
                else
                {
                    this.Materials = new Dictionary<string, Material>();
                }

                if ((this.MaterialsPath != null) && (this.MaterialsPath.Count > 0))
                {
                    foreach (var kv in this.MaterialsPath)
                    {
                        var materialModel = this.Assets.LoadModel<MaterialModel>(kv.Value, !this.useMaterialCopy);
                        this.Materials[kv.Key] = materialModel.Material;
                    }
                }
            }

            this.RefreshMaterials();
        }

        /// <summary>
        /// Refreshes the default material path.
        /// </summary>
        private void RefreshDefaultMaterialPath()
        {
            if (!this.useDefaultMaterial)
            {
                if ((this.Assets != null) && !string.IsNullOrEmpty(this.defaultMaterialPath))
                {
                    var materialModel = this.Assets.LoadModel<MaterialModel>(this.defaultMaterialPath, !this.useMaterialCopy);
                    this.defaultMaterial = materialModel.Material;
                    this.useDefaultMaterial = false;
                }
            }
            else
            {
                this.defaultMaterial = new WaveEngine.Materials.StandardMaterial(Color.White, DefaultLayers.Opaque)
                {
                    LightingEnabled = false
                };

                this.useDummyMaterial = true;
            }

            this.RefreshDefaultMaterial();
        }
        #endregion
    }
}