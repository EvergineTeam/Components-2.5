#region File Description
//-----------------------------------------------------------------------------
// Plane
//
// Copyright © 2014 Wave Corporation
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

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
        /// <param name="size">Size of plane.</param>
        public Plane(Vector3 normal, float size)
        {
            // Get two vectors perpendicular to the face normal.
            Vector3 side1 = new Vector3(normal.Y, normal.Z, normal.X) * (size / 2);
            Vector3 side2 = Vector3.Cross(normal, side1);

            // Six indices (two triangles).
            this.AddIndex(0);
            this.AddIndex(1);
            this.AddIndex(2);

            this.AddIndex(0);
            this.AddIndex(2);
            this.AddIndex(3);

            this.AddIndex(4);
            this.AddIndex(6);
            this.AddIndex(5);

            this.AddIndex(4);
            this.AddIndex(7);
            this.AddIndex(6);

            Vector3 v1 = -side1 - side2;
            Vector3 v2 = -side1 + side2;
            Vector3 v3 = side1 + side2;
            Vector3 v4 = side1 - side2;

            // Four vertices.
            this.AddVertex(v1, normal, new Vector2(0, 1));
            this.AddVertex(v2, normal, new Vector2(0, 0));
            this.AddVertex(v3, normal, new Vector2(1, 0));
            this.AddVertex(v4, normal, new Vector2(1, 1));

            this.AddVertex(v1, normal, new Vector2(1, 1));
            this.AddVertex(v2, normal, new Vector2(1, 0));
            this.AddVertex(v3, normal, new Vector2(0, 0));
            this.AddVertex(v4, normal, new Vector2(0, 1));
        }

        #endregion
    }
}
