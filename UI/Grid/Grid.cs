#region File Description
//-----------------------------------------------------------------------------
// Grid
//
// Copyright © 2014 Wave Corporation
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using WaveEngine.Common.Graphics;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.UI;
#endregion

namespace WaveEngine.Components.UI
{
    /// <summary>
    /// Grid decorate class
    /// </summary>
    public class Grid : UIBase
    {
        /// <summary>
        /// The instances
        /// </summary>
        private static int instances;

        #region Properties

        /// <summary>
        /// Gets the row definitions.
        /// </summary>
        /// <value>
        /// The row definitions.
        /// </value>
        public List<RowDefinition> RowDefinitions
        {
            get
            {
                return this.entity.FindComponent<GridControl>().RowDefinitions;
            }
        }

        /// <summary>
        /// Gets the column definitions.
        /// </summary>
        /// <value>
        /// The column definitions.
        /// </value>
        public List<ColumnDefinition> ColumnDefinitions
        {
            get
            {
                return this.entity.FindComponent<GridControl>().ColumnDefinitions;
            }
        }

        /// <summary>
        /// Gets or sets the margin.
        /// </summary>
        /// <value>
        /// The margin.
        /// </value>
        public Thickness Margin
        {
            get
            {
                return this.entity.FindComponent<GridControl>().Margin;
            }

            set
            {
                this.entity.FindComponent<GridControl>().Margin = value;
            }
        }

        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        /// <value>
        /// The width.
        /// </value>
        public float Width
        {
            get
            {
                return this.entity.FindComponent<GridControl>().Width;
            }

            set
            {
                this.entity.FindComponent<GridControl>().Width = value;
            }
        }

        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        /// <value>
        /// The height.
        /// </value>
        public float Height
        {
            get
            {
                return this.entity.FindComponent<GridControl>().Height;
            }

            set
            {
                this.entity.FindComponent<GridControl>().Height = value;
            }
        }

        /// <summary>
        /// Gets or sets the horizontal alignment.
        /// </summary>
        /// <value>
        /// The horizontal alignment.
        /// </value>
        public HorizontalAlignment HorizontalAlignment
        {
            get
            {
                return this.entity.FindComponent<GridControl>().HorizontalAlignment;
            }

            set
            {
                this.entity.FindComponent<GridControl>().HorizontalAlignment = value;
            }
        }

        /// <summary>
        /// Gets or sets the vertical alignment.
        /// </summary>
        /// <value>
        /// The vertical alignment.
        /// </value>
        public VerticalAlignment VerticalAlignment
        {
            get
            {
                return this.entity.FindComponent<GridControl>().VerticalAlignment;
            }

            set
            {
                this.entity.FindComponent<GridControl>().VerticalAlignment = value;
            }
        }

        /// <summary>
        /// Gets or sets the color of the background.
        /// </summary>
        /// <value>
        /// The color of the background.
        /// </value>
        /// <exception cref="System.Exception">This panel haven't background assigned</exception>
        public Color BackgroundColor
        {
            get
            {
                Color color = Color.White;

                ImageControl imageControl = this.entity.FindComponent<ImageControl>();
                if (imageControl != null)
                {
                    color = imageControl.TintColor;
                }
                else
                {
                    throw new Exception("This panel haven't background assigned");
                }

                return color;
            }

            set
            {
                ImageControl imageControl = this.entity.FindComponent<ImageControl>();
                if (imageControl != null)
                {
                    imageControl.TintColor = value;
                }
                else
                {
                    this.entity.AddComponent(new ImageControl(value, (int)this.Width, (int)this.Height))
                               .AddComponent(new ImageControlRenderer());
                }
            }
        }

        #endregion

        #region Initialize

        /// <summary>
        /// Initializes a new instance of the <see cref="Grid" /> class.
        /// </summary>
        public Grid()
            : this("Grid" + instances++)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Grid" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public Grid(string name)
        {
            this.entity = new Entity(name)
                           .AddComponent(new Transform2D()
                           {
                               DrawOrder = 0.6f,
                           })
                           .AddComponent(new GridControl())
                           .AddComponent(new GridRenderer());
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Adds the specified UI.
        /// </summary>
        /// <param name="ui">The UI.</param>
        /// <exception cref="System.ArgumentNullException">UI component is null.</exception>
        public void Add(UIBase ui)
        {
            if (ui == null)
            {
                throw new ArgumentNullException("UI component is null.");
            }

            this.entity.AddChild(ui.Entity);
        }

        /// <summary>
        /// Removes the specified UI.
        /// </summary>
        /// <param name="ui">The UI.</param>
        /// <exception cref="System.ArgumentNullException">UI component is null.</exception>
        public void Remove(UIBase ui)
        {
            if (ui == null)
            {
                throw new ArgumentNullException("UI component is null.");
            }

            this.entity.RemoveChild(ui.Entity.Name);
        }
        #endregion

        #region Private Methods
        #endregion
    }
}
