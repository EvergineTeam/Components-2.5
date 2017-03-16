#region File Description
//-----------------------------------------------------------------------------
// ButtonBehavior
//
// Copyright © 2017 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Runtime.Serialization;
using WaveEngine.Components.Gestures;
using WaveEngine.Framework;
using WaveEngine.Framework.Animation;
using WaveEngine.Framework.Graphics;

#endregion

namespace WaveEngine.Components.UI
{
    /// <summary>
    /// The ButtonBehavior class.
    /// </summary>
    public class ButtonBehavior : FocusBehavior
    {
        /// <summary>
        /// The transform
        /// </summary>
        [RequiredComponent]
        public Transform2D Transform;

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
        /// The text entity
        /// </summary>
        private Entity textEntity;

        /// <summary>
        /// Initializes a new instance of the <see cref="ButtonBehavior" /> class.
        /// </summary>
        public ButtonBehavior()
            : base("ButtonBehavior")
        {
        }

        /// <summary>
        /// Sets default values for this instance.
        /// </summary>
        protected override void DefaultValues()
        {
            base.DefaultValues();

            this.fadeIn = new SingleAnimation(0.2f, 1, new Duration(TimeSpan.FromSeconds(.4f)));
            this.fadeOut = new SingleAnimation(1, 0.2f, new Duration(TimeSpan.FromSeconds(.4f)));
        }

        /// <summary>
        /// Resolves the dependencies needed for this instance to work.
        /// </summary>
        protected override void ResolveDependencies()
        {
            base.ResolveDependencies();

            this.textEntity = Owner.FindChild("TextEntity");        
            this.Animation = this.textEntity.FindComponent<AnimationUI>();
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

            this.Gestures.TouchPressed -= this.Gestures_TouchPressed;
            this.Gestures.TouchPressed += this.Gestures_TouchPressed;
            this.Gestures.TouchReleased -= this.Gestures_TouchReleased;
            this.Gestures.TouchReleased += this.Gestures_TouchReleased;                    
        }

        /// <summary>
        /// Handles the TouchPressed event of the gestures control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="GestureEventArgs" /> instance containing the event data.</param>
        private void Gestures_TouchPressed(object sender, GestureEventArgs e)
        {
            this.IsFocus = true;
            this.Animation.BeginAnimation(Transform2D.OpacityProperty, this.fadeOut);
        }

        /// <summary>
        /// Handles the TouchReleased event of the gestures control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="GestureEventArgs" /> instance containing the event data.</param>
        private void Gestures_TouchReleased(object sender, GestureEventArgs e)
        {
            this.Animation.BeginAnimation(Transform2D.OpacityProperty, this.fadeIn);
        }

        /// <summary>
        /// Allows this instance to execute custom logic during its <c>Update</c>.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        /// <remarks>
        /// This method will not be executed if the <see cref="T:WaveEngine.Framework.Component" />, or the <see cref="T:WaveEngine.Framework.Entity" />
        /// owning it are not <c>Active</c>.
        /// </remarks>
        protected override void Update(TimeSpan gameTime)
        {
        }
    }
}
