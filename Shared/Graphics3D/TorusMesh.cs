// Copyright © 2017 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region MyRegion
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using WaveEngine.Common.Attributes;
using WaveEngine.Components.Primitives;
using WaveEngine.Framework.Services;
#endregion

namespace WaveEngine.Components.Graphics3D
{
    /// <summary>
    /// Torus primitive mesh. To render this mesh use the <see cref="MeshRenderer"/> class.
    /// </summary>
    [DataContract]
    public class TorusMesh : MeshComponent
    {
        /// <summary>
        /// Diameter torus
        /// </summary>
        private float diameter;

        /// <summary>
        /// Thickness torus
        /// </summary>
        private float thickness;

        /// <summary>
        /// Tessellation torus
        /// </summary>
        private int tessellation;

        #region Properties

        /// <summary>
        /// Gets or sets the diameter of the torus mesh
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
                this.GenerateTorus();
            }
        }

        /// <summary>
        /// Gets or sets the thickness of the torus mesh
        /// </summary>
        [DataMember]
        public float Thickness
        {
            get
            {
                return this.thickness;
            }

            set
            {
                this.thickness = value;
                this.GenerateTorus();
            }
        }

        /// <summary>
        /// Gets or sets the tesellation of the torus mesh
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
                this.GenerateTorus();
            }
        }

        #endregion

        /// <summary>
        /// Default values method
        /// </summary>
        protected override void DefaultValues()
        {
            base.DefaultValues();

            this.diameter = 1.0f;
            this.thickness = 0.333f;
            this.tessellation = 16;
        }

        /// <summary>
        /// Initialize method
        /// </summary>
        protected override void Initialize()
        {
            this.ModelMeshName = "Primitive";
            this.GenerateTorus();
        }

        /// <summary>
        /// Regenerate torus mesh
        /// </summary>
        protected void GenerateTorus()
        {
            if (this.InternalModel != null)
            {
                this.InternalModel.Unload();
                this.InternalModel = null;
            }

            this.InternalModel = new InternalStaticModel();
            this.InternalModel.FromPrimitive(WaveServices.GraphicsDevice, new Torus(this.Diameter, this.Thickness, this.Tessellation));

            ////if (this.Assets != null && !string.IsNullOrEmpty(this.ModelPath))
            ////{
            ////    this.InternalModel = this.Assets.LoadAsset<InternalStaticModel>(this.ModelPath);
            ////}

            this.ThrowRefreshEvent();
        }
    }
}
