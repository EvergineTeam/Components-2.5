#region File Description
//-----------------------------------------------------------------------------
// Capsule
//
// Copyright © 2010 - 2013 Wave Coorporation. All rights reserved.
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
    /// A 3D capsule.
    /// </summary>
    internal sealed class Capsule : Geometric
    {
        #region Initialize
        /// <summary>
        /// Initializes a new instance of the <see cref="Capsule" /> class.
        /// </summary>
        /// <param name="height">The height.</param>
        /// <param name="radius">The radius.</param>
        /// <param name="tessellation">The tessellation.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">If tessellation is not divisible by 2.</exception>
        public Capsule(float height, float radius, int tessellation)
        {
            if (tessellation % 2 != 0)
            {
                throw new ArgumentOutOfRangeException("tessellation should be even");
            }

            int verticalSegments = tessellation;
            int horizontalSegments = tessellation / 2;

            // Start with a single vertex at the bottom of the sphere.
            this.AddVertex((Vector3.Down * radius) + (Vector3.Down * 0.5f * height), Vector3.Down, new Vector2(0.5f, 0));

            // Create rings of vertices at progressively higher latitudes.
            for (int i = 0; i < verticalSegments - 1; i++)
            {
                float latitude = ((i + 1) * MathHelper.Pi /
                                            verticalSegments) - MathHelper.PiOver2;
                float dy = (float)Math.Sin(latitude);
                float dxz = (float)Math.Cos(latitude);

                // TODO: WTF is bla???????
                bool bla = false;

                if (i > (verticalSegments - 2) / 2)
                {
                    bla = true;
                }

                // Create a single ring of vertices at this latitude.
                for (int j = 0; j < horizontalSegments; j++)
                {
                    float longitude = j * MathHelper.TwoPi / horizontalSegments;

                    float dx = (float)Math.Cos(longitude) * dxz;
                    float dz = (float)Math.Sin(longitude) * dxz;

                    var normal = new Vector3(dx, dy, dz);
                    Vector3 position = normal * radius;

                    if (bla)
                    {
                        position += Vector3.Up * 0.5f * height;
                    }
                    else
                    {
                        position += Vector3.Down * 0.5f * height;
                    }

                    this.AddVertex(position, normal, this.GetSphericalTexCoord(normal));
                }
            }

            // Finish with a single vertex at the top of the sphere.
            this.AddVertex((Vector3.Up * radius) + (Vector3.Up * 0.5f * height), Vector3.Up, new Vector2(0.5f, 1));

            // Create a fan connecting the bottom vertex to the bottom latitude ring.
            for (int i = 0; i < horizontalSegments; i++)
            {
                this.AddIndex(0);
                this.AddIndex(1 + ((i + 1) % horizontalSegments));
                this.AddIndex(1 + i);
            }

            // Fill the sphere body with triangles joining each pair of latitude rings.
            for (int i = 0; i < verticalSegments - 2; i++)
            {
                for (int j = 0; j < horizontalSegments; j++)
                {
                    int nextI = i + 1;
                    int nextJ = (j + 1) % horizontalSegments;

                    this.AddIndex(1 + (i * horizontalSegments) + j);
                    this.AddIndex(1 + (i * horizontalSegments) + nextJ);
                    this.AddIndex(1 + (nextI * horizontalSegments) + j);

                    this.AddIndex(1 + (i * horizontalSegments) + nextJ);
                    this.AddIndex(1 + (nextI * horizontalSegments) + nextJ);
                    this.AddIndex(1 + (nextI * horizontalSegments) + j);
                }
            }

            // Create a fan connecting the top vertex to the top latitude ring.
            for (int i = 0; i < horizontalSegments; i++)
            {
                this.AddIndex(this.VerticesCount - 1);
                this.AddIndex(this.VerticesCount - 2 - ((i + 1) % horizontalSegments));
                this.AddIndex(this.VerticesCount - 2 - i);
            }
        }

        #endregion
    }
}
