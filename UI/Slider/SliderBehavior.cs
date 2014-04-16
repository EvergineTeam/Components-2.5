#region File Description
//-----------------------------------------------------------------------------
// SliderBehavior
//
// Copyright © 2014 Wave Corporation
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using WaveEngine.Common.Math;
using WaveEngine.Components.Gestures;
using WaveEngine.Framework;
using WaveEngine.Framework.Animation;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.UI;
#endregion

namespace WaveEngine.Components.UI
{
    /// <summary>
    /// The SliderBehavior class.
    /// </summary>
    public class SliderBehavior : FocusBehavior
    {
        #region Constants
        /// <summary>
        /// The default unchecked image
        /// </summary>
        private const int DefaultSliderWeight = 20;

        /// <summary>
        /// The default text offset
        /// </summary>
        private const int DefaultTextOffset = 40;
        #endregion

        /// <summary>
        /// Occurs when [value changed].
        /// </summary>
        public event ChangedEventHandler ValueChanged;

        /// <summary>
        /// Occurs when [real time value changed].
        /// </summary>
        public event ChangedEventHandler RealTimeValueChanged;

        /// <summary>
        /// The panel
        /// </summary>
        [RequiredComponent]
        public PanelControl Panel;

        /// <summary>
        /// The gestures
        /// </summary>
        [RequiredComponent]
        public TouchGestures Gestures;

        /// <summary>
        /// The bullet image
        /// </summary>
        private ImageControl bulletImage;

        /// <summary>
        /// The foreground image
        /// </summary>
        private ImageControl foregroundImage;

        /// <summary>
        /// The foreground transform
        /// </summary>
        private Transform2D foregroundTransform;

        /// <summary>
        /// The background image
        /// </summary>
        private ImageControl backgroundImage;

        /// <summary>
        /// The bullet transform
        /// </summary>
        private Transform2D bulletTransform;

        /// <summary>
        /// The text control
        /// </summary>
        private TextControl textControl;

        /// <summary>
        /// The text transform
        /// </summary>
        private Transform2D textTransform;

        /// <summary>
        /// The orientation
        /// </summary>
        private Orientation orientation;

        /// <summary>
        /// The maximum value
        /// </summary>
        private int maximum;

        /// <summary>
        /// The minimum value
        /// </summary>
        private int minimum;

        /// <summary>
        /// The current value
        /// </summary>
        private int value;

        /// <summary>
        /// The fadein and fadeOut
        /// </summary>
        private SingleAnimation fadeIn, fadeOut;

        /// <summary>
        /// The animation
        /// </summary>
        private AnimationUI animation;

        #region Cached values
        /// <summary>
        /// The cached difference between maximun and minimun
        /// </summary>
        private int difference;

        /// <summary>
        /// The maximum offset
        /// </summary>
        private float maximunOffset;

        /// <summary>
        /// The maximun offset over2
        /// </summary>
        private float maximunOffsetOver2;

        /// <summary>
        /// The bullet with over2
        /// </summary>
        private float bulletWeightOver2;

        /// <summary>
        /// The old cached value
        /// </summary>
        private int oldCachedValue1, oldCachedValue2;
        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the maximum.
        /// </summary>
        /// <value>
        /// The maximum.
        /// </value>
        public int Maximum
        {
            get
            {
                return this.maximum;
            }

            set
            {
                this.maximum = value;
                this.UpdateDifference();
            }
        }

        /// <summary>
        /// Gets or sets the minimum.
        /// </summary>
        /// <value>
        /// The minimum.
        /// </value>
        public int Minimum
        {
            get
            {
                return this.minimum;
            }

            set
            {
                this.minimum = value;
                this.UpdateDifference();
            }
        }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public int Value
        {
            get
            {
                return this.value;
            }

            set
            {
                if (value < this.minimum || value > this.maximum)
                {
                    throw new ArgumentOutOfRangeException("the value must be between minimun:" + this.minimum + " and maximun:" + this.maximum);
                }

                if (this.value != value)
                {                    
                    this.value = value;

                    if (this.bulletTransform != null)
                    {
                        switch (this.orientation)
                        {
                            case Orientation.Vertical:
                                float result = this.maximunOffset * (value - this.minimum) / this.difference;
                                this.bulletTransform.Y = -result;
                                this.foregroundTransform.YScale = result;
                                break;
                            case Orientation.Horizontal:
                                this.bulletTransform.X = this.maximunOffset * (value - this.minimum) / this.difference;
                                this.foregroundTransform.XScale = this.bulletTransform.X;
                                break;
                        }
                    }

                    // Events

                    // Stable Change event
                    if (this.ValueChanged != null && this.oldCachedValue1 != this.value)
                    {
                        this.ValueChanged(this, new ChangedEventArgs(this.oldCachedValue1, value));
                        this.oldCachedValue1 = this.value;
                    }

                    // RealTime Change event
                    if (this.RealTimeValueChanged != null && this.oldCachedValue2 != this.value)
                    {
                        this.RealTimeValueChanged(this, new ChangedEventArgs(this.oldCachedValue2, this.value));
                        this.oldCachedValue2 = this.value;
                    }
                }
            }
        }

