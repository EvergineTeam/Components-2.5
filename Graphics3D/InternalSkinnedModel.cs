#region File Description
//-----------------------------------------------------------------------------
// InternalSkinnedModel
//
// Copyright © 2010 - 2013 Wave Coorporation. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using System.IO;
using WaveEngine.Common;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Math;
using WaveEngine.Framework.Services;
using WaveEngine.Common.Graphics.VertexFormats;
#endregion

namespace WaveEngine.Components.Graphics3D
{
    /// <summary>
    /// Class that holds the data of an animated 3D model.
    /// </summary>
    public class InternalSkinnedModel : ILoadable<GraphicsDevice>
    {
        /// <summary>
        /// Bounding box of the model.
        /// </summary>
        public BoundingBox BoundingBox;

        /// <summary>
        /// Bounding box bone index.
        /// </summary>
        public int BoundingBoxBoneIndex;

        /// <summary>
        /// Meshes that form the animated model.
        /// </summary>
        public List<SkinnedMesh> Meshes;

        /// <summary>
        /// The graphics
        /// </summary>
        private GraphicsDevice graphics;

        /// <summary>
        /// The internalindices
        /// </summary>
        private List<ushort[]> internalindices;

        #region Properties
        /// <summary>
        /// Gets or sets the asset path from where this model is located.
        /// </summary>
        /// <value>
        /// The asset path.
        /// </value>
        public string AssetPath { get; set; }

        /// <summary>
        /// Gets the reader version.
        /// </summary>
        /// <value>
        /// The reader version.
        /// </value>
        public Version ReaderVersion
        {
            get { return new Version(1, 0, 0, 0); }
        }
        #endregion

