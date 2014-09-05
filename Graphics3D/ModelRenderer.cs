#region File Description
//-----------------------------------------------------------------------------
// ModelRenderer
//
// Copyright © 2014 Wave Corporation
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

        /// <summary>
        /// Meshes world matrix for static entities
        /// </summary>
        private Matrix[] cachedWorlds;

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
        /// Initialize model renderer
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();

            // For static entities create a world cache
            if (this.Owner.IsStatic)
            {
                this.cachedWorlds = new Matrix[this.Model.MeshCount];

                for (int i = 0; i < this.cachedWorlds.Length; i++)
                {
                    InternalStaticModel internalModel = this.Model.InternalModel;
                    int index = internalModel.MeshBonePairs[i];
                    Matrix absoluteTransform = internalModel.Bones[index].AbsoluteTransform;
                    Matrix world = this.Transform.WorldTransform;
                    Matrix.Multiply(ref absoluteTransform, ref world, out this.cachedWorlds[i]);
                }
            }
        }

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

            float zOrder = Vector3.DistanceSquared(this.RenderManager.CurrentDrawingCamera3D.Position, this.Transform.Position);

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
                    Matrix world;
                                        
                    if (!this.Owner.IsStatic)
                    {
                        // Obtain world matrix from scrach
                        InternalStaticModel internalModel = this.Model.InternalModel;
                        int index = internalModel.MeshBonePairs[i];
                        Matrix absoluteTransform = internalModel.Bones[index].AbsoluteTransform;
                        Matrix worldTransform = this.Transform.WorldTransform;
                        Matrix.Multiply(ref absoluteTransform, ref worldTransform, out world);
                    }
                    else
                    {
                        // obtain world from cached array
                        world = this.cachedWorlds[i];
                    }

                    currentMesh.ZOrder = zOrder;

                    // Draw mesh
                    this.RenderManager.DrawMesh(currentMesh, currentMaterial, ref world, this.Owner.IsStatic);
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
