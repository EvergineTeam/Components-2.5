#region File Description
//-----------------------------------------------------------------------------
// ToggleSwitch
//
// Copyright © 2010 - 2013 Wave Coorporation. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using WaveEngine.Common.Graphics;
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
    /// ToggleSwitch decorate class
    /// </summary>
    public class ToggleSwitch : UIBase
    {
        /// <summary>
        /// The instances
        /// </summary>
        private static int instances;

        #region Constants

        /// <summary>
        /// The default width
        /// </summary>
        private const int DefaultWidth = 50;

        /// <summary>
        /// The default height
        /// </summary>
        private const int DefaultHeight = 20;

        /// <summary>
        /// The default text margin
        /// </summary>
        private static readonly Thickness DefaultTextMargin = new Thickness(5);

        /// <summary>
        /// The default slider margin
        /// </summary>
        private static readonly Thickness DefaultSliderMargin = new Thickness(5, 15, 5, 5);

        #endregion

        /// <summary>
        /// Occurs when [toggled].
        /// </summary>
        public event EventHandler Toggled;

        #region Properties

        /// <summary>
        /// Gets or sets the on text.
        /// </summary>
        /// <value>
        /// The on text.
        /// </value>
        public string OnText
        {
            get
            {
                return this.entity.FindComponent<ToggleSwitchBehavior>().OnText;
            }

            set
            {
                this.entity.FindComponent<ToggleSwitchBehavior>().OnText = value;
            }
        }

        /// <summary>
        /// Gets or sets the off text.
        /// </summary>
        /// <value>
        /// The off text.
        /// </value>
        public string OffText
        {
            get
            {
                return this.entity.FindComponent<ToggleSwitchBehavior>().OffText;
            }

            set
            {
                this.entity.FindComponent<ToggleSwitchBehavior>().OffText = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is on.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is on; otherwise, <c>false</c>.
        /// </value>
        public bool IsOn
        {
            get
            {
                return this.entity.FindComponent<ToggleSwitchBehavior>().IsOn;
            }

            set
            {
                this.entity.FindComponent<ToggleSwitchBehavior>().IsOn = value;
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
        /// Gets or sets the foreground.
        /// </summary>
        /// <value>
        /// The foreground.
        /// </value>
        public Color Foreground
        {
            get
            {
                return this.entity.FindChild("ForegroundEntity").FindComponent<ImageControl>().TintColor;
            }

            set
            {
                this.entity.FindChild("ForegroundEntity").FindComponent<ImageControl>().TintColor = value;
            }
        }

        /// <summary>
        /// Gets or sets the background.
        /// </summary>
        /// <value>
        /// The background.
        /// </value>
        public Color Background
        {
            get
            {
                return this.entity.FindChild("BackgroundEntity").FindComponent<ImageControl>().TintColor;
            }

            set
            {
                this.entity.FindChild("BackgroundEntity").FindComponent<ImageControl>().TintColor = value;
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

                TextControl text = textEntity.FindComponent<TextControl>();
                text.SetValue(GridControl.RowProperty, 0);
                text.SetValue(GridControl.ColumnProperty, 0);

                textEntity.RefreshDependencies();
            }
        }

        /// <summary>
        /// Gets or sets the foreground.
        /// </summary>
        /// <value>
        /// The foreground.
        /// </value>
        public Color TextColor
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
        #endregion

        #region Initialize

        /// <summary>
        /// Initializes a new instance of the <see cref="ToggleSwitch" /> class.
        /// </summary>
        public ToggleSwitch()
            : this("ToggleSwitch" + instances++)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ToggleSwitch" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public ToggleSwitch(string name)
        {
            this.entity = new Entity(name)
                           .AddComponent(new Transform2D())
                           .AddComponent(new RectangleCollider())
                           .AddComponent(new TouchGestures())
                           .AddComponent(new GridControl(100, 42))
                           .AddComponent(new GridRenderer())
                           .AddComponent(new ToggleSwitchBehavior());

            GridControl gridPanel = this.entity.FindComponent<GridControl>();
            gridPanel.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Proportional) });
            gridPanel.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Auto) });
            gridPanel.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Proportional) });

            // Text
            Entity textEntity = new Entity("TextEntity")
                                .AddComponent(new Transform2D()
                                {
                                    DrawOrder = 0.4f
                                })
                                .AddComponent(new TextControl()
                                {
                                    Text = "Off",
                                    Margin = DefaultTextMargin
                                })
                                .AddComponent(new TextControlRenderer());

            TextControl text = textEntity.FindComponent<TextControl>();
            text.SetValue(GridControl.RowProperty, 0);
            text.SetValue(GridControl.ColumnProperty, 0);

            this.entity.AddChild(textEntity);

            // Background
            Entity backgroundEntity = new Entity("BackgroundEntity")
                                .AddComponent(new Transform2D()
                                {
                                    DrawOrder = 0.5f
                                })
                                .AddComponent(new ImageControl(Color.Blue, DefaultWidth, DefaultHeight)
                                {
                                    Margin = DefaultSliderMargin
                                })
                                .AddComponent(new ImageControlRenderer());

            ImageControl background = backgroundEntity.FindComponent<ImageControl>();
            background.SetValue(GridControl.RowProperty, 0);
            background.SetValue(GridControl.ColumnProperty, 1);

            this.entity.AddChild(backgroundEntity);

            // Foreground
            Entity foregroundEntity = new Entity("ForegroundEntity")
                                .AddComponent(new Transform2D()
                                {
                                    DrawOrder = 0.45f
                                })
                                .AddComponent(new AnimationUI())
                                .AddComponent(new ImageControl(Color.LightBlue, 1, DefaultHeight)
                                {
                                    Margin = DefaultSliderMargin
                                })
                                .AddComponent(new ImageControlRenderer());

            ImageControl foreground = foregroundEntity.FindComponent<ImageControl>();
            foreground.SetValue(GridControl.RowProperty, 0);
            foreground.SetValue(GridControl.ColumnProperty, 1);

            this.entity.AddChild(foregroundEntity);

            // Bullet
            Entity bulletEntity = new Entity("BulletEntity")
                                .AddComponent(new Transform2D()
                                {
                                    DrawOrder = 0.4f
                                })
                                .AddComponent(new AnimationUI())
                                .AddComponent(new ImageControl(Color.White, DefaultHeight, DefaultHeight)
                                {
                                    Margin = DefaultSliderMargin
                                })
                                .AddComponent(new ImageControlRenderer());

            ImageControl bullet = bulletEntity.FindComponent<ImageControl>();
            bullet.SetValue(GridControl.RowProperty, 0);
            bullet.SetValue(GridControl.ColumnProperty, 1);

            this.entity.AddChild(bulletEntity);

            // Event
            this.entity.FindComponent<ToggleSwitchBehavior>().Toggled += this.ToggleSwitch_Toggled;
        }
        #endregion

        #region Public Methods
        #endregion

        #region Private Methods

        /// <summary>
        /// Handles the Toggled event of the ToggleSwitch control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void ToggleSwitch_Toggled(object sender, EventArgs e)
        {
            if (this.Toggled != null)
            {
                this.Toggled(this, e);
            }
        }

        #endregion
    }
}
