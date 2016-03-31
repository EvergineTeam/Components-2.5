#region File Description
//-----------------------------------------------------------------------------
// ModelRenderer
//
// Copyright © 2016 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Math;
using WaveEngine.Common.Attributes;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Materials;

#endregion

namespace WaveEngine.Components.Graphics3D
{
    /// <summary>
    /// Renders a 3D model on the screen.
    /// </summary>
    [DataContract(Namespace = "WaveEngine.Components.Graphics3D")]
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
        /// The bone names
        /// </summary>
        private Dictionary<string, int> boneNames;

        /// <summary>
        /// Wether this instance has been disposed.
        /// </summary>
        private bool disposed;

        /// <summary>
        /// Meshes world matrix for static entities
        /// </summary>
        private Matrix[] cachedWorlds;

        #region Properties

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
        }

        /// <summary>
        /// Sets the default values
        /// </summary>
        protected override void DefaultValues()
        {
            this.boneNames = new Dictionary<string, int>();
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
            if (this.Owner.IsFinalStatic)
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
            if (this.Model.InternalModel == null)
            {
                return;
            }

            float zOrder = Vector3.DistanceSquared(this.RenderManager.CurrentDrawingCamera3D.Position, this.Transform.Position);

            for (int i = 0; i < this.Model.InternalModel.Meshes.Count; i++)
            {
                Mesh currentMesh = this.Model.InternalModel.Meshes[i];
                Material currentMaterial;

                if (currentMesh.Name != null && this.MaterialMap.Materials.ContainsKey(currentMesh.Name))
                {
                    currentMaterial = this.MaterialMap.Materials[currentMesh.Name];
                }
                else
                {
                    currentMaterial = this.MaterialMap.DefaultMaterial;
                }

                if (currentMaterial != null)
                {
                    Matrix world;

                    if (!this.Owner.IsFinalStatic)
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
                    this.RenderManager.DrawMesh(currentMesh, currentMaterial, ref world, this.Owner.IsFinalStatic);
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
