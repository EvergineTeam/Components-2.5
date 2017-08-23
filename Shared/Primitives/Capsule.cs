// Copyright © 2017 Wave Engine S.L. All rights reserved. Use is subject to license terms.

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

            int halfTessellation = tessellation / 2;

            int stride = 0;

            // Start with a single vertex at the bottom of the sphere.
            this.AddVertex((Vector3.Down * radius) + (Vector3.Down * 0.5f * height), Vector3.Down, new Vector2(0.5f, 0.5f));

            // Create the lower sphere
            for (int i = 1; i <= halfTessellation; i++)
            {
                float vPercentaje = 1 - (i / (float)halfTessellation);
                float latitude = vPercentaje * MathHelper.PiOver2;
                float dy = -(float)Math.Sin(latitude);
                float dxz = (float)Math.Cos(latitude);

                for (int j = 0; j < tessellation; j++)
                {
                    float hPercentaje = j / (float)tessellation;
                    float longitude = hPercentaje * MathHelper.TwoPi;

                    float dx = (float)Math.Cos(longitude) * dxz;
                    float dz = (float)Math.Sin(longitude) * dxz;

                    var normal = new Vector3(dx, dy, dz);
                    Vector3 position = normal * radius;

                    position += Vector3.Down * 0.5f * height;

                    this.AddVertex(position, normal, new Vector2(1 - ((0.5f * dx) + 0.5f), (0.5f * dz) + 0.5f));

                    if (i < halfTessellation)
                    {
                        int nextI = i + 1;
                        int nextJ = (j + 1) % tessellation;

                        var i1 = 1 + ((i - 1) * tessellation) + j;
                        var i2 = 1 + ((i - 1) * tessellation) + nextJ;
                        var i3 = 1 + ((nextI - 1) * tessellation) + j;

                        var i4 = 1 + ((i - 1) * tessellation) + nextJ;
                        var i5 = 1 + ((nextI - 1) * tessellation) + nextJ;
                        var i6 = 1 + ((nextI - 1) * tessellation) + j;

                        this.AddIndex(i1);
                        this.AddIndex(i2);
                        this.AddIndex(i3);

                        this.AddIndex(i4);
                        this.AddIndex(i5);
                        this.AddIndex(i6);
                    }
                }
            }

            stride = this.VerticesCount;
            float cylinderHeight = height * 0.5f;

            // Creates the cylinder
            for (int i = 0; i <= tessellation; i++)
            {
                float percent = i / (float)tessellation;
                float angle = percent * MathHelper.TwoPi;

                float dx = (float)Math.Cos(angle);
                float dz = (float)Math.Sin(angle);

                Vector3 normal = new Vector3(dx, 0, dz);

                this.AddVertex((normal * radius) + (Vector3.Up * cylinderHeight), normal, new Vector2(1 - percent, 0));
                this.AddVertex((normal * radius) + (Vector3.Down * cylinderHeight), normal, new Vector2(1 - percent, 1));

                if (i < tessellation)
                {
                    this.AddIndex(stride + (i * 2));
                    this.AddIndex(stride + ((i * 2) + 1));
                    this.AddIndex(stride + (((i * 2) + 2) % ((tessellation + 1) * 2)));

                    this.AddIndex(stride + ((i * 2) + 1));
                    this.AddIndex(stride + (((i * 2) + 3) % ((tessellation + 1) * 2)));
                    this.AddIndex(stride + (((i * 2) + 2) % ((tessellation + 1) * 2)));
                }
            }

            stride = this.VerticesCount;

            // Creates a single vertex at the top of the sphere.
            this.AddVertex((Vector3.Up * radius) + (Vector3.Up * 0.5f * height), Vector3.Up, new Vector2(0.5f, 0.5f));

            // Create the upper sphere
            for (int i = 1; i <= halfTessellation; i++)
            {
                float vPercentaje = 1 - (i / (float)halfTessellation);
                float latitude = vPercentaje * MathHelper.PiOver2;
                float dy = (float)Math.Sin(latitude);
                float dxz = (float)Math.Cos(latitude);

                for (int j = 0; j < tessellation; j++)
                {
                    float hPercentaje = j / (float)tessellation;
                    float longitude = hPercentaje * MathHelper.TwoPi;

                    float dx = (float)Math.Cos(longitude) * dxz;
                    float dz = (float)Math.Sin(longitude) * dxz;

                    var normal = new Vector3(dx, dy, dz);
                    Vector3 position = normal * radius;

                    position += Vector3.Up * 0.5f * height;

                    this.AddVertex(position, normal, new Vector2((0.5f * dx) + 0.5f, (0.5f * dz) + 0.5f));

                    if (i < halfTessellation)
                    {
                        int nextI = i + 1;
                        int nextJ = (j + 1) % tessellation;

                        var i1 = 1 + ((i - 1) * tessellation) + j;
                        var i2 = 1 + ((nextI - 1) * tessellation) + j;
                        var i3 = 1 + ((i - 1) * tessellation) + nextJ;

                        var i4 = 1 + ((i - 1) * tessellation) + nextJ;
                        var i5 = 1 + ((nextI - 1) * tessellation) + j;
                        var i6 = 1 + ((nextI - 1) * tessellation) + nextJ;

                        this.AddIndex(i1 + stride);
                        this.AddIndex(i2 + stride);
                        this.AddIndex(i3 + stride);

                        this.AddIndex(i4 + stride);
                        this.AddIndex(i5 + stride);
                        this.AddIndex(i6 + stride);
                    }
                }
            }

            // Create a fan connecting the bottom vertex to the bottom latitude ring.
            for (int i = 0; i < tessellation; i++)
            {
                this.AddIndex(0);
                this.AddIndex(1 + ((i + 1) % tessellation));
                this.AddIndex(i + 1);
            }

            // Create a fan connecting the top vertex to the top latitude ring.
            for (int i = 0; i < tessellation; i++)
            {
                this.AddIndex(stride);
                this.AddIndex(stride + i + 1);
                this.AddIndex(stride + 1 + ((i + 1) % tessellation));
            }
        }

        #endregion
    }
}
