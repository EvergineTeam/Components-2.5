#region File Description
//-----------------------------------------------------------------------------
// GridControl
//
// Copyright © 2014 Wave Corporation
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System.Collections.Generic;
using WaveEngine.Framework.UI;
using WaveEngine.Framework;
using WaveEngine.Common.Math;
using System.Linq;
#endregion

namespace WaveEngine.Components.UI
{
    /// <summary>
    /// The grid panel.
    /// </summary>
    public class GridControl : Control
    {
        /// <summary>
        /// The instances
        /// </summary>
        private static int instances;

        /// <summary>
        /// The data
        /// </summary>
        private List<Control>[][] data;

        #region DepencencyProperties
        /// <summary>
        /// The row property
        /// </summary>
        public static readonly DependencyProperty RowProperty =
            DependencyProperty.Register(
                "Row",
                typeof(int),
                typeof(GridControl),
                new PropertyMetadata(0));

        /// <summary>
        /// The column property
        /// </summary>
        public static readonly DependencyProperty ColumnProperty =
            DependencyProperty.Register(
                "Column",
                typeof(int),
                typeof(GridControl),
                new PropertyMetadata(0));

        /// <summary>
        /// The row span property
        /// </summary>
        public static readonly DependencyProperty RowSpanProperty =
            DependencyProperty.Register(
                "RowSpan",
                typeof(int),
                typeof(GridControl),
                new PropertyMetadata(1));

        /// <summary>
        /// The column span property
        /// </summary>
        public static readonly DependencyProperty ColumnSpanProperty =
            DependencyProperty.Register(
                "ColumnSpan",
                typeof(int),
                typeof(GridControl),
                new PropertyMetadata(1));

        #endregion

        #region Properties
        /// <summary>
        /// Gets the column definitions.
        /// </summary>
        /// <value>
        /// The column definitions.
        /// </value>
        public List<ColumnDefinition> ColumnDefinitions { get; private set; }

