// Copyright © 2017 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using WaveEngine.Common.Graphics;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.UI;
#endregion

namespace WaveEngine.Components.UI
{
    /// <summary>
    /// Image decorate class
    /// </summary>
    public class Image : UIBase
    {
        /// <summary>
        /// The instances
        /// </summary>
        private static int instances;

        /// <summary>
        /// The size define by user
        /// </summary>
        private bool sizeDefineByUser;

        #region Properties

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
                return this.entity.FindComponent<PanelControl>().Margin;
            }

            set
            {
                this.entity.FindComponent<PanelControl>().Margin = value;
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
                return this.entity.FindComponent<PanelControl>().Width;
            }

            set
            {
                this.entity.FindComponent<PanelControl>().Width = value;
                this.sizeDefineByUser = true;
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
                return this.entity.FindComponent<PanelControl>().Height;
            }

            set
            {
                this.entity.FindComponent<PanelControl>().Height = value;
                this.sizeDefineByUser = true;
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
                return this.entity.FindComponent<PanelControl>().HorizontalAlignment;
            }

            set
            {
                this.entity.FindComponent<PanelControl>().HorizontalAlignment = value;
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
                return this.entity.FindComponent<PanelControl>().VerticalAlignment;
            }

            set
            {
                this.entity.FindComponent<PanelControl>().VerticalAlignment = value;
            }
        }

        /// <summary>
        /// Sets the image path.
        /// </summary>
        /// <value>
        /// The image path.
        /// </value>
        public string Source
        {
            set
            {
                Entity imageEntity = this.entity.FindChild("ImageControlEntity");

                if (imageEntity != null)
                {
                    // If ImageControlEntity exist
                    ImageControl restore = imageEntity.FindComponent<ImageControl>();
                    imageEntity.RemoveComponent<ImageControl>();
                    ImageControl newImageControl = new ImageControl(value);
                    if (restore != null)
                    {
                        newImageControl.Stretch = restore.Stretch;
                    }

                    imageEntity.AddComponent(newImageControl);
                    imageEntity.RefreshDependencies();
                }
                else
                {
                    // If ImageControlEntity doesn't exist
                    this.entity.AddChild(new Entity("ImageControlEntity")
                                    .AddComponent(new Transform2D())
                                    .AddComponent(new ImageControl(value))
                                    .AddComponent(new ImageControlRenderer()));

                    this.entity.RefreshDependencies();
                }
            }
        }

        /// <summary>
        /// Gets or sets the stretch.
        /// </summary>
        /// <value>
        /// The stretch.
        /// </value>
        public Stretch Stretch
        {
            get
            {
                return this.entity.FindChild("ImageControlEntity").FindComponent<ImageControl>().Stretch;
            }

            set
            {
                this.entity.FindChild("ImageControlEntity").FindComponent<ImageControl>().Stretch = value;
            }
        }

        #endregion

        #region Initialize

        /// <summary>
        /// Initializes a new instance of the <see cref="Image" /> class.
        /// </summary>
        /// <param name="source">The source.</param>
        public Image(string source)
            : this("Image" + instances++, source)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Image" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="source">The source.</param>
        public Image(string name, string source)
        {
            this.entity = new Entity(name)
                           .AddComponent(new Transform2D())
                           .AddComponent(new PanelControl())
                           .AddComponent(new PanelControlRenderer())
                                .AddChild(new Entity("ImageControlEntity")
                                    .AddComponent(new Transform2D())
                                    .AddComponent(new ImageControl(source)
                                    {
                                        HorizontalAlignment = HorizontalAlignment.Center,
                                        VerticalAlignment = VerticalAlignment.Center
                                    })
                                    .AddComponent(new ImageControlRenderer()));

            this.entity.EntityInitialized += this.Entity_EntityInitialized;
        }

        /// <summary>
        /// Handles the EntityInitialized event of the entity control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void Entity_EntityInitialized(object sender, EventArgs e)
        {
            if (!this.sizeDefineByUser)
            {
                ImageControl ic = this.entity.FindChild("ImageControlEntity").FindComponent<ImageControl>();
                PanelControl panel = this.entity.FindComponent<PanelControl>();

                panel.Width = ic.Width;
                panel.Height = ic.Height;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Image" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="texture">Image texture.</param>
        public Image(string name, Texture texture)
        {
            this.entity = new Entity(name)
                           .AddComponent(new Transform2D())
                           .AddComponent(new PanelControl())
                           .AddComponent(new PanelControlRenderer())
                                .AddChild(new Entity("ImageControlEntity")
                                    .AddComponent(new Transform2D())
                                    .AddComponent(new ImageControl(texture)
                                    {
                                        HorizontalAlignment = HorizontalAlignment.Center,
                                        VerticalAlignment = VerticalAlignment.Center
                                    })
                                    .AddComponent(new ImageControlRenderer()));
        }
        #endregion
    }
}
