// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Using Statements
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using WaveEngine.Common.Attributes;
using WaveEngine.Common.Math;
using WaveEngine.Components.Primitives;
using WaveEngine.Framework.Graphics3D;
using WaveEngine.Framework.Services;
#endregion

namespace WaveEngine.Components.Graphics3D
{
    /// <summary>
    /// Sphere primitive mesh. To render this mesh use the <see cref="MeshRenderer"/> class.
    /// </summary>
    [DataContract]
    public class SphereMesh : MeshComponent
    {
        /// <summary>
        /// Diameter  sphere
        /// </summary>
        private float diameter;

        /// <summary>
        /// Tessellation sphere
        /// </summary>
        private int tessellation;

        /// <summary>
        /// Sphere horizontal flip with UV coords
        /// </summary>
        private bool uvHorizontalFlip;

        /// <summary>
        /// Sphere vertical flip with UV coords
        /// </summary>
        private bool uvVerticalFlip;

        #region Properties

        /// <summary>
        /// Gets or sets the diameter of the sphere mesh
        /// </summary>
        [DataMember]
        public float Diameter
        {
            get
            {
                return this.diameter;
            }

            set
            {
                this.diameter = value;
                this.GenerateSphere();
            }
        }

        /// <summary>
        /// Gets or sets the tesellation of the sphere mesh
        /// </summary>
        [RenderPropertyAsInput(3, 50)]
        [DataMember]
        public int Tessellation
        {
            get
            {
                return this.tessellation;
            }

            set
            {
                this.tessellation = value;
                this.GenerateSphere();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the sphere has its uv coords with a horizontal flip
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
                this.GenerateSphere();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the sphere has its uv coords with a vertical flip
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
                this.GenerateSphere();
            }
        }

        #endregion

        /// <summary>
        /// Default values method
        /// </summary>
        protected override void DefaultValues()
        {
            base.DefaultValues();
            this.diameter = 1;
            this.tessellation = 16;
        }

        /// <summary>
        /// Initialize method
        /// </summary>
        protected override void Initialize()
        {
            this.ModelMeshName = "Primitive";
            this.GenerateSphere();
        }

        /// <summary>
        /// Regenerate sphere mesh
        /// </summary>
        protected void GenerateSphere()
        {
            if (this.InternalModel != null)
            {
                this.InternalModel.Unload();
                this.InternalModel = null;
            }

            this.InternalModel = new InternalModel();
            this.InternalModel.FromPrimitive(WaveServices.GraphicsDevice, new Sphere(this.Diameter, this.Tessellation, this.UVHorizontalFlip, this.UVVerticalFlip));

            this.ThrowRefreshEvent();
        }
    }
}
