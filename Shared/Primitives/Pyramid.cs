// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Using Statements
using WaveEngine.Common.Math;
using WaveEngine.Framework.Graphics3D;
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
            Vector3 basePos = Vector3.Down;
            float sizeOverTwo = size / 2;

            // Get two vectors perpendicular to the face normal.
            Vector3 side1 = new Vector3(basePos.Y, basePos.Z, basePos.X);
            Vector3 side2 = Vector3.Cross(basePos, side1);

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
            this.AddIndex(6);

            this.AddIndex(7);
            this.AddIndex(8);
            this.AddIndex(9);

            this.AddIndex(10);
            this.AddIndex(11);
            this.AddIndex(12);

            this.AddIndex(13);
            this.AddIndex(14);
            this.AddIndex(15);

            // 0   3
            // 1   2
            // Four vertices for a face.
            Vector3 normal = Vector3.Down;
            this.AddVertex((basePos - side1 - side2) * sizeOverTwo, normal, new Vector2(1, 0));
            this.AddVertex((basePos - side1 + side2) * sizeOverTwo, normal, new Vector2(1, 1));
            this.AddVertex((basePos + side1 + side2) * sizeOverTwo, normal, new Vector2(0, 1));
            this.AddVertex((basePos + side1 - side2) * sizeOverTwo, normal, new Vector2(0, 0));

            // First triangle
            normal = new Vector3(0, 0.5f, 1);
            normal.Normalize();
            this.AddVertex((basePos - side1 - side2) * sizeOverTwo, normal, new Vector2(1, 1));
            this.AddVertex((basePos + side1 - side2) * sizeOverTwo, normal, new Vector2(0, 1));
            this.AddVertex(basePos * -(size / 2), normal, new Vector2(0.5f, 0));

            // Second triangle
            normal = new Vector3(-1, 0.5f, 0);
            normal.Normalize();
            this.AddVertex((basePos + side1 - side2) * sizeOverTwo, normal, new Vector2(1, 1));
            this.AddVertex((basePos + side1 + side2) * sizeOverTwo, normal, new Vector2(0, 1));
            this.AddVertex(basePos * -(size / 2), normal, new Vector2(0.5f, 0));

            // Thrid triangle
            normal = new Vector3(0, 0.5f, -1);
            normal.Normalize();
            this.AddVertex((basePos + side1 + side2) * sizeOverTwo, normal, new Vector2(1, 1));
            this.AddVertex((basePos - side1 + side2) * sizeOverTwo, normal, new Vector2(0, 1));
            this.AddVertex(basePos * -(size / 2), normal, new Vector2(0.5f, 0));

            // Fourth triangle
            normal = new Vector3(1, 0.5f, 0);
            normal.Normalize();
            this.AddVertex((basePos - side1 + side2) * sizeOverTwo, normal, new Vector2(1, 1));
            this.AddVertex((basePos - side1 - side2) * sizeOverTwo, normal, new Vector2(0, 1));
            this.AddVertex(basePos * -(size / 2), normal, new Vector2(0.5f, 0));

            // Up Vertex
        }
        #endregion
    }
}
