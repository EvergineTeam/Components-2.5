#region File Description
//-----------------------------------------------------------------------------
// Model
//
// Copyright © 2017 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using WaveEngine.Common.Attributes;
using WaveEngine.Common.Math;
using WaveEngine.Common.Shared.Graphics;
using WaveEngine.Components.Primitives;
using WaveEngine.Framework.Services;
using WaveEngine.Framework.Graphics;
using WaveEngine.Common.Graphics;
#endregion

namespace WaveEngine.Components.Graphics3D
{
    /// <summary>
    /// A 3D model. To render a model use the <see cref="ModelRenderer"/> class.
    /// </summary>
    [DataContract(Namespace = "WaveEngine.Components.Graphics3D")]
    public class Model : LoadableModel, IDisposable
    {
        /// <summary>
        /// Number of instances of this component created.
        /// </summary>
        private static int instances;

        /// <summary>
        /// The is primitive.
        /// </summary>
        private bool isPrimitive;

        /// <summary>
        /// Model type
        /// </summary>
        private ModelType modelType = ModelType.Custom;

        #region Properties
        /// <summary>
        /// Gets the number of meshes of this model.
        /// </summary>
        [DontRenderProperty]
        public override int MeshCount
        {
            get
            {
                return (this.InternalModel != null) ? this.InternalModel.Meshes.Count : 0;
            }
        }

        /// <summary>
        /// Gets or sets the model path.
        /// </summary>
        /// <value>
        /// The model path.
        /// </value>
        [RenderPropertyAsAsset(AssetType.Model)]
        [DataMember]
        public override string ModelPath
        {
            get
            {
                return base.ModelPath;
            }

            set
            {
                base.ModelPath = value;

                if (!string.IsNullOrEmpty(value))
                {
                    this.ModelType = ModelType.Custom;                    
                }                
            }
        }

        /// <summary>
        /// Gets or sets the model type.
        /// </summary>   
        [DontRenderProperty]
        [DataMember]
        public ModelType ModelType
        {
            get
            {
                return this.modelType;
            }

            set
            {
                this.modelType = value;
                this.isPrimitive = true;

                if (this.modelType != ModelType.Custom)
                {
                    base.ModelPath = null;
                }

                if (this.isInitialized)
                {
                    this.RefreshPrimitive();
                }
            }
        }

        /// <summary>
        /// Gets the model data.
        /// </summary>
        /// <value>
        /// The internal model.
        /// </value>
        [DontRenderProperty]
        public InternalStaticModel InternalModel { get; private set; }
        #endregion

        #region Initialize

        /// <summary>
        /// Initializes a new instance of the <see cref="Model"/> class.
        /// </summary>         
        public Model()
            : this("Model" + instances++, string.Empty)
        {
        }

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
        /// <param name="mesh">The mesh</param>
        /// <returns>A <see cref="Model"/> representing a custom mesh.</returns>
        public static Model CreateFromMesh(Mesh mesh)
        {
            Model model = new Model() { InternalModel = new InternalStaticModel() };
            model.InternalModel.FromMesh(WaveServices.GraphicsDevice, mesh);

            return model;
        }

        /// <summary>
        /// Creates the cube.
        /// </summary>
        /// <param name="mesh">The mesh</param>
        /// <param name="boundingBox">The mesh bounding box</param>
        /// <returns>A <see cref="Model"/> representing a custom mesh.</returns>
        public static Model CreateFromMesh(Mesh mesh, BoundingBox boundingBox)
        {
            Model model = new Model() { InternalModel = new InternalStaticModel() };
            model.InternalModel.FromMesh(WaveServices.GraphicsDevice, mesh, boundingBox);

            return model;
        }

        /// <summary>
        /// Creates the cube.
        /// </summary>
        /// <param name="meshes">The meshes</param>
        /// <returns>A <see cref="Model"/> representing a custom mesh.</returns>
        public static Model CreateFromMeshes(IEnumerable<Mesh> meshes)
        {
            Model model = new Model() { InternalModel = new InternalStaticModel() };
            model.InternalModel.FromMeshes(WaveServices.GraphicsDevice, meshes);

            return model;
        }

        /// <summary>
        /// Creates the cube.
        /// </summary>
        /// <param name="meshes">The meshes</param>
        /// <param name="boundingBox">The mesh bounding box</param>
        /// <returns>A <see cref="Model"/> representing a custom mesh.</returns>
        public static Model CreateFromMeshes(IEnumerable<Mesh> meshes, BoundingBox boundingBox)
        {
            Model model = new Model() { InternalModel = new InternalStaticModel() };
            model.InternalModel.FromMeshes(WaveServices.GraphicsDevice, meshes, boundingBox);

            return model;
        }

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
            cube.modelType = ModelType.Cube;

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
            sphere.modelType = ModelType.Sphere;

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
            plane.modelType = ModelType.Plane;

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
            pyramid.InternalModel.FromPrimitive(WaveServices.GraphicsDevice, new Pyramid(size));
            pyramid.isPrimitive = true;
            pyramid.modelType = ModelType.Pyramid;

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
            torus.modelType = ModelType.Torus;

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
            cylinder.modelType = ModelType.Cylinder;

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
            capsule.modelType = ModelType.Capsule;

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
            cone.modelType = ModelType.Cone;

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
            teapot.modelType = ModelType.Teapot;

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
            if (this.InternalModel == null)
            {
                return null;
            }

