#region File Description
//-----------------------------------------------------------------------------
// TextBox
//
// Copyright © 2014 Wave Corporation
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
    /// TextBox decorate class
    /// </summary>
    public class TextBox : UIBase
    {
        /// <summary>
        /// The instances
        /// </summary>
        private static int instances;

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether [accepts return].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [accepts return]; otherwise, <c>false</c>.
        /// </value>
        public bool AcceptsReturn
        {
            get
            {
                return this.entity.FindComponent<TextBoxBehavior>().AcceptsReturn;
            }

            set
            {
                this.entity.FindComponent<TextBoxBehavior>().AcceptsReturn = value;
            }
        }

        /// <summary>
        /// Gets the height of the line.
        /// </summary>
        /// <value>
        /// The height of the line.
        /// </value>
        public float LineHeight
        {
            get
            {
                return this.entity.FindChild("TextEntity").FindComponent<TextControl>().FontHeight;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is read only.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is read only; otherwise, <c>false</c>.
        /// </value>
        public bool IsReadOnly
        {
            get
            {
                return this.entity.FindComponent<TextBoxBehavior>().IsReadOnly;
            }

            set
            {
                this.entity.FindComponent<TextBoxBehavior>().IsReadOnly = value;
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
                return this.entity.FindComponent<PanelControl>().Margin;
            }

            set
            {
                this.entity.FindComponent<PanelControl>().Margin = value;
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
                this.entity.FindComponent<TextBoxBehavior>().UpdateText = value;
            }
        }

        /// <summary>
        /// Gets or sets the text alignment.
        /// </summary>
        /// <value>
        /// The text alignment.
        /// </value>
        public TextAlignment TextAlignment
        {
            get
            {
                return this.entity.FindChild("TextEntity").FindComponent<TextControl>().TextAlignment;
            }

            set
            {
                this.entity.FindChild("TextEntity").FindComponent<TextControl>().TextAlignment = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [text wrapping].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [text wrapping]; otherwise, <c>false</c>.
        /// </value>
        public bool TextWrapping
        {
            get
            {
                return this.entity.FindChild("TextEntity").FindComponent<TextControl>().TextWrapping;
            }

            set
            {
                this.entity.FindChild("TextEntity").FindComponent<TextControl>().TextWrapping = value;
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
        /// Gets or sets the background.
        /// </summary>
        /// <value>
        /// The background.
        /// </value>
        public Color Background
        {
            get
            {
                return this.entity.FindChild("ImageEntity").FindComponent<ImageControl>().TintColor;
            }

            set
            {
                this.entity.FindChild("ImageEntity").FindComponent<ImageControl>().TintColor = value;
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
                this.entity.FindChild("TextEntity").FindComponent<TextControl>().LineWidth = (int)value;
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
        #endregion

        #region Initialize

        /// <summary>
        /// Initializes a new instance of the <see cref="TextBox" /> class.
        /// </summary>
        public TextBox()
            : this("TextBox" + instances++)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TextBox" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public TextBox(string name)
        {
            this.entity = new Entity(name)
                                .AddComponent(new Transform2D())
                                .AddComponent(new TextBoxBehavior())
                                .AddComponent(new RectangleCollider())
                                .AddComponent(new TouchGestures())
                                .AddComponent(new PanelControl(100, 30))
                                .AddComponent(new PanelControlRenderer())
                                .AddChild(new Entity("ImageEntity")
                                    .AddComponent(new Transform2D()
                                    {
                                        DrawOrder = 0.55f
                                    })
                                    .AddComponent(new ImageControl(Color.White, 100, 30))
                                    .AddComponent(new ImageControlRenderer()))
                                .AddChild(new Entity("TextEntity")
                                    .AddComponent(new Transform2D()
                                    {
                                        DrawOrder = 0.4f
                                    })
                                    .AddComponent(new TextControl()
                                    {
                                        Text = "TextBox",
                                        Foreground = Color.Black
                                    })
                                    .AddComponent(new TextControlRenderer()))
                                .AddChild(new Entity("CursorEntity")
                                    .AddComponent(new Transform2D()
                                    {
                                        DrawOrder = 0.35f,
                                        Opacity = 0f
                                    })
                                    .AddComponent(new AnimationUI())
                                    .AddComponent(new ImageControl(Color.Black, 2, 30))
                                    .AddComponent(new ImageControlRenderer()));
        }
        #endregion

        #region Public Methods
        #endregion

        #region Private Methods
        #endregion
    }
}
