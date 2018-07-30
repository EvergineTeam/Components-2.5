// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Using Statements
using System;
using System.Collections.Generic;
using System.IO;
using WaveEngine.Common;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Math;
using WaveEngine.Framework.Services;
using System.Linq;
using WaveEngine.Framework.Animation;
using System.Text;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics3D;
using WaveEngine.Components.Animation;
using WaveEngine.Materials;
using WaveEngine.Common.Graphics.VertexFormats;
using WaveEngine.Framework.Helpers;
#endregion

namespace WaveEngine.Components.Graphics3D
{
    /// <summary>
    /// Class that holds the data of a 3D model.
    /// </summary>
    public class InternalModel : ILoadable<GraphicsDevice>
    {
        /// <summary>
        /// Bones in the model.
        /// </summary>
        public List<Bone> Bones;

        /// <summary>
        /// Materials in the fbx file
        /// </summary>
        public List<string> Materials;

        /// <summary>
        /// Bounding box of the model.
        /// </summary>
        public BoundingBox BoundingBox;

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
        /// Gets or sets the mesh content array
        /// </summary>
        public MeshContent[] Meshes { get; set; }

        /// <summary>
        /// Gets or sets the skin content array
        /// </summary>
        public SkinContent[] Skins { get; set; }

        /// <summary>
        /// Gets or sets the node array
        /// </summary>
        public NodeContent[] Nodes { get; set; }

        /// <summary>
        /// Gets or sets the root node array
        /// </summary>
        public int[] RootNodes { get; set; }

        /// <summary>
        /// Gets or sets the animation list
        /// </summary>
        public Dictionary<string, AnimationClip> Animations { get; set; }

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
        /// Initializes a new instance of the <see cref="InternalModel"/> class.
        /// </summary>
        public InternalModel()
        {
            this.BoundingBox = new BoundingBox();
            this.Bones = new List<Bone>();
            this.Materials = new List<string>();
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

            this.Meshes = meshes
                .GroupBy(m => m.Name)
                .Select(g =>
                {
                    var content = new MeshContent()
                    {
                        Name = g.Key ?? "Default",
                        MeshParts = g.ToList()
                    };
                    var boundingBoxes = g.Where(m => m.BoundingBox.HasValue)
                                         .Select(m => m.BoundingBox.Value);

                    if (boundingBoxes.Count() > 0)
                    {
                        content.BoundingBox = boundingBoxes.Aggregate((m1, m2) => WaveEngine.Common.Math.BoundingBox.CreateMerged(m1, m2));
                    }

                    return content;
                })
                .ToArray();

            NodeContent node = new NodeContent()
            {
                Name = "Root",
                Mesh = 0,
                Orientation = Quaternion.Identity,
                Scale = Vector3.One,
            };

            this.Nodes = new NodeContent[] { node };
            this.RootNodes = new int[] { 0 };
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
            List<Mesh> meshes = new List<Mesh>();

            // Get Initial BoundingBox
            var min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            var max = new Vector3(float.MinValue, float.MinValue, float.MinValue);

            var primitiveVertices = primitive.Vertices;

            for (int i = 0; i < primitiveVertices.Length; i++)
            {
                Vector3 vertex = primitiveVertices[i].Position;

                Vector3.Min(ref vertex, ref min, out min);
                Vector3.Max(ref vertex, ref max, out max);
            }

            BoundingBox bbox = new Common.Math.BoundingBox(min, max);

            // Load Primitive
            int vertexCount = primitive.Vertices.Length;
            int indexCount = primitive.Indices.Length;
            int primitiveCount = indexCount / 3;

            var meshVBuffer = new VertexBuffer(VertexPositionNormalTangentColorDualTexture.VertexFormat);
            meshVBuffer.SetData(primitive.ByteVertices, vertexCount);
            var meshIBuffer = new IndexBuffer(primitive.Indices);

            var baseMesh = new Mesh(0, vertexCount, 0, primitiveCount, meshVBuffer, meshIBuffer, PrimitiveType.TriangleList);
            baseMesh.Name = "Primitive";
            baseMesh.BoundingBox = bbox;
            meshes.Add(baseMesh);

            primitive.Dispose();

            this.BoundingBox = bbox;

            this.FromMeshes(graphicsDevice, meshes, bbox);
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
                // Model Bounding Box
                this.BoundingBox.Min = reader.ReadVector3();
                this.BoundingBox.Max = reader.ReadVector3();

                // Materials
                int materialCount = reader.ReadInt32();
                this.Materials = new List<string>();
                for (int i = 0; i < materialCount; i++)
                {
                    string materialName = reader.ReadString();
                    this.Materials.Add(materialName);
                }

                // Meshes
                int meshCount = reader.ReadInt32();
                this.Meshes = new MeshContent[meshCount];
                for (int i = 0; i < meshCount; i++)
                {
                    this.Meshes[i] = MeshContent.Read(reader);
                }

                // Skins
                int skinCount = reader.ReadInt32();
                this.Skins = new SkinContent[skinCount];
                for (int i = 0; i < skinCount; i++)
                {
                    this.Skins[i] = SkinContent.Read(reader);
                }

                this.ReadNodes(reader);

                // Animations
                int animationsCount = reader.ReadInt32();
                this.Animations = new Dictionary<string, AnimationClip>();
                for (int i = 0; i < animationsCount; i++)
                {
                    var animation = AnimationClip.Read(reader);
                    this.Animations[animation.Name] = animation;
                }
            }
        }

