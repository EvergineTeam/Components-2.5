// Copyright © 2017 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Using Statements
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using WaveEngine.Common.Attributes;
using WaveEngine.Common.Math;
using WaveEngine.Framework.Services;
#endregion

namespace WaveEngine.Components.Graphics3D
{
    /// <summary>
    /// Plane primitive mesh. To render this mesh use the <see cref="MeshRenderer"/> class.
    /// </summary>
    [DataContract]
    public class PlaneMesh : MeshComponent
    {
        /// <summary>
        /// Plane normal
        /// </summary>
        private Vector3 normal;

        /// <summary>
        /// Plane width
        /// </summary>
        private float width;

        /// <summary>
        /// Plane height
        /// </summary>
        private float height;

        /// <summary>
        /// Plane two sides
        /// </summary>
        private bool twoSides;

        /// <summary>
        /// Plane horizontal flip with UV coords
        /// </summary>
        private bool uvHorizontalFlip;

        /// <summary>
        /// Plane vertical flip with UV coords
        /// </summary>
        private bool uvVerticalFlip;

        #region Properties

        /// <summary>
        /// Gets or sets the plane normal
        /// </summary>
        [DontRenderProperty]
        [DataMember]
        public Vector3 Normal
        {
            get
            {
                return this.normal;
            }

            set
            {
                this.normal = value;
                this.GeneratePlane();
            }
        }

        /// <summary>
        /// Gets or sets the plane normal
        /// </summary>
        [RenderPropertyAsSelector("Normals")]
        public string PlaneNormal
        {
            get
            {
                return this.GetNormalString();
            }

            set
            {
                this.Normal = this.GetNormalFromString(value);
            }
        }

        /// <summary>
        /// Gets normals vector list
        /// </summary>
        [DontRenderProperty]
        public IEnumerable Normals
        {
            get
            {
                return new List<string>() { "X", "Y", "Z", "-X", "-Y", "-Z" };
            }
        }

        /// <summary>
        /// Gets or sets the plane width
        /// </summary>
        [DataMember]
        public float Width
        {
            get
            {
                return this.width;
            }

            set
            {
                this.width = value;
                this.GeneratePlane();
            }
        }

        /// <summary>
        /// Gets or sets the plane height
        /// </summary>
        [DataMember]
        public float Height
        {
            get
            {
                return this.height;
            }

            set
            {
                this.height = value;
                this.GeneratePlane();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the plane has two sides or not
        /// </summary>
        [DataMember]
        public bool TwoSides
        {
            get
            {
                return this.twoSides;
            }

            set
            {
                this.twoSides = value;
                this.GeneratePlane();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the plane has its uv coords with a horizontal flip
        /// </summary>
        [DataMember]
        public bool UVHorizontalFlip
        {
            get
            {
                return this.uvHorizontalFlip;
            }

            set
            {
                this.uvHorizontalFlip = value;
                this.GeneratePlane();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the plane has its uv coords with a vertical flip
        /// </summary>
        [DataMember]
        public bool UVVerticalFlip
        {
            get
            {
                return this.uvVerticalFlip;
            }

            set
            {
                this.uvVerticalFlip = value;
                this.GeneratePlane();
            }
        }

        #endregion

        /// <summary>
        /// Default values method
        /// </summary>
        protected override void DefaultValues()
        {
            base.DefaultValues();

            this.normal = Vector3.UnitY;
            this.width = 1.0f;
            this.height = 1.0f;
            this.twoSides = false;
            this.uvHorizontalFlip = false;
            this.uvVerticalFlip = false;
        }

        /// <summary>
        /// Initialize method
        /// </summary>
        protected override void Initialize()
        {
            this.ModelMeshName = "Primitive";
            this.GeneratePlane();
        }

        /// <summary>
        /// Regenerate sphere mesh
        /// </summary>
        protected void GeneratePlane()
        {
            if (this.InternalModel != null)
            {
                this.InternalModel.Unload();
                this.InternalModel = null;
            }

            this.InternalModel = new InternalStaticModel();
            this.InternalModel.FromPrimitive(WaveServices.GraphicsDevice, new Primitives.Plane(this.Normal, this.Width, this.Height, this.TwoSides, this.UVHorizontalFlip, this.UVVerticalFlip));

            this.ThrowRefreshEvent();
        }

        /// <summary>
        /// Gets Normal vector as string
        /// </summary>
        /// <returns>normal vector string name</returns>
        private string GetNormalString()
        {
            if (this.normal.X > 0)
            {
                return "X";
            }
            else if (this.normal.X < 0)
            {
                return "-X";
            }
            else if (this.normal.Y > 0)
            {
                return "Y";
            }
            else if (this.normal.Y < 0)
            {
                return "-Y";
            }
            else if (this.normal.Z > 0)
            {
                return "Z";
            }
            else if (this.normal.Z < 0)
            {
                return "-Z";
            }

            return null;
        }

        /// <summary>
        /// Gets Normal vector from string
        /// </summary>
        /// <param name="name">Normal name</param>
        /// <returns>Normal vector3</returns>
        private Vector3 GetNormalFromString(string name)
        {
            if (name == "X")
            {
                return Vector3.UnitX;
            }
            else if (name == "-X")
            {
                return -Vector3.UnitX;
            }
            else if (name == "Y")
            {
                return Vector3.UnitY;
            }
            else if (name == "-Y")
            {
                return -Vector3.UnitY;
            }
            else if (name == "Z")
            {
                return Vector3.UnitZ;
            }
            else if (name == "-Z")
            {
                return -Vector3.UnitZ;
            }
            else
            {
                return Vector3.Zero;
            }
        }
    }
}