        /// <summary>
        /// Gets the row definitions.
        /// </summary>
        /// <value>
        /// The row definitions.
        /// </value>
        public List<RowDefinition> RowDefinitions { get; private set; }

        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        /// <value>
        /// The width.
        /// </value>
        public override float Width
        {
            get
            {
                return base.Width;
            }

            set
            {
                base.Width = value;
                if (this.Owner != null)
                {
                    ImageControl imageControl = this.Owner.FindComponent<ImageControl>();
                    if (imageControl != null)
                    {
                        imageControl.Width = value;
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        /// <value>
        /// The height.
        /// </value>
        public override float Height
        {
            get
            {
                return base.Height;
            }

            set
            {
                base.Height = value;
                if (this.Owner != null)
                {
                    ImageControl imageControl = this.Owner.FindComponent<ImageControl>();
                    if (imageControl != null)
                    {
                        imageControl.Height = value;
                    }
                }
            }
        }

        #endregion

        #region Initialize

        /// <summary>
        /// Initializes a new instance of the <see cref="GridControl" /> class.
        /// </summary>
        public GridControl()
            : this(100, 100)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GridControl" /> class.
        /// </summary>
        /// <param name="width">The initial width.</param>
        /// <param name="height">The initial height.</param>
        public GridControl(int width, int height)
            : base("Grid" + instances++)
        {
            this.ColumnDefinitions = new List<ColumnDefinition>();
            this.RowDefinitions = new List<RowDefinition>();
            this.Width = width;
            this.Height = height;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Measures the specified available size.
        /// </summary>
        /// <param name="availableSize">Size of the available.</param>
        /// <returns>
        /// Size result.
        /// </returns>
        public override Vector2 Measure(Vector2 availableSize)
        {
            this.desiredSize = base.Measure(availableSize);

            Vector2 childSize = Vector2.Zero;

            Vector2 availableChildSize = availableSize;

            if (this.width > 0)
            {
                availableChildSize.X = this.width;
            }

            if (this.height > 0)
            {
                availableChildSize.Y = this.height;
            }

            // Initialize data
            this.data = new List<Control>[this.RowDefinitions.Count][];
            for (int i = 0; i < this.RowDefinitions.Count; i++)
            {
                this.data[i] = new List<Control>[this.ColumnDefinitions.Count];
                for (int j = 0; j < this.ColumnDefinitions.Count; j++)
                {
                    this.data[i][j] = new List<Control>();
                }
            }

            // Initialize source controls matrix
            foreach (Entity entity in this.Owner.ChildEntities)
            {
                Control control = entity.FindComponent<Control>(false);

                if (control != null)
                {
                    // Vector2 size = control.Measure(availableChildSize);
                    int row = (int)control.GetValue(GridControl.RowProperty);
                    if (row >= this.RowDefinitions.Count)
                    {
                        row = this.RowDefinitions.Count - 1;
                    }

                    int column = (int)control.GetValue(GridControl.ColumnProperty);
                    if (column >= this.ColumnDefinitions.Count)
                    {
                        column = this.ColumnDefinitions.Count - 1;
                    }

                    this.data[row][column].Add(control);
                }
            }

            // ----------  Calculate grid size ---------- 
            // Calculate Pixel
            foreach (var row in this.RowDefinitions)
            {
                if (row.Height.IsPixel)
                {
                    row.ActualHeight = row.Height.Value;
                }
            }

            foreach (var column in this.ColumnDefinitions)
            {
                if (column.Width.IsPixel)
                {
                    column.ActualWidth = column.Width.Value;
                }
            }

            // Calculate Auto
            int rowIndex = 0;
            foreach (var row in this.RowDefinitions)
            {
                int columnIndex = 0;
                foreach (var column in this.ColumnDefinitions)
                {
                    var list = this.data[rowIndex][columnIndex];
                    foreach (var control in list)
                    {
                        Vector2 size = control.Measure(availableSize);

                        if (!row.Height.IsPixel)
                        {
                            row.ActualHeight = MathHelper.Max(row.ActualHeight, size.Y);
                        }

                        if (!column.Width.IsPixel)
                        {
                            column.ActualWidth = MathHelper.Max(column.ActualWidth, size.X);
                        }
                    }

                    columnIndex++;
                }

                rowIndex++;
            }

            // ---------- Calculate Star ----------
            // Rows
            float totalHeightStar = this.Height;
            float totalRowStar = 0;
            foreach (var row in this.RowDefinitions)
            {
                if (row.Height.IsProportional)
                {
                    if (this.Height == 0)
                    {
                        totalHeightStar += row.ActualHeight;
                    }

                    totalRowStar += row.Height.Value;
                }
                else
                {
                    totalHeightStar -= row.ActualHeight;
                }
            }

            foreach (var row in this.RowDefinitions)
            {
                if (row.Height.IsProportional)
                {
                    row.ActualHeight = (totalHeightStar * row.Height.Value) / totalRowStar;
                }
            }

            // Columns
            float totalWidthStar = this.Width;
            float totalColumnStar = 0;
            foreach (var column in this.ColumnDefinitions)
            {
                if (column.Width.IsProportional)
                {
                    if (this.Width == 0)
                    {
                        totalWidthStar += column.ActualWidth;
                    }

                    totalColumnStar += column.Width.Value;
                }
                else
                {
                    totalWidthStar -= column.ActualWidth;
                }
            }

            foreach (var column in this.ColumnDefinitions)
            {
                if (column.Width.IsProportional)
                {
                    column.ActualWidth = (totalWidthStar * column.Width.Value) / totalColumnStar;
                }
            }

            // Get total result
            foreach (var row in this.RowDefinitions)
            {
                childSize.Y += row.ActualHeight;
            }

            foreach (var column in this.ColumnDefinitions)
            {
                childSize.X += column.ActualWidth;
            }

            // Two special case when width != childSize.X or height != childSize.Y
            if (this.Width > 0)
            {
                if (this.Width > childSize.X)
                {
                    if (this.ColumnDefinitions.Count > 0)
                    {
                        var last = this.ColumnDefinitions.Last();
                        if (last.Width.IsPixel)
                        {
                            last.ActualWidth += this.Width - childSize.X;
                        }
                    }
                }
                else
                {
                    this.Width = childSize.X;
                }
            }

            if (this.Height > 0)
            {
                if (this.Height > childSize.Y)
                {
                    if (this.RowDefinitions.Count > 0)
                    {
                        var last = this.RowDefinitions.Last();
                        if (last.Height.IsPixel)
                        {
                            last.ActualHeight += this.Height - childSize.Y;
                        }
                    }
                }
                else
                {
                    this.Height = childSize.Y;
                }
            }

            this.desiredSize.X = MathHelper.Max(childSize.X, this.desiredSize.X);
            this.desiredSize.Y = MathHelper.Max(childSize.Y, this.desiredSize.Y);

            return this.desiredSize;
        }

        /// <summary>
        /// Arranges the specified final size.
        /// </summary>
        /// <param name="finalSize">The final size.</param>
        public override void Arrange(RectangleF finalSize)
        {
            base.Arrange(finalSize);

            Vector2 totalSize = Vector2.Zero;

            int rowIndex = 0;
            foreach (var row in this.RowDefinitions)
            {
                Vector2 rowSize = Vector2.Zero;
                int columnIndex = 0;
                foreach (var column in this.ColumnDefinitions)
                {
                    var list = this.data[rowIndex][columnIndex];
                    foreach (var control in list)
                    {
                        // Calculate RectangleF
                        RectangleF rect = new RectangleF()
                        {
                            X = Transform2D.Rectangle.X + rowSize.X,
                            Y = Transform2D.Rectangle.Y + totalSize.Y,
                            Width = column.ActualWidth,
                            Height = row.ActualHeight
                        };

                        control.Arrange(rect);
                    }

                    columnIndex++;
                    rowSize.X += column.ActualWidth;
                    rowSize.Y = MathHelper.Max(rowSize.Y, row.ActualHeight);
                }

                rowIndex++;
                totalSize.X = MathHelper.Max(totalSize.X, rowSize.X);
                totalSize.Y += rowSize.Y;
            }

            // Free memory
            this.data = null;
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            // ToDo
        }
        #endregion
    }
}
