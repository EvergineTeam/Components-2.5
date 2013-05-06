#region File Description
//----------------------------------------------------------------------------- 
// PanelControlRenderer
// 
// Copyright © $year$ Weekend Game Studio. All rights reserved. 
// Use is subject to license terms. 
//----------------------------------------------------------------------------- 
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WaveEngine.Common.Graphics;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics; 
#endregion

namespace WaveEngine.Components.UI
{
    /// <summary>
    /// The Panel renderer
    /// </summary>
    public class PanelControlRenderer : Drawable2D
    {
        /// <summary>
        /// Total number of instances
        /// </summary>
        private static int instances;

        /// <summary>
        /// The transform2D
        /// </summary>
        [RequiredComponent]
        public Transform2D Transform2D;

        #region Initialize

        /// <summary>
        /// Initializes a new instance of the <see cref="PanelControlRenderer" /> class.
        /// </summary>
        public PanelControlRenderer()
            : this(DefaultLayers.GUI)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PanelControlRenderer" /> class.
        /// </summary>
        /// <param name="layerType">Type of the layer.</param>
        public PanelControlRenderer(Type layerType)
            : this("PanelRenderer" + instances, layerType)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PanelControlRenderer" /> class.
        /// </summary>
        /// <param name="name">Name of this instance.</param>
        /// <param name="layerType">Type of the layer.</param>
        public PanelControlRenderer(string name, Type layerType)
            : base(name, layerType)
        {
            instances++;
        }
        #endregion

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {            
        }

        /// <summary>
        /// Draws the basic unit.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        protected override void DrawBasicUnit(int parameter)
        {            
        }

        /// <summary>
        /// Helper method that draws debug lines.
        /// </summary>
        /// <remarks>
        /// This method will only work on debug mode and if RenderManager.DebugLines /&gt;
        /// is set to <c>true</c>.
        /// </remarks>
        protected override void DrawDebugLines()
        {
            base.DrawDebugLines();

            // Rectangle
            RenderManager.LineBatch2D.DrawRectangleVM(this.Transform2D.Rectangle, Color.Orange);

            // Origin
            RenderManager.LineBatch2D.DrawPointVM(this.Transform2D.Rectangle.Location + this.Transform2D.Origin, 10f, Color.Red);
        }
    }
}
