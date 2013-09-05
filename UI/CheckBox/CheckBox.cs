#region File Description
//-----------------------------------------------------------------------------
// CheckBox
//
// Copyright © 2010 - 2013 Wave Coorporation. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using WaveEngine.Common.Graphics;
<<<<<<< HEAD
=======
using WaveEngine.Common.Helpers;
>>>>>>> Added all files in Component library
using WaveEngine.Components.Gestures;
using WaveEngine.Framework;
using WaveEngine.Framework.Animation;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Physics2D;
using WaveEngine.Framework.UI;
#endregion

namespace WaveEngine.Components.UI
{
    /// <summary>
    /// ChekcBox decorate class
    /// </summary>
    public class CheckBox : UIBase
    {
        /// <summary>
        /// The instances
        /// </summary>
        private static int instances;

        #region Constants
        /// <summary>
        /// The default margin
        /// </summary>
        private static readonly Thickness DefaultMargin = new Thickness(5);

        /// <summary>
        /// The default checked image margin
        /// </summary>
        private static readonly Thickness DefaultCheckedImageMargin = new Thickness(5, 10, 5, 5);

        /// <summary>
        /// The default unchecked image
        /// </summary>
        private const int DefaultUncheckedImage = 30;

        /// <summary>
        /// The default checked image
        /// </summary>
        private const int DefaultCheckedImage = 20;
        #endregion

        #region Events
        /// <summary>
        /// Occurs when [Checked].
        /// </summary>
<<<<<<< HEAD
        public event EventHandler Checked;

        /// <summary>
        /// Occurs when [unchecked].
        /// </summary>
        public event EventHandler Unchecked;
=======
        public event EventHandler<BoolEventArgs> Checked;

>>>>>>> Added all files in Component library
        #endregion

