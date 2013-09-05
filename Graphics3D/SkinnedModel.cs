#region File Description
//-----------------------------------------------------------------------------
// SkinnedModel
//
// Copyright © 2010 - 2013 Wave Coorporation. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using WaveEngine.Common.Math;
using WaveEngine.Graphics;
#endregion

namespace WaveEngine.Components.Graphics3D
{
    /// <summary>
    /// Class that holds the data of an animated 3D model.
    /// </summary>
    public class SkinnedModel : BaseModel
    {
        /// <summary>
        /// Number of instances of this component created.
        /// </summary>
        private static int instances;

        #region Properties
        /// <summary>
        /// Gets the number of meshes of this model.
        /// </summary>
        public override int MeshCount
        {
            get { return this.InternalModel.Meshes.Count; }
        }

        /// <summary>
        /// Gets the model data.
        /// </summary>
        protected internal InternalSkinnedModel InternalModel { get; private set; }
        #endregion

        #region Initialize
        /// <summary>
        /// Initializes a new instance of the <see cref="SkinnedModel"/> class.
        /// </summary>
        /// <param name="modelPath">The model path.</param>
        public SkinnedModel(string modelPath)
            : this("SkinnedModel" + instances, modelPath)
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
            instances++;
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
            this.InternalModel = Assets.LoadAsset<InternalSkinnedModel>(this.ModelPath);
            this.BoundingBox = this.InternalModel.BoundingBox;
        }
        #endregion
    }
}
