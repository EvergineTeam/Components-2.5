#region File Description
//-----------------------------------------------------------------------------
// Geometric
//
// Copyright © 2015 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using WaveEngine.Common.Math;
using WaveEngine.Common.Graphics.VertexFormats;
using WaveEngine.Common.Graphics;
#endregion

namespace WaveEngine.Components.Primitives
{
    /// <summary>
    /// Base class for geometric primitives.
    /// </summary>
    public abstract class Geometric
    {
        /// <summary>
        /// During the process of constructing a primitive model, vertex data is stored on the CPU in these managed lists.
        /// </summary>
        private readonly List<VertexPositionNormalTangentColorDualTexture> vertices = new List<VertexPositionNormalTangentColorDualTexture>();

        /// <summary>
        /// During the process of constructing a primitive model, index data is stored on the CPU in these managed lists.
        /// </summary>
        private readonly List<ushort> indices = new List<ushort>();

        #region Properties
        /// <summary>
        /// Gets the vertices count.
        /// </summary>
        protected int VerticesCount
        {
            get { return this.vertices.Count; }
        }

        /// <summary>
        /// Gets the indices count.
        /// </summary>
        protected int IndicesCount
        {
            get { return this.indices.Count; }
        }

        /// <summary>
        /// Gets the vertices.
        /// </summary>
        public VertexPositionNormalTangentColorDualTexture[] Vertices
        {
            get
            {
                return this.vertices.ToArray();
            }
        }

        /// <summary>
        /// Gets the vertices as a byte array.
        /// </summary>
        public byte[] ByteVertices
        {
            get
            {
                this.CalculateTangentSpace();

                int vertexStride = VertexPositionNormalTangentColorDualTexture.VertexFormat.Stride;
                int vertexCount = this.vertices.Count;

                byte[] byteArray = new byte[vertexCount * vertexStride];

                for (int i = 0; i < vertexCount; i++)
                {
                    int baseOffset = i * vertexStride;
                    VertexPositionNormalTangentColorDualTexture currentVertex = this.vertices[i];

                    BitConverter.GetBytes(currentVertex.Position.X).CopyTo(byteArray, baseOffset);
                    BitConverter.GetBytes(currentVertex.Position.Y).CopyTo(byteArray, baseOffset + 4);
                    BitConverter.GetBytes(currentVertex.Position.Z).CopyTo(byteArray, baseOffset + 8);

                    BitConverter.GetBytes(currentVertex.Normal.X).CopyTo(byteArray, baseOffset + 12);
                    BitConverter.GetBytes(currentVertex.Normal.Y).CopyTo(byteArray, baseOffset + 16);
                    BitConverter.GetBytes(currentVertex.Normal.Z).CopyTo(byteArray, baseOffset + 20);

                    BitConverter.GetBytes(currentVertex.Tangent.X).CopyTo(byteArray, baseOffset + 24);
                    BitConverter.GetBytes(currentVertex.Tangent.Y).CopyTo(byteArray, baseOffset + 28);
                    BitConverter.GetBytes(currentVertex.Tangent.Z).CopyTo(byteArray, baseOffset + 32);

                    BitConverter.GetBytes(currentVertex.Binormal.X).CopyTo(byteArray, baseOffset + 36);
                    BitConverter.GetBytes(currentVertex.Binormal.Y).CopyTo(byteArray, baseOffset + 40);
                    BitConverter.GetBytes(currentVertex.Binormal.Z).CopyTo(byteArray, baseOffset + 44);

                    BitConverter.GetBytes(currentVertex.Color.ToUnsignedInt()).CopyTo(byteArray, baseOffset + 48);

                    BitConverter.GetBytes(currentVertex.TexCoord.X).CopyTo(byteArray, baseOffset + 52);
                    BitConverter.GetBytes(currentVertex.TexCoord.Y).CopyTo(byteArray, baseOffset + 56);

                    BitConverter.GetBytes(currentVertex.TexCoord.X).CopyTo(byteArray, baseOffset + 60);
                    BitConverter.GetBytes(currentVertex.TexCoord.Y).CopyTo(byteArray, baseOffset + 64);
                }

                return byteArray;
            }
        }

