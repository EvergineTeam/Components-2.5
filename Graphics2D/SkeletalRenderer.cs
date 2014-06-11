#region File Description
//-----------------------------------------------------------------------------
// SkeletalRenderer
//
// Copyright © 2014 Wave Corporation
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Using statements
using System;
using System.Collections.Generic;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Graphics.VertexFormats;
using WaveEngine.Common.Math;
using WaveEngine.Components.Animation;
using WaveEngine.Components.Animation.Spine;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Services;
using WaveEngine.Materials;
#endregion

namespace WaveEngine.Components.Graphics2D
{
    /// <summary>
    /// Render a 2D skeletal on screen
    /// </summary>
    public class SkeletalRenderer : Drawable2D
    {
        /// <summary>
        /// Number of instances of this component created.
        /// </summary>
        private static int instances;

        /// <summary>
        /// Transform of the <see cref="Graphics2D.Sprite"/>.
        /// </summary>
        [RequiredComponent]
        public Transform2D Transform2D;

        /// <summary>
        /// The skeletal data
        /// </summary>
        [RequiredComponent]
        public SkeletalData SkeletalData;

        /// <summary>
        /// The skeletal animation
        /// </summary>
        [RequiredComponent]
        public SkeletalAnimation SkeletalAnimation;

        /// <summary>
        /// The material
        /// </summary>
        private BasicMaterial2D material;

        /// <summary>
        /// The draw order
        /// </summary>
        private List<Slot> drawOrder;

        /// <summary>
        /// The vertices
        /// </summary>
        private VertexPositionColorTexture[] vertices;

        /// <summary>
        /// Temporal vertice used to set values to vertex array
        /// </summary>
        private VertexPositionColorTexture tempVertice;

        /// <summary>
        /// The mesh
        /// </summary>
        private Mesh spineMesh;

        #region Cached fields
        /// <summary>
        /// The viewport manager cached
        /// </summary>
        private ViewportManager viewportManager;

        /// <summary>
        /// The position cached
        /// </summary>
        private Vector2 position;

        /// <summary>
        /// The scale cached
        /// </summary>
        private Vector2 scale;

        /// <summary>
        /// The internal position cached
        /// </summary>
        private Vector3 internalPosition;

        /// <summary>
        /// The internal scale cached
        /// </summary>
        private Vector3 internalScale;

        /// <summary>
        /// The local world
        /// </summary>
        private Matrix localWorld;

        /// <summary>
        /// The quaternion matrix
        /// </summary>
        private Matrix quaternionMatrix;

        /// <summary>
        /// The translation matrix
        /// </summary>
        private Matrix translationMatrix;

        /// <summary>
        /// The scale matrix
        /// </summary>
        private Matrix scaleMatrix;

        /// <summary>
        /// The orientation
        /// </summary>
        private Quaternion orientation;

        #endregion

        /// <summary>
        /// Debug modes
        /// </summary>
        [Flags]
        public enum DebugMode
        {
            /// <summary>
            /// The none
            /// </summary>
            None = 1,

            /// <summary>
            /// The bones
            /// </summary>
            Bones = 2,

            /// <summary>
            /// The quads
            /// </summary>
            Quads = 4
        }

        #region Properties
        /// <summary>
        /// Gets or sets the color of the debug bones.
        /// </summary>
        /// <value>
        /// The color of the debug bones.
        /// </value>
        public Color DebugBonesColor { get; set; }