            return this.InternalModel.CollisionVertices;
        }

        /// <summary>
        /// The get indices
        /// </summary>
        /// <returns>
        /// Indices array
        /// </returns>
        public override int[] GetIndices()
        {
            if (this.InternalModel == null)
            {
                return null;
            }

            return this.InternalModel.CollisionIndices;
        }

        /// <summary>
        /// Clone this model instance
        /// </summary>
        /// <returns>The cloned instance.</returns>
        public override Framework.Component Clone()
        {
            if (this.InternalModel == null)
            {
                return null;
            }

            Model clonedModel = base.Clone() as Model;
            clonedModel.InternalModel = this.InternalModel.Clone();

            return clonedModel;
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Refresh primitive model
        /// </summary>
        private void RefreshPrimitive()
        {
            switch (this.modelType)
            {
                case ModelType.Custom:
                    this.isPrimitive = false;
                    this.UnloadModel();
                    this.LoadModel();
                    break;
                case ModelType.Capsule:
                    this.InternalModel = new InternalStaticModel();
                    this.InternalModel.FromPrimitive(WaveServices.GraphicsDevice, new Capsule(1.0f, 0.5f, 16));
                    break;
                case ModelType.Cone:
                    this.InternalModel = new InternalStaticModel();
                    this.InternalModel.FromPrimitive(WaveServices.GraphicsDevice, new Cone(1.0f, 1.0f, 16));
                    break;
                case ModelType.Cube:
                    this.InternalModel = new InternalStaticModel();
                    this.InternalModel.FromPrimitive(WaveServices.GraphicsDevice, new Cube(1.0f));
                    break;
                case ModelType.Cylinder:
                    this.InternalModel = new InternalStaticModel();
                    this.InternalModel.FromPrimitive(WaveServices.GraphicsDevice, new Cylinder(1.0f, 1.0f, 16));
                    break;
                case ModelType.Plane:
                    this.InternalModel = new InternalStaticModel();
                    this.InternalModel.FromPrimitive(WaveServices.GraphicsDevice, new Primitives.Plane(Vector3.UnitY, 1.0f));
                    break;
                case ModelType.Pyramid:
                    this.InternalModel = new InternalStaticModel();
                    this.InternalModel.FromPrimitive(WaveServices.GraphicsDevice, new Pyramid(1.0f));
                    break;
                case ModelType.Sphere:
                    this.InternalModel = new InternalStaticModel();
                    this.InternalModel.FromPrimitive(WaveServices.GraphicsDevice, new Sphere(1.0f, 16));
                    break;
                case ModelType.Teapot:
                    this.InternalModel = new InternalStaticModel();
                    this.InternalModel.FromPrimitive(WaveServices.GraphicsDevice, new Teapot(1.0f, 16));
                    break;
                case ModelType.Torus:
                    this.InternalModel = new InternalStaticModel();
                    this.InternalModel.FromPrimitive(WaveServices.GraphicsDevice, new Torus(1.0f, 0.333f, 16));
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Performs further custom initialization for this instance.
        /// </summary>
        protected override void Initialize()
        {
            if (!string.IsNullOrEmpty(this.ModelPath))
            {
                this.LoadModel();
            }

            if (this.isPrimitive && this.InternalModel == null)
            {
                this.RefreshPrimitive();
            }

            if (this.InternalModel != null && !this.customBoundingBoxSet)
            {
                this.BoundingBox = this.InternalModel.BoundingBox;
            }
        }

        /// <summary>
        /// Unload the static model
        /// </summary>
        protected override void UnloadModel()
        {
            if (this.InternalModel == null)
            {
                return;
            }

            if (!string.IsNullOrEmpty(this.InternalModel.AssetPath))
            {
                if (this.Assets != null)
                {
                    this.Assets.UnloadAsset(this.InternalModel.AssetPath);
                }
            }
            else
            {
                this.InternalModel.Unload();
            }

            this.InternalModel = null;
            this.BoundingBox = new BoundingBox();
        }

        /// <summary>
        /// Load the static model
        /// </summary>
        protected override void LoadModel()
        {
            if (this.isPrimitive)
            {
                return;
            }

            if (this.Assets != null && !string.IsNullOrEmpty(this.ModelPath))
            {
                this.InternalModel = this.Assets.LoadAsset<InternalStaticModel>(this.ModelPath);
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
