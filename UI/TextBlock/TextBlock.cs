#region File Description
//-----------------------------------------------------------------------------
// TextBlock
//
// Copyright © 2014 Wave Corporation
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WaveEngine.Common.Graphics;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.UI;
#endregion

namespace WaveEngine.Components.UI
{
    /// <summary>
    /// TextBlock decorate class
    /// </summary>
    public class TextBlock : UIBase
    {
        /// <summary>
        /// The instances
        /// </summary>
        private static int instances;

        #region Properties

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

        /// <summary>
        /// Gets or sets a value indicating whether [rich text enabled].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [rich text enabled]; otherwise, <c>false</c>.
        /// </value>
        public bool RichTextEnabled
        {
            get
            {
                return this.entity.FindChild("TextEntity").FindComponent<TextControl>().RichTextEnabled;
            }

            set
            {
                this.entity.FindChild("TextEntity").FindComponent<TextControl>().RichTextEnabled = value;
            }
        }

        /// <summary>
        /// Gets or sets the line spacing.
        /// </summary>
        /// <value>
        ///     The line spacing.
        /// </value>
        public float LineSpacing
        {
            get
            {
                return this.entity.FindChild("TextEntity").FindComponent<TextControl>().LineSpacing;
            }

            set
            {
                this.entity.FindChild("TextEntity").FindComponent<TextControl>().LineSpacing = value;
            }
        }

        #endregion

        #region Initialize

        /// <summary>
        /// Initializes a new instance of the <see cref="TextBlock" /> class.
        /// </summary>
        public TextBlock()
            : this("TextBlock" + instances++, DefaultLayers.GUI)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TextBlock" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public TextBlock(string name)
            : this(name, DefaultLayers.GUI)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TextBlock" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="layer">The layer.</param>
        public TextBlock(string name, Type layer)
        {
            this.entity = new Entity(name)
                                .AddComponent(new Transform2D())
                                .AddComponent(new PanelControl(100, 30))
                                .AddComponent(new PanelControlRenderer())
                                .AddChild(new Entity("TextEntity")
                                    .AddComponent(new Transform2D())
                                    .AddComponent(new TextControl()
                                    {
                                        Text = "TextBlock"
                                    })
                                    .AddComponent(new TextControlRenderer(layer)));
        }
        #endregion
    }
}
