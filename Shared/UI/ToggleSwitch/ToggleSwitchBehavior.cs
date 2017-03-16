#region File Description
//-----------------------------------------------------------------------------
// ToggleSwitchBehavior
//
// Copyright © 2017 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using WaveEngine.Components.Gestures;
using WaveEngine.Framework;
using WaveEngine.Framework.Animation;
using WaveEngine.Framework.Graphics;
#endregion

namespace WaveEngine.Components.UI
{
    /// <summary>
    /// The ToggleSwitch Behavior
    /// </summary>
    public class ToggleSwitchBehavior : FocusBehavior
    {
        #region Constants

        /// <summary>
        /// The default width
        /// </summary>
        private const int DefaultOffset = 30;

        #endregion

        /// <summary>
        /// Occurs when [toggled].
        /// </summary>
        public event EventHandler Toggled;

        /// <summary>
        /// The gestures
        /// </summary>
        [RequiredComponent]
        public TouchGestures Gestures;

        /// <summary>
        /// The foreground transform
        /// </summary>
        private Transform2D foregroundTransform;

        /// <summary>
        /// The bullet transform
        /// </summary>
        private Transform2D bulletTransform;

        /// <summary>
        /// The text control
        /// </summary>
        private TextControl textControl;

        /// <summary>
        /// The animations
        /// </summary>
        private AnimationUI bulletAnimation, foregroundAnimation;

        /// <summary>
        /// The animOn and animOff animations
        /// </summary>
        private SingleAnimation animOn, animOff;

        /// <summary>
        /// The on
        /// </summary>
        private bool on;

        /// <summary>
        /// The on text
        /// </summary>
        private string onText;

        /// <summary>
        /// The off text
        /// </summary>
        private string offText;

        #region Properties

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
                return this.on; 
            }

            set
            {
                if (this.on == value)
                {
                    return;
                }

                this.on = value;

                if (this.bulletAnimation != null && this.foregroundAnimation != null)
                {
                    if (this.on)
                    {
                        this.bulletAnimation.BeginAnimation(Transform2D.XProperty, this.animOn);
                        this.foregroundAnimation.BeginAnimation(Transform2D.XScaleProperty, this.animOn);
                        this.textControl.Text = this.onText;
                    }
                    else
                    {
                        this.bulletAnimation.BeginAnimation(Transform2D.XProperty, this.animOff);
                        this.foregroundAnimation.BeginAnimation(Transform2D.XScaleProperty, this.animOff);
                        this.textControl.Text = this.offText;
                    }
                }

                // Event
                if (this.Toggled != null)
                {
                    this.Toggled(this, new EventArgs());
                }
            }
        }

        /// <summary>
        /// Gets or sets the on text.
        /// </summary>
        /// <value>
        /// The on text.
        /// </value>
        public string OnText
        {
            get { return this.onText; }
            set { this.onText = value; }
        }

        /// <summary>
        /// Gets or sets the off text.
        /// </summary>
        /// <value>
        /// The off text.
        /// </value>
        public string OffText
        {
            get { return this.offText; }
            set { this.offText = value; }
        }
        #endregion

        #region Initialize
        /// <summary>
        /// Initializes a new instance of the <see cref="ToggleSwitchBehavior" /> class.
        /// </summary>
        public ToggleSwitchBehavior()
            : base("ToggleSwitchBehavior")
        {
            this.on = false;
            this.onText = "On";
            this.offText = "Off";

            Duration duration = new Duration(TimeSpan.FromSeconds(.4f));
            this.animOn = new SingleAnimation(0, DefaultOffset, duration);
            this.animOff = new SingleAnimation(DefaultOffset, 0, duration);
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

            this.Gestures.TouchReleased -= this.Gestures_TouchReleased;
            this.Gestures.TouchReleased += this.Gestures_TouchReleased;
        }

        /// <summary>
        /// Resolves the dependencies needed for this instance to work.
        /// </summary>
        protected override void ResolveDependencies()
        {
            base.ResolveDependencies();

            Entity foregroundEntity = Owner.FindChild("ForegroundEntity");
            this.foregroundTransform = foregroundEntity.FindComponent<Transform2D>();
            this.foregroundAnimation = foregroundEntity.FindComponent<AnimationUI>();

            Entity bulletEntity = Owner.FindChild("BulletEntity");
            this.bulletTransform = bulletEntity.FindComponent<Transform2D>();
            this.bulletAnimation = bulletEntity.FindComponent<AnimationUI>();

            this.textControl = Owner.FindChild("TextEntity").FindComponent<TextControl>();

            if (this.on)
            {
                this.bulletTransform.X = DefaultOffset;
                this.foregroundTransform.XScale = DefaultOffset;
                this.textControl.Text = this.onText;
            }
            else
            {
                this.bulletTransform.X = 0;
                this.foregroundTransform.XScale = 0;
                this.textControl.Text = this.offText;
            }
        }

        /// <summary>
        /// Handles the TouchReleased event of the Gestures control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="GestureEventArgs" /> instance containing the event data.</param>
        private void Gestures_TouchReleased(object sender, GestureEventArgs e)
        {
            this.IsFocus = true;

            // Switch
            this.on = !this.on;

            if (this.on)
            {
                this.bulletAnimation.BeginAnimation(Transform2D.XProperty, this.animOn);
                this.foregroundAnimation.BeginAnimation(Transform2D.XScaleProperty, this.animOn);
                this.textControl.Text = this.onText;
            }
            else
            {
                this.bulletAnimation.BeginAnimation(Transform2D.XProperty, this.animOff);
                this.foregroundAnimation.BeginAnimation(Transform2D.XScaleProperty, this.animOff);
                this.textControl.Text = this.offText;
            }

            // Event
            if (this.Toggled != null)
            {
                this.Toggled(this, new EventArgs());
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
