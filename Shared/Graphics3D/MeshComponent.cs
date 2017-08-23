// Copyright © 2017 Wave Engine S.L. All rights reserved. Use is subject to license terms.

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
        /// Refresh event
        /// </summary>
        public event EventHandler Refreshed;

        #region Properties

        /// <summary>
        /// Gets or sets the model data.
        /// </summary>
        [DontRenderProperty]
        public InternalStaticModel InternalModel { get; protected set; }

        /// <summary>
        /// Gets the number of meshes of this model.
        /// </summary>
        [DontRenderProperty]
        public override int MeshCount
        {
            get
            {
                int result = 0;
                var meshes = this.MeshGroupByName;
                if (meshes != null)
                {
                    result = meshes.Count;
                }

                return result;
            }
        }

        /// <summary>
        /// Gets or sets the specify the mesh name to render
        /// </summary>
        [RenderPropertyAsSelector("MeshNames")]
        [DataMember]
        public virtual string ModelMeshName { get; set; }

        /// <summary>
        /// Gets the mesh name list on FBX file
        /// </summary>
        [DontRenderProperty]
        public IEnumerable MeshNames
        {
            get
            {
                List<string> meshNames = new List<string>();
                if (this.InternalModel != null)
                {
                    foreach (Mesh mesh in this.InternalModel.Meshes)
                    {
                        if (!meshNames.Contains(mesh.Name))
                        {
                            meshNames.Add(mesh.Name);
                        }
                    }
                }

                return meshNames;
            }
        }

        /// <summary>
        /// Gets the Mesh selected
        /// </summary>
        [DontRenderProperty]
        public List<Mesh> MeshGroupByName
        {
            get
            {
                if (this.InternalModel != null && this.InternalModel.Meshes != null)
                {
                    return (from e in this.InternalModel.Meshes where e.Name == this.ModelMeshName select e).ToList();
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Gets Mesh bouding box
        /// </summary>
        /// <returns>boudning box</returns>
        public override BoundingBox BoundingBox
        {
            get
            {
                BoundingBox result = new BoundingBox();

                if (this.InternalModel != null)
                {
                    var meshes = this.InternalModel.Meshes;
                    for (int i = 0; i < meshes.Count; i++)
                    {
                        if (meshes[i].Name == this.ModelMeshName)
                        {
                            result = this.InternalModel.BoundingBoxes[i];
                            break;
                        }
                    }
                }

                return result;
            }
        }
        #endregion

        /// <summary>
        /// Gets the indices info.
        /// </summary>
        /// <returns>Indices array</returns>
        public override int[] GetIndices()
        {
            if (this.InternalModel == null)
            {
                return null;
            }

            return this.InternalModel.GetCollisionIndices(this.ModelMeshName);
        }

        /// <summary>
        /// Gets the vertices info.
        /// </summary>
        /// <returns>
        /// Vertex array.
        /// </returns>
        public override Vector3[] GetVertices()
        {
            if (this.InternalModel == null)
            {
                return null;
            }

            return this.InternalModel.GetCollisionVertices(this.ModelMeshName);
        }

        /// <summary>
        /// Throw refresh event
        /// </summary>
        protected void ThrowRefreshEvent()
        {
            if (this.Refreshed != null)
            {
                this.Refreshed(this, null);
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public virtual void Dispose()
        {
            if (this.InternalModel != null)
            {
                this.InternalModel.Unload();
            }

            this.InternalModel = null;
            this.BoundingBox = new BoundingBox();
        }

        /// <summary>
        /// Find a VertexElement on all meshes
        /// </summary>
        /// <param name="vertexElement">Vertex Element</param>
        /// <returns>Return true whether all meshes have vertexElement or false</returns>
        public bool IsVertexElementSupported(VertexElementUsage vertexElement)
        {
            if (this.InternalModel == null ||
                this.InternalModel.Meshes == null ||
                this.InternalModel.Meshes.Count == 0)
            {
                return false;
            }

            foreach (Mesh mesh in this.InternalModel.Meshes)
            {
                if (!mesh.VertexBuffer.HasVertexElementUsage(vertexElement))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
