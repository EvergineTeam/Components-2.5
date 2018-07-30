// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Math;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
#endregion

namespace WaveEngine.Components.Graphics3D
{
    /// <summary>
    /// Renders a mesh on the screen.
    /// </summary>
    [DataContract(Namespace = "WaveEngine.Components.Graphics3D")]
    public class MeshRenderer : Drawable3D
    {
        /// <summary>
        /// Number of instances of this component created.
        /// </summary>
        private static int instances;

        /// <summary>
        /// <see cref="MeshComponent"/> to render.
        /// </summary>
        [RequiredComponent(false)]
        public MeshComponent ModelMesh;

        /// <summary>
        /// Materials used rendering the <see cref="MeshComponent"/>.
        /// </summary>
        public MaterialComponent[] Materials;

        /// <summary>
        /// Transform of the <see cref="MeshRenderer"/>.
        /// </summary>
        [RequiredComponent]
        public Transform3D Transform;

        /// <summary>
        /// Wether this instance has been disposed.
        /// </summary>
        private bool disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="MeshRenderer"/> class.
        /// </summary>
        public MeshRenderer()
            : base("ModelMeshRenderer" + instances++)
        {
        }

        /// <summary>
        /// Sets the default values
        /// </summary>
        protected override void DefaultValues()
        {
            base.DefaultValues();
        }

        /// <summary>
        /// Resolve dependencies
        /// </summary>
        protected override void ResolveDependencies()
        {
            base.ResolveDependencies();

            this.Materials = this.Owner.FindComponents<MaterialComponent>()
                                       .ToArray();
        }

        /// <summary>
        /// Initialize model renderer
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
        }

        /// <summary>
        /// Draws the model.
        /// </summary>
        /// <param name="gameTime">The elapsed game time.</param>
        public override void Draw(TimeSpan gameTime)
        {
            if (this.ModelMesh.InternalModel == null)
            {
                return;
            }

            float zOrder;
            if (this.BoundingBox.HasValue)
            {
                zOrder = Vector3.DistanceSquared(this.RenderManager.CurrentDrawingCamera3D.Position, this.BoundingBox.Value.Center);
            }
            else
            {
                zOrder = Vector3.DistanceSquared(this.RenderManager.CurrentDrawingCamera3D.Position, this.Transform.Position);
            }

            List<Mesh> meshGroup = this.ModelMesh.Meshes;
            if (meshGroup != null)
            {
                for (int i = 0; i < meshGroup.Count; i++)
                {
                    Mesh currentMesh = meshGroup[i];

                    Material currentMaterial = null;
                    if (this.Materials != null && this.Materials.Length > 0 &&
                        this.ModelMesh.InternalModel.Materials != null && this.ModelMesh.InternalModel.Materials.Count > currentMesh.MaterialIndex)
                    {
                        string fileMaterialName = this.ModelMesh.InternalModel.Materials[currentMesh.MaterialIndex];

                        var materialComponent = (from m in this.Materials where m.AsignedTo == fileMaterialName select m).FirstOrDefault();
                        if (materialComponent != null)
                        {
                            currentMaterial = materialComponent.Material;
                        }
                        else
                        {
                            currentMaterial = this.Materials[0].Material;
                        }
                    }

                    if (currentMaterial != null)
                    {
                        Matrix world = this.Transform.WorldTransform;

                        currentMesh.ZOrder = zOrder;

                        // Draw mesh
                        this.RenderManager.DrawMesh(currentMesh, currentMaterial, ref world, this.Owner.IsFinalStatic);
                    }
                }
            }
        }

        /// <summary>
        /// Refresh the bounding box of this drawable
        /// </summary>
        protected override void RefreshBoundingBox()
        {
            if (this.ModelMesh != null && this.ModelMesh.BoundingBox.HasValue)
            {
                var bbox = this.ModelMesh.BoundingBox.Value;
                bbox.Transform(this.Transform.WorldTransform);
                this.BoundingBox = bbox;
            }
            else
            {
                this.BoundingBox = null;
            }
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
                    this.disposed = true;
                }
            }
        }
    }
}
