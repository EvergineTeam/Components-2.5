// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Using Statements
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using WaveEngine.Common.Attributes;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Math;
using WaveEngine.Components.Graphics3D;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Graphics3D;
#endregion

namespace WaveEngine.Components.Graphics3D
{
    /// <summary>
    /// A 3D mesh. To render this mesh use the <see cref="MeshRenderer"/> class.
    /// </summary>
    [DataContract(Namespace = "WaveEngine.Components.Graphics3D")]
    public abstract class MeshComponent : BaseModel, IDisposable
    {
        /// <summary>
        /// The mesh content
        /// </summary>
        private MeshContent meshContent;

        /// <summary>
        /// The model mesh name
        /// </summary>
        private string modelMeshName;

        #region Properties

        /// <summary>
        /// Gets or sets the model data.
        /// </summary>
        [DontRenderProperty]
        public InternalModel InternalModel { get; protected set; }

        /// <summary>
        /// Gets the number of meshes of this model.
        /// </summary>
        [DontRenderProperty]
        public override int MeshCount
        {
            get
            {
                int result = 0;
                var meshes = this.Meshes;
                if (meshes != null)
                {
                    result = meshes.Count;
                }

                return result;
            }
        }

        /// <summary>
        /// Gets the mesh content
        /// </summary>
        [DontRenderProperty]
        public MeshContent MeshContent
        {
            get
            {
                return this.meshContent;
            }
        }

        /// <summary>
        /// Gets or sets the specify the mesh name to render
        /// </summary>
        [RenderPropertyAsSelector("MeshNames")]
        [DataMember]
        public virtual string ModelMeshName
        {
            get
            {
                return this.modelMeshName;
            }

            set
            {
                this.modelMeshName = value;
                this.RefreshMesh();
            }
        }

        /// <summary>
        /// Gets the mesh name list on FBX file
        /// </summary>
        [DontRenderProperty]
        public IEnumerable MeshNames
        {
            get
            {
                return this.InternalModel?.Meshes?.Select(m => m.Name);
            }
        }

        /// <summary>
        /// Gets the mesh group
        /// </summary>
        [DontRenderProperty]
        public override List<Mesh> Meshes
        {
            get
            {
                return this.meshContent?.MeshParts;
            }
        }

        /// <summary>
        /// Gets the mesh bounding box
        /// </summary>
        /// <returns>The bounding box</returns>
        public override BoundingBox? BoundingBox
        {
            get
            {
                return this.meshContent?.BoundingBox;
            }
        }
        #endregion

        /// <summary>
        /// Initialize this instance
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
            this.RefreshMesh();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public virtual void Dispose()
        {
            this.UnloadModel();
        }

        /// <summary>
        /// Find a VertexElement on all meshes
        /// </summary>
        /// <param name="vertexElement">Vertex Element</param>
        /// <returns>Return true whether all meshes have vertexElement or false</returns>
        public bool IsVertexElementSupported(VertexElementUsage vertexElement)
        {
            if (this.InternalModel?.Meshes == null ||
                this.InternalModel.Meshes.Length == 0)
            {
                return false;
            }

            foreach (MeshContent meshContent in this.InternalModel.Meshes)
            {
                foreach (var mesh in meshContent.MeshParts)
                {
                    if (!mesh.VertexBuffer.HasVertexElementUsage(vertexElement))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Unloads the internal model
        /// </summary>
        protected void UnloadModel()
        {
            if (!string.IsNullOrEmpty(this.InternalModel?.AssetPath))
            {
                if (this.Assets != null)
                {
                    this.Assets.UnloadAsset(this.InternalModel.AssetPath);
                }
            }
            else
            {
                this.InternalModel?.Unload();
            }

            this.InternalModel = null;

            this.BoundingBox = new BoundingBox();
        }

        /// <summary>
        /// Throw refresh model
        /// </summary>
        protected override void ThrowRefreshEvent()
        {
            this.RefreshMesh();
            base.ThrowRefreshEvent();
        }

        /// <summary>
        /// Refresh the mesh content
        /// </summary>
        protected virtual void RefreshMesh()
        {
            this.meshContent = this.InternalModel?.FindMeshContentByName(this.modelMeshName);
        }
    }
}
