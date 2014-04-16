#region File Description
//-----------------------------------------------------------------------------
// Pyramid
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
    /// A 3D pyramid.
    /// </summary>
    internal sealed class Pyramid : Geometric
    {
        #region Initialize
        /// <summary>
        /// Initializes a new instance of the <see cref="Pyramid" /> class.
        /// </summary>
        /// <param name="size">The size of the base.</param>
        public Pyramid(float size)
        {
            Vector3 normal = Vector3.Down;
            float sizeOverTwo = size / 2;

            // Get two vectors perpendicular to the face normal.
            Vector3 side1 = new Vector3(normal.Y, normal.Z, normal.X);
            Vector3 side2 = Vector3.Cross(normal, side1);

            // Six indices (two triangles) for down face.
            this.AddIndex(0);
            this.AddIndex(1);
            this.AddIndex(2);

            this.AddIndex(0);
            this.AddIndex(2);
            this.AddIndex(3);

            // Twelve indices for rest of face
            this.AddIndex(4);
            this.AddIndex(5);
            this.AddIndex(12);

            this.AddIndex(6);
            this.AddIndex(7);
            this.AddIndex(12);

            this.AddIndex(8);
            this.AddIndex(9);
            this.AddIndex(12);

            this.AddIndex(10);
            this.AddIndex(11);
            this.AddIndex(12);

            // 0   3 
            // 1   2
            // Four vertices for a face.
            this.AddVertex((normal - side1 - side2) * sizeOverTwo, normal, new Vector2(1, 0));
            this.AddVertex((normal - side1 + side2) * sizeOverTwo, normal, new Vector2(1, 1));
            this.AddVertex((normal + side1 + side2) * sizeOverTwo, normal, new Vector2(0, 1));
            this.AddVertex((normal + side1 - side2) * sizeOverTwo, normal, new Vector2(0, 0));

            // First triangle
            this.AddVertex((normal - side1 - side2) * sizeOverTwo, normal, new Vector2(1, 1));
            this.AddVertex((normal + side1 - side2) * sizeOverTwo, normal, new Vector2(0, 1));

            // Second triangle
            this.AddVertex((normal + side1 - side2) * sizeOverTwo, normal, new Vector2(1, 1));
            this.AddVertex((normal + side1 + side2) * sizeOverTwo, normal, new Vector2(0, 1));

            // Thrid triangle
            this.AddVertex((normal + side1 + side2) * sizeOverTwo, normal, new Vector2(1, 1));
            this.AddVertex((normal - side1 + side2) * sizeOverTwo, normal, new Vector2(0, 1));

            // Fourth triangle
            this.AddVertex((normal - side1 + side2) * sizeOverTwo, normal, new Vector2(1, 1));
            this.AddVertex((normal - side1 - side2) * sizeOverTwo, normal, new Vector2(0, 1));

            // Up Vertex
            this.AddVertex(normal * -(size / 2), normal, new Vector2(0.5f, 0));
        }
        #endregion
    }
}
