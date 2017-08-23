// Copyright © 2017 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Using Statements
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using WaveEngine.Common.Attributes;
using WaveEngine.Common.Math;
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
                this.InternalModel = this.Assets.LoadAsset<InternalStaticModel>(this.ModelPath);

                if (this.ModelMeshName == null &&
                    this.InternalModel != null &&
                    this.InternalModel.Meshes != null &&
                    this.InternalModel.Meshes.Count > 0)
                {
                    this.ModelMeshName = this.InternalModel.Meshes[0].Name;
                }
            }
        }

        /// <summary>
        /// Unload the static model
        /// </summary>
        protected void UnloadModel()
        {
            if (this.InternalModel == null)
            {
                return;
            }

            if (!string.IsNullOrEmpty(this.InternalModel.AssetPath))
            {
                if (this.Assets != null)
                {
                    this.Assets.UnloadAsset(this.InternalModel.AssetPath);
                }
            }
            else
            {
                this.InternalModel.Unload();
            }

            this.InternalModel = null;
            this.BoundingBox = new BoundingBox();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public override void Dispose()
        {
            this.UnloadModel();
        }
    }
}
