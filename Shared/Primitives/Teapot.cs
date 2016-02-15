#region File Description
//-----------------------------------------------------------------------------
// Teapot
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
    /// Util for create teapot primitives.
    /// </summary>
    internal sealed class Teapot : Geometric
    {
        #region Initialize
        /// <summary>
        /// Initializes a new instance of the <see cref="Teapot" /> class.
        /// </summary>
        /// <param name="size">The size (1 by default).</param>
        /// <param name="tessellation">The tessellation (8 by default).</param>
        public Teapot(float size, int tessellation)
        {
            if (tessellation < 1)
            {
                throw new ArgumentOutOfRangeException("tesselation must be greater than 0");
            }

            foreach (TeapotPatch patch in teapotPatches)
            {
                // Because the teapot is symmetrical from left to right, we only store
                // data for one side, then tessellate each patch twice, mirroring in X.
                this.TessellatePatch(patch, tessellation, new Vector3(size, size, size));
                this.TessellatePatch(patch, tessellation, new Vector3(-size, size, size));

                if (patch.MirrorZ)
                {
                    // Some parts of the teapot (the body, lid, and rim, but not the
                    // handle or spout) are also symmetrical from front to back, so
                    // we tessellate them four times, mirroring in Z as well as X.
                    this.TessellatePatch(patch, tessellation, new Vector3(size, size, -size));
                    this.TessellatePatch(patch, tessellation, new Vector3(-size, size, -size));
                }
            }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// The teapot model consiste of 10 bezier patches. Each path has 16 controls points
        /// </summary>
        private class TeapotPatch
        {
            /// <summary>
            /// All control points are stored as integers.
            /// </summary>
            public readonly int[] Indices;

            /// <summary>
            /// A teapot is symmetrical we use it for draw the other side.
            /// </summary>
            public readonly bool MirrorZ;

            /// <summary>
            /// Initializes a new instance of the <see cref="TeapotPatch" /> class.
            /// </summary>
            /// <param name="mirrorZ">if set to <c>true</c> [mirror Z].</param>
            /// <param name="indices">The indices.</param>
            public TeapotPatch(bool mirrorZ, int[] indices)
            {
                this.Indices = indices;
                this.MirrorZ = mirrorZ;
            }
        }

        /// <summary>
        /// Tessellates the specified bezier patch.
        /// </summary>
        /// <param name="patch">The patch.</param>
        /// <param name="tessellation">The tessellation.</param>
        /// <param name="scale">The scale.</param>
        private void TessellatePatch(TeapotPatch patch, int tessellation, Vector3 scale)
        {
            // Look up the 16 control points for this patch.
            Vector3[] controlPoints = new Vector3[16];

            for (int i = 0; i < 16; i++)
            {
                int index = patch.Indices[i];
                controlPoints[i] = teapotControlPoints[index] * scale;
            }

            // Is this patch being mirrored?
            bool isMirrored = Math.Sign(scale.X) != Math.Sign(scale.Z);

            // Create the index and vertex data.
            this.CreatePatchIndices(tessellation, isMirrored);
            this.CreatePatchVertices(controlPoints, tessellation, isMirrored);
        }

        /// <summary>
        /// Creates indices for a patch that is tessellated at the specified level.
        /// </summary>
        /// <param name="tessellation">The tessellation.</param>
        /// <param name="isMirrored">if set to <c>true</c> [is mirrored].</param>
        private void CreatePatchIndices(int tessellation, bool isMirrored)
        {
            int stride = tessellation + 1;

            for (int i = 0; i < tessellation; i++)
            {
                for (int j = 0; j < tessellation; j++)
                {
                    // Make a list of six index values (two triangles).
                    int[] indices =
                    {
                        (i * stride) + j,
                        ((i + 1) * stride) + j,
                        ((i + 1) * stride) + j + 1,

                        (i * stride) + j,
                        ((i + 1) * stride) + j + 1,
                        (i * stride) + j + 1,
                    };

                    // If this patch is mirrored, reverse the
                    // indices to keep the correct winding order.
                    if (isMirrored)
                    {
                        Array.Reverse(indices);
                    }

                    // Create the indices.
                    foreach (int index in indices)
                    {
                        this.AddIndex(this.VerticesCount + index);
                    }
                }
            }
        }

        /// <summary>
        /// Creates vertices for a patch that is tessellated at the specified level.
        /// </summary>
        /// <param name="patch">The patch.</param>
        /// <param name="tessellation">The tessellation.</param>
        /// <param name="isMirrored">if set to <c>true</c> [is mirrored].</param>
        private void CreatePatchVertices(Vector3[] patch, int tessellation, bool isMirrored)
        {
            for (int i = 0; i <= tessellation; i++)
            {
                float ti = (float)i / tessellation;
                float uCoord = ti;

                for (int j = 0; j <= tessellation; j++)
                {
                    float tj = (float)j / tessellation;
                    float vCoord = tj;

                    // Perform four horizontal bezier interpolations
                    // between the control points of this patch.
                    Vector3 p1 = this.Bezier(patch[0], patch[1], patch[2], patch[3], ti);
                    Vector3 p2 = this.Bezier(patch[4], patch[5], patch[6], patch[7], ti);
                    Vector3 p3 = this.Bezier(patch[8], patch[9], patch[10], patch[11], ti);
                    Vector3 p4 = this.Bezier(patch[12], patch[13], patch[14], patch[15], ti);

                    // Perform a vertical interpolation between the results of the
                    // previous horizontal interpolations, to compute the position.
                    Vector3 position = this.Bezier(p1, p2, p3, p4, tj);

                    // Perform another four bezier interpolations between the control
                    // points, but this time vertically rather than horizontally.
                    Vector3 q1 = this.Bezier(patch[0], patch[4], patch[8], patch[12], tj);
                    Vector3 q2 = this.Bezier(patch[1], patch[5], patch[9], patch[13], tj);
                    Vector3 q3 = this.Bezier(patch[2], patch[6], patch[10], patch[14], tj);
                    Vector3 q4 = this.Bezier(patch[3], patch[7], patch[11], patch[15], tj);

                    // Compute vertical and horizontal tangent vectors.
                    Vector3 tangentA = this.BezierTangent(p1, p2, p3, p4, tj);
                    Vector3 tangentB = this.BezierTangent(q1, q2, q3, q4, ti);

                    // Cross the two tangent vectors to compute the normal.
                    Vector3 normal = Vector3.Cross(tangentA, tangentB);

                    if (normal.Length() > 0.0001f)
                    {
                        normal.Normalize();

                        // If this patch is mirrored, we must invert the normal.
                        if (isMirrored)
                        {
                            normal = -normal;
                        }
                    }
                    else
                    {
                        if (position.Y > 0)
                        {
                            normal = Vector3.Up;
                        }
                        else
                        {
                            normal = Vector3.Down;
                        }
                    }

                    if (isMirrored)
                    {
                        vCoord = -vCoord;
                    }

                    // Create the vertex.
                    this.AddVertex(position, normal, new Vector2(uCoord, vCoord));
                }
            }
        }

        /// <summary>
        /// Performs a cubic bezier interpolation between four scalar control
        /// points, returning the value at the specified time (t ranges 0 to 1).
        /// </summary>
        /// <param name="p1">The p1.</param>
        /// <param name="p2">The p2.</param>
        /// <param name="p3">The p3.</param>
        /// <param name="p4">The p4.</param>
        /// <param name="t">The t.</param>
        /// <returns>Float result.</returns>
        private float Bezier(float p1, float p2, float p3, float p4, float t)
        {
            return (p1 * (1 - t) * (1 - t) * (1 - t)) +
                   (p2 * 3 * t * (1 - t) * (1 - t)) +
                   (p3 * 3 * t * t * (1 - t)) +
                   (p4 * t * t * t);
        }

        /// <summary>
        /// Performs a cubic bezier interpolation between four Vector3 control
        /// points, returning the value at the specified time (t ranges 0 to 1).
        /// </summary>
        /// <param name="p1">The p1.</param>
        /// <param name="p2">The p2.</param>
        /// <param name="p3">The p3.</param>
        /// <param name="p4">The p4.</param>
        /// <param name="t">The t.</param>
        /// <returns>Vector3 result.</returns>
        private Vector3 Bezier(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4, float t)
        {
            Vector3 result = new Vector3();

            result.X = this.Bezier(p1.X, p2.X, p3.X, p4.X, t);
            result.Y = this.Bezier(p1.Y, p2.Y, p3.Y, p4.Y, t);
            result.Z = this.Bezier(p1.Z, p2.Z, p3.Z, p4.Z, t);

            return result;
        }

        /// <summary>
        /// Computes the tangent of a cubic bezier curve at the specified time,
        /// when given four scalar control points.
        /// </summary>
        /// <param name="p1">The p1.</param>
        /// <param name="p2">The p2.</param>
        /// <param name="p3">The p3.</param>
        /// <param name="p4">The p4.</param>
        /// <param name="t">The t.</param>
        /// <returns>float result.</returns>
        private float BezierTangent(float p1, float p2, float p3, float p4, float t)
        {
            return (p1 * (-1 + (2 * t) - (t * t))) +
                   (p2 * (1 - (4 * t) + (3 * t * t))) +
                   (p3 * ((2 * t) - (3 * t * t))) +
                   (p4 * (t * t));
        }

        /// <summary>
        /// Computes the tangent of a cubic bezier curve at the specified time,
        /// when given four Vector3 control points. This is used for calculating
        /// normals (by crossing the horizontal and vertical tangent vectors).
        /// </summary>
        /// <param name="p1">The p1.</param>
        /// <param name="p2">The p2.</param>
        /// <param name="p3">The p3.</param>
        /// <param name="p4">The p4.</param>
        /// <param name="t">The t.</param>
        /// <returns>Vector3 result.</returns>
        private Vector3 BezierTangent(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4, float t)
        {
            Vector3 result = new Vector3();

            result.X = this.BezierTangent(p1.X, p2.X, p3.X, p4.X, t);
            result.Y = this.BezierTangent(p1.Y, p2.Y, p3.Y, p4.Y, t);
            result.Z = this.BezierTangent(p1.Z, p2.Z, p3.Z, p4.Z, t);

            result.Normalize();

            return result;
        }
        #endregion

        #region Data
        /// <summary>
        /// Static data array defines the bezier patches that make up the teapot.
        /// </summary>
        private static TeapotPatch[] teapotPatches =
        {
            // Rim.
            new TeapotPatch(true, new int[] { 102, 103, 104, 105, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 }),

            // Body.
            new TeapotPatch(true, new int[] { 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27 }),

            new TeapotPatch(true, new int[] { 24, 25, 26, 27, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40 }),

            // Lid.
            new TeapotPatch(true, new int[] { 96, 96, 96, 96, 97, 98, 99, 100, 101, 101, 101, 101, 0, 1, 2, 3 }),
            
            new TeapotPatch(true, new int[] { 0, 1, 2, 3, 106, 107, 108, 109, 110, 111, 112, 113, 114, 115, 116, 117 }),

            // Handle.
            new TeapotPatch(false, new int[] { 41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, 52, 53, 54, 55, 56 }),

            new TeapotPatch(false, new int[] { 53, 54, 55, 56, 57, 58, 59, 60, 61, 62, 63, 64, 28, 65, 66, 67 }),

            // Spout.
            new TeapotPatch(false, new int[] { 68, 69, 70, 71, 72, 73, 74, 75, 76, 77, 78, 79, 80, 81, 82, 83 }),

            new TeapotPatch(false, new int[] { 80, 81, 82, 83, 84, 85, 86, 87, 88, 89, 90, 91, 92, 93, 94, 95 }),

            // Bottom.
            new TeapotPatch(true, new int[] { 118, 118, 118, 118, 124, 122, 119, 121, 123, 126, 125, 120, 40, 39, 38, 37 }),
        };

        /// <summary>
        /// Static array defines the control point positions that make up the teapot.
        /// </summary>
        private static Vector3[] teapotControlPoints = 
        {
            new Vector3(0f, 0.345f, -0.05f),
            new Vector3(-0.028f, 0.345f, -0.05f),
            new Vector3(-0.05f, 0.345f, -0.028f),
            new Vector3(-0.05f, 0.345f, -0f),
            new Vector3(0f, 0.3028125f, -0.334375f),
            new Vector3(-0.18725f, 0.3028125f, -0.334375f),
            new Vector3(-0.334375f, 0.3028125f, -0.18725f),
            new Vector3(-0.334375f, 0.3028125f, -0f),
            new Vector3(0f, 0.3028125f, -0.359375f),
            new Vector3(-0.20125f, 0.3028125f, -0.359375f),
            new Vector3(-0.359375f, 0.3028125f, -0.20125f),
            new Vector3(-0.359375f, 0.3028125f, -0f),
            new Vector3(0f, 0.27f, -0.375f),
            new Vector3(-0.21f, 0.27f, -0.375f),
            new Vector3(-0.375f, 0.27f, -0.21f),
            new Vector3(-0.375f, 0.27f, -0f),
            new Vector3(0f, 0.13875f, -0.4375f),
            new Vector3(-0.245f, 0.13875f, -0.4375f),
            new Vector3(-0.4375f, 0.13875f, -0.245f),
            new Vector3(-0.4375f, 0.13875f, -0f),
            new Vector3(0f, 0.007499993f, -0.5f),
            new Vector3(-0.28f, 0.007499993f, -0.5f),
            new Vector3(-0.5f, 0.007499993f, -0.28f),
            new Vector3(-0.5f, 0.007499993f, -0f),
            new Vector3(0f, -0.105f, -0.5f),
            new Vector3(-0.28f, -0.105f, -0.5f),
            new Vector3(-0.5f, -0.105f, -0.28f),
            new Vector3(-0.5f, -0.105f, -0f),
            new Vector3(0f, -0.105f, 0.5f),
            new Vector3(0f, -0.2175f, -0.5f),
            new Vector3(-0.28f, -0.2175f, -0.5f),
            new Vector3(-0.5f, -0.2175f, -0.28f),
            new Vector3(-0.5f, -0.2175f, -0f),
            new Vector3(0f, -0.27375f, -0.375f),
            new Vector3(-0.21f, -0.27375f, -0.375f),
            new Vector3(-0.375f, -0.27375f, -0.21f),
            new Vector3(-0.375f, -0.27375f, -0f),
            new Vector3(0f, -0.2925f, -0.375f),
            new Vector3(-0.21f, -0.2925f, -0.375f),
            new Vector3(-0.375f, -0.2925f, -0.21f),
            new Vector3(-0.375f, -0.2925f, -0f),
            new Vector3(0f, 0.17625f, 0.4f),
            new Vector3(-0.075f, 0.17625f, 0.4f),
            new Vector3(-0.075f, 0.2325f, 0.375f),
            new Vector3(0f, 0.2325f, 0.375f),
            new Vector3(0f, 0.17625f, 0.575f),
            new Vector3(-0.075f, 0.17625f, 0.575f),
            new Vector3(-0.075f, 0.2325f, 0.625f),
            new Vector3(0f, 0.2325f, 0.625f),
            new Vector3(0f, 0.17625f, 0.675f),
            new Vector3(-0.075f, 0.17625f, 0.675f),
            new Vector3(-0.075f, 0.2325f, 0.75f),
            new Vector3(0f, 0.2325f, 0.75f),
            new Vector3(0f, 0.12f, 0.675f),
            new Vector3(-0.075f, 0.12f, 0.675f),
            new Vector3(-0.075f, 0.12f, 0.75f),
            new Vector3(0f, 0.12f, 0.75f),
            new Vector3(0f, 0.06375f, 0.675f),
            new Vector3(-0.075f, 0.06375f, 0.675f),
            new Vector3(-0.075f, 0.007499993f, 0.75f),
            new Vector3(0f, 0.007499993f, 0.75f),
            new Vector3(0f, -0.04875001f, 0.625f),
            new Vector3(-0.075f, -0.04875001f, 0.625f),
            new Vector3(-0.075f, -0.09562501f, 0.6625f),
            new Vector3(0f, -0.09562501f, 0.6625f),
            new Vector3(-0.075f, -0.105f, 0.5f),
            new Vector3(-0.075f, -0.18f, 0.475f),
            new Vector3(0f, -0.18f, 0.475f),
            new Vector3(0f, 0.02624997f, -0.425f),
            new Vector3(-0.165f, 0.02624997f, -0.425f),
            new Vector3(-0.165f, -0.18f, -0.425f),
            new Vector3(0f, -0.18f, -0.425f),
            new Vector3(0f, 0.02624997f, -0.65f),
            new Vector3(-0.165f, 0.02624997f, -0.65f),
            new Vector3(-0.165f, -0.12375f, -0.775f),
            new Vector3(0f, -0.12375f, -0.775f),
            new Vector3(0f, 0.195f, -0.575f),
            new Vector3(-0.0625f, 0.195f, -0.575f),
            new Vector3(-0.0625f, 0.17625f, -0.6f),
            new Vector3(0f, 0.17625f, -0.6f),
            new Vector3(0f, 0.27f, -0.675f),
            new Vector3(-0.0625f, 0.27f, -0.675f),
            new Vector3(-0.0625f, 0.27f, -0.825f),
            new Vector3(0f, 0.27f, -0.825f),
            new Vector3(0f, 0.28875f, -0.7f),
            new Vector3(-0.0625f, 0.28875f, -0.7f),
            new Vector3(-0.0625f, 0.2934375f, -0.88125f),
            new Vector3(0f, 0.2934375f, -0.88125f),
            new Vector3(0f, 0.28875f, -0.725f),
            new Vector3(-0.0375f, 0.28875f, -0.725f),
            new Vector3(-0.0375f, 0.298125f, -0.8625f),
            new Vector3(0f, 0.298125f, -0.8625f),
            new Vector3(0f, 0.27f, -0.7f),
            new Vector3(-0.0375f, 0.27f, -0.7f),
            new Vector3(-0.0375f, 0.27f, -0.8f),
            new Vector3(0f, 0.27f, -0.8f),
            new Vector3(0f, 0.4575f, -0f),
            new Vector3(0f, 0.4575f, -0.2f),
            new Vector3(-0.1125f, 0.4575f, -0.2f),
            new Vector3(-0.2f, 0.4575f, -0.1125f),
            new Vector3(-0.2f, 0.4575f, -0f),
            new Vector3(0f, 0.3825f, -0f),
            new Vector3(0f, 0.27f, -0.35f),
            new Vector3(-0.196f, 0.27f, -0.35f),
            new Vector3(-0.35f, 0.27f, -0.196f),
            new Vector3(-0.35f, 0.27f, -0f),
            new Vector3(0f, 0.3075f, -0.1f),
            new Vector3(-0.056f, 0.3075f, -0.1f),
            new Vector3(-0.1f, 0.3075f, -0.056f),
            new Vector3(-0.1f, 0.3075f, -0f),
            new Vector3(0f, 0.3075f, -0.325f),
            new Vector3(-0.182f, 0.3075f, -0.325f),
            new Vector3(-0.325f, 0.3075f, -0.182f),
            new Vector3(-0.325f, 0.3075f, -0f),
            new Vector3(0f, 0.27f, -0.325f),
            new Vector3(-0.182f, 0.27f, -0.325f),
            new Vector3(-0.325f, 0.27f, -0.182f),
            new Vector3(-0.325f, 0.27f, -0f),
            new Vector3(0f, -0.33f, -0f),
            new Vector3(-0.1995f, -0.33f, -0.35625f),
            new Vector3(0f, -0.31125f, -0.375f),
            new Vector3(0f, -0.33f, -0.35625f),
            new Vector3(-0.35625f, -0.33f, -0.1995f),
            new Vector3(-0.375f, -0.31125f, -0f),
            new Vector3(-0.35625f, -0.33f, -0f),
            new Vector3(-0.21f, -0.31125f, -0.375f),
            new Vector3(-0.375f, -0.31125f, -0.21f),
        };

        #endregion
    }
}