        /// <summary>
        /// The check box behavior
        /// </summary>
        private CheckBoxBehavior checkBoxBehavior;

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether this instance is checked.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is checked; otherwise, <c>false</c>.
        /// </value>
        public bool IsChecked
        {
            get
            {
                return this.checkBoxBehavior.IsChecked;
            }

            set
            {
                this.checkBoxBehavior.IsChecked = value;
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
        /// Gets or sets the text.
        /// </summary>
        /// <value>
        /// The text.
        /// </value>
        public string Text
        {
            get
            {
                return this.entity.FindChild("TextEntity").FindComponent<TextControl>().Text;
            }

            set
            {
                this.entity.FindChild("TextEntity").FindComponent<TextControl>().Text = value;
            }
        }

        /// <summary>
        /// Sets the font.
        /// </summary>
        /// <value>
        /// The font.
        /// </value>
        public string FontPath
        {
            set
            {
                Entity textEntity = this.entity.FindChild("TextEntity");
                TextControl textBlock = textEntity.FindComponent<TextControl>();
                textEntity.RemoveComponent<TextControl>();
                textEntity.AddComponent(new TextControl(value)
                {
                    Text = textBlock.Text,
                    Foreground = textBlock.Foreground,
                    Margin = textBlock.Margin,
                    HorizontalAlignment = textBlock.HorizontalAlignment,
                    VerticalAlignment = textBlock.VerticalAlignment,
                    LineSpacing = textBlock.LineSpacing,
                    LineWidth = textBlock.LineWidth,
                    TouchMargin = textBlock.TouchMargin,
                    TextWrapping = textBlock.TextWrapping
                });

                TextControl textControl = textEntity.FindComponent<TextControl>();
                textControl.SetValue(GridControl.RowProperty, 0);
                textControl.SetValue(GridControl.ColumnProperty, 1);

                textEntity.RefreshDependencies();
            }
        }

        /// <summary>
        /// Gets or sets the foreground.
        /// </summary>
        /// <value>
        /// The foreground.
        /// </value>
        public Color Foreground
        {
            get
            {
                return this.entity.FindChild("TextEntity").FindComponent<TextControl>().Foreground;
            }

            set
            {
                this.entity.FindChild("TextEntity").FindComponent<TextControl>().Foreground = value;
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
        /// Gets or sets the horizontal text alignment.
        /// </summary>
        /// <value>
        /// The horizontal text alignment.
        /// </value>
        public HorizontalAlignment HorizontalTextAlignment
        {
            get
            {
                return this.entity.FindChild("TextEntity").FindComponent<TextControl>().HorizontalAlignment;
            }

            set
            {
                this.entity.FindChild("TextEntity").FindComponent<TextControl>().HorizontalAlignment = value;
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
        /// Gets or sets the vertical text alignment.
        /// </summary>
        /// <value>
        /// The vertical text alignment.
        /// </value>
        public VerticalAlignment VerticalTextAlignment
        {
            get
            {
                return this.entity.FindChild("TextEntity").FindComponent<TextControl>().VerticalAlignment;
            }

            set
            {
                this.entity.FindChild("TextEntity").FindComponent<TextControl>().VerticalAlignment = value;
            }
        }

        #endregion

        #region Initialize

        /// <summary>
        /// Initializes a new instance of the <see cref="CheckBox" /> class.
        /// </summary>
        public CheckBox()
            : this("CheckBox" + instances++)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CheckBox" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public CheckBox(string name)
        {
            this.entity = new Entity(name)
                           .AddComponent(new Transform2D())
                           .AddComponent(new RectangleCollider())
                           .AddComponent(new TouchGestures())
                           .AddComponent(new CheckBoxBehavior())
                           .AddComponent(new GridControl(150, 42))
                           .AddComponent(new GridRenderer());

            GridControl gridPanel = this.entity.FindComponent<GridControl>();
            gridPanel.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Proportional) });
            gridPanel.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Auto) });
            gridPanel.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Proportional) });

            // Image Unchecked
            Entity imageUnCheckedEntity = new Entity("ImageUncheckedEntity")
                                    .AddComponent(new Transform2D()
                                    {
                                        DrawOrder = 0.5f
                                    })
                                    .AddComponent(new ImageControl(Color.White, DefaultUncheckedImage, DefaultUncheckedImage)
                                    {
                                        Margin = DefaultMargin,
                                        HorizontalAlignment = HorizontalAlignment.Center
                                    })
                                    .AddComponent(new ImageControlRenderer());

            ImageControl imageUnchecked = imageUnCheckedEntity.FindComponent<ImageControl>();
            imageUnchecked.SetValue(GridControl.RowProperty, 0);
            imageUnchecked.SetValue(GridControl.ColumnProperty, 0);

            this.entity.AddChild(imageUnCheckedEntity);

            // Image Checked
            Entity imageCheckedEntity = new Entity("ImageCheckedEntity")
                                    .AddComponent(new Transform2D()
                                    {
                                        DrawOrder = 0.45f,
                                        Opacity = 0
                                    })
                                    .AddComponent(new AnimationUI())
                                    .AddComponent(new ImageControl(Color.Black, DefaultCheckedImage, DefaultCheckedImage)
                                    {
                                        Margin = DefaultCheckedImageMargin,
                                        HorizontalAlignment = HorizontalAlignment.Center
                                    })
                                    .AddComponent(new ImageControlRenderer());

            ImageControl imageChecked = imageCheckedEntity.FindComponent<ImageControl>();
            imageChecked.SetValue(GridControl.RowProperty, 0);
            imageChecked.SetValue(GridControl.ColumnProperty, 0);

            this.entity.AddChild(imageCheckedEntity);

            // Text
            Entity textEntity = new Entity("TextEntity")
                                    .AddComponent(new Transform2D()
                                    {
                                        DrawOrder = 0.4f
                                    })
                                    .AddComponent(new TextControl()
                                    {
                                        Text = "CheckBox",
                                        Margin = DefaultMargin
                                    })
                                    .AddComponent(new TextControlRenderer());

            TextControl textControl = textEntity.FindComponent<TextControl>();
            textControl.SetValue(GridControl.RowProperty, 0);
            textControl.SetValue(GridControl.ColumnProperty, 1);

            this.entity.AddChild(textEntity);

            // Cached
            this.checkBoxBehavior = this.entity.FindComponent<CheckBoxBehavior>();

            // Events
            this.checkBoxBehavior.CheckedChanged -= this.CheckBox_CheckedChanged;
            this.checkBoxBehavior.CheckedChanged += this.CheckBox_CheckedChanged;
        }
        #endregion

        #region Private Methods

        /// <summary>
        /// Handles the CheckedChanged event of the CheckBox control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void CheckBox_CheckedChanged(object sender, EventArgs e)
        {
<<<<<<< HEAD
            if (this.checkBoxBehavior.IsChecked)
            {
                if (this.Checked != null)
                {
                    this.Checked(this, e);
                }
            }
            else
            {
                if (this.Unchecked != null)
                {
                    this.Unchecked(this, e);
                }
=======
            if (this.Checked != null)
            {
                this.Checked(this, new BoolEventArgs(this.checkBoxBehavior.IsChecked));
>>>>>>> Added all files in Component library
            }
        }
        #endregion
    }
}
