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
    /// Capsule primitive mesh. To render this mesh use the <see cref="MeshRenderer"/> class.
    /// </summary>
    [DataContract]
    public class CapsuleMesh : MeshComponent
    {
        /// <summary>
        /// Height capsule
        /// </summary>
        private float height;

        /// <summary>
        /// Radius capsule
        /// </summary>
        private float radius;

        /// <summary>
        /// Tessellation capsule
        /// </summary>
        private int tessellation;

        #region Properties

        /// <summary>
        /// Gets or sets the height of the capsule
        /// </summary>
        [RenderPropertyAsFInput(0, float.MaxValue)]
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
                this.GenerateCapsule();
            }
        }

        /// <summary>
        /// Gets or sets the radius of the capsule
        /// </summary>
        [DataMember]
        public float Radius
        {
            get
            {
                return this.radius;
            }

            set
            {
                this.radius = value;
                this.GenerateCapsule();
            }
        }

        /// <summary>
        /// Gets or sets the tesellation of the capsule mesh
        /// </summary>
        [RenderPropertyAsInput(2, 25)]
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
                this.GenerateCapsule();
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
            this.radius = 0.5f;
            this.tessellation = 16;
        }

        /// <summary>
        /// Initialize method
        /// </summary>
        protected override void Initialize()
        {
            this.ModelMeshName = "Primitive";
            this.GenerateCapsule();
        }

        /// <summary>
        /// Regenerate capsule mesh
        /// </summary>
        protected void GenerateCapsule()
        {
            if (this.InternalModel != null)
            {
                this.InternalModel.Unload();
                this.InternalModel = null;
            }

            this.InternalModel = new InternalModel();
            this.InternalModel.FromPrimitive(WaveServices.GraphicsDevice, new Capsule(this.Height, this.Radius, this.Tessellation * 2));

            this.ThrowRefreshEvent();
        }
    }
}