        /// <summary>
        /// Calculate tangent space of the geometry
        /// </summary>
        private unsafe void CalculateTangentSpace()
        {
            int vertexCount = this.vertices.Count;
            int triangleCount = this.indices.Count / 3;

            Vector3* tan1 = stackalloc Vector3[vertexCount * 2];
            Vector3* tan2 = tan1 + vertexCount;

            VertexPositionNormalTangentColorDualTexture a1, a2, a3;
            Vector3 v1, v2, v3;
            Vector2 w1, w2, w3;

            for (int a = 0; a < triangleCount; a++)
            {
                ushort i1 = this.indices[(a * 3) + 0];
                ushort i2 = this.indices[(a * 3) + 1];
                ushort i3 = this.indices[(a * 3) + 2];

                a1 = this.vertices[i1];
                a2 = this.vertices[i2];
                a3 = this.vertices[i3];

                v1 = a1.Position;
                v2 = a2.Position;
                v3 = a3.Position;

                w1 = a1.TexCoord;
                w2 = a2.TexCoord;
                w3 = a3.TexCoord;

                float x1 = v2.X - v1.X;
                float x2 = v3.X - v1.X;
                float y1 = v2.Y - v1.Y;
                float y2 = v3.Y - v1.Y;
                float z1 = v2.Z - v1.Z;
                float z2 = v3.Z - v1.Z;

                float s1 = w2.X - w1.X;
                float s2 = w3.X - w1.X;
                float t1 = w2.Y - w1.Y;
                float t2 = w3.Y - w1.Y;

                float r = 1.0F / ((s1 * t2) - (s2 * t1));
                Vector3 sdir = new Vector3(((t2 * x1) - (t1 * x2)) * r, ((t2 * y1) - (t1 * y2)) * r, ((t2 * z1) - (t1 * z2)) * r);
                Vector3 tdir = new Vector3(((s1 * x2) - (s2 * x1)) * r, ((s1 * y2) - (s2 * y1)) * r, ((s1 * z2) - (s2 * z1)) * r);

                tan1[i1] += sdir;
                tan1[i2] += sdir;
                tan1[i3] += sdir;

                tan2[i1] += tdir;
                tan2[i2] += tdir;
                tan2[i3] += tdir;
            }

            for (int a = 0; a < vertexCount; a++)
            {
                var vertex = this.vertices[a];

                Vector3 n = vertex.Normal;
                Vector3 t = tan1[a];

                // Gram-Schmidt orthogonalize
                vertex.Tangent = t - (n * Vector3.Dot(n, t));
                vertex.Tangent.Normalize();
                float sign = (Vector3.Dot(Vector3.Cross(n, t), tan2[a]) < 0.0F) ? -1.0F : 1.0F;
                vertex.Binormal = Vector3.Cross(vertex.Normal, vertex.Tangent);
                vertex.Binormal *= sign;

                this.vertices[a] = vertex;
            }
        }

        /// <summary>
        /// Gets the indices.
        /// </summary>
        public ushort[] Indices
        {
            get
            {
                return this.indices.ToArray();
            }
        }
        #endregion

        #region Initialize
        /// <summary>
        /// Initializes a new instance of the <see cref="Geometric" /> class.
        /// </summary>
        protected Geometric()
        {
            this.vertices = new List<VertexPositionNormalTangentColorDualTexture>();
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="Geometric" /> class.
        /// </summary>
        ~Geometric()
        {
            this.Dispose(false);
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Adds a new vertex to the primitive model.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <param name="normal">The normal.</param>
        /// <remarks>
        /// This should only be called during the initialization process, 
        /// before InitializePrimitive.
        /// </remarks>
        protected void AddVertex(Vector3 position, Vector3 normal)
        {
            this.vertices.Add(new VertexPositionNormalTangentColorDualTexture(position, normal, Vector3.Zero, Vector3.Zero, Color.Black, Vector2.Zero, Vector2.Zero));
        }

        /// <summary>
        /// Adds a new vertex to the primitive model.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <param name="normal">The normal.</param>
        /// <param name="texcoord">The texture coordinate.</param>
        /// <remarks>
        /// This should only be called during the initialization process, 
        /// before InitializePrimitive.
        /// </remarks>
        protected void AddVertex(Vector3 position, Vector3 normal, Vector2 texcoord)
        {
            this.vertices.Add(new VertexPositionNormalTangentColorDualTexture(position, normal, Vector3.Zero, Vector3.Zero, Color.Black, texcoord, texcoord));
        }

        /// <summary>
        /// Adds a new vertex to the primitive model.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <param name="normal">The normal.</param>
        /// <param name="tangent">The tangent.</param>
        /// <param name="texcoord">The texture coordinate.</param>
        /// <remarks>
        /// This should only be called during the initialization process, 
        /// before InitializePrimitive.
        /// </remarks>
        protected void AddVertex(Vector3 position, Vector3 normal, Vector3 tangent, Vector2 texcoord)
        {
            Vector3 binormal = Vector3.Cross(tangent, normal);
            this.vertices.Add(new VertexPositionNormalTangentColorDualTexture(position, normal, tangent, binormal, Color.Black, texcoord, texcoord));
        }

        /// <summary>
        /// Adds a new index to the primitive model.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <remarks>
        /// This should only be called during the initialization process, 
        /// before InitializePrimitive.
        /// </remarks>
        protected void AddIndex(int index)
        {
            if (index > ushort.MaxValue)
            {
                throw new ArgumentOutOfRangeException("index");
            }

            this.indices.Add((ushort)index);
        }

        /// <summary>
        /// Gets the spherical texture coordinates.
        /// </summary>
        /// <param name="normal">The normal.</param>
        /// <returns>Spherical coordinates.</returns>
        protected Vector2 GetSphericalTexCoord(Vector3 normal)
        {
            double tx = (Math.Atan2(normal.X, normal.Z) / (Math.PI * 2)) + 0.25;
            double ty = (Math.Asin(normal.Y) / MathHelper.Pi) + 0.5;

            return new Vector2((float)tx, (float)ty);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.vertices.Clear();
                this.indices.Clear();
            }
        }
        #endregion
    }
}
