#region File Description
//----------------------------------------------------------------------------- 
// PanelControl
// 
// Copyright © 2016 Wave Engine S.L. All rights reserved. 
// Use is subject to license terms. 
//----------------------------------------------------------------------------- 
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using WaveEngine.Framework.UI;
using WaveEngine.Common.Math;
using WaveEngine.Framework;
using WaveEngine.Common.Graphics;
using System.Runtime.Serialization;
#endregion

namespace WaveEngine.Components.UI
{
    /// <summary>
    /// Simple panel as a grid with one row and one column
    /// </summary>
    [DataContract(Namespace = "WaveEngine.Components.UI")]
    public class PanelControl : Control
    {
        /// <summary> 
        /// Total number of instances. 
        /// </summary> 
        private static int instances;

        #region Initialize
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="PanelControl" /> class.
        /// </summary>
        public PanelControl()
            : this(100, 100)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PanelControl" /> class.
        /// </summary>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        public PanelControl(int width, int height)
            : base("Panel" + instances++)
        {
            this.Width = width;
            this.height = height;
        }

        #region Public Methods       

        /// <summary>
        /// Arranges the specified final size.
        /// </summary>
        /// <param name="finalSize">The final size.</param>
        public override void Arrange(RectangleF finalSize)
        {
            base.Arrange(finalSize);

            foreach (Entity entity in Owner.ChildEntities)
            {
                Control control = entity.FindComponent<Control>(false);

                if (control != null)
                {
                    control.Arrange(Transform2D.Rectangle);
                }
            }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
        }
        #endregion
    }
}
