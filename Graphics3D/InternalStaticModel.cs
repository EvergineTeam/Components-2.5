#region File Description
//-----------------------------------------------------------------------------
// InternalStaticModel
//
// Copyright © 2014 Wave Corporation
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
        public List<StaticMesh> Meshes;

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
        /// Initializes a new instance of the <see cref="InternalStaticModel"/> class.
        /// </summary>
        public InternalStaticModel()
        {
            this.BoundingBox = new BoundingBox();
            this.Meshes = new List<StaticMesh>();
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

            for (int i = 0; i < primitive.Vertices.Length; i++)
            {
                Vector3 vertex = primitive.Vertices[i].Position;
                collisionVertices.Add(vertex);

                Vector3.Min(ref vertex, ref min, out min);
                Vector3.Max(ref vertex, ref max, out max);
            }

            this.BoundingBox.Min = min;
            this.BoundingBox.Max = max;

            // Load Primitive
            int vertexCount = primitive.Indices.Length;
            int primitiveCount = vertexCount / 3;

            var meshVBuffer = new VertexBuffer(VertexPositionNormalTexture.VertexFormat);
            meshVBuffer.SetData(primitive.ByteVertices, vertexCount);
            var meshIBuffer = new IndexBuffer(primitive.Indices);

            var baseMesh = new StaticMesh(0, vertexCount, 0, primitiveCount, meshVBuffer, meshIBuffer, true, collisionVertices.ToArray(), primitive.Indices, PrimitiveType.TriangleList);
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
                StaticMesh m = this.Meshes[i];
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

                        bool hasCollision = reader.ReadBoolean();
                        Vector3[] collisionVertices = null;
                        ushort[] collisionIndices = null;

                        if (hasCollision)
                        {
                            int collisionVerticesCount = reader.ReadInt32();
                            collisionVertices = new Vector3[collisionVerticesCount];
                            for (int k = 0; k < collisionVerticesCount; k++)
                            {
                                collisionVertices[k] = reader.ReadVector3();
                            }

                            int collisionIndicesCount = reader.ReadInt32();
                            collisionIndices = new ushort[collisionIndicesCount];
                            for (int k = 0; k < collisionIndicesCount; k++)
                            {
                                collisionIndices[k] = reader.ReadUInt16();
                            }
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
                        var mesh = new StaticMesh(vertexOffset, numVertices, startIndex, primitiveCount, vertexBuffer, indexBuffer, hasCollision, collisionVertices, collisionIndices, PrimitiveType.TriangleList);
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
        public virtual void Unload() // REVIEW: IDisposable
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
    }
}