        /// <summary>
        /// Read nodes
        /// </summary>
        /// <param name="reader">The nodes</param>
        private void ReadNodes(BinaryReader reader)
        {
            // Nodes
            int nodeCount = reader.ReadInt32();
            this.Nodes = new NodeContent[nodeCount];
            for (int i = 0; i < nodeCount; i++)
            {
                this.Nodes[i] = NodeContent.Read(reader);
            }

            // Refresh node hierarchy
            for (int i = 0; i < nodeCount; i++)
            {
                var node = this.Nodes[i];
                if (node.ChildIndices != null)
                {
                    node.Children = new NodeContent[node.ChildIndices.Length];
                    for (int j = 0; j < node.ChildIndices.Length; j++)
                    {
                        var childNode = this.Nodes[node.ChildIndices[j]];
                        childNode.Parent = node;

                        node.Children[j] = childNode;
                    }
                }
            }

            // Root nodes
            int rootNodesCount = reader.ReadInt32();
            this.RootNodes = new int[rootNodesCount];
            for (int i = 0; i < rootNodesCount; i++)
            {
                this.RootNodes[i] = reader.ReadInt32();
            }
        }

        /// <summary>
        /// Unloads the model data from memory.
        /// </summary>
        public virtual void Unload()
        {
            for (int i = 0; i < this.Meshes?.Length; i++)
            {
                var meshContent = this.Meshes[i];

                for (int j = 0; j < meshContent?.MeshParts?.Count; j++)
                {
                    var mesh = meshContent.MeshParts[j];
                    this.graphics.DestroyIndexBuffer(mesh.IndexBuffer);
                    this.graphics.DestroyVertexBuffer(mesh.VertexBuffer);
                }
            }

            this.Meshes = null;
        }

        /// <summary>
        /// Find a mesh by name
        /// </summary>
        /// <param name="meshName">mesh name</param>
        /// <returns>The mesh name</returns>
        public MeshContent FindMeshContentByName(string meshName)
        {
            return this.Meshes?.FirstOrDefault(m => m.Name == meshName);
        }

        /// <summary>
        /// Instantiate model hierarchy
        /// </summary>
        /// <returns>The entity hierarchy</returns>
        public Entity InstantiateModelHierarchy()
        {
            return this.InstantiateModelHierarchy(null);
        }

        /// <summary>
        /// Instantiate model hierarchy
        /// </summary>
        /// <param name="entityName">The entity name</param>
        /// <returns>The entity hierarchy</returns>
        public Entity InstantiateModelHierarchy(string entityName)
        {
            return this.InstantiateModelHierarchy(entityName, true);
        }

        /// <summary>
        /// Instantiate model hierarchy
        /// </summary>
        /// <param name="entityName">The entity name</param>
        /// <param name="setMaterials">Set the materials to entities</param>
        /// <returns>The entity hierarchy</returns>
        public Entity InstantiateModelHierarchy(string entityName, bool setMaterials)
        {
            var nodeEntities = new Entity[this.Nodes.Length];

            for (int i = 0; i < this.Nodes.Length; i++)
            {
                nodeEntities[i] = this.CreateNodeEntity(this.Nodes[i]);
            }

            Material material = new StandardMaterial();

            Entity rootEntity = null;
            if (this.RootNodes.Length == 1)
            {
                var rootNodeIx = this.RootNodes[0];
                rootEntity = nodeEntities[rootNodeIx];
                this.GenerateModelsHierarchy(nodeEntities, rootNodeIx, material, null);
            }
            else
            {
                rootEntity = new Entity()
                    .AddComponent(new Transform3D());

                for (int i = 0; i < this.RootNodes.Length; i++)
                {
                    this.GenerateModelsHierarchy(nodeEntities, this.RootNodes[i], material, rootEntity);
                }
            }

            for (int i = 0; i < this.Nodes.Length; i++)
            {
                this.FillNode(i, nodeEntities, setMaterials);
            }

            if (this.Animations.Count > 0)
            {
                var animation = new Animation3D()
                {
                    ModelPath = this.AssetPath
                };

                rootEntity.AddComponent(animation);
            }

            if (string.IsNullOrEmpty(entityName))
            {
                rootEntity.Name = Entity.NextDefaultName();
            }
            else
            {
                rootEntity.Name = entityName;
            }

            return rootEntity;
        }

