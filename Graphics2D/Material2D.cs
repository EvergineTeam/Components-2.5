#region File Description
//-----------------------------------------------------------------------------
// Material2D
//
// Copyright © 2010 - 2013 Wave Coorporation. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using WaveEngine.Components.Animation.Spine;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
#endregion

namespace WaveEngine.Components.Graphics2D
{
    /// <summary>
    /// Component to managed materials 2D
    /// </summary>
    public class Material2D : Component
    {
        /// <summary>
        /// Number of instances of this component created.
        /// </summary>
        private static int instances;

        /// <summary>
        /// The disposed
        /// </summary>
        protected bool disposed;

        #region Properties
        /// <summary>
        /// Gets or sets the material.
        /// </summary>
        /// <value>
        /// The material.
        /// </value>
        public Material Material { get; set; }
        #endregion

        #region Initialize
        /// <summary>
        /// Initializes a new instance of the <see cref="Material2D"/> class.
        /// </summary>
        /// <param name="material">The material.</param>
        public Material2D(Material material)
            : base("Material2D" + instances)
        {
            instances++;
            this.Material = material;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Initialices the component.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();

            this.Material.Initialize(this.Assets);
        }

        /// <summary>
        ///     Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing">
        ///     <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    // ToDo
                    this.disposed = true;
                }
            }
        }
        #endregion
    }
}
