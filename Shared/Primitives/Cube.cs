// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Using Statements
using WaveEngine.Common.Math;
using WaveEngine.Framework.Graphics3D;
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
        /// <param name="uvHorizontalFlip">UV coord horizontal flip</param>
        /// <param name="uvVerticalFlip">UV coord vertical flip</param>
        /// <param name="uTileFactor">Tile U factor</param>
        /// <param name="vTileFactor">Tile V factor</param>
        public Cube(float size, bool uvHorizontalFlip = false, bool uvVerticalFlip = false, float uTileFactor = 1, float vTileFactor = 1)
        {
            float uCoordMin = uvHorizontalFlip ? uTileFactor : 0;
            float uCoordMax = uvHorizontalFlip ? 0 : uTileFactor;
            float vCoordMin = uvVerticalFlip ? vTileFactor : 0;
            float vCoordMax = uvVerticalFlip ? 0 : vTileFactor;

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
                new Vector2(uCoordMax, vCoordMax), new Vector2(uCoordMin, vCoordMax), new Vector2(uCoordMin, vCoordMin), new Vector2(uCoordMax, vCoordMin),
                new Vector2(uCoordMin, vCoordMin), new Vector2(uCoordMax, vCoordMin), new Vector2(uCoordMax, vCoordMax), new Vector2(uCoordMin, vCoordMax),
                new Vector2(uCoordMax, vCoordMin), new Vector2(uCoordMax, vCoordMax), new Vector2(uCoordMin, vCoordMax), new Vector2(uCoordMin, vCoordMin),
                new Vector2(uCoordMax, vCoordMin), new Vector2(uCoordMax, vCoordMax), new Vector2(uCoordMin, vCoordMax), new Vector2(uCoordMin, vCoordMin),
                new Vector2(uCoordMin, vCoordMax), new Vector2(uCoordMin, vCoordMin), new Vector2(uCoordMax, vCoordMin), new Vector2(uCoordMax, vCoordMax),
                new Vector2(uCoordMax, vCoordMin), new Vector2(uCoordMax, vCoordMax), new Vector2(uCoordMin, vCoordMax), new Vector2(uCoordMin, vCoordMin),
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
                float sideOverTwo = size * 0.5f;

                // Four vertices per face.
                this.AddVertex((normal - side1 - side2) * sideOverTwo, normal, tangent, texCoord[j]);
                this.AddVertex((normal - side1 + side2) * sideOverTwo, normal, tangent, texCoord[j + 1]);
                this.AddVertex((normal + side1 + side2) * sideOverTwo, normal, tangent, texCoord[j + 2]);
                this.AddVertex((normal + side1 - side2) * sideOverTwo, normal, tangent, texCoord[j + 3]);
            }
        }

        #endregion
    }
}
