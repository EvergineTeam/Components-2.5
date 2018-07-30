// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Using Statements
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using WaveEngine.Common.Attributes;
using WaveEngine.Common.Graphics;
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
    public class CustomMesh : MeshComponent
    {
        /// <summary>
        /// Diameter  sphere
        /// </summary>
        private Mesh mesh;

        #region Properties

        /// <summary>
        /// Gets or sets the diameter of the sphere mesh
        /// </summary>
        public Mesh Mesh
        {
            get
            {
                return this.mesh;
            }

            set
            {
                this.mesh = value;
                this.GenerateInternalModel();
            }
        }

        #endregion

        /// <summary>
        /// Default values method
        /// </summary>
        protected override void DefaultValues()
        {
            base.DefaultValues();
        }

        /// <summary>
        /// Initialize method
        /// </summary>
        protected override void Initialize()
        {
            if (this.mesh != null)
            {
                this.GenerateInternalModel();
            }
        }

        /// <summary>
        /// Regenerate sphere mesh
        /// </summary>
        protected void GenerateInternalModel()
        {
            if (this.InternalModel != null)
            {
                this.InternalModel.Unload();
                this.InternalModel = null;
            }

            this.ModelMeshName = this.mesh.Name;

            this.InternalModel = new InternalModel();
            this.InternalModel.FromMesh(WaveServices.GraphicsDevice, this.mesh);

            this.ThrowRefreshEvent();
        }
    }
}
