#region File Description
//-----------------------------------------------------------------------------
// GridRenderer
//
// Copyright © 2014 Wave Corporation
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Math;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
#endregion

namespace WaveEngine.Components.UI
{
    /// <summary>
    /// The Grid renderer.
    /// </summary>
    public class GridRenderer : Drawable2D
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

        /// <summary>
        /// The grid
        /// </summary>
        [RequiredComponent]
        public GridControl Grid;

        #region Initialize

        /// <summary>
        /// Initializes a new instance of the <see cref="GridRenderer" /> class.
        /// </summary>
        public GridRenderer()
            : this(DefaultLayers.GUI)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GridRenderer" /> class.
        /// </summary>
        /// <param name="layerType">Type of the layer.</param>
        public GridRenderer(Type layerType)
            : this("GridRenderer" + instances, layerType)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GridRenderer" /> class.
        /// </summary>
        /// <param name="name">Name of this instance.</param>
        /// <param name="layerType">Type of the layer.</param>
        public GridRenderer(string name, Type layerType)
            : base(name, layerType)
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
            RenderManager.LineBatch2D.DrawRectangleVM(this.Transform2D.Rectangle, Color.Orange);

            // Origin
            RenderManager.LineBatch2D.DrawPointVM(this.Transform2D.Rectangle.Location + this.Transform2D.Origin, 10f, Color.Red);

            // Rows and Columns
            float totalRow = 0;
            for (int i = 0; i < this.Grid.RowDefinitions.Count - 1; i++)
            {
                var row = this.Grid.RowDefinitions[i];
                float currentY = this.Transform2D.Rectangle.Y + row.ActualHeight;

                Vector2 start = new Vector2(this.Transform2D.Rectangle.X, currentY + totalRow);
                Vector2 end = new Vector2(this.Transform2D.Rectangle.X + this.Transform2D.Rectangle.Width, currentY + totalRow);

                RenderManager.LineBatch2D.DrawLineVM(start, end, Color.Green);

                totalRow += row.ActualHeight;
            }

            float totalColumn = 0;
            for (int i = 0; i < this.Grid.ColumnDefinitions.Count - 1; i++)
            {
                var column = this.Grid.ColumnDefinitions[i];
                float currentX = this.Transform2D.Rectangle.X + column.ActualWidth;

                Vector2 start = new Vector2(currentX + totalColumn, this.Transform2D.Rectangle.Y);
                Vector2 end = new Vector2(currentX + totalColumn, this.Transform2D.Rectangle.Y + this.Transform2D.Rectangle.Height);

                RenderManager.LineBatch2D.DrawLineVM(start, end, Color.Green);

                totalColumn += column.ActualWidth;
            }
        }
        #endregion
    }
}