        private void FillNode(int nodeId, Entity[] nodeEntities, bool setMaterials)
        {
            var node = this.Nodes[nodeId];
            var entityNode = nodeEntities[nodeId];

            if (node.Mesh >= 0)
            {
                var meshContent = this.Meshes[node.Mesh];

                // Gets all materials
                bool isSkinnedMesh = false;
                HashSet<int> materialIndices = new HashSet<int>();
                foreach (var mesh in meshContent.MeshParts)
                {
                    materialIndices.Add(mesh.MaterialIndex);
                    if (mesh is SkinnedMesh)
                    {
                        isSkinnedMesh = true;
                    }
                }

                // Add Material components
                foreach (var materialIndex in materialIndices)
                {
                    string materialPath = null;
                    var materialName = this.Materials[materialIndex];

                    if (setMaterials)
                    {
                        string assetDir = Path.GetDirectoryName(this.AssetPath);
                        materialPath = Path.Combine(assetDir, "Materials", materialName + ".wmat").Replace('\\', '/');
                    }

                    entityNode.AddComponent(new MaterialComponent()
                    {
                        MaterialPath = materialPath,
                        AsignedTo = materialName
                    });
                }

                // Add the file mesh
                entityNode.AddComponent(new FileMesh()
                {
                    ModelPath = this.AssetPath,
                    ModelMeshName = meshContent.Name
                });

                if (!isSkinnedMesh)
                {
                    // Normal mesh
                    entityNode.AddComponent(new MeshRenderer());
                }
                else
                {
                    // Skinned mesh
                    var skinnedMeshRenderer = new SkinnedMeshRenderer();

                    // Sets morph weights
                    skinnedMeshRenderer.MorphTargetWeights = node.MorphTargetWeights ?? meshContent.MorphTargetWeights ?? null;

                    // Skin
                    if (node.Skin >= 0)
                    {
                        var skin = this.Skins[node.Skin];
                        var rootJointPath = this.GetRelativeNodePath(skin.RootJoint, nodeId);
                        var joints = new string[skin.Joints.Length];
                        for (int i = 0; i < joints.Length; i++)
                        {
                            var jointNode = this.Nodes[skin.Joints[i].NodeId];
                            var jointEntity = nodeEntities[skin.Joints[i].NodeId];

                            string jointNodePath = EntityPathHelper.RelativePathFromEntityPath(jointNode.NodePath, node.NodePath);

                            joints[i] = this.GetRelativeNodePath(skin.Joints[i].NodeId, nodeId);
                        }

                        skinnedMeshRenderer.RootJointPath = rootJointPath;
                        skinnedMeshRenderer.JointPaths = joints;
                    }

                    entityNode.AddComponent(skinnedMeshRenderer);
                }
            }
        }

        #endregion

        #region Private Methods

        private string GetRelativeNodePath(int nodeTargetId, int nodeSourceId)
        {
            var targetNode = this.Nodes[nodeTargetId];
            var sourceNode = this.Nodes[nodeSourceId];

            return EntityPathHelper.RelativePathFromEntityPath(targetNode.NodePath, sourceNode.NodePath);
        }

        /// <summary>
        /// Create an entity from a node
        /// </summary>
        /// <param name="node">The node</param>
        /// <returns>The entity</returns>
        private Entity CreateNodeEntity(NodeContent node)
        {
            Transform3D transform = new Transform3D();
            transform.SetLocalTransform(ref node.Translation, ref node.Orientation, ref node.Scale);

            Entity entityNode = new Entity(node.Name)
                .AddComponent(transform);

            return entityNode;
        }

        /// <summary>
        /// Generate Models Hierarchy
        /// </summary>
        /// <param name="nodeEntities">The node entities</param>
        /// <param name="nodeId">node index</param>
        /// <param name="material">default material</param>
        /// <param name="parent">Parent entity</param>
        private void GenerateModelsHierarchy(Entity[] nodeEntities, int nodeId, Material material, Entity parent)
        {
            NodeContent node = this.Nodes[nodeId];
            Entity entityNode = nodeEntities[nodeId];

            if (parent != null)
            {
                parent.AddChild(entityNode);
            }

            foreach (var childId in node.ChildIndices)
            {
                this.GenerateModelsHierarchy(nodeEntities, childId, material, entityNode);
            }
        }
        #endregion
    }
}
