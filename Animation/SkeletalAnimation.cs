#region File Description
//-----------------------------------------------------------------------------
// SkeletalAnimation
//
// Copyright © 2010 - 2013 Wave Coorporation. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
<<<<<<< HEAD
=======
using WaveEngine.Common.Helpers;
>>>>>>> Added all files in Component library
using WaveEngine.Components.Animation.Spine;
using WaveEngine.Components.Graphics2D;
using WaveEngine.Framework;
#endregion

namespace WaveEngine.Components.Animation
{
    /// <summary>
    /// Behavior to control skeletal 2D animations
    /// </summary>
    public class SkeletalAnimation : Behavior
    {
        /// <summary>
<<<<<<< HEAD
=======
        /// Event raised when an animation has finalized.
        /// </summary>
        public event EventHandler<StringEventArgs> EndAnimation;

        /// <summary>
>>>>>>> Added all files in Component library
        /// The skeletal data
        /// </summary>
        [RequiredComponent]
        public SkeletalData SkeletalData;

        /// <summary>
        /// The skeleton
        /// </summary>
        public Skeleton Skeleton;

        /// <summary>
        /// The state
        /// </summary>
        private AnimationState state;

        /// <summary>
        /// The animation path
        /// </summary>
        private string animationPath;

        /// <summary>
        /// The current skin
        /// </summary>
        private string currentSkin;

        #region Properties

        /// <summary>
        /// Gets or sets the speed.
        /// </summary>
        /// <value>
        /// The speed.
        /// </value>
        public float Speed { get; set; }

        /// <summary>
        /// Gets or sets the current animation.
        /// </summary>
        /// <value>
        /// The current animation.
        /// </value>
        public string CurrentAnimation { get; set; }

        /// <summary>
        /// Gets or sets the state.
        /// </summary>
        /// <value>
        /// The state.
        /// </value>
        public AnimationState State
        {
<<<<<<< HEAD
            get { return this.state; }
            set { this.state = value; }
=======
            get 
            { 
                return this.state; 
            }

            set 
            {
                if (this.state != null)
                {
                    this.state.EndAnimation -= this.OnEndAnimation;
                }

                this.state = value;

                if (this.state != null)
                {
                    this.state.EndAnimation -= this.OnEndAnimation;
                    this.state.EndAnimation += this.OnEndAnimation;
                }
            }
>>>>>>> Added all files in Component library
        }

        /// <summary>
        /// Gets or sets the skin.
        /// </summary>
        /// <value>
        /// The skin.
        /// </value>
        public string Skin
        {
            set 
            {
                this.currentSkin = value;

                ////if (this.isInitialized)
                ////{
                ////    this.Skeleton.SetSkin(value);
                ////    this.Skeleton.SetSlotsToBindPose();
                ////}
            }

            get
            {
                return this.currentSkin;
            }
        }

        #endregion

        #region Initialize
        /// <summary>
        /// Initializes a new instance of the <see cref="SkeletalAnimation" /> class.
        /// </summary>
        /// <param name="animationPath">The animation path.</param>
        public SkeletalAnimation(string animationPath)
            : base("SkeletalAnimation")
        {
            this.animationPath = animationPath;
            this.Speed = 1;
        }
        #endregion

        #region Public Methods
<<<<<<< HEAD
=======

>>>>>>> Added all files in Component library
        /// <summary>
        /// Plays this instance.
        /// </summary>
        public void Play()
        {
<<<<<<< HEAD
            this.state.SetAnimation(this.CurrentAnimation, false);
=======
            this.state.SetAnimation(this.CurrentAnimation, false, 0, this.Skeleton);
        }

        /// <summary>
        /// Plays this instance.
        /// </summary>
        /// <param name="mixDuration">Mix duration.</param>
        public void Play(float mixDuration)
        {
            this.state.SetAnimation(this.CurrentAnimation, false, mixDuration, this.Skeleton);
        }

        /// <summary>
        /// Plays the specified loop.
        /// </summary>
        /// <param name="loop">if set to <c>true</c> [loop].</param>
        /// <param name="mixDuration">Mix duration.</param>
        public void Play(bool loop, float mixDuration)
        {
            this.state.SetAnimation(this.CurrentAnimation, loop, mixDuration, this.Skeleton);
>>>>>>> Added all files in Component library
        }

        /// <summary>
        /// Plays the specified loop.
        /// </summary>
        /// <param name="loop">if set to <c>true</c> [loop].</param>
        public void Play(bool loop)
        {
<<<<<<< HEAD
            this.state.SetAnimation(this.CurrentAnimation, loop);
=======
            this.state.SetAnimation(this.CurrentAnimation, loop, 0, this.Skeleton);
        }

        /// <summary>
        /// Search if the skeletal animation contains 
        /// </summary>
        /// <param name="animation">Animation name</param>
        /// <returns>Returns true if the skeletal animation contains the animation. False otherwise.</returns>
        public bool ContainsAnimation(string animation)
        {
            return this.state.Data.SkeletonData.FindAnimation(animation) != null;
        }

        /// <summary>
        /// Stops the current animation.
        /// </summary>
        public void Stop()
        {
            this.state.ClearAnimation();
            this.Skeleton.Update(0);
>>>>>>> Added all files in Component library
        }

        #endregion

        #region Private Methods
        /// <summary>
        /// Resolves the dependencies needed for this instance to work.
        /// </summary>
        protected override void ResolveDependencies()
        {
            base.ResolveDependencies();
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

            SkeletonJson json = new SkeletonJson(this.SkeletalData.Atlas);
            this.Skeleton = new Skeleton(json.ReadSkeletonData(this.animationPath));

            if (string.IsNullOrEmpty(this.currentSkin))
            {
                this.Skeleton.SetSkin(this.Skeleton.Data.DefaultSkin);
            }
            else
            {
                this.Skeleton.SetSkin(this.currentSkin);
                this.Skeleton.SetSlotsToBindPose();
            }

            AnimationStateData stateData = new AnimationStateData(this.Skeleton.Data);
            this.state = new AnimationState(stateData);
<<<<<<< HEAD
=======
            this.state.EndAnimation += this.OnEndAnimation;
        }

        /// <summary>
        /// Event handler of the end animation event.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Animation name.</param>
        private void OnEndAnimation(object sender, StringEventArgs e)
        {
            if (this.EndAnimation != null)
            {
                this.EndAnimation(sender, e);
            }
>>>>>>> Added all files in Component library
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
<<<<<<< HEAD
            this.state.Update(gameTime.Milliseconds / 1000f * this.Speed);
=======
            this.state.Update((float)gameTime.TotalSeconds * this.Speed);
>>>>>>> Added all files in Component library
            this.state.Apply(this.Skeleton);
            this.Skeleton.UpdateWorldTransform();
        }

        #endregion
    }
}
