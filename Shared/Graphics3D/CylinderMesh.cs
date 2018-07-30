// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Using Statements
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using WaveEngine.Common.Attributes;
using WaveEngine.Components.Primitives;
using WaveEngine.Framework.Graphics3D;
using WaveEngine.Framework.Services;
#endregion

namespace WaveEngine.Components.Graphics3D
{
    /// <summary>
    /// Cylinder primitive mesh. To render this mesh use the <see cref="MeshRenderer"/> class.
    /// </summary>
    [DataContract]
    public class CylinderMesh : MeshComponent
    {
        /// <summary>
        /// Height cube
        /// </summary>
        private float height;

        /// <summary>
        /// Diameter cube
        /// </summary>
        private float diameter;

        /// <summary>
        /// Tessellation cube
        /// </summary>
        private int tessellation;

        #region Properties

        /// <summary>
        /// Gets or sets the height of the cube
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
                this.GenerateCylinder();
            }
        }

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
                this.GenerateCylinder();
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
                this.GenerateCylinder();
            }
        }

        #endregion

        /// <summary>
        /// Default values method
        /// </summary>
        protected override void DefaultValues()
        {
            base.DefaultValues();

            this.height = 1.0f;
            this.diameter = 1.0f;
            this.tessellation = 16;
        }

        /// <summary>
        /// Initialize method
        /// </summary>
        protected override void Initialize()
        {
            this.ModelMeshName = "Primitive";
            this.GenerateCylinder();
        }

        /// <summary>
        /// Regenerate cylinder mesh
        /// </summary>
        protected void GenerateCylinder()
        {
            if (this.InternalModel != null)
            {
                this.InternalModel.Unload();
                this.InternalModel = null;
            }

            this.InternalModel = new InternalModel();
            this.InternalModel.FromPrimitive(WaveServices.GraphicsDevice, new Cylinder(this.Height, this.Diameter, this.Tessellation));

            this.ThrowRefreshEvent();
        }
    }
}
