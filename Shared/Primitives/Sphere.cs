#region File Description
//-----------------------------------------------------------------------------
// Sphere
//
// Copyright © 2017 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using WaveEngine.Common.Math;
#endregion

namespace WaveEngine.Components.Primitives
{
    /// <summary>
    /// A 3D sphere.
    /// </summary>
    internal sealed class Sphere : Geometric
    {
        #region Initialize
        /// <summary>
        /// Initializes a new instance of the <see cref="Sphere" /> class.
        /// </summary>
        /// <param name="diameter">The sphere diameter.</param>
        /// <param name="tessellation">The sphere tessellation.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">If tessellation is less than 3.</exception>
        public Sphere(float diameter, int tessellation)
        {
            if (tessellation < 3)
            {
                throw new ArgumentOutOfRangeException("tessellation");
            }

            int verticalSegments = tessellation;
            int horizontalSegments = tessellation * 2;
            float uIncrement = 1f / horizontalSegments;
            float vIncrement = 1f / verticalSegments;
            float radius = diameter / 2;

            float u = 0;
            float v = 0;

            // Start with a single vertex at the bottom of the sphere.
            v = 1;
            for (int i = 0; i < horizontalSegments; i++)
            {
                u += uIncrement;
                this.AddVertex(Vector3.Down * radius, Vector3.Down, new Vector2(u, v));
            }

            // Create rings of vertices at progressively higher latitudes.
            v = 1;
            for (int i = 0; i < verticalSegments - 1; i++)
            {
                float latitude = (((i + 1) * MathHelper.Pi) / verticalSegments) - MathHelper.PiOver2;
                u = 0;
                v -= vIncrement;
                float dy = (float)Math.Sin(latitude);
                float dxz = (float)Math.Cos(latitude);

                // Create a single ring of vertices at this latitude.
                for (int j = 0; j <= horizontalSegments; j++)
                {
                    float longitude = j * MathHelper.TwoPi / horizontalSegments;

                    float dx = (float)Math.Cos(longitude) * dxz;
                    float dz = (float)Math.Sin(longitude) * dxz;

                    Vector3 normal = new Vector3(dx, dy, dz);

                    Vector2 texCoord = new Vector2(u, v);
                    u += uIncrement;

                    this.AddVertex(normal * radius, normal, texCoord);
                }
            }

            // Finish with a single vertex at the top of the sphere.
            v = 0;
            u = 0;
            for (int i = 0; i < horizontalSegments; i++)
            {
                u += uIncrement;
                this.AddVertex(Vector3.Up * radius, Vector3.Up, new Vector2(u, v));
            }

            // Create a fan connecting the bottom vertex to the bottom latitude ring.
            for (int i = 0; i < horizontalSegments; i++)
            {
                this.AddIndex(i);
                this.AddIndex(1 + i + horizontalSegments);
                this.AddIndex(i + horizontalSegments);
            }

            // Fill the sphere body with triangles joining each pair of latitude rings.
            for (int i = 0; i < verticalSegments - 2; i++)
            {
                for (int j = 0; j < horizontalSegments; j++)
                {
                    int nextI = i + 1;
                    int nextJ = j + 1;
                    int num = horizontalSegments + 1;

                    int i1 = horizontalSegments + (i * num) + j;
                    int i2 = horizontalSegments + (i * num) + nextJ;
                    int i3 = horizontalSegments + (nextI * num) + j;
                    int i4 = i3 + 1;

                    this.AddIndex(i1);
                    this.AddIndex(i2);
                    this.AddIndex(i3);

                    this.AddIndex(i2);
                    this.AddIndex(i4);
                    this.AddIndex(i3);
                }
            }

            // Create a fan connecting the top vertex to the top latitude ring.
            for (int i = 0; i < horizontalSegments; i++)
            {
                this.AddIndex(this.VerticesCount - 1 - i);
                this.AddIndex(this.VerticesCount - horizontalSegments - 2 - i);
                this.AddIndex(this.VerticesCount - horizontalSegments - 1 - i);
            }
        }
        #endregion
    }
}
