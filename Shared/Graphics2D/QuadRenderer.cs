#region File Description
//-----------------------------------------------------------------------------
// QuadRenderer
//
// Copyright © 2015 Wave Engine S.L. All rights reserved.
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
using System.Runtime.Serialization;
#endregion

namespace WaveEngine.Components.Graphics2D
{
    /// <summary>
    /// Drawable for spare quads.
    /// </summary>
    [DataContract(Namespace = "WaveEngine.Components.Graphics2D")]
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
        [DataMember]
        private Vector2[] texcoord1;

        /// <summary>
        /// The texcoord2
        /// </summary>
        [DataMember]
        private Vector2[] texcoord2;

        /// <summary>
        /// The quad mesh.
        /// </summary>
        private Mesh quadMesh;

        /// <summary>
        /// Gets or sets the upper-right point texcoord
        /// </summary>
        public Vector2[] Texcoord1
        {
            get
            {
                return this.texcoord1;
            }

            set
            {
                this.texcoord1 = value;

                if (this.isInitialized)
                {
                    this.RefreshQuadMesh();
                }
            }
        }

        /// <summary>
        /// Gets or sets the upper-right point texcoord
        /// </summary>
        public Vector2[] Texcoord2
        {
            get
            {
                return this.texcoord2;
            }

            set
            {
                this.texcoord2 = value;

                if (this.isInitialized)
                {
                    this.RefreshQuadMesh();
                }
            }
        }

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
            this.quadMesh.ZOrder = this.Transform2D.DrawOrder;
            Matrix worldTransform = this.Transform2D.WorldTransform;

            // Draw mesh
            this.RenderManager.DrawMesh(this.quadMesh, this.Material.Material, ref worldTransform, false);
        }

        #endregion

        #region Private Methods
        /// <summary>
        /// Performs further custom initialization for this instance.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();

            this.RefreshQuadMesh();
        }

        /// <summary>
        /// Refresh the quad mesh
        /// </summary>
        private void RefreshQuadMesh()
        {
            if (this.quadMesh != null)
            {
                this.GraphicsDevice.DestroyIndexBuffer(this.quadMesh.IndexBuffer);
                this.GraphicsDevice.DestroyVertexBuffer(this.quadMesh.VertexBuffer);
                this.quadMesh = null;
            }

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

            Vector2 origin = this.Transform2D.Origin;
            float halfWidth = this.Transform2D.Rectangle.Width;
            float halfHeight = this.Transform2D.Rectangle.Height;
            float originCorrectionWidth = origin.X * halfWidth;
            float originCorrectionHeight = origin.Y * halfHeight;

            float opacity = this.Transform2D.GlobalOpacity;
            Color color = new Color(opacity, opacity, opacity, opacity);

            VertexPositionDualTexture[] vertices = new VertexPositionDualTexture[4];
            vertices[0].Position = new Vector3(-originCorrectionWidth, -originCorrectionHeight, 0);
            vertices[0].TexCoord = this.texcoord1[0];
            vertices[0].TexCoord2 = this.texcoord2[0];

            vertices[1].Position = new Vector3(halfWidth - originCorrectionWidth, -originCorrectionHeight, 0);
            vertices[1].TexCoord = this.texcoord1[1];
            vertices[1].TexCoord2 = this.texcoord2[1];

            vertices[2].Position = new Vector3(halfWidth - originCorrectionWidth, halfHeight - originCorrectionHeight, 0);
            vertices[2].TexCoord = this.texcoord1[2];
            vertices[2].TexCoord2 = this.texcoord2[2];

            vertices[3].Position = new Vector3(-originCorrectionWidth, halfHeight - originCorrectionHeight, 0);
            vertices[3].TexCoord = this.texcoord1[3];
            vertices[3].TexCoord2 = this.texcoord2[3];

            VertexBuffer vertexBuffer = new VertexBuffer(VertexPositionDualTexture.VertexFormat);
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
                    if (this.quadMesh != null)
                    {
                        GraphicsDevice.DestroyIndexBuffer(this.quadMesh.IndexBuffer);
                        GraphicsDevice.DestroyVertexBuffer(this.quadMesh.VertexBuffer);
                    }

                    this.disposed = true;
                }
            }
        }

        #endregion
    }
}
