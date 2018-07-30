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
    /// Teapot primitive mesh. To render this mesh use the <see cref="MeshRenderer"/> class.
    /// </summary>
    [DataContract]
    public class TeapotMesh : MeshComponent
    {
        /// <summary>
        /// Teapot size
        /// </summary>
        private float size;

        /// <summary>
        /// Tessellation teapot
        /// </summary>
        private int tessellation;

        #region Properties

        /// <summary>
        /// Gets or sets the size of the Teapot
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
                this.GenerateTeapot();
            }
        }

        /// <summary>
        /// Gets or sets the tesellation of the teapot mesh
        /// </summary>
        [RenderPropertyAsInput(3, 25)]
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
                this.GenerateTeapot();
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
            this.tessellation = 16;
        }

        /// <summary>
        /// Initialize method
        /// </summary>
        protected override void Initialize()
        {
            this.ModelMeshName = "Primitive";
            this.GenerateTeapot();
        }

        /// <summary>
        /// Regenerate teapot mesh
        /// </summary>
        protected void GenerateTeapot()
        {
            if (this.InternalModel != null)
            {
                this.InternalModel.Unload();
                this.InternalModel = null;
            }

            this.InternalModel = new InternalModel();
            this.InternalModel.FromPrimitive(WaveServices.GraphicsDevice, new Teapot(this.Size, this.Tessellation));

            this.ThrowRefreshEvent();
        }
    }
}
