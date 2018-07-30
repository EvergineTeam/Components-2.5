// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Using Statements
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using WaveEngine.Common.Attributes;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Graphics.VertexFormats;
using WaveEngine.Common.Math;
using WaveEngine.Components.Graphics2D;
using WaveEngine.Components.Graphics3D;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Services;
#endregion

namespace WaveEngine.Components.Primitives
{
    /// <summary>
    /// Line primitive mesh base component. To render this mesh use the <see cref="LineMeshRenderer3D"/> class.
    /// </summary>
    [DataContract]
    public abstract class LineMeshBase : MeshComponent
    {
        private const int VerticesPerPoint = 2;
        private const int IndicesPerPoint = 3;
        private const int MaxLinePointsPerMesh = ushort.MaxValue / VerticesPerPoint;

        private Vector2 textureTiling;

        private List<Mesh> meshes;
        private BoundingBox boundingBox;
        private LineMaterial material;

        /// <summary>
        /// Indicates whether the mesh will be renderer by a <see cref="LineMeshRenderer2D"/>
        /// </summary>
        internal bool is2DMode;

        /// <summary>
        /// The line points list
        /// </summary>
        internal List<LinePointInfo> linePoints;

        /// <summary>
        /// The line type
        /// </summary>
        [DataMember]
        internal LineTypes lineType;

        [DataMember]
        private string texturePath;

        /// <summary>
        /// Indicates whether the first point of the list is appended with the last one.
        /// </summary>
        [DataMember]
        protected bool isLoop;

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether the points are considered as world space coordinates, instead of being subject to
        /// the <see cref="Transform3D"/> of the owner <see cref="Entity"/> of this component.
        /// </summary>
        [DataMember]
        [RenderProperty(CustomPropertyName = "Use World Space", Tooltip = "Indicates whether the points are considered as world space coordinates")]
        public bool UseWorldSpace { get; set; }

        /// <summary>
        /// Gets or sets the texture coordinates offset.
        /// </summary>
        [DataMember]
        [RenderProperty(CustomPropertyName = "Texture Coordinate Offset", Tooltip = "Texture coordinate offset")]
        public Vector2 TexcoordOffset
        {
            get
            {
                return this.material.TextureOffset;
            }

            set
            {
                this.material.TextureOffset = value;
            }
        }

        /// <summary>
        /// Gets or sets the texture path.
        /// Such path is platform agnostic, and will always start with "Content/".
        /// Example: "Content/Characters/Tim.wpk"
        /// </summary>
        /// <value>
        /// The texture path.
        /// </value>
        [RenderPropertyAsAsset(AssetType.Texture)]
        public string TexturePath
        {
            get
            {
                return this.texturePath;
            }

            set
            {
                this.texturePath = value;

                if (this.isInitialized)
                {
                    this.RefreshTexture();
                }
            }
        }

