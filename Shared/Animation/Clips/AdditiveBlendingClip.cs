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
    /// Blend joints between two clips using a mask
    /// </summary>
    public class AdditiveBlendingClip : BinaryAnimationBlendClip
    {
        #region Properties

        /// <summary>
        /// Gets the start frame
        /// </summary>
        public override float StartAnimationTime
        {
            get
            {
                return this.clipA.StartAnimationTime;
            }
        }

        /// <summary>
        /// Gets the en frame
        /// </summary>
        public override float EndAnimationTime
        {
            get
            {
                return this.clipA.EndAnimationTime;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the animation is looping
        /// </summary>
        public override bool Loop
        {
            get
            {
                return this.clipA.Loop;
            }

            set
            {
                this.clipA.Loop = value;
            }
        }

        /// <summary>
        /// Gets or sets the playback rate
        /// </summary>
        public override float PlaybackRate
        {
            get
            {
                return this.clipA.PlaybackRate;
            }

            set
            {
                this.clipA.PlaybackRate = value;
            }
        }

        /// <summary>
        /// Gets or sets the duration
        /// </summary>
        public override float PlayTime
        {
            get
            {
                return this.clipA.PlayTime;
            }

            set
            {
                this.clipA.PlayTime = value;
            }
        }

        /// <summary>
        /// Gets the duration
        /// </summary>
        public override float Duration
        {
            get
            {
                return this.clipA.Duration;
            }
        }

        /// <summary>
        /// Gets the frames per second of the clip
        /// </summary>
        public override float Framerate
        {
            get
            {
                return this.clipA.Framerate;
            }
        }

        /// <summary>
        /// Gets or sets the blend factor
        /// </summary>
        public float BlendFactor { get; set; }
        #endregion

        #region Initialize

        /// <summary>
        /// Initializes a new instance of the <see cref="AdditiveBlendingClip" /> class.
        /// </summary>
        /// <param name="clipA">The A clip</param>
        /// <param name="clipB">The B clip</param>
        /// <param name="loop">The animation is looping</param>
        /// <param name="blendFactor">The blend factor</param>
        public AdditiveBlendingClip(AnimationBlendClip clipA, AnimationBlendClip clipB, bool loop = true, float blendFactor = 1)
            : base(clipA, clipB)
        {
            this.sample = new AnimationSample();
            this.Loop = loop;
            this.BlendFactor = 1;
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// Gets the animation sample of this clip
        /// </summary>
        /// <returns>The animation sample</returns>
        protected override AnimationBlendClip UpdateBinaryClip()
        {
            if (this.clipA != null)
            {
                this.clipA.FinalListenKeyframeEvents = this.FinalListenKeyframeEvents;

                var newClipA = this.clipA.UpdateClip();
                this.sampleA = this.clipA.Sample;
                this.clipA = newClipA;
            }

            if (this.BlendFactor != 0)
            {
                if (this.clipB != null)
                {
                    this.clipB.Sample.Events.Clear();
                    this.clipB.FinalListenKeyframeEvents = this.FinalListenKeyframeEvents && (this.BlendFactor > this.ListenAnimationThreshold);

                    var newClipB = this.clipB.UpdateClip();
                    this.sampleB = this.clipB.Sample;
                    this.clipB = newClipB;
                }

                this.binarySample.Add(this.clipA.Sample, this.clipB.Sample, this.BlendFactor);
                this.sample = this.binarySample;
            }
            else
            {
                this.sample = this.sampleA;
            }

            return this;
        }
        #endregion
    }
}
