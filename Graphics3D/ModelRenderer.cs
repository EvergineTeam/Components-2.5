#region File Description
//-----------------------------------------------------------------------------
// ModelRenderer
//
// Copyright © 2010 - 2013 Wave Coorporation. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Math;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Materials;

#endregion

namespace WaveEngine.Components.Graphics3D
{
    /// <summary>
    /// Renders a 3D model on the screen.
    /// </summary>
    public class ModelRenderer : Drawable3D
    {
        /// <summary>
        /// Number of instances of this component created.
        /// </summary>
        private static int instances;

        /// <summary>
        /// <see cref="Model"/> to render.
        /// </summary>
        [RequiredComponent]
        public Model Model;

        /// <summary>
        /// Materials used rendering the <see cref="Model"/>.
        /// </summary>
        [RequiredComponent]
        public MaterialsMap MaterialMap;

        /// <summary>
        /// Transform of the <see cref="Model"/>.
        /// </summary>
        [RequiredComponent]
        public Transform3D Transform;

        /// <summary>
        /// The diffuse color
        /// </summary>
        private Color diffuseColor;

        /// <summary>
        /// The alpha
        /// </summary>
        private float alpha;

        /// <summary>
        /// The bone names
        /// </summary>
        private readonly Dictionary<string, int> boneNames;

        /// <summary>
        /// The last mesh id
        /// </summary>
        private int lastModelId;

        /// <summary>
        /// The mesh materials
        /// </summary>
        private Material[] meshMaterials;

        /// <summary>
        /// Wether this instance has been disposed.
        /// </summary>
        private bool disposed;

        #region Properties
        /// <summary>
        /// Gets or sets the diffuse color used when rendering the model.
        /// </summary>
        /// <value>
        /// The diffuse color.
        /// </value>
        public Color DiffuseColor
        {
            get
            {
                return this.diffuseColor;
            }

            set
            {
                if (value != this.diffuseColor)
                {
                    this.diffuseColor = value;

                    if (this.MaterialMap != null)
                    {
                        foreach (KeyValuePair<string, Material> kv in this.MaterialMap.Materials)
                        {
                            var material = kv.Value as BasicMaterial;
                            if (material != null)
                            {
                                material.DiffuseColor = this.diffuseColor;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the alpha value used when rendering the model.
        /// </summary>
        /// <value>
        /// The alpha value.
        /// </value>
        public float Alpha
        {
            get
            {
                return this.alpha;
            }

            set
            {
                if (value != this.alpha)
                {
                    this.alpha = value;

                    if (this.MaterialMap != null)
                    {
                        foreach (KeyValuePair<string, Material> kv in this.MaterialMap.Materials)
                        {
                            var material = kv.Value as BasicMaterial;
                            if (material != null)
                            {
                                material.Alpha = this.alpha;
                            }
                        }
                    }
                }
            }
        }
        #endregion

        #region Initialize
        /// <summary>
        /// Initializes a new instance of the <see cref="ModelRenderer"/> class.
        /// </summary>
        public ModelRenderer()
            : this("ModelRenderer" + instances)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ModelRenderer"/> class.
        /// </summary>
        /// <param name="name">Name of this instance.</param>
        public ModelRenderer(string name)
            : base(name)
        {
            this.boneNames = new Dictionary<string, int>();
            this.DiffuseColor = Color.White;
            this.Alpha = 1.0f;
            instances++;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Draws the model.
        /// </summary>
        /// <param name="gameTime">The elapsed game time.</param>
        public override void Draw(TimeSpan gameTime)
        {
            if (this.lastModelId != this.Model.GetHashCode())
            {
                this.meshMaterials = new Material[this.Model.InternalModel.Meshes.Count];
                this.lastModelId = this.Model.GetHashCode();
            }

            for (int i = 0; i < this.Model.InternalModel.Meshes.Count; i++)
            {
                StaticMesh currentMesh = this.Model.InternalModel.Meshes[i];
                Material currentMaterial;

                if (currentMesh.Name != null && this.MaterialMap.Materials.ContainsKey(currentMesh.Name))
                {
                    currentMaterial = this.MaterialMap.Materials[currentMesh.Name];
                }
                else
                {
                    currentMaterial = this.MaterialMap.DefaultMaterial;
                }

                this.meshMaterials[i] = currentMaterial;

                if (currentMaterial != null)
                {
                    var layer = this.RenderManager.FindLayer(currentMaterial.LayerType);

                    layer.AddDrawable(i, this, currentMaterial.GetHashCode());
                }
            }
        }

        /// <summary>
        /// Tries to get the world transform of a given bone.
        /// </summary>
        /// <param name="boneName">Name of the bone.</param>
        /// <param name="transform">The transform of the bone.</param>
        /// <returns><c>true</c> if it was possible to get the world transform, otherwise <c>false</c></returns>
        public bool TryGetBoneWorldTransform(string boneName, out Matrix transform)
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException("ModelRenderer");
            }

            int count = this.Model.InternalModel.Bones.Count;

            int index;
            if (this.boneNames.TryGetValue(boneName, out index))
            {
                transform = this.Model.InternalModel.Bones[index].AbsoluteTransform;
                return true;
            }

            for (int i = 0; i < count; i++)
            {
                Bone bone = this.Model.InternalModel.Bones[i];
                if (bone.Name == boneName)
                {
                    this.boneNames.Add(boneName, i);
                    transform = bone.AbsoluteTransform;

                    return true;
                }
            }

            transform = Matrix.Identity;

            return false;
        }
        #endregion

        #region Private Methods

        /// <summary>
        /// The draw basic unit.
        /// </summary>
        /// <param name="parameter">The mesh index.</param>
        protected override void DrawBasicUnit(int parameter)
        {
            StaticMesh currentMesh = this.Model.InternalModel.Meshes[parameter];
            Material currentMaterial = this.meshMaterials[parameter];

            if (currentMaterial != null)
            {
                InternalStaticModel internalModel = this.Model.InternalModel;
                int index = internalModel.MeshBonePairs[parameter];
                Matrix absoluteTransform = internalModel.Bones[index].AbsoluteTransform;
                Matrix.Multiply(ref absoluteTransform, ref this.Transform.LocalWorld, out currentMaterial.Matrices.World);

                currentMaterial.Apply(this.RenderManager);

                this.GraphicsDevice.DrawVertexBuffer(
                                          currentMesh.NumVertices,
                                          currentMesh.PrimitiveCount,
                                          PrimitiveType.TriangleList,
                                          currentMesh.VertexBuffer,
                                          currentMesh.IndexBuffer);
            }
        }

        /// <summary>
        /// Resolves the dependencies.
        /// </summary>
        protected override void ResolveDependencies()
        {
            base.ResolveDependencies();

            this.boneNames.Clear();
        }

        /// <summary>
        /// Deletes the dependencies.
        /// </summary>
        protected override void DeleteDependencies()
        {
            base.DeleteDependencies();

            this.boneNames.Clear();
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    this.boneNames.Clear();
                    this.disposed = true;
                }
            }
        }
        #endregion
    }
}
