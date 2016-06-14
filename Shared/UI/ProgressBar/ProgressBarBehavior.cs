#region File Description
//-----------------------------------------------------------------------------
// ProgressBarBehavior
//
// Copyright © 2016 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
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
    /// The ProgressBar behavior
    /// </summary>
    public class ProgressBarBehavior : FocusBehavior
    {
        #region Constants

        /// <summary>
        /// The default unchecked image
        /// </summary>
        private const int DefaultProgressBarWeight = 20;

        #endregion

        /// <summary>
        /// Occurs when [value changed].
        /// </summary>
        public event ChangedEventHandler ValueChanged;

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
        /// The animation
        /// </summary>
        private AnimationUI animation;

        /// <summary>
        /// The move
        /// </summary>
        private SingleAnimation move;

        /// <summary>
        /// The duration
        /// </summary>
        private readonly Duration duration;

        #region Cached values
        /// <summary>
        /// The cached difference between maximun and minimun
        /// </summary>
        private int difference;

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

                if (this.isInitialized)
                {
                    this.UpdateDifference();
                }
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

                if (this.isInitialized)
                {
                    this.UpdateDifference();
                }
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

                if (this.isInitialized)
                {
                    this.UpdateValue(value);
                }
                else
                {
                    this.value = value;
                }
            }
        }

        /// <summary>
        /// Initializes the value.
        /// </summary>
        private void InitializeValue()
        {
            if (this.foregroundTransform != null)
            {
                // UpdateUI without animation
                this.foregroundTransform.XScale = this.Panel.Width * (this.value - this.minimum) / this.difference;
            }
        }

        /// <summary>
        /// Updates the value.
        /// </summary>
        /// <param name="value">The value.</param>
        private void UpdateValue(int value)
        {
            if (this.value != value)
            {
                int oldValue = this.value;
                this.value = value;

                // Event
                if (this.ValueChanged != null)
                {
                    this.ValueChanged(this, new ChangedEventArgs(oldValue, value));
                }

                // Update
                if (this.animation != null)
                {
                    // UpdateUI with animation
                    float convertionValue = this.Panel.Width * (value - this.minimum) / this.difference;
                    this.move = new SingleAnimation(this.foregroundTransform.XScale, convertionValue, this.duration);
                    this.animation.BeginAnimation(Transform2D.XScaleProperty, this.move);
                }
                else if (this.foregroundTransform != null)
                {
                    // UpdateUI without animation
                    this.foregroundTransform.XScale = this.Panel.Width * (value - this.minimum) / this.difference;
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
                if (this.backgroundImage != null)
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
                if (this.backgroundImage != null && this.foregroundImage != null)
                {
                    this.backgroundImage.Height = value;
                    this.foregroundImage.Height = value;
                }
            }
        }
        #endregion

        #region Initialize
        /// <summary>
        /// Initializes a new instance of the <see cref="ProgressBarBehavior" /> class.
        /// </summary>
        public ProgressBarBehavior()
            : base("ProgressBarBehavior")
        {
            this.maximum = 100;
            this.minimum = 0;
            this.value = this.minimum;

            this.duration = new Duration(TimeSpan.FromSeconds(.4f));

            ////this.move = new SingleAnimation(0, 100, this.duration);
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

            this.UpdateDifference();
            this.InitializeValue();
        }

        /// <summary>
        /// Resolves the dependencies needed for this instance to work.
        /// </summary>
        protected override void ResolveDependencies()
        {
            base.ResolveDependencies();

            Entity foregroundEntity = Owner.FindChild("ForegroundEntity");
            this.foregroundImage = foregroundEntity.FindComponent<ImageControl>();
            this.foregroundTransform = foregroundEntity.FindComponent<Transform2D>();

            this.foregroundImage.Height = this.Panel.Height;
            this.foregroundImage.Width = 1;
            this.foregroundTransform.XScale = this.Panel.Width * (this.value - this.minimum) / this.difference;

            this.backgroundImage = Owner.FindChild("BackgroundEntity").FindComponent<ImageControl>();

            this.backgroundImage.Width = this.Panel.Width;
            this.backgroundImage.Height = this.Panel.Height;

            this.animation = foregroundEntity.FindComponent<AnimationUI>();
        }

        /// <summary>
        /// Updates the difference.
        /// </summary>
        private void UpdateDifference()
        {
            if (this.maximum < this.minimum)
            {
                throw new ArgumentException(string.Format("Minimum: {0} can not be greather than Maximum: {1}", this.minimum, this.maximum));
            }

            this.difference = this.maximum - this.minimum;
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
