// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Using Statements
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using WaveEngine.Common.Attributes;
using WaveEngine.Common.Graphics;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Models;
#endregion

namespace WaveEngine.Components.Graphics3D
{
    /// <summary>
    /// A material uses to render meshes.
    /// </summary>
    [DataContract]
    [AllowMultipleInstances]
    public class MaterialComponent : Component
    {
        /// <summary>
        /// Number of instances of this component created.
        /// </summary>
        private static int instances;

        /// <summary>
        /// The use material copy
        /// </summary>
        private bool useCopy;

        /// <summary>
        /// Dummy material is used (only serialization information)
        /// </summary>
        [DataMember]
        private bool useDummyMaterial;

        /// <summary>
        /// The Material instance
        /// </summary>
        private Material material;

        /// <summary>
        /// The material path
        /// </summary>
        private string materialPath;

        /// <summary>
        /// Instance of MeshComponent
        /// </summary>
        private MeshComponent meshComponent;

        #region properties

        /// <summary>
        /// Gets or sets the material.
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

                this.useDummyMaterial = value == null;

                this.materialPath = string.Empty;

                if (this.isInitialized)
                {
                    this.RefreshMaterial();
                }
            }
        }

        /// <summary>
        /// Gets or sets the specify material included on FBX file
        /// </summary>
        [RenderPropertyAsSelector("FileMaterialNames")]
        [DataMember]
        public string AsignedTo { get; set; }

        /// <summary>
        /// Gets the material name list on FBX file
        /// </summary>
        [DontRenderProperty]
        public IEnumerable FileMaterialNames
        {
            get
            {
                List<string> materialNames = new List<string>();
                if (this.meshComponent != null && this.meshComponent.Meshes != null && this.meshComponent.InternalModel != null)
                {
                    var meshMaterials = this.meshComponent.InternalModel.Materials;
                    var meshes = this.meshComponent.Meshes;
                    foreach (Mesh mesh in meshes)
                    {
                        string materialName = meshMaterials[mesh.MaterialIndex];

                        if (!materialNames.Contains(materialName))
                        {
                            materialNames.Add(materialName);
                        }
                    }
                }

                return materialNames;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the component will use an individual copy of the material file, instead of sharing the material instance.
        /// </summary>
        [DataMember]
        public bool UseCopy
        {
            get
            {
                return this.useCopy;
            }

            set
            {
                this.useCopy = value;
            }
        }

        /// <summary>
        /// Gets or sets the default material path.
        /// </summary>
        [RenderPropertyAsAsset(AssetType.Material)]
        [DataMember]
        public string MaterialPath
        {
            get
            {
                return this.materialPath;
            }

            set
            {
                this.materialPath = value;
                this.useDummyMaterial = string.IsNullOrEmpty(value);

                this.material = null;

                if (this.isInitialized)
                {
                    this.RefreshMaterial();
                }
            }
        }
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="MaterialComponent" /> class.
        /// </summary>
        public MaterialComponent()
                : base("MaterialComponent" + instances++)
        {
        }

        /// <summary>
        /// Default values method
        /// </summary>
        protected override void DefaultValues()
        {
            base.DefaultValues();

            this.useDummyMaterial = true;
        }

        /// <summary>
        /// Performs further custom initialization for this instance.
        /// </summary>
        protected override void ResolveDependencies()
        {
            this.meshComponent = this.Owner.FindComponent<MeshComponent>(false);
        }

        /// <summary>
        /// Initialize method
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();

            this.RefreshMaterial();
        }

        /// <summary>
        /// Refreshes the default material path.
        /// </summary>
        private void RefreshMaterial()
        {
            if (!this.useDummyMaterial)
            {
                if ((this.Assets != null) && !string.IsNullOrEmpty(this.materialPath))
                {
                    var materialModel = this.Assets.LoadModel<MaterialModel>(this.materialPath, !this.useCopy);
                    this.material = materialModel.Material;
                }

                if (this.Material != null)
                {
                    this.Material.Initialize(this.Assets);
                }
            }
            else
            {
                if (this.meshComponent != null)
                {
                    if (this.Owner.IsInitialized)
                    {
                        this.SetDummyMaterial();
                    }
                    else
                    {
                        this.meshComponent.OnComponentInitialized += (s, e) =>
                        {
                            this.SetDummyMaterial();
                        };
                    }
                }
                else
                {
                    // Particle Systems 2D y 3D
                    this.material = new WaveEngine.Materials.StandardMaterial(Color.White, DefaultLayers.Opaque)
                    {
                        LightingEnabled = false
                    };

                    this.Material.Initialize(this.Assets);
                }
            }
        }

        private void SetDummyMaterial()
        {
            this.material = new WaveEngine.Materials.StandardMaterial(Color.White, DefaultLayers.Opaque)
            {
                LightingEnabled = this.meshComponent.IsVertexElementSupported(VertexElementUsage.Normal)
            };

            this.Material.Initialize(this.Assets);
        }
    }
}
