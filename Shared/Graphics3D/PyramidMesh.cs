// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Using Statements
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using WaveEngine.Components.Primitives;
using WaveEngine.Framework.Graphics3D;
using WaveEngine.Framework.Services;
#endregion

namespace WaveEngine.Components.Graphics3D
{
    /// <summary>
    /// Pyramid primitive mesh. To render this mesh use the <see cref="MeshRenderer"/> class.
    /// </summary>
    [DataContract]
    public class PyramidMesh : MeshComponent
    {
        /// <summary>
        /// Pyramid size
        /// </summary>
        private float size;

        #region Properties

        /// <summary>
        /// Gets or sets the size of the Pyramid
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
                this.GeneratePyramid();
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
            this.GeneratePyramid();
        }

        /// <summary>
        /// Regenerate pyramid mesh
        /// </summary>
        protected void GeneratePyramid()
        {
            if (this.InternalModel != null)
            {
                this.InternalModel.Unload();
                this.InternalModel = null;
            }

            this.InternalModel = new InternalModel();
            this.InternalModel.FromPrimitive(WaveServices.GraphicsDevice, new Pyramid(this.Size));

            this.ThrowRefreshEvent();
        }
    }
}
