#region File Description
//-----------------------------------------------------------------------------
// WrapPanelControl
//
// Copyright © 2017 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements

using WaveEngine.Framework.UI;
using WaveEngine.Framework;
using WaveEngine.Common.Math;
using System.Runtime.Serialization;
#endregion

namespace WaveEngine.Components.UI
{
    /// <summary>
    /// The wrap panel.
    /// </summary>
    public class WrapPanelControl : Control
    {
        /// <summary>
        /// Total number of instances.
        /// </summary>
        private static int instances;

        #region Properties

        /// <summary>
        /// Gets or sets the orientation.
        /// </summary>
        public Orientation Orientation { get; set; }
        #endregion

        #region Initialize

        /// <summary>
        /// Initializes a new instance of the <see cref="WrapPanelControl"/> class.
        /// </summary>
        public WrapPanelControl()
            : base("WrapPanel" + instances++)
        {
            Orientation = Orientation.Horizontal;
        }

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

        #region Public Methods

        /// <summary>
        /// The measure.
        /// </summary>
        /// <param name="availableSize">
        /// The available size.
        /// </param>
        /// <returns>
        /// The <see cref="Vector2"/>.
        /// </returns>
        public override Vector2 Measure(Vector2 availableSize)
        {
            this.desiredSize = base.Measure(availableSize);

            Vector2 childSize = Vector2.Zero;

            Vector2 line = Vector2.Zero;

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
                Control control = entity.FindComponent<Control>(false);

                if (control != null)
                {
                    Vector2 size = control.Measure(availableChildSize);

                    if (Orientation == Orientation.Horizontal)
                    {
                        // if new Line
                        if (line.X + size.X > availableChildSize.X)
                        {
                            childSize.X = MathHelper.Max(childSize.X, line.X);
                            line.X = 0;
                            childSize.Y += line.Y;
                            line.Y = 0;
                        }

                        line.X += size.X;
                        line.Y = MathHelper.Max(line.Y, size.Y);
                    }
                    else
                    {
                        // if new Line
                        if (line.Y + size.Y > availableChildSize.Y)
                        {
                            childSize.Y = MathHelper.Max(childSize.Y, line.Y);
                            line.Y = 0;
                            childSize.X += line.X;
                            line.X = 0;
                        }

                        line.Y += size.Y;
                        line.X = MathHelper.Max(line.X, size.X);
                    }
                }
            }

            if (line.X > 0 || line.Y > 0)
            {
                if (Orientation == Orientation.Horizontal)
                {
                    childSize.X = MathHelper.Max(childSize.X, line.X);
                    line.X = 0;
                    childSize.Y += line.Y;
                    line.Y = 0;
                }
                else
                {
                    childSize.Y = MathHelper.Max(childSize.Y, line.Y);
                    line.Y = 0;
                    childSize.X += line.X;
                    line.X = 0;
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

            Vector2 line = Vector2.Zero;
            float accum = 0;

            foreach (Entity entity in Owner.ChildEntities)
            {
                Control control = entity.FindComponent<Control>(false);

                if (control != null)
                {
                    float x;
                    float y;

                    if (Orientation == Orientation.Horizontal)
                    {
                        // if new Line
                        if (line.X + control.DesiredSize.X > Transform2D.Rectangle.Width)
                        {
                            line.X = 0;
                            accum += line.Y;
                            line.Y = 0;
                        }

                        x = Transform2D.Rectangle.X + line.X;
                        line.X += control.DesiredSize.X;
                        line.Y = MathHelper.Max(line.Y, control.DesiredSize.Y);
                        y = Transform2D.Rectangle.Y + accum;
                    }
                    else
                    {
                        // if new Line
                        if (line.Y + control.DesiredSize.Y > Transform2D.Rectangle.Height)
                        {
                            line.Y = 0;
                            accum += line.X;
                            line.X = 0;
                        }

                        y = Transform2D.Rectangle.Y + line.Y;
                        line.Y += control.DesiredSize.Y;
                        line.X = MathHelper.Max(line.X, control.DesiredSize.X);
                        x = Transform2D.Rectangle.X + accum;
                    }

                    RectangleF childRect = new RectangleF(
                        x,
                        y,
                        control.DesiredSize.X,
                        control.DesiredSize.Y);

                    control.Arrange(childRect);
                }
            }
        }
        #endregion

        #region Private Methods

        /// <summary>
        /// The dispose.
        /// </summary>
        /// <param name="disposing">
        /// The disposing.
        /// </param>
        protected override void Dispose(bool disposing)
        {
            // ToDo
        }
        #endregion
    }
}
