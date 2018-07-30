// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Using Statements
using System;
using System.Collections.Generic;
using WaveEngine.Common.Math;
using WaveEngine.Framework.Animation;
using WaveEngine.Framework.Services;
#endregion

namespace WaveEngine.Components.Animation
{
    /// <summary>
    /// An animation clip
    /// </summary>
    public abstract class AnimationBlendClip
    {
        /// <summary>
        /// The clock service
        /// </summary>
        protected static Clock clock = WaveServices.Clock;

        /// <summary>
        /// The animation sample
        /// </summary>
        protected AnimationSample sample;

        /// <summary>
        /// The hierarchy mapping
        /// </summary>
        public NodeHierarchyMapping HierarchyMapping;

        /// <summary>
        /// Listen keyframe events
        /// </summary>
        public bool ListenKeyframeEvents = true;

        /// <summary>
        /// Listen keyframe events
        /// </summary>
        internal bool FinalListenKeyframeEvents;

        private bool isInitialized;

        #region Properties

        /// <summary>
        /// Gets or sets the global start time
        /// </summary>
        public float GlobalStartTime
        {
            get
            {
                return (float)AnimationBlendClip.clock.TotalTime.TotalSeconds - this.PlayTime;
            }

            set
            {
                this.PlayTime = (float)AnimationBlendClip.clock.TotalTime.TotalSeconds + value;
            }
        }

        /// <summary>
        /// Gets the start frame
        /// </summary>
        public abstract float StartAnimationTime
        {
            get;
        }

        /// <summary>
        /// Gets the en frame
        /// </summary>
        public abstract float EndAnimationTime
        {
            get;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the animation is looping
        /// </summary>
        public abstract bool Loop
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the current time
        /// </summary>
        public abstract float PlayTime
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the clip duration
        /// </summary>
        public abstract float Duration
        {
            get;
        }

        /// <summary>
        /// Gets the frames per second of the clip
        /// </summary>
        public abstract float Framerate
        {
            get;
        }

        /// <summary>
        /// Gets or sets the current frame of this clip.
        /// </summary>
        public virtual float Frame
        {
            get
            {
                return this.PlayTime * this.Framerate;
            }

            set
            {
                this.PlayTime = value / this.Framerate;
            }
        }

        /// <summary>
        /// Gets or sets the normalized time [0, 1], where 1 is the end of animation clip.
        /// </summary>
        public virtual float Phase
        {
            get
            {
                return this.PlayTime / this.Duration;
            }

            set
            {
                this.PlayTime = value * this.Duration;
            }
        }

        /// <summary>
        /// Gets or sets the playback rate
        /// </summary>
        public abstract float PlaybackRate
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the animation sample
        /// </summary>
        public virtual AnimationSample Sample => this.sample;
        #endregion

        #region Public Methods

        /// <summary>
        /// Gets the animation sample of this clip
        /// </summary>
        /// <returns>The animation sample</returns>
        public abstract AnimationBlendClip UpdateClip();

        /// <summary>
        /// Base initialzie clip
        /// </summary>
        /// <param name="hierarchyMapping">The hierarchy mapping</param>
        internal void BaseInitializeClip(NodeHierarchyMapping hierarchyMapping)
        {
            if (!this.isInitialized)
            {
                this.HierarchyMapping = hierarchyMapping;
                this.InitializeClip();
                this.isInitialized = true;
            }
        }

        /// <summary>
        /// Initialize this clip
        /// </summary>
        internal abstract protected void InitializeClip();
        #endregion
    }
}
