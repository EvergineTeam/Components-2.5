#region File Description
//-----------------------------------------------------------------------------
// CheckBoxBehavior
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
using WaveEngine.Components.Gestures;
using WaveEngine.Framework;
using WaveEngine.Framework.Animation;
using WaveEngine.Framework.Graphics;
#endregion

namespace WaveEngine.Components.UI
{
    /// <summary>
    /// The CheckBoxBehavior class.
    /// </summary>
    public class CheckBoxBehavior : FocusBehavior
    {
        /// <summary>
        /// Occurs when [Checked].
        /// </summary>
        public event EventHandler CheckedChanged;

        /// <summary>
        /// The gestures
        /// </summary>
        [RequiredComponent]
        public TouchGestures Gestures;

        /// <summary>
        /// The animation
        /// </summary>
        public AnimationUI Animation;

        /// <summary>
        /// The fadein and fadeOut
        /// </summary>
        private SingleAnimation fadeIn, fadeOut;

        /// <summary>
        /// The image checked entity
        /// </summary>
        private Entity imageCheckedEntity;

        /// <summary>
        /// The image checked transform
        /// </summary>
        private Transform2D imageCheckedTransform;

        /// <summary>
        /// The is checked
        /// </summary>
        private bool isChecked;

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
                return this.isChecked;
            }

            set
            {
                // Assert
                if (this.isChecked == value)
                {
                    return;
                }

                this.isChecked = value;

                // UpdateUI
                if (this.Animation != null)
                {
                    if (this.isChecked)
                    {
                        this.Animation.BeginAnimation(Transform2D.OpacityProperty, this.fadeIn);
                    }
                    else
                    {
                        this.Animation.BeginAnimation(Transform2D.OpacityProperty, this.fadeOut);
                    }
                }
                else if (this.imageCheckedTransform != null)
                {
                    if (this.isChecked)
                    {
                        this.imageCheckedTransform.Opacity = 1.0f;
                    }
                    else
                    {
                        this.imageCheckedTransform.Opacity = 0.0f;
                    }
                }
            }
        }
        #endregion

        #region Initialize
        /// <summary>
        /// Initializes a new instance of the <see cref="CheckBoxBehavior" /> class.
        /// </summary>
        public CheckBoxBehavior()
            : base("CheckBehavior")
        {
            Duration duration = new Duration(TimeSpan.FromSeconds(.4f));
            this.fadeIn = new SingleAnimation(0, 1, duration);
            this.fadeOut = new SingleAnimation(1, 0, duration);
            this.isChecked = false;
        }
        #endregion

        #region Public Methods
        #endregion

        #region Private Methods
        /// <summary>
        /// Resolves the dependencies needed for this instance to work.
        /// </summary>
        protected override void ResolveDependencies()
        {
            base.ResolveDependencies();

            this.imageCheckedEntity = Owner.FindChild("ImageCheckedEntity");
            this.imageCheckedTransform = this.imageCheckedEntity.FindComponent<Transform2D>();
            this.Animation = this.imageCheckedEntity.FindComponent<AnimationUI>();

            // Initial configuration
            if (this.isChecked)
            {
                this.imageCheckedTransform.Opacity = 1.0f;
            }
            else
            {
                this.imageCheckedTransform.Opacity = 0.0f;
            }
        }

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
        /// Handles the TouchReleased event of the gestures control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="GestureEventArgs" /> instance containing the event data.</param>
        private void Gestures_TouchReleased(object sender, GestureEventArgs e)
        {
            this.IsFocus = true;

            if (this.isChecked)
            {
                this.Animation.BeginAnimation(Transform2D.OpacityProperty, this.fadeOut);
                this.isChecked = false;
            }
            else
            {
                this.Animation.BeginAnimation(Transform2D.OpacityProperty, this.fadeIn);
                this.isChecked = true;
            }

            if (this.CheckedChanged != null)
            {
                this.CheckedChanged(this, e);
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
