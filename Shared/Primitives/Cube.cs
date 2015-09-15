#region File Description
//-----------------------------------------------------------------------------
// Cube
//
// Copyright © 2015 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using WaveEngine.Common.Math;
#endregion

namespace WaveEngine.Components.Primitives
{
    /// <summary>
    /// A 3D cube.
    /// </summary>
    internal sealed class Cube : Geometric
    {
        #region Initialize
        /// <summary>
        /// Initializes a new instance of the <see cref="Cube" /> class.
        /// </summary>
        /// <param name="size">The size of cube.</param>
        public Cube(float size)
        {
            // A cube has six faces, each one pointing in a different direction.
            Vector3[] normals =
            {
                new Vector3(0, 0, 1), 
                new Vector3(0, 0, -1), 
                new Vector3(1, 0, 0),
                new Vector3(-1, 0, 0), 
                new Vector3(0, 1, 0), 
                new Vector3(0, -1, 0)
            };

            Vector2[] texCoord = 
            {
                new Vector2(1, 1), new Vector2(0, 1), new Vector2(0, 0), new Vector2(1, 0),
                new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1),
                new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1), new Vector2(0, 0),
                new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1), new Vector2(0, 0),
                new Vector2(0, 1), new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1),
                new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1), new Vector2(0, 0),
            };

            Vector3[] tangents = 
            {
                new Vector3(1, 0, 0), 
                new Vector3(-1, 0, 0), 
                new Vector3(0, 0, -1),
                new Vector3(0, 0, 1), 
                new Vector3(1, 0, 0), 
                new Vector3(1, 0, 0)
            };

            float sizeOverTwo = size / 2;

            // Create each face in turn.
            for (int i = 0, j = 0; i < normals.Length; i++, j += 4)
            {
                Vector3 normal = normals[i];
                Vector3 tangent = tangents[i];

                // Get two vectors perpendicular to the face normal and to each other.
                Vector3 side1 = new Vector3(normal.Y, normal.Z, normal.X);
                Vector3 side2 = Vector3.Cross(normal, side1);

                // Six indices (two triangles) per face.
                this.AddIndex(this.VerticesCount + 0);
                this.AddIndex(this.VerticesCount + 1);
                this.AddIndex(this.VerticesCount + 3);

                this.AddIndex(this.VerticesCount + 1);
                this.AddIndex(this.VerticesCount + 2);
                this.AddIndex(this.VerticesCount + 3);

                // 0   3
                // 1   2

                // Four vertices per face.
                this.AddVertex((normal - side1 - side2) * sizeOverTwo, normal, tangent, texCoord[j]);
                this.AddVertex((normal - side1 + side2) * sizeOverTwo, normal, tangent, texCoord[j + 1]);
                this.AddVertex((normal + side1 + side2) * sizeOverTwo, normal, tangent, texCoord[j + 2]);
                this.AddVertex((normal + side1 - side2) * sizeOverTwo, normal, tangent, texCoord[j + 3]);
            }
        }

        #endregion
    }
}
