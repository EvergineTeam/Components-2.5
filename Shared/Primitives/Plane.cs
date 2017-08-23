// Copyright © 2017 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Using Statements
using WaveEngine.Common.Math;
#endregion

namespace WaveEngine.Components.Primitives
{
    /// <summary>
    /// A 2D plane.
    /// </summary>
    internal sealed class Plane : Geometric
    {
        #region Initialize

        /// <summary>
        /// Initializes a new instance of the <see cref="Plane" /> class.
        /// </summary>
        /// <param name="normal">Normal of plane.</param>
        /// <param name="width">Width of the plane</param>
        /// <param name="height">Height of the plane</param>
        /// <param name="twoSides">Plane with two sides</param>
        /// <param name="uvHorizontalFlip">UV coord horizontal flip</param>
        /// <param name="uvVerticalFlip">UV coord vertical flip</param>
        public Plane(Vector3 normal, float width, float height, bool twoSides = true, bool uvHorizontalFlip = false, bool uvVerticalFlip = false)
        {
            Vector3 position = Vector3.Zero;
            Vector3 up;

            if (normal == Vector3.UnitY)
            {
                up = Vector3.UnitZ;
            }
            else if (normal == -Vector3.UnitY)
            {
                up = -Vector3.UnitZ;
            }
            else
            {
                up = Vector3.UnitY;
            }

            Matrix matrix;
            Matrix.CreateLookAt(ref position, ref normal, ref up, out matrix);

            // Get two vectors perpendicular to the face normal.
            Vector3 side1 = 0.5f * width * Vector3.UnitX; ////new Vector3(normal.Y, normal.Z, normal.X) * 0.5f;
            Vector3 side2 = 0.5f * height * Vector3.UnitY; //// Vector3.Cross(normal, side1);

            Vector3 v1 = -side1 - side2;
            Vector3 v2 = -side1 + side2;
            Vector3 v3 = side1 + side2;
            Vector3 v4 = side1 - side2;

            Vector3.Transform(ref v1, ref matrix, out v1);
            Vector3.Transform(ref v2, ref matrix, out v2);
            Vector3.Transform(ref v3, ref matrix, out v3);
            Vector3.Transform(ref v4, ref matrix, out v4);

            // Fout indices (two triangles).
            this.AddIndex(0);
            this.AddIndex(1);
            this.AddIndex(2);

            this.AddIndex(0);
            this.AddIndex(2);
            this.AddIndex(3);

            Vector2[] uv = new Vector2[4];
            uv[0] = new Vector2(0, 1);
            uv[1] = new Vector2(0, 0);
            uv[2] = new Vector2(1, 0);
            uv[3] = new Vector2(1, 1);

            Vector2 aux = Vector2.Zero;
            if (uvHorizontalFlip)
            {
                aux = uv[0];
                uv[0] = uv[3];
                uv[3] = aux;

                aux = uv[1];
                uv[1] = uv[2];
                uv[2] = aux;
            }

            if (uvVerticalFlip)
            {
                aux = uv[0];
                uv[0] = uv[1];
                uv[1] = aux;

                aux = uv[3];
                uv[3] = uv[2];
                uv[2] = aux;
            }

            // Four vertices.
            this.AddVertex(v1, normal, uv[0]);
            this.AddVertex(v2, normal, uv[1]);
            this.AddVertex(v3, normal, uv[2]);
            this.AddVertex(v4, normal, uv[3]);

            if (twoSides)
            {
                this.AddIndex(4);
                this.AddIndex(6);
                this.AddIndex(5);

                this.AddIndex(4);
                this.AddIndex(7);
                this.AddIndex(6);

                this.AddVertex(v1, normal, uv[3]);
                this.AddVertex(v2, normal, uv[2]);
                this.AddVertex(v3, normal, uv[1]);
                this.AddVertex(v4, normal, uv[0]);
            }
        }

        #endregion
    }
}