        /// <summary>
        /// Gets or sets the line diffuse texture.
        /// Such is the in-memory representation for the given asset.
        /// See <see cref="Texture"/> for more information.
        /// </summary>
        /// <value>
        /// The line diffuse texture.
        /// </value>
        [DontRenderProperty]
        public Texture Diffuse
        {
            get
            {
                return this.material.Texture;
            }

            set
            {
                this.UnloadDiffuseTexture();
                this.material.Texture = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the line will be oriented to the camera.
        /// </summary>
        [DataMember]
        [RenderProperty(CustomPropertyName = "Is Camera Aligned", Tooltip = "Indicates whether the line will be oriented to the camera")]
        public bool IsCameraAligned
        {
            get
            {
                return this.material.IsAligned;
            }

            set
            {
                if (this.material.IsAligned != value)
                {
                    this.material.IsAligned = value;

                    if (this.isInitialized)
                    {
                        this.RefreshMeshes();
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating texture tiling (U,V).
        /// </summary>
        [DataMember]
        [RenderProperty(CustomPropertyName = "Texture Tiling", Tooltip = "Sets the tiling for the texture [U,V] coordinates")]
        public Vector2 TextureTiling
        {
            get
            {
                return this.textureTiling;
            }

            set
            {
                this.textureTiling = value;
                this.RefreshMeshes();
            }
        }

        /// <inheritdoc/>
        [DontRenderProperty]
        public override string ModelMeshName
        {
            get
            {
                return base.ModelMeshName;
            }
        }

        /// <summary>
        /// Gets the material used to render the line.
        /// </summary>
        [DontRenderProperty]
        internal Material Material
        {
            get
            {
                return this.material;
            }
        }
        #endregion

        /// <summary>
        /// Default values method
        /// </summary>
        protected override void DefaultValues()
        {
            base.DefaultValues();

            this.meshes = new List<Mesh>();
            this.boundingBox = new BoundingBox();
            this.UseWorldSpace = false;
            this.lineType = LineTypes.LineList;
            this.material = new LineMaterial(WaveServices.GraphicsDevice.Graphics);
            this.linePoints = new List<LinePointInfo>();
            this.textureTiling = Vector2.One;
            this.ModelMeshName = "Default";
        }

        /// <summary>
        /// Initialize method
        /// </summary>
        protected override void Initialize()
        {
            this.RefreshMeshes();
            this.LoadTexture();
        }

        /// <summary>
        /// Refresh meshes method
        /// </summary>
        protected virtual void RefreshMeshes()
        {
            this.DisposeMeshes();

            if (this.linePoints == null || this.linePoints.Count < 2)
            {
                return;
            }

            this.ResetBoundingBox();

            switch (this.lineType)
            {
                case LineTypes.LineStrip:
                    this.FillStripLines();
                    break;
                case LineTypes.LineList:
                    this.FillLineList();
                    break;
            }

            this.GenerateInternalModel();
        }

        /// <summary>
        /// Resets the current bounding box
        /// </summary>
        private void ResetBoundingBox()
        {
            this.boundingBox.Max = new Vector3(float.MinValue);
            this.boundingBox.Min = new Vector3(float.MaxValue);
        }

        /// <summary>
        /// Refresh the line material texture
        /// </summary>
        private void RefreshTexture()
        {
            this.UnloadDiffuseTexture();
            this.LoadTexture();
        }

        /// <summary>
        /// Unload texture
        /// </summary>
        private void UnloadDiffuseTexture()
        {
            if (this.material.Texture != null && !string.IsNullOrEmpty(this.material.Texture.AssetPath))
            {
                this.Assets.UnloadAsset(this.material.Texture.AssetPath);
                this.material.Texture = null;
            }
        }

        /// <summary>
        /// Load texture
        /// </summary>
        private void LoadTexture()
        {
            if (this.material.Texture == null && !string.IsNullOrEmpty(this.TexturePath))
            {
                this.material.Texture = this.Assets.LoadAsset<Texture2D>(this.TexturePath);
            }
        }

        /// <summary>
        /// Regenerate line mesh
        /// </summary>
        private void GenerateInternalModel()
        {
            if (this.InternalModel != null)
            {
                this.InternalModel.Unload();
                this.InternalModel = null;
            }

            this.InternalModel = new InternalModel();
            this.InternalModel.FromMeshes(WaveServices.GraphicsDevice, this.meshes);

            this.ThrowRefreshEvent();

            this.RefreshTransformRectangle();
        }

        /// <summary>
        /// Refresh the <see cref="Transform2D"/> rectangle
        /// </summary>
        private void RefreshTransformRectangle()
        {
            var transform2D = this.Owner?.FindComponent<Transform2D>();

            if (transform2D != null)
            {
                var boundingBox = this.BoundingBox;

                if (boundingBox.HasValue)
                {
                    var bbMin = boundingBox.Value.Min;
                    var bbMax = boundingBox.Value.Max;
                    var size = Vector3.Abs(boundingBox.Value.Max) + Vector3.Abs(bbMin);
                    transform2D.Rectangle = new RectangleF(bbMin.X, bbMin.Y, size.X, size.Y);
                }
                else
                {
                    transform2D.Rectangle = Rectangle.Empty;
                }
            }
        }

        /// <summary>
        /// Strip lines
        /// </summary>
        private void FillStripLines()
        {
            if (this.isLoop)
            {
                this.linePoints.Add(this.linePoints[0]);
            }

            var forward = Vector3.Forward;
            LinePointInfo currentPoint;

            Vector3 direction;
            Vector3 prevPosition;
            Vector3 currentPosition;
            Vector3 nextPosition;

            float totalLenght = 0;
            float[] lenghtByPoint = new float[this.linePoints.Count];
            prevPosition = this.linePoints[0].Position;
            for (int p = 1; p < lenghtByPoint.Length; p++)
            {
                currentPosition = this.linePoints[p].Position;
                direction = currentPosition - prevPosition;
                totalLenght += direction.Length();
                lenghtByPoint[p] = totalLenght;
                prevPosition = currentPosition;
            }

            for (int startIndex = 0; startIndex < (this.linePoints.Count - 1); startIndex += MaxLinePointsPerMesh)
            {
                int nPoints = Math.Min(MaxLinePointsPerMesh, this.linePoints.Count - startIndex);
                var lastIndex = startIndex + nPoints - 1;

                // Set vertex buffer
                var vertices = new VertexPositionColorTextureAxis[nPoints * VerticesPerPoint];

                for (int p = startIndex; p <= lastIndex; p++)
                {
                    currentPoint = this.linePoints[p];
                    currentPosition = currentPoint.Position;

                    if (p == 0)
                    {
                        prevPosition = this.isLoop ? this.linePoints[lastIndex - 1].Position : currentPosition;
                    }

                    if (p < lastIndex)
                    {
                        nextPosition = this.linePoints[p + 1].Position;
                    }
                    else
                    {
                        nextPosition = this.isLoop ? this.linePoints[1].Position : currentPosition;
                    }

                    direction = nextPosition - currentPosition;
                    direction.Normalize();
                    var prevDirection = prevPosition - currentPosition;
                    prevDirection.Normalize();

                    var thicknessDirection = direction - prevDirection;

                    if (!this.IsCameraAligned)
                    {
                        thicknessDirection.Normalize();
                        Vector3.Cross(ref forward, ref thicknessDirection, out thicknessDirection);
                        thicknessDirection.Normalize();
                    }

                    float thicknessFactor = 1;
                    if ((p > 0 && p < lastIndex) ||
                        this.isLoop)
                    {
                        var angle = Vector3.Angle(ref direction, ref prevDirection);
                        thicknessFactor = 1f / (float)Math.Sin(angle * 0.5);
                    }

                    this.AddVertex(ref currentPoint, ref thicknessDirection, lenghtByPoint[p] / totalLenght, p * 2, vertices, thicknessFactor);

                    prevPosition = currentPosition;
                }

                // Set index buffer
                var indices = new ushort[vertices.Length];

                for (int i = 1; i < vertices.Length; i++)
                {
                    indices[i] = (ushort)i;
                }

                this.AddMesh(vertices, indices, PrimitiveType.TriangleStrip);
            }

            if (this.isLoop)
            {
                this.linePoints.RemoveAt(this.linePoints.Count - 1);
            }
        }

        /// <summary>
        /// Triangle lines
        /// </summary>
        private void FillLineList()
        {
            var forward = Vector3.Forward;

            for (int startIndex = 0; startIndex < this.linePoints.Count - 1; startIndex += MaxLinePointsPerMesh)
            {
                int nPoints = Math.Min(MaxLinePointsPerMesh, this.linePoints.Count - startIndex);

                // Set vertex buffer
                var vertices = new VertexPositionColorTextureAxis[nPoints * VerticesPerPoint];
                var indices = new ushort[nPoints * IndicesPerPoint];

                var startPoint = this.linePoints[startIndex + 0];
                var endPoint = this.linePoints[startIndex + 1];
                float uCoord = startIndex / (float)this.linePoints.Count;
                Vector3 thicknessDirection;
                int pointIndex = 0;
                int indexArray = 0;

                for (int i = 0; i < nPoints - 1; i += 2)
                {
                    startPoint = this.linePoints[startIndex + i];
                    endPoint = this.linePoints[startIndex + i + 1];
                    thicknessDirection = endPoint.Position - startPoint.Position;

                    if (!this.IsCameraAligned)
                    {
                        thicknessDirection.Normalize();
                        Vector3.Cross(ref forward, ref thicknessDirection, out thicknessDirection);
                        thicknessDirection.Normalize();
                    }

                    int vertexIndex = pointIndex;

                    this.AddVertex(ref startPoint, ref thicknessDirection, 0, pointIndex, vertices);
                    pointIndex += 2;

                    this.AddVertex(ref endPoint, ref thicknessDirection, 1, pointIndex, vertices);
                    pointIndex += 2;

                    indices[indexArray + 0] = (ushort)(vertexIndex + 0);
                    indices[indexArray + 1] = (ushort)(vertexIndex + 1);
                    indices[indexArray + 2] = (ushort)(vertexIndex + 2);

                    indices[indexArray + 3] = (ushort)(vertexIndex + 2);
                    indices[indexArray + 4] = (ushort)(vertexIndex + 1);
                    indices[indexArray + 5] = (ushort)(vertexIndex + 3);
                    indexArray += IndicesPerPoint * 2;
                }

                this.AddMesh(vertices, indices, PrimitiveType.TriangleList);
            }
        }

        // Adds a mesh to the list of meshes
        private void AddMesh<T>(T[] vertices, ushort[] indices, PrimitiveType primitiveType)
            where T : struct, IBasicVertex
        {
            if (vertices.Length == 0 ||
                indices.Length == 0)
            {
                return;
            }

            var vertexBuffer = new VertexBuffer(vertices[0].VertexFormat);
            vertexBuffer.SetData(vertices);

            var indexBuffer = new IndexBuffer(indices);

            var primitiveCount = primitiveType == PrimitiveType.TriangleStrip ? indices.Length - 2 : indices.Length / 3;
            var mesh = new Mesh(0, vertices.Length, 0, primitiveCount, vertexBuffer, indexBuffer, primitiveType);
            mesh.BoundingBox = this.boundingBox;
            this.ResetBoundingBox();

            this.meshes.Add(mesh);
        }

        private void AddVertex(ref LinePointInfo info, ref Vector3 thicknessDirection, float uCoord, int vertexIndex, VertexPositionColorTextureAxis[] vertices, float thicknessFactor = 1)
        {
            vertices[vertexIndex].Color = info.Color;
            vertices[vertexIndex].TexCoord = new Vector2(uCoord, 1) * this.textureTiling;

            vertices[vertexIndex + 1].Color = info.Color;
            vertices[vertexIndex + 1].TexCoord = new Vector2(uCoord, 0) * this.textureTiling;

            float halfThickness = info.Thickness * 0.5f;
            halfThickness *= this.is2DMode ? -thicknessFactor : thicknessFactor;

            if (this.IsCameraAligned)
            {
                vertices[vertexIndex].Position = info.Position;
                vertices[vertexIndex + 1].Position = info.Position;
                vertices[vertexIndex].AxisSize = new Vector4(thicknessDirection, halfThickness);
                vertices[vertexIndex + 1].AxisSize = new Vector4(thicknessDirection, -halfThickness);
            }
            else
            {
                var axisThickness = thicknessDirection * halfThickness;
                vertices[vertexIndex].Position = info.Position + axisThickness;
                vertices[vertexIndex + 1].Position = info.Position - axisThickness;
            }

            var halfThicknessAbs = Math.Abs(halfThickness);
            var thicknessVector = new Vector3(halfThicknessAbs, halfThicknessAbs, 0);
            this.boundingBox.Max = Vector3.Max(info.Position + thicknessVector, this.boundingBox.Max);
            this.boundingBox.Min = Vector3.Min(info.Position - thicknessVector, this.boundingBox.Min);
        }

        private void DisposeMeshes()
        {
            var graphicsDevice = WaveServices.GraphicsDevice;
            foreach (var mesh in this.meshes)
            {
                graphicsDevice.DestroyIndexBuffer(mesh.IndexBuffer);
                graphicsDevice.DestroyVertexBuffer(mesh.VertexBuffer);
            }

            this.meshes.Clear();
        }

        /// <inheritdoc/>
        public override void Dispose()
        {
            base.Dispose();

            this.DisposeMeshes();
        }
    }
}
