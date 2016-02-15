#region File Description
//-----------------------------------------------------------------------------
// Torus
//
// Copyright © 2016 Wave Engine S.L. All rights reserved.
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

            int tessellationPlus = tessellation + 1;

            // First we loop around the main ring of the torus.
            for (int i = 0; i <= tessellation; i++)
            {
                float outerPercent = i / (float)tessellation;
                float outerAngle = outerPercent * MathHelper.TwoPi;

                // Create a transform matrix that will align geometry to
                // slice perpendicularly though the current ring position.
                Matrix transform = Matrix.CreateTranslation(diameter / 2, 0, 0) *
                                   Matrix.CreateRotationY(outerAngle);

                // Now we loop along the other axis, around the side of the tube.
                for (int j = 0; j <= tessellation; j++)
                {
                    float innerPercent = j / (float)tessellation;
                    float innerAngle = MathHelper.TwoPi * innerPercent;

                    float dx = (float)Math.Cos(innerAngle);
                    float dy = (float)Math.Sin(innerAngle);

                    // Create a vertex.
                    Vector3 normal = new Vector3(dx, dy, 0);
                    Vector3 position = normal * thickness / 2;

                    position = Vector3.Transform(position, transform);
                    normal = Vector3.TransformNormal(normal, transform);

                    this.AddVertex(position, normal, new Vector2(outerPercent, innerPercent));

                    // And create indices for two triangles.
                    int nextI = (i + 1) % tessellationPlus;
                    int nextJ = (j + 1) % tessellationPlus;

                    if ((j < tessellation) && (i < tessellation))
                    {
                        this.AddIndex((i * tessellationPlus) + j);
                        this.AddIndex((i * tessellationPlus) + nextJ);
                        this.AddIndex((nextI * tessellationPlus) + j);

                        this.AddIndex((i * tessellationPlus) + nextJ);
                        this.AddIndex((nextI * tessellationPlus) + nextJ);
                        this.AddIndex((nextI * tessellationPlus) + j);
                    }
                }
            }
        }
        #endregion
    }
}
