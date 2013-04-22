#region File Description
//-----------------------------------------------------------------------------
// BorderRenderer
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
    /// Draw a simple border over controls
    /// </summary>
    public class BorderRenderer : Drawable2D
    {
        /// <summary>
        /// The transform2D
        /// </summary>
        [RequiredComponent]
        public Transform2D Transform2D;

        /// <summary>
        /// Total number of instances.
        /// </summary>
        private static int instances;

        #region Properties
        /// <summary>
        /// Gets or sets the color.
        /// </summary>
        /// <value>
        /// The color.
        /// </value>
        public Color Color { get; set; }
        #endregion

        #region Initialize
        /// <summary>
        /// Initializes a new instance of the <see cref="BorderRenderer" /> class.
        /// </summary>
        public BorderRenderer()
            : this(Color.White)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BorderRenderer" /> class.
        /// </summary>
        /// <param name="color">The color.</param>
        public BorderRenderer(Color color)
            : this("BorderRenderer" + instances, DefaultLayers.GUI, color)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BorderRenderer" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="layerType">Type of the layer.</param>
        /// <param name="color">The color.</param>
        public BorderRenderer(string name, Type layerType, Color color)
            : base(name, layerType)
        {
            instances++;
            this.Color = color;
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
            RenderManager.LineBatch2D.DrawRectangle(this.Transform2D.Rectangle, this.Color);
        }
    }
}
