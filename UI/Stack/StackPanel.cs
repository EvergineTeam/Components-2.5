#region File Description
//-----------------------------------------------------------------------------
// StackPanel
//
// Copyright © 2010 - 2013 Wave Coorporation. All rights reserved.
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
    /// StackPanel decorate class
    /// </summary>
    public class StackPanel : UIBase
    {
        /// <summary>
        /// The instances
        /// </summary>
        private static int instances;

        #region Properties

        /// <summary>
        /// Gets or sets the orientation.
        /// </summary>
        /// <value>
        /// The orientation.
        /// </value>
        public Orientation Orientation
        {
            get
            {
                return this.entity.FindComponent<StackPanelControl>().Orientation;
            }

            set
            {
                this.entity.FindComponent<StackPanelControl>().Orientation = value;
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
                return this.entity.FindComponent<StackPanelControl>().Margin;
            }

            set
            {
                this.entity.FindComponent<StackPanelControl>().Margin = value;
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
                return this.entity.FindComponent<StackPanelControl>().Width;
            }

            set
            {
                this.entity.FindComponent<StackPanelControl>().Width = value;
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
                return this.entity.FindComponent<StackPanelControl>().Height;
            }

            set
            {
                this.entity.FindComponent<StackPanelControl>().Height = value;
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
                return this.entity.FindComponent<StackPanelControl>().HorizontalAlignment;
            }

            set
            {
                this.entity.FindComponent<StackPanelControl>().HorizontalAlignment = value;
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
                return this.entity.FindComponent<StackPanelControl>().VerticalAlignment;
            }

            set
            {
                this.entity.FindComponent<StackPanelControl>().VerticalAlignment = value;
            }
        }

        /// <summary>
        /// Gets or sets the color of the background.
        /// </summary>
        /// <value>
        /// The color of the background.
        /// </value>
        /// <exception cref="System.Exception">The panel haven't background assigned</exception>
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
        /// Initializes a new instance of the <see cref="StackPanel" /> class.
        /// </summary>
        public StackPanel()
            : this("StackPanel" + instances++)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StackPanel" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public StackPanel(string name)
        {
            this.entity = new Entity(name)
                           .AddComponent(new Transform2D()
                           {
                               DrawOrder = 0.6f,
                           })
                           .AddComponent(new StackPanelControl())
                           .AddComponent(new StackPanelRenderer());
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
    }
}
