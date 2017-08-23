// Copyright © 2017 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Using Statements

using WaveEngine.Framework.UI;
using WaveEngine.Framework;
using WaveEngine.Common.Math;
using System.Runtime.Serialization;
#endregion

namespace WaveEngine.Components.UI
{
    /// <summary>
    /// The stack panel.
    /// </summary>
    public class StackPanelControl : Control
    {
        /// <summary>
        /// Total number of instances.
        /// </summary>
        private static int instances;

        #region Properties

        /// <summary>
        /// Gets or sets the orientation.
        /// </summary>
        /// <value>
        /// The orientation.
        /// </value>
        public Orientation Orientation { get; set; }

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
        /// Initializes a new instance of the <see cref="StackPanelControl" /> class.
        /// </summary>
        public StackPanelControl()
            : base("StackPanelControl" + instances++)
        {
            this.Orientation = Orientation.Vertical;
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

            foreach (Entity entity in this.Owner.ChildEntities)
            {
                Control control = entity.FindComponent<Control>(false);

                if (control != null)
                {
                    Vector2 size = control.Measure(availableChildSize);

                    if (this.Orientation == Orientation.Vertical)
                    {
                        childSize.X = MathHelper.Max(childSize.X, size.X);
                        childSize.Y += size.Y;
                    }
                    else
                    {
                        childSize.X += size.X;
                        childSize.Y = MathHelper.Max(childSize.Y, size.Y);
                    }
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

            float accum;
            if (this.Orientation == Orientation.Vertical)
            {
                accum = this.Transform2D.Rectangle.Y;
            }
            else
            {
                accum = this.Transform2D.Rectangle.X;
            }

            foreach (Entity entity in this.Owner.ChildEntities)
            {
                Control control = entity.FindComponent<Control>(false);

                if (control != null)
                {
                    float x;
                    float y;

                    RectangleF childRect;

                    if (this.Orientation == Orientation.Vertical)
                    {
                        x = this.Transform2D.Rectangle.X;
                        y = accum;
                        accum += control.DesiredSize.Y;

                        childRect = new RectangleF(
                        x,
                        y,
                        this.Transform2D.Rectangle.Width,
                        control.DesiredSize.Y);
                    }
                    else
                    {
                        x = accum;
                        y = this.Transform2D.Rectangle.Y;
                        accum += control.DesiredSize.X;

                        childRect = new RectangleF(
                        x,
                        y,
                        control.DesiredSize.X,
                        this.Transform2D.Rectangle.Height);
                    }

                    control.Arrange(childRect);
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
            // ToDo
        }
        #endregion
    }
}
