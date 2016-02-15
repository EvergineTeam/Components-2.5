#region File Description
//-----------------------------------------------------------------------------
// InternalStaticModel
//
// Copyright © 2016 Wave Engine S.L. All rights reserved.
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
using WaveEngine.Components.Primitives;
using WaveEngine.Framework.Services;
using WaveEngine.Common.Graphics.VertexFormats;
#endregion

namespace WaveEngine.Components.Graphics3D
{
    /// <summary>
    /// Class that holds the data of a 3D model.
    /// </summary>
    public class InternalStaticModel : ILoadable<GraphicsDevice>
    {
        /// <summary>
        /// Meshes that form the model.
        /// </summary>
        public List<Mesh> Meshes;

        /// <summary>
        /// Bones in the model.
        /// </summary>
        public List<Bone> Bones;

        /// <summary>
        /// Relation between bones and pairs.
        /// </summary>
        public Dictionary<int, int> MeshBonePairs;

        /// <summary>
        /// Bounding box of the model.
        /// </summary>
        internal BoundingBox BoundingBox;

        /// <summary>
        /// The graphicsDevice
        /// </summary>
        private GraphicsDevice graphics;

        /// <summary>
        /// The collision info has been created
        /// </summary>
        private bool hasCollisionInfo;

        /// <summary>
        /// The collision vertices
        /// </summary>
        private Vector3[] collisionVertices;

        /// <summary>
        /// The collision indices
        /// </summary>
        private int[] collisionIndices;

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

        /// <summary>
        /// Gets the collision vertices
        /// </summary>
        public Vector3[] CollisionVertices
        {
            get
            {
                if (!this.hasCollisionInfo)
                {
                    this.GenerateCollisionInfo();
                }

                return this.collisionVertices;
            }
        }

        /// <summary>
        /// Gets the collision indices
        /// </summary>
        public int[] CollisionIndices
        {
            get
            {
                if (!this.hasCollisionInfo)
                {
                    this.GenerateCollisionInfo();
                }

                return this.collisionIndices;
            }
        }
        #endregion

