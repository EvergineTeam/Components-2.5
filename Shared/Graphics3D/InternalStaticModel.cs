// Copyright © 2017 Wave Engine S.L. All rights reserved. Use is subject to license terms.

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
using System.Linq;
using WaveEngine.Framework.Models.Assets;
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
        /// Materials in the fbx file
        /// </summary>
        public List<string> Materials;

        /// <summary>
        /// Relation between bones and pairs.
        /// </summary>
        public Dictionary<int, int> MeshBonePairs;

        /// <summary>
        /// Bounding box of the model.
        /// </summary>
        public BoundingBox BoundingBox;

        /// <summary>
        /// Bounding box by mesh
        /// </summary>
        public List<BoundingBox> BoundingBoxes;

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
        /// The collision vertices per mesh
        /// </summary>
        private Dictionary<string, Vector3[]> collisionVerticesPerMesh;

        /// <summary>
        /// The collision indices
        /// </summary>
        private int[] collisionIndices;

        /// <summary>
        /// The collision indices per mesh
        /// </summary>
        private Dictionary<string, int[]> collisionIndicesPerMesh;

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
            this.BoundingBoxes = new List<BoundingBox>();
            this.Bones = new List<Bone>();
            this.Materials = new List<string>();
            this.MeshBonePairs = new Dictionary<int, int>();
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// Create an internal model from a mesh
        /// </summary>
        /// <param name="graphicsDevice">The graphicsDevice device.</param>
        /// <param name="meshes">The meshes</param>
        public void FromMeshes(GraphicsDevice graphicsDevice, IEnumerable<Mesh> meshes)
        {
            this.graphics = graphicsDevice;

            this.Meshes = meshes.ToList();

            var rootBone = new Bone(0, -1, "Root", Matrix.Identity);
            this.Bones.Add(rootBone);

            for (int i = 0; i < this.Meshes.Count; i++)
            {
                this.MeshBonePairs.Add(i, 0);
            }

            this.Materials.Add("Default");
        }

        /// <summary>
        /// Create an internal model from a mesh
        /// </summary>
        /// <param name="graphicsDevice">The graphicsDevice device.</param>
        /// <param name="meshes">The meshes</param>
        /// <param name="boundingBox">The bounding box</param>
        public void FromMeshes(GraphicsDevice graphicsDevice, IEnumerable<Mesh> meshes, BoundingBox boundingBox)
        {
            this.FromMeshes(graphicsDevice, meshes);
            this.BoundingBox = boundingBox;
        }

        /// <summary>
        /// Create an internal model from a mesh
        /// </summary>
        /// <param name="graphicsDevice">The graphicsDevice device.</param>
        /// <param name="mesh">The mesh</param>
        public void FromMesh(GraphicsDevice graphicsDevice, Mesh mesh)
        {
            this.FromMeshes(graphicsDevice, new List<Mesh>() { mesh });
        }

        /// <summary>
        /// Create an internal model from a mesh
        /// </summary>
        /// <param name="graphicsDevice">The graphicsDevice device.</param>
        /// <param name="mesh">The mesh</param>
        /// <param name="boundingBox">The bounding box</param>
        public void FromMesh(GraphicsDevice graphicsDevice, Mesh mesh, BoundingBox boundingBox)
        {
            this.FromMesh(graphicsDevice, mesh);
            this.BoundingBox = boundingBox;
        }

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

            this.BoundingBoxes.Add(this.BoundingBox);

            this.Materials.Add("Default");

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

                // Bones
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

                // Materials
                int numMaterials = reader.ReadInt32();

                for (int i = 0; i < numMaterials; i++)
                {
                    string name = reader.ReadString();
                    ////float opacity = reader.ReadSingle();
                    ////Color diffuseColor = new Color(reader.ReadUInt32());
                    ////Color ambientColor = new Color(reader.ReadUInt32());
                    ////Color emissiveColor = new Color(reader.ReadUInt32());
                    ////Color specularColor = new Color(reader.ReadUInt32());
                    ////string diffuseTextureFile = reader.ReadString();
                    ////string ambientTextureFile = reader.ReadString();
                    ////string emissiveTextureFile = reader.ReadString();
                    ////string specularTextureFile = reader.ReadString();

                    ////ModelMaterialModel material = new ModelMaterialModel()
                    ////{
                    ////    Name = name,
                    ////    Opacity = opacity,
                    ////    DiffuseColor = diffuseColor,
                    ////    AmbientColor = ambientColor,
                    ////    EmissiveColor = emissiveColor,
                    ////    SpecularColor = specularColor,
                    ////    DiffuseTextureFile = diffuseTextureFile,
                    ////    AmbientTextureFile = ambientTextureFile,
                    ////    EmissiveTextureFile = emissiveTextureFile,
                    ////    SpecularTextureFile = specularTextureFile,
                    ////};

                    this.Materials.Add(name);
                }

                // Meshes
                this.ProcessMeshNodes(reader);
            }

            for (int i = 0; i < this.Bones.Count; i++)
            {
                Bone bone = this.Bones[i];
                if (bone.ParentIndex == -1)
                {
                    this.Bones[i].AbsoluteTransform = bone.LocalTransform;
                    this.Bones[i].LocalTransform = bone.LocalTransform;
                }
                else
                {
                    this.Bones[i].AbsoluteTransform = bone.LocalTransform
                                                      * this.Bones[bone.ParentIndex].AbsoluteTransform;
                    this.Bones[i].LocalTransform = bone.LocalTransform;
                }
            }
        }

        /// <summary>
        /// Process Mesh nodes
        /// </summary>
        /// <param name="reader">Binary reader</param>
        private void ProcessMeshNodes(BinaryReader reader)
        {
            int numNodes = reader.ReadInt32();
            for (int n = 0; n < numNodes; n++)
            {
                int numMeshes = reader.ReadInt32();
                for (int m = 0; m < numMeshes; m++)
                {
                    this.ReadMesh(reader);
                }

                // Children
                this.ProcessMeshNodes(reader);
            }
        }

        /// <summary>
        /// Read mesh method
        /// </summary>
        /// <param name="reader">Binary reader</param>
        private void ReadMesh(BinaryReader reader)
        {
            string meshName = reader.ReadString();
            int materialID = reader.ReadInt32();

            BoundingBox meshBBox = new BoundingBox();
            meshBBox.Min = reader.ReadVector3();
            meshBBox.Max = reader.ReadVector3();
            this.BoundingBoxes.Add(meshBBox);

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
                byte[] dataIndices = reader.ReadBytes(indexSize * 2);

                indexSize = (indexSize / 3) * 3;
                var indices = new ushort[indexSize];

                for (int k = 0; k < indexSize; k++)
                {
                    indices[k] = BitConverter.ToUInt16(dataIndices, k * 2);
                }

                var vertexBuffer = new VertexBuffer(new VertexBufferFormat(properties));
                vertexBuffer.SetData(bufferData, numVertices);
                var indexBuffer = new IndexBuffer(indices);
                var mesh = new Mesh(vertexOffset, numVertices, startIndex, primitiveCount, vertexBuffer, indexBuffer, PrimitiveType.TriangleList, materialID);
                mesh.Name = meshName;

                this.Meshes.Add(mesh);
                this.MeshBonePairs.Add(this.Meshes.Count - 1, parentBone);
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
        /// Gets the collision vertices of a specified mesh
        /// </summary>
        /// <param name="meshName">The mesh name</param>
        /// <returns>The vertex position</returns>
        public Vector3[] GetCollisionVertices(string meshName)
        {
            if (!this.hasCollisionInfo)
            {
                this.GenerateCollisionInfo();
            }

            if (this.collisionVerticesPerMesh.ContainsKey(meshName))
            {
                return this.collisionVerticesPerMesh[meshName];
            }

            return null;
        }

        /// <summary>
        /// Gets the collision indices of a specified mesh
        /// </summary>
        /// <param name="meshName">The mesh name</param>
        /// <returns>The vertex indices</returns>
        public int[] GetCollisionIndices(string meshName)
        {
            if (!this.hasCollisionInfo)
            {
                this.GenerateCollisionInfo();
            }

            if (this.collisionVerticesPerMesh.ContainsKey(meshName))
            {
                return this.collisionIndicesPerMesh[meshName];
            }

            return null;
        }
        #endregion

        /// <summary>
        /// Generate the collision info
        /// </summary>
        private void GenerateCollisionInfo()
        {
            this.collisionVertices = new Vector3[0];
            this.collisionIndices = new int[0];

            this.collisionVerticesPerMesh = new Dictionary<string, Vector3[]>();
            this.collisionIndicesPerMesh = new Dictionary<string, int[]>();

            HashSet<string> visitedMeshes = new HashSet<string>();

            for (int i = 0; i < this.Meshes.Count; i++)
            {
                // vertices
                int previousVerticesSize = this.collisionVertices.Length;
                Mesh mesh = this.Meshes[i];

                if (mesh.PrimitiveType != PrimitiveType.TriangleList)
                {
                    continue;
                }

                var vertexPositions = new Vector3[mesh.NumVertices];
                mesh.VertexBuffer.GetVertexProperties(ref vertexPositions, VertexElementUsage.Position, 0, mesh.NumVertices, mesh.VertexOffset);

                int meshPreviousVerticesSize = 0;
                int meshPreviousIndicesSize = 0;
                Vector3[] meshVertices;
                int[] meshIndices;
                bool newMesh = !visitedMeshes.Contains(mesh.Name);

                if (newMesh)
                {
                    visitedMeshes.Add(mesh.Name);
                    meshVertices = vertexPositions;
                }
                else
                {
                    meshVertices = this.collisionVerticesPerMesh[mesh.Name];
                    meshPreviousVerticesSize = meshVertices.Length;
                    Array.Resize(ref meshVertices, meshPreviousVerticesSize + vertexPositions.Length);
                }

                Matrix absoluteTransform = this.Bones[this.MeshBonePairs[i]].AbsoluteTransform;

                Array.Resize(ref this.collisionVertices, previousVerticesSize + vertexPositions.Length);

                if (newMesh)
                {
                    for (int j = 0; j < vertexPositions.Length; j++)
                    {
                        Vector3.Transform(ref vertexPositions[j], ref absoluteTransform, out this.collisionVertices[previousVerticesSize + j]);
                    }
                }
                else
                {
                    for (int j = 0; j < vertexPositions.Length; j++)
                    {
                        meshVertices[meshPreviousVerticesSize + j] = vertexPositions[j];
                        Vector3.Transform(ref vertexPositions[j], ref absoluteTransform, out this.collisionVertices[previousVerticesSize + j]);
                    }
                }

                // indices
                int previousIndicesSize = this.collisionIndices.Length;

                var indices = mesh.IndexBuffer.Data;

                int nIndices = mesh.NumPrimitives * 3;
                int startIndex = previousIndicesSize;

                if (newMesh)
                {
                    meshIndices = new int[nIndices];
                }
                else
                {
                    meshIndices = this.collisionIndicesPerMesh[mesh.Name];
                    meshPreviousIndicesSize = meshIndices.Length;

                    Array.Resize(ref meshIndices, meshPreviousIndicesSize + nIndices);
                }

                Array.Resize(ref this.collisionIndices, previousIndicesSize + nIndices);

                for (int j = 0; j < nIndices; j++)
                {
                    int indice = indices[j + mesh.IndexOffset];
                    meshIndices[meshPreviousIndicesSize + j] = meshPreviousVerticesSize + indice;
                    this.collisionIndices[startIndex] = previousVerticesSize + indice;
                    startIndex++;
                }

                this.collisionVerticesPerMesh[mesh.Name] = meshVertices;
                this.collisionIndicesPerMesh[mesh.Name] = meshIndices;
            }

            this.hasCollisionInfo = true;
        }
    }
}
