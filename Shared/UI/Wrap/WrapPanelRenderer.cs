// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Using Statements
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using WaveEngine.Common.Graphics;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
#endregion

namespace WaveEngine.Components.UI
{
    /// <summary>
    /// The WrapPanel Renderer
    /// </summary>
    public class WrapPanelRenderer : DrawableGUI
    {
        /// <summary>
        /// Total number of instances
        /// </summary>
        private static int instances;

        #region Properties
        #endregion

        #region Initialize

        /// <summary>
        /// Initializes a new instance of the <see cref="WrapPanelRenderer" /> class.
        /// </summary>
        public WrapPanelRenderer()
            : this(DefaultLayers.GUI)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WrapPanelRenderer" /> class.
        /// </summary>
        /// <param name="layerId">Type of the layer.</param>
        public WrapPanelRenderer(int layerId)
            : this("StackPanelRenderer" + instances, layerId)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WrapPanelRenderer" /> class.
        /// </summary>
        /// <param name="name">Name of this instance.</param>
        /// <param name="layerId">Type of the layer.</param>
        public WrapPanelRenderer(string name, int layerId)
            : base(name, layerId)
        {
            instances++;
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// Allows to perform custom drawing.
        /// </summary>
        /// <param name="gameTime">The elapsed game time.</param>
        /// <remarks>
        /// This method will only be called if all the following points are true:
        /// <list type="bullet">
        /// <item>
        /// <description>The entity passes the culling test.</description>
        /// </item>
        /// <item>
        /// <description>The parent of the owner <see cref="Entity" /> of the <see cref="Drawable" /> cascades its visibility to its children and it is visible.</description>
        /// </item>
        /// <item>
        /// <description>The <see cref="Drawable" /> is active.</description>
        /// </item>
        /// <item>
        /// <description>The owner <see cref="Entity" /> of the <see cref="Drawable" /> is active and visible.</description>
        /// </item>
        /// </list>
        /// </remarks>
        public override void Draw(TimeSpan gameTime)
        {
        }
        #endregion

        #region Private Methods

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
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
            this.RenderManager.LineBatch2D.DrawRectangle(this.Transform2D.Rectangle, Color.Orange, this.Transform2D.DrawOrder);

            // Origin
            this.RenderManager.LineBatch2D.DrawPoint(this.Transform2D.Rectangle.Location + this.Transform2D.Origin, 10f, Color.Red, this.Transform2D.DrawOrder);
        }
        #endregion
    }
}