        /// <summary>
        /// Gets or sets the color of the debug quad.
        /// </summary>
        /// <value>
        /// The color of the debug quad.
        /// </value>
        public Color DebugQuadColor { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [debug mode].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [debug mode]; otherwise, <c>false</c>.
        /// </value>
        public DebugMode ActualDebugMode { get; set; }
        #endregion

        #region Initialize
        /// <summary>
        /// Initializes a new instance of the <see cref="SkeletalRenderer" /> class.
        /// </summary>
        /// <remarks>
        /// This constructor uses Alpha layer.
        /// </remarks>
        public SkeletalRenderer()
            : this(DefaultLayers.Alpha)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SkeletalRenderer" /> class.
        /// </summary>
        /// <param name="layerType">Type of the layer.</param>
        public SkeletalRenderer(Type layerType)
            : base("SkeletalRenderer" + instances++, layerType)
        {
            this.Transform2D = null;
            this.material = new BasicMaterial2D() { LayerType = layerType };
            this.position = new Vector2();
            this.scale = new Vector2(1);
            this.ActualDebugMode = DebugMode.Bones;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Allows to perform custom drawing.
        /// </summary>
        /// <param name="gameTime">The elapsed game time.</param>
        public override void Draw(TimeSpan gameTime)
        {
            this.position.X = this.Transform2D.X;
            this.position.Y = this.Transform2D.Y;
            this.scale.X = this.Transform2D.XScale;
            this.scale.Y = this.Transform2D.YScale;

            if (this.viewportManager.IsActivated)
            {
                this.viewportManager.Translate(ref this.position, ref this.scale);
            }

            Quaternion.CreateFromYawPitchRoll(0, 0, this.Transform2D.Rotation, out this.orientation);
            Matrix.CreateFromQuaternion(ref this.orientation, out this.quaternionMatrix);

            this.internalScale.X = this.scale.X;
            this.internalScale.Y = this.scale.Y;
            Matrix.CreateScale(ref this.internalScale, out this.scaleMatrix);

            this.internalPosition.X = this.position.X - (this.Transform2D.Origin.X * this.Transform2D.Rectangle.Width);
            this.internalPosition.Y = this.position.Y - (this.Transform2D.Origin.Y * this.Transform2D.Rectangle.Height);
            Matrix.CreateTranslation(ref this.internalPosition, out this.translationMatrix);

            Matrix.Multiply(ref this.scaleMatrix, ref this.quaternionMatrix, out this.localWorld);
            Matrix.Multiply(ref this.localWorld, ref this.translationMatrix, out this.localWorld);

            // Process Mesh
            int j = 0;
            for (int i = 0; i < this.drawOrder.Count; i++)
            {
                Slot slot = this.drawOrder[i];
                RegionAttachment regionAttachment = slot.Attachment as RegionAttachment;
                if (regionAttachment != null)
                {
                    float opacity = this.RenderManager.DebugLines ? this.DebugAlpha : this.Transform2D.Opacity;
                    float alpha = this.SkeletalAnimation.Skeleton.A * slot.A * opacity;

                    byte r = (byte)(this.SkeletalAnimation.Skeleton.R * slot.R * 255 * alpha);
                    byte g = (byte)(this.SkeletalAnimation.Skeleton.G * slot.G * 255 * alpha);
                    byte b = (byte)(this.SkeletalAnimation.Skeleton.B * slot.B * 255 * alpha);
                    byte a = (byte)(alpha * 255);

                    regionAttachment.UpdateVertices(slot.Bone);
                    float[] spineVertices = regionAttachment.Vertices;
                    float[] uvs = regionAttachment.UVs;

                    // Vertex TL
                    int index = j * 4;
                    this.tempVertice.Position.X = spineVertices[RegionAttachment.X1];
                    this.tempVertice.Position.Y = spineVertices[RegionAttachment.Y1];
                    this.tempVertice.Position.Z = 0;
                    this.tempVertice.Color.R = r;
                    this.tempVertice.Color.G = g;
                    this.tempVertice.Color.B = b;
                    this.tempVertice.Color.A = a;
                    this.tempVertice.TexCoord.X = uvs[RegionAttachment.X1];
                    this.tempVertice.TexCoord.Y = uvs[RegionAttachment.Y1];
                    this.vertices[index] = this.tempVertice;

                    // Vertex TR
                    index++;
                    this.tempVertice = this.vertices[index];
                    this.tempVertice.Position.X = spineVertices[RegionAttachment.X4];
                    this.tempVertice.Position.Y = spineVertices[RegionAttachment.Y4];
                    this.tempVertice.Position.Z = 0;
                    this.tempVertice.Color.R = r;
                    this.tempVertice.Color.G = g;
                    this.tempVertice.Color.B = b;
                    this.tempVertice.Color.A = a;
                    this.tempVertice.TexCoord.X = uvs[RegionAttachment.X4];
                    this.tempVertice.TexCoord.Y = uvs[RegionAttachment.Y4];
                    this.vertices[index] = this.tempVertice;

                    // Vertex BR
                    index++;
                    this.tempVertice = this.vertices[index];
                    this.tempVertice.Position.X = spineVertices[RegionAttachment.X3];
                    this.tempVertice.Position.Y = spineVertices[RegionAttachment.Y3];
                    this.tempVertice.Position.Z = 0;
                    this.tempVertice.Color.R = r;
                    this.tempVertice.Color.G = g;
                    this.tempVertice.Color.B = b;
                    this.tempVertice.Color.A = a;
                    this.tempVertice.TexCoord.X = uvs[RegionAttachment.X3];
                    this.tempVertice.TexCoord.Y = uvs[RegionAttachment.Y3];
                    this.vertices[index] = this.tempVertice;

                    // Vertex BL
                    index++;
                    this.tempVertice = this.vertices[index];
                    this.tempVertice.Position.X = spineVertices[RegionAttachment.X2];
                    this.tempVertice.Position.Y = spineVertices[RegionAttachment.Y2];
                    this.tempVertice.Position.Z = 0;
                    this.tempVertice.Color.R = r;
                    this.tempVertice.Color.G = g;
                    this.tempVertice.Color.B = b;
                    this.tempVertice.Color.A = a;
                    this.tempVertice.TexCoord.X = uvs[RegionAttachment.X2];
                    this.tempVertice.TexCoord.Y = uvs[RegionAttachment.Y2];
                    this.vertices[index] = this.tempVertice;

                    j++;
                }
            }

            if (this.SkeletalData.Atlas.Pages.Count > 0)
            {
                this.material.Texture = this.SkeletalData.Atlas.Pages[0].Texture as Texture2D;
            }

            this.spineMesh.NumPrimitives = j * 2;
            this.spineMesh.NumVertices = j * 4;
            this.spineMesh.VertexBuffer.SetData(this.vertices);
            this.GraphicsDevice.BindVertexBuffer(this.spineMesh.VertexBuffer);

            this.RenderManager.DrawMesh(this.spineMesh, this.material, ref this.localWorld, false);
        }

        #endregion

        #region Private Methods
        /// <summary>
        /// Performs further custom initialization for this instance.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();

            this.viewportManager = WaveServices.ViewportManager;
            this.drawOrder = this.SkeletalAnimation.Skeleton.DrawOrder;

            int numIndices = this.drawOrder.Count * 6;
            int numPrimitives = this.drawOrder.Count * 2;
            int numVertices = this.drawOrder.Count * 4;

            // Create Indexbuffer
            ushort[] indices = new ushort[numIndices];

            for (int i = 0; i < this.drawOrder.Count; i++)
            {
                indices[(i * 6) + 0] = (ushort)((i * 4) + 0);
                indices[(i * 6) + 1] = (ushort)((i * 4) + 2);
                indices[(i * 6) + 2] = (ushort)((i * 4) + 1);

                indices[(i * 6) + 3] = (ushort)((i * 4) + 0);
                indices[(i * 6) + 4] = (ushort)((i * 4) + 3);
                indices[(i * 6) + 5] = (ushort)((i * 4) + 2);
            }

            this.spineMesh = new Mesh(
                0,
                numVertices,
                0,
                numPrimitives,
                new DynamicVertexBuffer(VertexPositionColorTexture.VertexFormat),
                new IndexBuffer(indices),
                PrimitiveType.TriangleList);

            this.vertices = new VertexPositionColorTexture[numVertices];
        }

        /// <summary>
        /// Helper method that draws debug lines.
        /// </summary>
        /// <remarks>
        /// This method will only work on debug mode and if RenderManager.DebugLines /&gt;
        /// is set to <c>true</c>.
        /// </remarks>
        protected override void DrawDebugLines()
        {
            base.DrawDebugLines();

            var platform = WaveServices.Platform;

            Vector2 start = new Vector2();
            Vector2 end = new Vector2();
            Color color = Color.Red;

            // Draw bones
            if ((this.ActualDebugMode & DebugMode.Bones) == DebugMode.Bones)
            {
                foreach (var bone in this.SkeletalAnimation.Skeleton.Bones)
                {
                    if (bone.Parent != null)
                    {
                        start.X = bone.WorldX;
                        start.Y = bone.WorldY;
                        end.X = (bone.Data.Length * bone.M00) + bone.WorldX;
                        end.Y = (bone.Data.Length * bone.M10) + bone.WorldY;

                        Vector2.Transform(ref start, ref this.localWorld, out start);
                        Vector2.Transform(ref end, ref this.localWorld, out end);

                        RenderManager.LineBatch2D.DrawLine(ref start, ref end, ref color);
                    }
                }
            }

            // Draw quads
            if ((this.ActualDebugMode & DebugMode.Quads) == DebugMode.Quads)
            {
                color = Color.Yellow;
                for (int i = 0; i < this.drawOrder.Count; i++)
                {
                    Slot slot = this.drawOrder[i];
                    RegionAttachment regionAttachment = slot.Attachment as RegionAttachment;
                    if (regionAttachment != null)
                    {
                        float[] spineVertices = regionAttachment.Vertices;

                        // Edge1
                        start.X = spineVertices[RegionAttachment.X1];
                        start.Y = spineVertices[RegionAttachment.Y1];
                        end.X = spineVertices[RegionAttachment.X2];
                        end.Y = spineVertices[RegionAttachment.Y2];

                        Vector2.Transform(ref start, ref this.localWorld, out start);
                        Vector2.Transform(ref end, ref this.localWorld, out end);

                        RenderManager.LineBatch2D.DrawLine(ref start, ref end, ref color);

                        // Edge2
                        start.X = spineVertices[RegionAttachment.X2];
                        start.Y = spineVertices[RegionAttachment.Y2];
                        end.X = spineVertices[RegionAttachment.X3];
                        end.Y = spineVertices[RegionAttachment.Y3];

                        Vector2.Transform(ref start, ref this.localWorld, out start);
                        Vector2.Transform(ref end, ref this.localWorld, out end);

                        RenderManager.LineBatch2D.DrawLine(ref start, ref end, ref color);

                        // Edge3
                        start.X = spineVertices[RegionAttachment.X3];
                        start.Y = spineVertices[RegionAttachment.Y3];
                        end.X = spineVertices[RegionAttachment.X4];
                        end.Y = spineVertices[RegionAttachment.Y4];

                        Vector2.Transform(ref start, ref this.localWorld, out start);
                        Vector2.Transform(ref end, ref this.localWorld, out end);

                        RenderManager.LineBatch2D.DrawLine(ref start, ref end, ref color);

                        // Edge4
                        start.X = spineVertices[RegionAttachment.X4];
                        start.Y = spineVertices[RegionAttachment.Y4];
                        end.X = spineVertices[RegionAttachment.X1];
                        end.Y = spineVertices[RegionAttachment.Y1];

                        Vector2.Transform(ref start, ref this.localWorld, out start);
                        Vector2.Transform(ref end, ref this.localWorld, out end);

                        RenderManager.LineBatch2D.DrawLine(ref start, ref end, ref color);
                    }
                }
            }
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.GraphicsDevice.DestroyIndexBuffer(this.spineMesh.IndexBuffer);
                this.GraphicsDevice.DestroyVertexBuffer(this.spineMesh.VertexBuffer);
            }
        }
        #endregion
    }
}
