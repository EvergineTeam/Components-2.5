#region File Description
//-----------------------------------------------------------------------------
// Model
//
// Copyright © 2010 - 2013 Wave Coorporation. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using WaveEngine.Common.Math;
using WaveEngine.Components.Primitives;
using WaveEngine.Framework.Services;
using WaveEngine.Graphics;
#endregion

namespace WaveEngine.Components.Graphics3D
{
    /// <summary>
    /// A 3D model. To render a model use the <see cref="ModelRenderer"/> class.
    /// </summary>
    public class Model : BaseModel, IDisposable
    {
        /// <summary>
        /// Number of instances of this component created.
        /// </summary>
        private static int instances;

        /// <summary>
        /// The is primitive.
        /// </summary>
        private bool isPrimitive;

        #region Properties
        /// <summary>
        /// Gets the number of meshes of this model.
        /// </summary>
        public override int MeshCount
        {
            get { return this.InternalModel.Meshes.Count; }
        }

        /// <summary>
        /// Gets the model data.
        /// </summary>
        /// <value>
        /// The internal model.
        /// </value>
        protected internal InternalStaticModel InternalModel { get; private set; }
        #endregion

        #region Initialize
        /// <summary>
        /// Initializes a new instance of the <see cref="Model"/> class.
        /// </summary>
        /// <param name="modelPath">The model path.</param>
        public Model(string modelPath)
            : this("Model" + instances++, modelPath)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Model"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="modelPath">The model path.</param>
        public Model(string name, string modelPath)
            : base(name)
        {
            this.ModelPath = modelPath;
            this.isPrimitive = false;
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// Creates the cube.
        /// </summary>
        /// <param name="size">The size (1 by default).</param>
        /// <returns>A <see cref="Model"/> representing a cube.</returns>
        public static Model CreateCube(float size = 1.0f)
        {
            Model cube = new Model(string.Empty) { InternalModel = new InternalStaticModel() };
            cube.InternalModel.FromPrimitive(WaveServices.GraphicsDevice, new Cube(size));
            cube.isPrimitive = true;

            return cube;
        }

        /// <summary>
        /// Creates the sphere.
        /// </summary>
        /// <param name="diameter">The diameter (1 by default).</param>
        /// <param name="tessellation">The tessellation (8 by default).</param>
        /// <returns>A <see cref="Model"/> representing a sphere.</returns>
        public static Model CreateSphere(float diameter = 1.0f, int tessellation = 8)
        {
            Model sphere = new Model(string.Empty) { InternalModel = new InternalStaticModel() };
            sphere.InternalModel.FromPrimitive(WaveServices.GraphicsDevice, new Sphere(diameter, tessellation));
            sphere.isPrimitive = true;

            return sphere;
        }

        /// <summary>
        /// Creates the plane.
        /// </summary>
        /// <param name="normal">The normal ( [0, 1, 0] by default).</param>
        /// <param name="size">The size (1 by default).</param>
        /// <returns>
        /// A <see cref="Model" /> representing a plane.
        /// </returns>
        public static Model CreatePlane(Vector3? normal = null, float size = 1.0f)
        {
            if (!normal.HasValue)
            {
                normal = Vector3.Up;
            }

            Model plane = new Model(string.Empty) { InternalModel = new InternalStaticModel() };
            plane.InternalModel.FromPrimitive(WaveServices.GraphicsDevice, new Primitives.Plane(normal.Value, size));
            plane.isPrimitive = true;

            return plane;
        }

        /// <summary>
        /// Creates the pyramid.
        /// </summary>
        /// <param name="size">The size (1 by default).</param>
        /// <returns>A <see cref="Model"/> representing a pyramid.</returns>
        public static Model CreatePyramid(float size = 1.0f)
        {
            Model pyramid = new Model(string.Empty) { InternalModel = new InternalStaticModel() };
            pyramid.InternalModel.FromPrimitive(WaveServices.GetService<GraphicsDevice>(), new Pyramid(size));
            pyramid.isPrimitive = true;

            return pyramid;
        }

        /// <summary>
        /// Creates the torus.
        /// </summary>
        /// <param name="diameter">The diameter (1 by default).</param>
        /// <param name="thickness">The thickness (0.333f by default).</param>
        /// <param name="tessellation">The tessellation (16 by default).</param>
        /// <returns>A <see cref="Model"/> representing a torus</returns>
        public static Model CreateTorus(float diameter = 1.0f, float thickness = 0.333f, int tessellation = 16)
        {
            Model torus = new Model(string.Empty) { InternalModel = new InternalStaticModel() };
            torus.InternalModel.FromPrimitive(WaveServices.GraphicsDevice, new Torus(diameter, thickness, tessellation));
            torus.isPrimitive = true;

            return torus;
        }

        /// <summary>
        /// Creates the cylinder.
        /// </summary>
        /// <param name="height">The height (1 by default).</param>
        /// <param name="diameter">The diameter (1 by default).</param>
        /// <param name="tessellation">The tessellation (16 by default).</param>
        /// <returns>A <see cref="Model"/> representing a cylinder.</returns>
        public static Model CreateCylinder(float height = 1.0f, float diameter = 1.0f, int tessellation = 16)
        {
            Model cylinder = new Model(string.Empty) { InternalModel = new InternalStaticModel() };
            cylinder.InternalModel.FromPrimitive(WaveServices.GraphicsDevice, new Cylinder(height, diameter, tessellation));
            cylinder.isPrimitive = true;

            return cylinder;
        }

        /// <summary>
        /// Creates the capsule.
        /// </summary>
        /// <param name="height">The height (1 by default).</param>
        /// <param name="radius">The radius (0.5f by default).</param>
        /// <param name="tessellation">The tessellation (16 by default).</param>
        /// <returns>A <see cref="Model"/> representing a capsule.</returns>
        public static Model CreateCapsule(float height = 1f, float radius = 0.5f, int tessellation = 16)
        {
            Model capsule = new Model(string.Empty) { InternalModel = new InternalStaticModel() };
            capsule.InternalModel.FromPrimitive(WaveServices.GraphicsDevice, new Capsule(height, radius, tessellation));
            capsule.isPrimitive = true;

            return capsule;
        }

        /// <summary>
        /// Creates the cone.
        /// </summary>
        /// <param name="height">The height (1 by default).</param>
        /// <param name="diameter">The diameter (1 by default).</param>
        /// <param name="tessellation">The tessellation (16 by default).</param>
        /// <returns>A <see cref="Model"/> representing a cone.</returns>
        public static Model CreateCone(float height = 1.0f, float diameter = 1.0f, int tessellation = 16)
        {
            Model cone = new Model(string.Empty) { InternalModel = new InternalStaticModel() };
            cone.InternalModel.FromPrimitive(WaveServices.GraphicsDevice, new Cone(height, diameter, tessellation));
            cone.isPrimitive = true;

            return cone;
        }

        /// <summary>
        /// Creates the teapot.
        /// </summary>
        /// <param name="size">The size.</param>
        /// <param name="tessellation">The tessellation.</param>
        /// <returns>A <see cref="Model"/> representing a teapot.</returns>
        public static Model CreateTeapot(float size = 1.0f, int tessellation = 8)
        {
            Model teapot = new Model(string.Empty) { InternalModel = new InternalStaticModel() };
            teapot.InternalModel.FromPrimitive(WaveServices.GraphicsDevice, new Teapot(size, tessellation));
            teapot.isPrimitive = true;

            return teapot;
        }

        /// <summary>
        /// Gets the collition info.
        /// </summary>
        /// <returns>
        /// Vertex array.
        /// </returns>
        public override Vector3[] GetVertices()
        {
            return this.InternalModel.Meshes[0].CollisionVertices;
        }

        /// <summary>
        /// The get indices
        /// </summary>
        /// <returns>
        /// Indices array
        /// </returns>
        public override int[] GetIndices()
        {
            var mesh = this.InternalModel.Meshes[0];
            int[] result = new int[mesh.CollisionIndices.Length];

            for (int i = 0; i < mesh.CollisionIndices.Length; i++)
            {
                result[i] = mesh.CollisionIndices[i];
            }

            return result;
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Performs further custom initialization for this instance.
        /// </summary>
        protected override void Initialize()
        {
            if (string.IsNullOrEmpty(this.ModelPath))
            {
                this.BoundingBox = this.InternalModel.BoundingBox;
            }
            else
            {
                this.InternalModel = Assets.LoadAsset<InternalStaticModel>(this.ModelPath);
                this.BoundingBox = this.InternalModel.BoundingBox;
            }
        }
        #endregion

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (this.isPrimitive && this.InternalModel != null)
            {
                this.InternalModel.Unload();
                this.InternalModel = null;
            }
        }
    }
}
