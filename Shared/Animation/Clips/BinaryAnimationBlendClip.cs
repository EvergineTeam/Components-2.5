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
    /// A transition between two clips
    /// </summary>
    public abstract class BinaryAnimationBlendClip : AnimationBlendClip
    {
        /// <summary>
        /// The first clip
        /// </summary>
        protected AnimationBlendClip clipA;

        /// <summary>
        /// The second clip
        /// </summary>
        protected AnimationBlendClip clipB;

        /// <summary>
        /// The sample A
        /// </summary>
        protected AnimationSample sampleA;

        /// <summary>
        /// The sample B
        /// </summary>
        protected AnimationSample sampleB;

        /// <summary>
        /// The binary sample instance
        /// </summary>
        protected BinaryAnimationSample binarySample;

        #region Properties

        /// <summary>
        /// Gets the Clip A
        /// </summary>
        public AnimationBlendClip ClipA => this.clipA;

        /// <summary>
        /// Gets the Clip B
        /// </summary>
        public AnimationBlendClip ClipB => this.clipB;

        /// <summary>
        /// Gets or sets a threshold to listen keyframe events
        /// </summary>
        /// <remarks>
        /// If the animation A or B has a weight greather than this threshold, the events are listened
        /// </remarks>
        public float ListenAnimationThreshold { get; set; }
        #endregion

        #region Initialize

        /// <summary>
        /// Initializes a new instance of the <see cref="BinaryAnimationBlendClip" /> class.
        /// </summary>
        /// <param name="clipA">The A clip</param>
        /// <param name="clipB">The B clip</param>
        public BinaryAnimationBlendClip(AnimationBlendClip clipA, AnimationBlendClip clipB)
        {
            if (clipA == null)
            {
                throw new ArgumentNullException("clipA");
            }

            if (clipB == null)
            {
                throw new ArgumentNullException("clipB");
            }

            this.clipA = clipA;
            this.clipB = clipB;

            this.ListenAnimationThreshold = 0;
            this.sample = this.binarySample = new BinaryAnimationSample();
        }
        #endregion

        #region Public Methods
        #endregion

        #region Private Methods

        /// <summary>
        /// Initialize this clip
        /// </summary>
        internal override protected void InitializeClip()
        {
            this.clipA.BaseInitializeClip(this.HierarchyMapping);
            this.clipB.BaseInitializeClip(this.HierarchyMapping);
        }

        /// <summary>
        /// Update the binary clip
        /// </summary>
        /// <returns>The blend clip</returns>
        protected abstract AnimationBlendClip UpdateBinaryClip();

        /// <summary>
        /// Gets the animation sample of this clip
        /// </summary>
        /// <returns>The animation sample</returns>
        public override AnimationBlendClip UpdateClip()
        {
            this.FinalListenKeyframeEvents = this.ListenKeyframeEvents;

            var clip = this.UpdateBinaryClip();

            this.FinalListenKeyframeEvents = true;

            return clip;
        }
        #endregion
    }
}