        #region Initialize
        /// <summary>
        /// Initializes a new instance of the <see cref="InternalSkinnedModel"/> class.
        /// </summary>
        public InternalSkinnedModel()
        {
            this.BoundingBox = new BoundingBox();
            this.Meshes = new List<SkinnedMesh>();
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Loads this class with data from a stream.
        /// </summary>
        /// <param name="graphicsDevice">The graphicsDevice device.</param>
        /// <param name="stream">The stream that contains the model data.</param>
        public void Load(GraphicsDevice graphicsDevice, Stream stream)
        {
            this.graphics = graphicsDevice;

            using (var reader = new BinaryReader(stream))
            {
                this.internalindices = new List<ushort[]>();
                this.BoundingBox.Min = reader.ReadVector3();
                this.BoundingBox.Max = reader.ReadVector3();
                this.BoundingBoxBoneIndex = reader.ReadInt32();

                int numMeshes = reader.ReadInt32();
                for (int i = 0; i < numMeshes; i++)
                {
                    string meshName = reader.ReadString();

                    reader.ReadInt32(); // mesh count

                    for (int j = 0; j < 1; j++)
                    {
                        ////bool usesTangent = false;

                        int vertexOffset = reader.ReadInt32();
                        int numVertices = reader.ReadInt32();
                        int startIndex = reader.ReadInt32();
                        int primitiveCount = reader.ReadInt32();

                        reader.ReadInt32(); // vertex stride
                        int numVertexElements = reader.ReadInt32();

                        var properties = new VertexElementProperties[numVertexElements];

                        for (int k = 0; k < numVertexElements; k++)
                        {
                            properties[k].Offset = reader.ReadInt32();
                            properties[k].Format = (VertexElementFormat)reader.ReadInt32();
                            properties[k].Usage = (VertexElementUsage)reader.ReadInt32();
                            properties[k].UsageIndex = reader.ReadInt32();

                            ////if (properties[k].Usage == VertexElementUsage.Tangent)
                            ////{
                            ////    usesTangent = true;
                            ////}
                        }

                        reader.ReadInt32(); // bufferSize

                        // TODO: this is ugly as hell...
                        ////if (usesTangent)
                        ////{
                        ////    var bufferData = new SkinnedNormalMappedVertex[numVertices];

                        ////    for (int k = 0; k < numVertices; k++)
                        ////    {
                        ////        foreach (VertexElementProperties property in properties)
                        ////        {
                        ////            switch (property.Usage)
                        ////            {
                        ////                case VertexElementUsage.Position:
                        ////                    bufferData[k].Position = reader.ReadVector3();
                        ////                    break;
                        ////                case VertexElementUsage.TextureCoordinate:
                        ////                    bufferData[k].TexCoord = reader.ReadVector2();
                        ////                    break;
                        ////                case VertexElementUsage.Normal:
                        ////                    bufferData[k].Normal = reader.ReadVector3();
                        ////                    break;
                        ////                case VertexElementUsage.BlendIndices:
                        ////                    bufferData[k].BlendIndices = reader.ReadByte4();
                        ////                    break;
                        ////                case VertexElementUsage.BlendWeight:
                        ////                    bufferData[k].BlendWeights = reader.ReadVector2();
                        ////                    break;
                        ////                case VertexElementUsage.Tangent:
                        ////                    bufferData[k].Tangent = reader.ReadVector3();
                        ////                    break;
                        ////                case VertexElementUsage.Binormal:
                        ////                    bufferData[k].Binormal = reader.ReadVector3();
                        ////                    break;
                        ////            }
                        ////        }
                        ////    }

                        ////    int indexSize = reader.ReadInt32();
                        ////    var indices = new ushort[indexSize];
                        ////    for (int k = 0; k < indexSize; k++)
                        ////    {
                        ////        indices[k] = reader.ReadUInt16();
                        ////    }

                        ////    this.internalindices.Add(indices);
                        ////    var mesh = new SkinnedNormalMappedMesh(
                        ////        vertexOffset,
                        ////        numVertices,
                        ////        startIndex,
                        ////        primitiveCount,
                        ////            new SkinnedVertexBuffer<SkinnedNormalMappedVertex>(
                        ////                numVertices, bufferData, SkinnedNormalMappedVertex.VertexFormat),
                        ////        new IndexBuffer(indices));
                        ////    mesh.Name = meshName;

                        ////    this.Meshes.Add(mesh);
                        ////}
                        ////else
                        ////{
                            var bufferData = new SkinnedVertex[numVertices];

                            for (int k = 0; k < numVertices; k++)
                            {
                                foreach (VertexElementProperties property in properties)
                                {
                                    switch (property.Usage)
                                    {
                                        case VertexElementUsage.Position:
                                            bufferData[k].Position = reader.ReadVector3();
                                            break;
                                        case VertexElementUsage.TextureCoordinate:
                                            bufferData[k].TexCoord = reader.ReadVector2();
                                            break;
                                        case VertexElementUsage.Normal:
                                            bufferData[k].Normal = reader.ReadVector3();
                                            break;
                                        case VertexElementUsage.BlendIndices:
                                            bufferData[k].BlendIndices = reader.ReadByte4();
                                            break;
                                        case VertexElementUsage.BlendWeight:
                                            bufferData[k].BlendWeights = reader.ReadVector2();
                                            break;

                                        case VertexElementUsage.Tangent:
                                            bufferData[k].Tangent = reader.ReadVector3();
                                            break;
                                        case VertexElementUsage.Binormal:
                                            bufferData[k].Binormal = reader.ReadVector3();
                                            break;
                                    }
                                }
                            }

                            int indexSize = reader.ReadInt32();
                            var indices = new ushort[indexSize];
                            for (int k = 0; k < indexSize; k++)
                            {
                                indices[k] = reader.ReadUInt16();
                            }

                            this.internalindices.Add(indices);

                            var newSkinnedBuffer = new SkinnedVertexBuffer(new VertexBufferFormat(properties));
                            newSkinnedBuffer.SetData(bufferData, numVertices);
                            var mesh = new SkinnedMesh(
                                vertexOffset,
                                numVertices,
                                startIndex,
                                primitiveCount,
                                newSkinnedBuffer,
                                new IndexBuffer(indices));
                            mesh.Name = meshName;

                            this.Meshes.Add(mesh);
                        ////}
                    }
                }
            }

            for (int i = 0; i < this.Meshes.Count; i++)
            {
                this.graphics.BindIndexBuffer(this.Meshes[i].IndexBuffer);
                this.graphics.BindVertexBuffer(this.Meshes[i].VertexBuffer);
            }
        }

        /// <summary>
        ///     Unloads the animated model data from memory.
        /// </summary>
        public virtual void Unload() // REVIEW: IDisposable
        {
            for (int i = 0; i < this.Meshes.Count; i++)
            {
                this.graphics.DestroyIndexBuffer(this.Meshes[i].IndexBuffer);
                this.graphics.DestroyVertexBuffer(this.Meshes[i].VertexBuffer);
            }

            this.Meshes.Clear();
        }

        /// <summary>
        ///     Clones this instance.
        /// </summary>
        /// <returns>The cloned animated model data.</returns>
        public InternalSkinnedModel Clone()
        {
            var newModel = new InternalSkinnedModel();
            newModel.graphics = this.graphics;
            newModel.BoundingBox = this.BoundingBox;

            for (int i = 0; i < this.Meshes.Count; i++)
            {
                SkinnedMesh currentMesh = this.Meshes[i];
                SkinnedVertexBuffer currentBuffer = currentMesh.VertexBuffer as SkinnedVertexBuffer;
                var newVertices = new SkinnedVertex[currentMesh.NumVertices];
                Array.Copy(currentBuffer.CpuVertices, newVertices, currentBuffer.CpuVertices.Length);

                var newBuffer = new SkinnedVertexBuffer(currentBuffer.VertexBufferFormat);
                newBuffer.SetData(newVertices, currentBuffer.VertexCount);
                var newIndexBuffer = new IndexBuffer(this.internalindices[i]);
                var newMesh = new SkinnedMesh(
                        currentMesh.VertexOffset,
                        currentMesh.NumVertices,
                        currentMesh.StartIndex,
                        currentMesh.PrimitiveCount,
                        newBuffer,
                        newIndexBuffer);
                newMesh.Name = currentMesh.Name;

                newModel.Meshes.Add(newMesh);

                this.graphics.BindIndexBuffer(newModel.Meshes[i].IndexBuffer);
                this.graphics.BindVertexBuffer(newModel.Meshes[i].VertexBuffer);
            }

            return newModel;
        }

        #endregion
    }
}
