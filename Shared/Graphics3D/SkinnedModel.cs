// Copyright © 2017 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Using Statements
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using WaveEngine.Common.Attributes;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Math;
using WaveEngine.Framework.Graphics;
#endregion

namespace WaveEngine.Components.Graphics3D
{
    /// <summary>
    /// Class that holds the data of an animated 3D model.
    /// </summary>
    [DataContract(Namespace = "WaveEngine.Components.Graphics3D")]
    public class SkinnedModel : LoadableModel
    {
        #region Properties

        /// <summary>
        /// Gets the number of meshes of this model.
        /// </summary>
        [DontRenderProperty]
        public override int MeshCount
        {
            get { return this.InternalModel.Meshes.Count; }
        }

        /// <summary>
        /// Gets or sets the model path.
        /// </summary>
        /// <value>
        /// The model path.
        /// </value>
        [DataMember]
        [RenderPropertyAsAsset(AssetType.SkinnedModel)]
        public override string ModelPath
        {
            get
            {
                return base.ModelPath;
            }

            set
            {
                base.ModelPath = value;
            }
        }

        /// <summary>
        /// Gets the model data.
        /// </summary>
        [DontRenderProperty]
        public InternalSkinnedModel InternalModel { get; private set; }
        #endregion

        #region Initialize

        /// <summary>
        /// Initializes a new instance of the <see cref="SkinnedModel"/> class.
        /// </summary>
        public SkinnedModel()
            : base("SkinnedModel" + BaseModel.instances)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SkinnedModel"/> class.
        /// </summary>
        /// <param name="modelPath">The model path.</param>
        public SkinnedModel(string modelPath)
            : this("SkinnedModel" + BaseModel.instances, modelPath)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SkinnedModel"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="modelPath">The model path.</param>
        public SkinnedModel(string name, string modelPath)
            : base(name)
        {
            if (string.IsNullOrEmpty(modelPath))
            {
                throw new NullReferenceException("ModelPath can not be null.");
            }

            this.ModelPath = modelPath;
            BaseModel.instances++;
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// Gets the collition info.
        /// </summary>
        /// <returns>
        /// Vertex array.
        /// </returns>
        public override Vector3[] GetVertices()
        {
            return null;
        }

        /// <summary>
        /// The get indices
        /// </summary>
        /// <returns>
        /// Indices array
        /// </returns>
        public override int[] GetIndices()
        {
            return null;
        }
        #endregion

        #region Private Methods

        /// <summary>
        /// Performs further custom initialization for this instance.
        /// </summary>
        protected override void Initialize()
        {
            this.LoadModel();
        }

        /// <summary>
        /// Reload the static model
        /// </summary>
        protected override void UnloadModel()
        {
            if (this.InternalModel != null && !string.IsNullOrEmpty(this.InternalModel.AssetPath))
            {
                this.Assets.UnloadAsset(this.InternalModel.AssetPath);
                this.BoundingBox = new BoundingBox();
                this.InternalModel = null;
            }
        }

        /// <summary>
        /// Reload the static model
        /// </summary>
        protected override void LoadModel()
        {
            if (!string.IsNullOrEmpty(this.ModelPath))
            {
                this.InternalModel = this.Assets.LoadAsset<InternalSkinnedModel>(this.ModelPath);

                if (!this.customBoundingBoxSet)
                {
                    this.BoundingBox = this.InternalModel.BoundingBox;
                }
            }
        }
        #endregion
    }
}
