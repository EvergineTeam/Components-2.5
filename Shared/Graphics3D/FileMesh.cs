// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Using Statements
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using WaveEngine.Common.Attributes;
using WaveEngine.Common.Math;
using WaveEngine.Framework.Graphics3D;
#endregion

namespace WaveEngine.Components.Graphics3D
{
    /// <summary>
    /// A fbx file mesh. To render this mesh use the <see cref="MeshRenderer"/> class.
    /// </summary>
    [DataContract]
    public class FileMesh : MeshComponent
    {
        /// <summary>
        /// The model path
        /// </summary>
        private string modelPath;

        #region Properties

        /// <summary>
        /// Gets or sets the model path.
        /// </summary>
        [RenderPropertyAsAsset(AssetType.Model)]
        [DataMember]
        public string ModelPath
        {
            get
            {
                return this.modelPath;
            }

            set
            {
                if (this.isInitialized)
                {
                    this.UnloadModel();
                }

                this.modelPath = value;
                if (this.isInitialized)
                {
                    this.LoadModel();
                }
            }
        }

        #endregion

        /// <summary>
        /// Performs further custom initialization for this instance.
        /// </summary>
        protected override void Initialize()
        {
            if (!string.IsNullOrEmpty(this.ModelPath))
            {
                this.LoadModel();
            }
        }

        /// <summary>
        /// Load the static model
        /// </summary>
        protected void LoadModel()
        {
            if (this.Assets != null && !string.IsNullOrEmpty(this.ModelPath))
            {
                this.InternalModel = this.Assets.LoadAsset<InternalModel>(this.ModelPath);

                if (this.ModelMeshName == null &&
                    this.InternalModel != null &&
                    this.InternalModel.Meshes != null &&
                    this.InternalModel.Meshes.Length > 0)
                {
                    this.ModelMeshName = this.InternalModel.Meshes[0].Name;
                }

                this.ThrowRefreshEvent();
            }
        }
    }
}
