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
            get { return this.state; }
            set { this.state = value; }
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
        /// <summary>
        /// Plays this instance.
        /// </summary>
        public void Play()
        {
            this.state.SetAnimation(this.CurrentAnimation, false);
        }

        /// <summary>
        /// Plays the specified loop.
        /// </summary>
        /// <param name="loop">if set to <c>true</c> [loop].</param>
        public void Play(bool loop)
        {
            this.state.SetAnimation(this.CurrentAnimation, loop);
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
            this.state.Update(gameTime.Milliseconds / 1000f * this.Speed);
            this.state.Apply(this.Skeleton);
            this.Skeleton.UpdateWorldTransform();
        }

        #endregion
    }
}
