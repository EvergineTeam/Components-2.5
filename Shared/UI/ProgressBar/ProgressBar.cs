#region File Description
//-----------------------------------------------------------------------------
// ProgressBar
//
// Copyright © 2016 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
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
    /// Progress bar decorate class
    /// </summary>
    [DataContract(Namespace = "WaveEngine.Components.UI")]
    public class ProgressBar : UIBase
    {
        /// <summary>
        /// The instances
        /// </summary>
        private static int instances;

        /// <summary>
        /// Occurs when [value changed].
        /// </summary>
        public event ChangedEventHandler ValueChanged;

        #region Properties

        /// <summary>
        /// Gets or sets the maximum.
        /// </summary>
        /// <value>
        /// The maximum.
        /// </value>
        [DataMember]
        public int Maximum
        {
            get
            {
                return this.entity.FindComponent<ProgressBarBehavior>().Maximum;
            }

            set
            {
                this.entity.FindComponent<ProgressBarBehavior>().Maximum = value;
            }
        }

        /// <summary>
        /// Gets or sets the minimum.
        /// </summary>
        /// <value>
        /// The minimum.
        /// </value>
        [DataMember]
        public int Minimum
        {
            get
            {
                return this.entity.FindComponent<ProgressBarBehavior>().Minimum;
            }

            set
            {
                this.entity.FindComponent<ProgressBarBehavior>().Minimum = value;
            }
        }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        [DataMember]
        public int Value
        {
            get
            {
                return this.entity.FindComponent<ProgressBarBehavior>().Value;
            }

            set
            {
                this.entity.FindComponent<ProgressBarBehavior>().Value = value;
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
        [DataMember]
        public float Width
        {
            get
            {
                return this.entity.FindComponent<PanelControl>().Width;
            }

            set
            {
                this.entity.FindComponent<PanelControl>().Width = value;
                this.entity.FindComponent<ProgressBarBehavior>().UpdateWidth = value;
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
                return this.entity.FindComponent<PanelControl>().Height;
            }

            set
            {
                this.entity.FindComponent<PanelControl>().Height = value;
                this.entity.FindComponent<ProgressBarBehavior>().UpdateHeight = value;
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
        [DataMember]
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
        [DataMember]
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
        #endregion

        #region Initialize

        /// <summary>
        /// Initializes a new instance of the <see cref="ProgressBar" /> class.
        /// </summary>
        public ProgressBar()
            : this("ProgressBar" + instances++)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProgressBar" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public ProgressBar(string name)
        {
            this.entity = new Entity(name)
                           .AddComponent(new Transform2D())
                           .AddComponent(new RectangleCollider2D())
                           .AddComponent(new TouchGestures(false))
                           .AddComponent(new PanelControl(100, 20))
                           .AddComponent(new PanelControlRenderer())
                           .AddComponent(new ProgressBarBehavior())
                           .AddChild(new Entity("BackgroundEntity")
                                .AddComponent(new Transform2D()
                                {
                                    DrawOrder = 0.5f
                                })
                                .AddComponent(new ImageControl(Color.Blue, 1, 1))
                                .AddComponent(new ImageControlRenderer()))
                            .AddChild(new Entity("ForegroundEntity")
                                .AddComponent(new Transform2D()
                                {
                                    DrawOrder = 0.45f
                                })
                                .AddComponent(new AnimationUI())
                                .AddComponent(new ImageControl(Color.LightBlue, 1, 1))
                                .AddComponent(new ImageControlRenderer()));

            // Event
            this.entity.FindComponent<ProgressBarBehavior>().ValueChanged += this.ProgressBar_ValueChanged;
        }

        #endregion

        #region Public Methods
        #endregion

        #region Private Methods

        /// <summary>
        /// Handles the ValueChanged event of the ProgressBar control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="ChangedEventArgs" /> instance containing the event data.</param>
        private void ProgressBar_ValueChanged(object sender, ChangedEventArgs e)
        {
            if (this.ValueChanged != null)
            {
                this.ValueChanged(this, e);
            }
        }

        #endregion
    }
}
