﻿#region File Description
//-----------------------------------------------------------------------------
// StackPanelControl
//
// Copyright © 2010 - 2013 Wave Coorporation. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements

using WaveEngine.Framework.UI;
using WaveEngine.Framework;
using WaveEngine.Common.Math;
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
            Orientation = Orientation.Vertical;
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

            foreach (Entity entity in Owner.ChildEntities)
            {
                Control control = entity.FindComponentOfType<Control>();

                if (control != null)
                {
                    Vector2 size = control.Measure(availableChildSize);

                    if (Orientation == Orientation.Vertical)
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

            desiredSize.X = MathHelper.Max(childSize.X, desiredSize.X);
            desiredSize.Y = MathHelper.Max(childSize.Y, desiredSize.Y);

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
            if (Orientation == Orientation.Vertical)
            {
                accum = Transform2D.Rectangle.Y;
            }
            else
            {
                accum = Transform2D.Rectangle.X;
            }

            foreach (Entity entity in Owner.ChildEntities)
            {
                Control control = entity.FindComponentOfType<Control>();

                if (control != null)
                {
                    float x;
                    float y;

                    RectangleF childRect;

                    if (Orientation == Orientation.Vertical)
                    {
                        x = Transform2D.Rectangle.X; 
                        y = accum;
                        accum += control.DesiredSize.Y;

                        childRect = new RectangleF(
                        x,
                        y,
                        Transform2D.Rectangle.Width,
                        control.DesiredSize.Y);
                    }
                    else
                    {
                        x = accum;
                        y = Transform2D.Rectangle.Y; 
                        accum += control.DesiredSize.X;

                        childRect = new RectangleF(
                        x,
                        y,
                        control.DesiredSize.X,
                        Transform2D.Rectangle.Height);
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
