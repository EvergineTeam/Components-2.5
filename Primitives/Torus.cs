#region File Description
//-----------------------------------------------------------------------------
// Torus
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
    /// A 3D torus.
    /// </summary>
    internal sealed class Torus : Geometric
    {
        #region Initialize
        /// <summary>
        /// Initializes a new instance of the <see cref="Torus" /> class.
        /// </summary>
        /// <param name="diameter">The diameter.</param>
        /// <param name="thickness">The thickness.</param>
        /// <param name="tessellation">The tessellation.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">If tessellation is less than 3.</exception>
        public Torus(float diameter, float thickness, int tessellation)
        {
            if (tessellation < 3)
            {
                throw new ArgumentOutOfRangeException("tessellation");
            }

            // First we loop around the main ring of the torus.
            for (int i = 0; i < tessellation; i++)
            {
                float outerAngle = i * MathHelper.TwoPi / tessellation;

                // Create a transform matrix that will align geometry to
                // slice perpendicularly though the current ring position.
                Matrix transform = Matrix.CreateTranslation(diameter / 2, 0, 0) *
                                   Matrix.CreateRotationY(outerAngle);

                // Now we loop along the other axis, around the side of the tube.
                for (int j = 0; j < tessellation; j++)
                {
                    float innerAngle = j * MathHelper.TwoPi / tessellation;

                    float dx = (float)Math.Cos(innerAngle);
                    float dy = (float)Math.Sin(innerAngle);

                    // Create a vertex.
                    Vector3 normal = new Vector3(dx, dy, 0);
                    Vector3 position = normal * thickness / 2;

                    position = Vector3.Transform(position, transform);
                    normal = Vector3.TransformNormal(normal, transform);

                    this.AddVertex(position, normal, this.GetSphericalTexCoord(normal));

                    // And create indices for two triangles.
                    int nextI = (i + 1) % tessellation;
                    int nextJ = (j + 1) % tessellation;

                    this.AddIndex((i * tessellation) + j);
                    this.AddIndex((i * tessellation) + nextJ);
                    this.AddIndex((nextI * tessellation) + j);

                    this.AddIndex((i * tessellation) + nextJ);
                    this.AddIndex((nextI * tessellation) + nextJ);
                    this.AddIndex((nextI * tessellation) + j);
                }
            }
        }
        #endregion
    }
}
