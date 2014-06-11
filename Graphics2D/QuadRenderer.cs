#region File Description
//-----------------------------------------------------------------------------
// QuadRenderer
//
// Copyright © 2014 Wave Corporation
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Graphics.VertexFormats;
using WaveEngine.Common.Math;
using WaveEngine.Framework.Services;
using WaveEngine.Materials.VertexFormats;
using System.Runtime.InteropServices;
using System.IO;
#endregion

namespace WaveEngine.Components.Graphics2D
{
    /// <summary>
    /// Drawable for spare quads.
    /// </summary>
    public class QuadRenderer : Drawable2D
    {
        /// <summary>
        /// Number of instances of this component created.
        /// </summary>
        private static int instances;

        /// <summary>
        /// The entity transform.
        /// </summary>
        [RequiredComponent]
        public Transform2D Transform2D;

        /// <summary>
        /// The material
        /// </summary>
        [RequiredComponent]
        public Material2D Material;

        /// <summary>
        /// The disposed
        /// </summary>
        protected bool disposed;
    
        /// <summary>
        /// The texcoord1
        /// </summary>
        private Vector2[] texcoord1;

        /// <summary>
        /// The texcoord2
        /// </summary>
        private Vector2[] texcoord2;

        /// <summary>
        /// The quad mesh.
        /// </summary>
        private Mesh quadMesh;

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

        #region Initialize
        /// <summary>
        /// Initializes a new instance of the <see cref="QuadRenderer" /> class.
        /// </summary>
        public QuadRenderer()
            : this(null, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QuadRenderer" /> class.
        /// </summary>
        /// <param name="texcoord1">The texcoord1.</param>
        /// <param name="texcoord2">The texcoord2.</param>
        public QuadRenderer(Vector2[] texcoord1, Vector2[] texcoord2)
            : base("QuadRenderer" + instances, null)
        {
            instances++;

            if (texcoord1 != null)
            {
                this.texcoord1 = texcoord1;
            }

            if (texcoord2 != null)
            {
                this.texcoord2 = texcoord2;
            }

            this.position = new Vector2();
            this.scale = new Vector2(1);            
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Allows to perform custom drawing.
        /// </summary>
        /// <param name="gameTime">The elapsed game time.</param>
        /// <remarks>
        /// This method will only be called if all the following points are true:
        /// <list type="bullet">
        /// <item>
        /// <description>The parent of the owner <see cref="Entity" /> of the <see cref="Drawable" /> cascades its visibility to its children and it is visible.</description>
        /// </item>
        /// <item>
        /// <description>The <see cref="Drawable" /> is active.</description>
        /// </item>
        /// <item>
        /// <description>The owner <see cref="Entity" /> of the <see cref="Drawable" /> is active and visible.</description>
        /// </item>
        /// </list>
        /// </remarks>
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
            this.internalPosition.Z = this.Transform2D.DrawOrder;
            Matrix.CreateTranslation(ref this.internalPosition, out this.translationMatrix);

            Matrix.Multiply(ref this.scaleMatrix, ref this.quaternionMatrix, out this.localWorld);
            Matrix.Multiply(ref this.localWorld, ref this.translationMatrix, out this.localWorld);

            // Draw mesh
            this.RenderManager.DrawMesh(this.quadMesh, this.Material.Material, ref this.localWorld, false);
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

            if (this.texcoord1 == null)
            {
                this.texcoord1 = new Vector2[4]
                {
                    new Vector2(0, 0),
                    new Vector2(1, 0),
                    new Vector2(1, 1),
                    new Vector2(0, 1),
                };
            }

            if (this.texcoord2 == null)
            {
                this.texcoord2 = new Vector2[4]
                {
                    new Vector2(0, 0),
                    new Vector2(1, 0),
                    new Vector2(1, 1),
                    new Vector2(0, 1),
                };
            }

            float halfWidth = this.Transform2D.Rectangle.Width;
            float halfHeight = this.Transform2D.Rectangle.Height;
            float opacity = this.Transform2D.Opacity;
            Color color = new Color(opacity, opacity, opacity, opacity);

            VertexPositionColorDualTexture[] vertices = new VertexPositionColorDualTexture[4];
            vertices[0].Position = new Vector3(0f, 0f, this.Transform2D.DrawOrder);
            vertices[0].Color = color;
            vertices[0].TexCoord = this.texcoord1[0];
            vertices[0].TexCoord2 = this.texcoord2[0];

            vertices[1].Position = new Vector3(halfWidth, 0f, this.Transform2D.DrawOrder);
            vertices[1].Color = color;
            vertices[1].TexCoord = this.texcoord1[1];
            vertices[1].TexCoord2 = this.texcoord2[1];

            vertices[2].Position = new Vector3(halfWidth, halfHeight, this.Transform2D.DrawOrder);
            vertices[2].Color = color;
            vertices[2].TexCoord = this.texcoord1[2];
            vertices[2].TexCoord2 = this.texcoord2[2];

            vertices[3].Position = new Vector3(0f, halfHeight, this.Transform2D.DrawOrder);
            vertices[3].Color = color;
            vertices[3].TexCoord = this.texcoord1[3];
            vertices[3].TexCoord2 = this.texcoord2[3];

            VertexBuffer vertexBuffer = new VertexBuffer(VertexPositionColorDualTexture.VertexFormat);
            vertexBuffer.SetData(vertices, 4);

            ushort[] indices = new ushort[6];
            indices[0] = 0;
            indices[1] = 1;
            indices[2] = 2;
            indices[3] = 2;
            indices[4] = 3;
            indices[5] = 0;

            IndexBuffer indexBuffer = new IndexBuffer(indices);           
            
            // create the quad
            this.quadMesh = new Mesh(0, 4, 0, 2, vertexBuffer, indexBuffer, PrimitiveType.TriangleList);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    // ToDo
                    this.disposed = true;
                }
            }
        }

        #endregion
    }
}