        /// <summary>
        /// Sets the width of the update.
        /// </summary>
        /// <value>
        /// The width of the update.
        /// </value>
        public float UpdateWidth
        {
            set
            {
                if (this.backgroundImage != null && this.orientation == Orientation.Horizontal)
                {
                    this.backgroundImage.Width = value;
                }
            }
        }

        /// <summary>
        /// Sets the height of the update.
        /// </summary>
        /// <value>
        /// The height of the update.
        /// </value>
        public float UpdateHeight
        {
            set
            {
                if (this.backgroundImage != null && this.orientation == Orientation.Vertical)
                {
                    this.backgroundImage.Height = value;
                }
            }
        }

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
                return this.orientation;
            }

            set
            {
                this.orientation = value;
                this.UpdateOrientation();
            }
        }

        #endregion

        #region Initialize
        /// <summary>
        /// Initializes a new instance of the <see cref="SliderBehavior" /> class.
        /// </summary>
        public SliderBehavior()
            : base("SliderBehavior")
        {
            this.maximum = 100;
            this.minimum = 0;
            this.difference = this.maximum - this.minimum;
            this.value = this.minimum;
            this.orientation = Orientation.Horizontal;

            Duration duration = new Duration(TimeSpan.FromSeconds(.4f));
            this.fadeIn = new SingleAnimation(0, 1, duration);
            this.fadeOut = new SingleAnimation(1, 0, duration);

            this.oldCachedValue1 = -1;
            this.oldCachedValue2 = -1;
        }
        #endregion

        #region Public Methods
        #endregion

        #region Private Methods

        /// <summary>
        /// Performs further custom initialization for this instance.
        /// </summary>
        /// <remarks>
        /// By default this method does nothing.
        /// </remarks>
        protected override void Initialize()
        {
            base.Initialize();

            this.Gestures.TouchPressed -= this.Gestures_TouchPressed;
            this.Gestures.TouchPressed += this.Gestures_TouchPressed;
            this.Gestures.TouchMoved -= this.Gestures_TouchMoved;
            this.Gestures.TouchMoved += this.Gestures_TouchMoved;
            this.Gestures.TouchReleased -= this.Gestures_TouchReleased;
            this.Gestures.TouchReleased += this.Gestures_TouchReleased;
        }

        /// <summary>
        /// Resolves the dependencies needed for this instance to work.
        /// </summary>
        protected override void ResolveDependencies()
        {
            base.ResolveDependencies();

            Entity bulletEntity = Owner.FindChild("BulletEntity");
            this.bulletTransform = bulletEntity.FindComponent<Transform2D>();
            this.bulletImage = bulletEntity.FindComponent<ImageControl>();
            this.bulletImage.Width = DefaultSliderWeight;
            this.bulletImage.Height = DefaultSliderWeight;

            Entity foregroundEntity = Owner.FindChild("ForegroundEntity");
            this.foregroundImage = foregroundEntity.FindComponent<ImageControl>();
            this.foregroundTransform = foregroundEntity.FindComponent<Transform2D>();

            this.backgroundImage = Owner.FindChild("BackgroundEntity").FindComponent<ImageControl>();

            Entity textEntity = Owner.FindChild("TextEntity");
            this.textControl = textEntity.FindComponent<TextControl>();
            this.textTransform = textEntity.FindComponent<Transform2D>();

            this.animation = textEntity.FindComponent<AnimationUI>();

            // Default parameters
            this.UpdateOrientation();

            // Initialization value
            switch (this.orientation)
            {
                case Orientation.Vertical:
                    float result = this.maximunOffset * (this.value - this.minimum) / this.difference;
                    this.bulletTransform.Y = -result;
                    this.foregroundTransform.YScale = result;
                    break;
                case Orientation.Horizontal:
                    this.bulletTransform.X = this.maximunOffset * (this.value - this.minimum) / this.difference;
                    this.foregroundTransform.XScale = this.bulletTransform.X;
                    break;
            }
        }

        /// <summary>
        /// Handles the TouchPressed event of the Gestures control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="GestureEventArgs" /> instance containing the event data.</param>
        private void Gestures_TouchPressed(object sender, GestureEventArgs e)
        {
            this.IsFocus = true;

            this.oldCachedValue1 = this.value;

            switch (this.orientation)
            {
                case Orientation.Vertical:
                    float offsetY = e.GestureSample.Position.Y - this.bulletTransform.Rectangle.Y;
                    this.UpdateWidthVerticalOffset(offsetY);

                    break;
                case Orientation.Horizontal:
                    float offsetX = e.GestureSample.Position.X - this.bulletTransform.Rectangle.X;
                    this.UpdateWidthHorizontalOffset(offsetX);

                    break;
            }

            this.animation.BeginAnimation(Transform2D.OpacityProperty, this.fadeIn);

            // RealTime Change event
            if (this.RealTimeValueChanged != null && this.oldCachedValue2 != this.value)
            {
                this.RealTimeValueChanged(this, new ChangedEventArgs(this.oldCachedValue2, this.value));
                this.oldCachedValue2 = this.value;
            }
        }

        /// <summary>
        /// Handles the TouchMoved event of the Gestures control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="GestureEventArgs" /> instance containing the event data.</param>
        private void Gestures_TouchMoved(object sender, GestureEventArgs e)
        {
            switch (this.orientation)
            {
                case Orientation.Vertical:
                    float offsetY = e.GestureSample.Position.Y - this.bulletTransform.Rectangle.Y;
                    this.UpdateWidthVerticalOffset(offsetY);

                    break;
                case Orientation.Horizontal:
                    float offsetX = e.GestureSample.Position.X - this.bulletTransform.Rectangle.X;
                    this.UpdateWidthHorizontalOffset(offsetX);

                    break;
            }

            // RealTime Change event
            if (this.RealTimeValueChanged != null  && this.oldCachedValue2 != this.value)
            {
                this.RealTimeValueChanged(this, new ChangedEventArgs(this.oldCachedValue2, this.value));
                this.oldCachedValue2 = this.value;
            }
        }

        /// <summary>
        /// Handles the TouchReleased event of the Gestures control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="GestureEventArgs" /> instance containing the event data.</param>
        private void Gestures_TouchReleased(object sender, GestureEventArgs e)
        {
            this.animation.BeginAnimation(Transform2D.OpacityProperty, this.fadeOut);

            // Stable Change event
            if (this.ValueChanged != null && this.oldCachedValue1 != this.value)
            {
                this.ValueChanged(this, new ChangedEventArgs(this.oldCachedValue1, this.value));
            }

            // RealTime Change event
            if (this.RealTimeValueChanged != null  && this.oldCachedValue2 != this.value)
            {
                this.RealTimeValueChanged(this, new ChangedEventArgs(this.oldCachedValue2, this.value));
                this.oldCachedValue2 = this.value;
            }
        }

        /// <summary>
        /// Updates the difference.
        /// </summary>
        private void UpdateDifference()
        {
            this.difference = this.maximum - this.minimum;
            this.UpdateValue();
        }

        /// <summary>
        /// Updates the value.
        /// </summary>
        private void UpdateValue()
        {
            if (this.bulletTransform != null)
            {
                switch (this.orientation)
                {
                    case Orientation.Vertical:
                        this.value = (int)(this.minimum + ((-this.bulletTransform.Y * this.difference) / this.maximunOffset));
                        break;
                    case Orientation.Horizontal:
                        this.value = (int)(this.minimum + ((this.bulletTransform.X * this.difference) / this.maximunOffset));
                        break;
                }
            }
            else
            {
                this.value = this.minimum;
            }

            if (this.textControl != null)
            {
                this.textControl.Text = this.value.ToString();
            }
        }

        /// <summary>
        /// Updates the width horizontal offset.
        /// </summary>
        /// <param name="offsetX">The offset X.</param>
        private void UpdateWidthHorizontalOffset(float offsetX)
        {
            float result = 0;

            if (offsetX < this.maximunOffsetOver2)
            {
                if (offsetX <= this.bulletWeightOver2)
                {
                    result = 0;
                }
                else
                {
                    result = offsetX - this.bulletWeightOver2;
                }
            }
            else
            {
                result = this.maximunOffset;
            }

            this.bulletTransform.X = result;
            this.textTransform.X = result;
            this.foregroundTransform.XScale = result;

            this.UpdateValue();
        }

        /// <summary>
        /// Updates the width vertical offset.
        /// </summary>
        /// <param name="offsetY">The offset Y.</param>
        public void UpdateWidthVerticalOffset(float offsetY)
        {
            float result = 0;
            if (offsetY > -this.maximunOffsetOver2)
            {
                if (offsetY < this.bulletWeightOver2)
                {
                    result = offsetY - this.bulletWeightOver2;
                }
                else
                {
                    result = 0;
                }
            }
            else
            {
                result = -this.maximunOffset;
            }

            this.bulletTransform.Y = result;
            this.textTransform.Y = result;
            this.foregroundTransform.YScale = -result;

            this.UpdateValue();
        }

        /// <summary>
        /// Updates the orientation.
        /// </summary>
        private void UpdateOrientation()
        {
            switch (this.orientation)
            {
                case Orientation.Vertical:

                    if (this.backgroundImage != null && this.foregroundImage != null && this.foregroundTransform != null
                        && this.bulletImage != null && this.bulletTransform != null && this.textControl != null &&
                        this.textTransform != null && this.Panel != null)
                    {
                        this.maximunOffset = this.Panel.Height - this.bulletImage.Height;
                        this.bulletWeightOver2 = this.bulletImage.Height / 2;
                        this.maximunOffsetOver2 = this.Panel.Height - this.bulletImage.Height - this.bulletWeightOver2;

                        this.backgroundImage.Width = DefaultSliderWeight;
                        this.backgroundImage.Height = this.Panel.Height;

                        this.foregroundImage.Width = DefaultSliderWeight;
                        this.foregroundImage.Height = 1;
                        this.foregroundImage.Margin = new Thickness(0, this.Panel.Height, 0, 0);

                        this.foregroundTransform.Origin = Vector2.UnitX / 2;
                        this.foregroundTransform.Rotation = MathHelper.Pi;
                        this.foregroundTransform.XScale = 1;

                        this.bulletImage.Margin = new Thickness(0, this.Panel.Height - this.bulletImage.Height, 0, 0);

                        this.bulletTransform.X = 0;

                        this.bulletTransform.Y = 0;

                        this.textControl.Margin = new Thickness(-DefaultTextOffset, this.Panel.Height - (this.bulletImage.Height * 1.5f), 0, 0);

                        this.textTransform.X = 0;
                    }

                    break;
                case Orientation.Horizontal:

                    if (this.backgroundImage != null && this.foregroundImage != null && this.foregroundTransform != null
                        && this.bulletImage != null && this.bulletTransform != null && this.textControl != null &&
                        this.textTransform != null && this.Panel != null)
                    {
                        this.maximunOffset = this.Panel.Width - this.bulletImage.Width;
                        this.bulletWeightOver2 = this.bulletImage.Width / 2;
                        this.maximunOffsetOver2 = this.Panel.Width - this.bulletWeightOver2;

                        this.backgroundImage.Width = this.Panel.Width;
                        this.backgroundImage.Height = DefaultSliderWeight;

                        this.foregroundImage.Height = DefaultSliderWeight;
                        this.foregroundImage.Width = 1;
                        this.foregroundImage.Margin = Thickness.Zero;

                        this.foregroundTransform.Origin = Vector2.Zero;
                        this.foregroundTransform.Rotation = 0;
                        this.foregroundTransform.YScale = 1;

                        this.bulletImage.Margin = Thickness.Zero;

                        this.bulletTransform.X = 0;

                        this.bulletTransform.Y = 0;

                        this.textControl.Margin = new Thickness(0, -DefaultTextOffset, 0, 0);

                        this.textTransform.Y = 0;
                    }

                    break;
            }
        }

        /// <summary>
        /// Allows this instance to execute custom logic during its <c>Update</c>.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        /// <remarks>
        /// This method will not be executed if the <see cref="Component" />, or the <see cref="Entity" />
        /// owning it are not <c>Active</c>.
        /// </remarks>
        protected override void Update(TimeSpan gameTime)
        {
        }

        #endregion
    }
}
