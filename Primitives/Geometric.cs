#region File Description
//-----------------------------------------------------------------------------
// Geometric
//
// Copyright © 2014 Wave Corporation
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using WaveEngine.Common.Math;
using WaveEngine.Common.Graphics.VertexFormats;
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
        private readonly List<VertexPositionNormalTexture> vertices = new List<VertexPositionNormalTexture>();

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
        public VertexPositionNormalTexture[] Vertices
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
                int vertexStride = VertexPositionNormalTexture.VertexFormat.Stride;
                int vertexCount = this.vertices.Count;

                byte[] byteArray = new byte[vertexCount * vertexStride];

                for (int i = 0; i < vertexCount; i++)
                {
                    int baseOffset = i * vertexStride;
                    VertexPositionNormalTexture currentVertex = this.vertices[i];

                    BitConverter.GetBytes(currentVertex.Position.X).CopyTo(byteArray, baseOffset);
                    BitConverter.GetBytes(currentVertex.Position.Y).CopyTo(byteArray, baseOffset + 4);
                    BitConverter.GetBytes(currentVertex.Position.Z).CopyTo(byteArray, baseOffset + 8);

                    BitConverter.GetBytes(currentVertex.Normal.X).CopyTo(byteArray, baseOffset + 12);
                    BitConverter.GetBytes(currentVertex.Normal.Y).CopyTo(byteArray, baseOffset + 16);
                    BitConverter.GetBytes(currentVertex.Normal.Z).CopyTo(byteArray, baseOffset + 20);

                    BitConverter.GetBytes(currentVertex.TexCoord.X).CopyTo(byteArray, baseOffset + 24);
                    BitConverter.GetBytes(currentVertex.TexCoord.Y).CopyTo(byteArray, baseOffset + 28);
                }

                return byteArray;
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
            this.vertices = new List<VertexPositionNormalTexture>();
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
            this.vertices.Add(new VertexPositionNormalTexture(position, normal, Vector2.Zero));
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
            this.vertices.Add(new VertexPositionNormalTexture(position, normal, texcoord));
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
