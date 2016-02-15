#region File Description
//-----------------------------------------------------------------------------
// CheckBox
//
// Copyright © 2016 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Runtime.Serialization;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Helpers;
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
    [DataContract(Namespace = "WaveEngine.Components.UI")]
    public class CheckBox : UIBase
    {
        #region Constants

        /// <summary>
        /// The instances
        /// </summary>
        private static int instances;

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
        public event EventHandler<BoolEventArgs> Checked;

        #endregion

        #region Variables
        /// <summary>
        /// The check box behavior
        /// </summary>
        private CheckBoxBehavior checkBoxBehavior;

        /// <summary>
        /// The grid panel
        /// </summary>
        private GridControl gridPanel;

        /// <summary>
        /// The text control
        /// </summary>
        private TextControl textControl;

        /// <summary>
        /// The image checked
        /// </summary>
        private ImageControl imageChecked;

        /// <summary>
        /// The image unchecked
        /// </summary>
        private ImageControl imageUnchecked;
        #endregion

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
        [DataMember]
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
        [DataMember]
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
        [DataMember]
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
        [DataMember]
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
        [DataMember]
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
        [DataMember]
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
        [DataMember]
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
        [DataMember]
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
        [DataMember]
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
                           .AddComponent(new RectangleCollider2D())
                           .AddComponent(new TouchGestures(false))
                           .AddComponent(new CheckBoxBehavior())
                           .AddComponent(new GridControl(150, 42))
                           .AddComponent(new GridRenderer());

            this.gridPanel = this.entity.FindComponent<GridControl>();
            this.gridPanel.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Proportional) });
            this.gridPanel.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Auto) });
            this.gridPanel.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Proportional) });

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

            this.imageUnchecked = imageUnCheckedEntity.FindComponent<ImageControl>();
            this.imageUnchecked.SetValue(GridControl.RowProperty, 0);
            this.imageUnchecked.SetValue(GridControl.ColumnProperty, 0);

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

            this.imageChecked = imageCheckedEntity.FindComponent<ImageControl>();
            this.imageChecked.SetValue(GridControl.RowProperty, 0);
            this.imageChecked.SetValue(GridControl.ColumnProperty, 0);

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

            this.textControl = textEntity.FindComponent<TextControl>();
            this.textControl.SetValue(GridControl.RowProperty, 0);
            this.textControl.SetValue(GridControl.ColumnProperty, 1);
            this.textControl.OnWidthChanged += this.TextControl_OnWidthChanged;

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
            if (this.Checked != null)
            {
                this.Checked(this, new BoolEventArgs(this.checkBoxBehavior.IsChecked));
            }
        }

        /// <summary>
        /// Texts the control_ on width changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="newWidth">The new width.</param>
        private void TextControl_OnWidthChanged(object sender, float newWidth)
        {
            float totalSize;
            float checkSize;
            if (this.IsChecked)
            {
                checkSize = this.imageChecked.Width + this.imageChecked.Margin.Left + this.imageChecked.Margin.Right;
            }
            else
            {
                checkSize = this.imageUnchecked.Width + this.imageUnchecked.Margin.Left + this.imageUnchecked.Margin.Right;
            }

            float textSize = this.textControl.Width + this.textControl.Margin.Left + this.textControl.Margin.Right;

            totalSize = checkSize + textSize;

            this.gridPanel.Width = totalSize;
        }

        #endregion
    }
}