        #region Initialize
        /// <summary>
        /// Initializes a new instance of the <see cref="InternalStaticModel"/> class.
        /// </summary>
        public InternalStaticModel()
        {
            this.BoundingBox = new BoundingBox();
            this.Meshes = new List<Mesh>();
            this.Bones = new List<Bone>();
            this.MeshBonePairs = new Dictionary<int, int>();
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Loads this class with data from a primitive.
        /// </summary>
        /// <param name="graphicsDevice">The graphicsDevice device.</param>
        /// <param name="primitive">The primitive to load.</param>
        public void FromPrimitive(GraphicsDevice graphicsDevice, Geometric primitive)
        {
            this.graphics = graphicsDevice;

            // Get Initial BoundingBox
            var min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            var max = new Vector3(float.MinValue, float.MinValue, float.MinValue);

            List<Vector3> collisionVertices = new List<Vector3>();
            var primitiveVertices = primitive.Vertices;

            for (int i = 0; i < primitiveVertices.Length; i++)
            {
                Vector3 vertex = primitiveVertices[i].Position;
                collisionVertices.Add(vertex);

                Vector3.Min(ref vertex, ref min, out min);
                Vector3.Max(ref vertex, ref max, out max);
            }

            this.BoundingBox.Min = min;
            this.BoundingBox.Max = max;

            // Load Primitive
            int vertexCount = primitive.Vertices.Length;
            int indexCount = primitive.Indices.Length;
            int primitiveCount = indexCount / 3;

            var meshVBuffer = new VertexBuffer(VertexPositionNormalTangentColorDualTexture.VertexFormat);
            meshVBuffer.SetData(primitive.ByteVertices, vertexCount);
            var meshIBuffer = new IndexBuffer(primitive.Indices);

            var baseMesh = new Mesh(0, vertexCount, 0, primitiveCount, meshVBuffer, meshIBuffer, PrimitiveType.TriangleList);
            baseMesh.Name = "Primitive";
            this.Meshes.Add(baseMesh);

            var rootBone = new Bone(0, -1, "Root", Matrix.Identity);
            this.Bones.Add(rootBone);

            this.MeshBonePairs.Add(0, 0);

            for (int i = 0; i < this.Bones.Count; i++)
            {
                Bone bone = this.Bones[i];
                if (bone.ParentIndex == -1)
                {
                    this.Bones[i].AbsoluteTransform = bone.LocalTransform;
                }
                else
                {
                    this.Bones[i].AbsoluteTransform = bone.LocalTransform
                                                      * this.Bones[bone.ParentIndex].AbsoluteTransform;
                }
            }

            for (int i = 0; i < this.Meshes.Count; i++)
            {
                Mesh m = this.Meshes[i];
                this.graphics.BindIndexBuffer(m.IndexBuffer);
                this.graphics.BindVertexBuffer(m.VertexBuffer);
            }

            primitive.Dispose();
        }

        /// <summary>
        ///     Loads this class with data from a stream.
        /// </summary>
        /// <param name="graphicsDevice">The graphicsDevice device.</param>
        /// <param name="stream">The stream that contains the model data.</param>
        public void Load(GraphicsDevice graphicsDevice, Stream stream)
        {
            this.graphics = graphicsDevice;

            using (var reader = new BinaryReader(stream))
            {
                this.BoundingBox.Min = reader.ReadVector3();
                this.BoundingBox.Max = reader.ReadVector3();

                int numBones = reader.ReadInt32();
                for (int i = 0; i < numBones; i++)
                {
                    byte[] dataBones = reader.ReadBytes(8);
                    int boneIndex = BitConverter.ToInt32(dataBones, 0);
                    int parentIndex = BitConverter.ToInt32(dataBones, 4);

                    string boneName = reader.ReadString();

                    Matrix matrix;
                    reader.ReadMatrix(out matrix);

                    this.Bones.Add(new Bone(boneIndex, parentIndex, boneName, matrix));
                }

                int numMeshes = reader.ReadInt32();

                for (int i = 0; i < numMeshes; i++)
                {
                    string meshName = reader.ReadString();

                    byte[] dataHeader = reader.ReadBytes(8);

                    int parentBone = BitConverter.ToInt32(dataHeader, 0);
                    int meshParts = BitConverter.ToInt32(dataHeader, 4);

                    for (int j = 0; j < meshParts; j++)
                    {
                        byte[] dataPart = reader.ReadBytes(24);

                        int vertexOffset = BitConverter.ToInt32(dataPart, 0);
                        int numVertices = BitConverter.ToInt32(dataPart, 4);
                        int startIndex = BitConverter.ToInt32(dataPart, 8);
                        int primitiveCount = BitConverter.ToInt32(dataPart, 12);
                        int numVertexElements = BitConverter.ToInt32(dataPart, 20);

                        var properties = new VertexElementProperties[numVertexElements];

                        byte[] data = reader.ReadBytes(numVertexElements * 16);
                        for (int k = 0; k < numVertexElements; k++)
                        {
                            VertexElementProperties item = properties[k];

                            item.Offset = BitConverter.ToInt32(data, 0 + (k * 16));
                            item.Format = (VertexElementFormat)BitConverter.ToInt32(data, 4 + (k * 16));
                            item.Usage = (VertexElementUsage)BitConverter.ToInt32(data, 8 + (k * 16));
                            item.UsageIndex = BitConverter.ToInt32(data, 12 + (k * 16));

                            properties[k] = item;
                        }

                        // Skip collision data (read from file for retrocompatibility)
                        bool hasCollision = reader.ReadBoolean();
                        if (hasCollision)
                        {
                            int collisionVerticesCount = reader.ReadInt32();
                            reader.ReadBytes(collisionVerticesCount * 12);

                            int collisionIndicesCount = reader.ReadInt32();
                            reader.ReadBytes(collisionVerticesCount * 2);
                        }

                        int bufferSize = reader.ReadInt32();
                        byte[] bufferData = reader.ReadBytes(bufferSize);

                        int indexSize = reader.ReadInt32();
                        var indices = new ushort[indexSize];

                        byte[] dataIndices = reader.ReadBytes(indexSize * 2);

                        for (int k = 0; k < indexSize; k++)
                        {
                            indices[k] = BitConverter.ToUInt16(dataIndices, k * 2);
                        }

                        var vertexBuffer = new VertexBuffer(new VertexBufferFormat(properties));
                        vertexBuffer.SetData(bufferData, numVertices);
                        var indexBuffer = new IndexBuffer(indices);
                        var mesh = new Mesh(vertexOffset, numVertices, startIndex, primitiveCount, vertexBuffer, indexBuffer, PrimitiveType.TriangleList);
                        mesh.Name = meshName;

                        this.Meshes.Add(mesh);
                        this.MeshBonePairs.Add(this.Meshes.Count - 1, parentBone);
                    }
                }
            }

            for (int i = 0; i < this.Bones.Count; i++)
            {
                Bone bone = this.Bones[i];
                if (bone.ParentIndex == -1)
                {
                    this.Bones[i].AbsoluteTransform = bone.LocalTransform;
                }
                else
                {
                    this.Bones[i].AbsoluteTransform = bone.LocalTransform
                                                      * this.Bones[bone.ParentIndex].AbsoluteTransform;
                }
            }
        }

        /// <summary>
        ///     Unloads the model data from memory.
        /// </summary>
        public virtual void Unload()
        {
            for (int i = 0; i < this.Meshes.Count; i++)
            {
                this.graphics.DestroyIndexBuffer(this.Meshes[i].IndexBuffer);
                this.graphics.DestroyVertexBuffer(this.Meshes[i].VertexBuffer);
            }

            this.Meshes.Clear();

            this.Bones.Clear();
            this.MeshBonePairs.Clear();
        }

        #endregion

        /// <summary>
        /// Clone the this instance
        /// </summary>
        /// <returns>The cloned instance</returns>
        public InternalStaticModel Clone()
        {
            var newModel = new InternalStaticModel();
            newModel.AssetPath = this.AssetPath;
            newModel.graphics = this.graphics;
            newModel.BoundingBox = this.BoundingBox;
            newModel.MeshBonePairs = new Dictionary<int, int>(this.MeshBonePairs);
            newModel.Bones = new List<Bone>(this.Bones);

            for (int i = 0; i < this.Meshes.Count; i++)
            {
                Mesh currentMesh = this.Meshes[i];
                VertexBuffer currentBuffer = currentMesh.VertexBuffer as VertexBuffer;

                var newBuffer = new VertexBuffer(currentBuffer.VertexBufferFormat);

                Matrix identity = Matrix.Identity;
                newBuffer.AppendBuffer(currentBuffer, ref identity);
                var newIndexBuffer = new IndexBuffer(currentMesh.IndexBuffer.Data);
                var newMesh = new Mesh(
                        currentMesh.VertexOffset,
                        currentMesh.NumVertices,
                        currentMesh.IndexOffset,
                        currentMesh.NumPrimitives,
                        newBuffer,
                        newIndexBuffer,
                        currentMesh.PrimitiveType);
                newMesh.Name = currentMesh.Name;

                newModel.Meshes.Add(newMesh);
            }

            return newModel;
        }

        /// <summary>
        /// Generate the collision info
        /// </summary>
        private void GenerateCollisionInfo()
        {
            Vector3[] collisionVertices = new Vector3[0];
            int[] collisionIndices = new int[0];

            for (int i = 0; i < this.Meshes.Count; i++)
            {
                // vertices
                int previousVerticesSize = collisionVertices.Length;
                Mesh mesh = this.Meshes[i];

                if (mesh.PrimitiveType != PrimitiveType.TriangleList)
                {
                    continue;
                }

                Vector3[] vertexPositions = new Vector3[mesh.NumVertices];
                mesh.VertexBuffer.GetVertexProperties(ref vertexPositions, VertexElementUsage.Position, 0, mesh.NumVertices, mesh.VertexOffset);

                int index = this.MeshBonePairs[i];
                Matrix absoluteTransform = this.Bones[index].AbsoluteTransform;

                for (int j = 0; j < vertexPositions.Length; j++)
                {
                    Vector3.Transform(ref vertexPositions[j], ref absoluteTransform, out vertexPositions[j]);
                }

                Array.Resize(ref collisionVertices, previousVerticesSize + vertexPositions.Length);
                Array.Copy(vertexPositions, 0, collisionVertices, previousVerticesSize, vertexPositions.Length);

                // indices
                int previousIndicesSize = collisionIndices.Length;

                ushort[] indices = mesh.IndexBuffer.Data;
                int nIndices = mesh.NumPrimitives * 3;
                Array.Resize(ref collisionIndices, previousIndicesSize + nIndices);
                int startIndex = previousIndicesSize;

                for (int j = 0; j < nIndices; j++)
                {
                    collisionIndices[startIndex] = previousVerticesSize + indices[j + mesh.IndexOffset];
                    startIndex++;
                }
            }

            this.hasCollisionInfo = true;
            this.collisionIndices = collisionIndices;
            this.collisionVertices = collisionVertices;
        }
    }
}
