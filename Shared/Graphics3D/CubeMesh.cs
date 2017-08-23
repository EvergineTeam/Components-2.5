// Copyright © 2017 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Using Statements
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using WaveEngine.Common.Math;
using WaveEngine.Components.Primitives;
using WaveEngine.Framework.Services;
#endregion

namespace WaveEngine.Components.Graphics3D
{
    /// <summary>
    /// Cube primitive mesh. To render this mesh use the <see cref="MeshRenderer"/> class.
    /// </summary>
    [DataContract]
    public class CubeMesh : MeshComponent
    {
        /// <summary>
        /// Cube width
        /// </summary>
        private float size;

        /// <summary>
        /// Cube horizontal flip with UV coords
        /// </summary>
        private bool uvHorizontalFlip;

        /// <summary>
        /// Cube vertical flip with UV coords
        /// </summary>
        private bool uvVerticalFlip;

        #region Properties

        /// <summary>
        /// Gets or sets the width of the cube
        /// </summary>
        [DataMember]
        public float Size
        {
            get
            {
                return this.size;
            }

            set
            {
                this.size = value;
                this.GenerateCube();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the cube has its uv coords with a horizontal flip
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
                this.GenerateCube();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the cube has its uv coords with a vertical flip
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
                this.GenerateCube();
            }
        }

        #endregion

        /// <summary>
        /// Default values method
        /// </summary>
        protected override void DefaultValues()
        {
            base.DefaultValues();

            this.size = 1.0f;
        }

        /// <summary>
        /// Initialize method
        /// </summary>
        protected override void Initialize()
        {
            this.ModelMeshName = "Primitive";
            this.GenerateCube();
        }

        /// <summary>
        /// Regenarte cube mesh
        /// </summary>
        protected void GenerateCube()
        {
            if (this.InternalModel != null)
            {
                this.InternalModel.Unload();
                this.InternalModel = null;
            }

            this.InternalModel = new InternalStaticModel();
            this.InternalModel.FromPrimitive(WaveServices.GraphicsDevice, new Cube(this.Size, this.UVHorizontalFlip, this.UVVerticalFlip));

            this.ThrowRefreshEvent();
        }
    }
}
