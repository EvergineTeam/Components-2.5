// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Using Statements
using System;
using System.Collections.Generic;
using WaveEngine.Common.Math;
using WaveEngine.Framework.Animation;
using WaveEngine.Framework.Diagnostic;
using WaveEngine.Framework.Services;
#endregion

namespace WaveEngine.Components.Animation
{
    /// <summary>
    /// A syncrhonized transition between two clips
    /// </summary>
    public class SynchronizedTransitionClip : BinaryAnimationBlendClip
    {
        /// <summary>
        /// The epsilon upper
        /// </summary>
        private const float UpperEpsilon = 1 - MathHelper.Epsilon;

        /// <summary>
        /// The animation lerp
        /// </summary>
        private float lerp;

        /// <summary>
        /// The loop
        /// </summary>
        private bool loop;

        #region Properties

        /// <summary>
        /// Gets the start frame
        /// </summary>
        public override float StartAnimationTime
        {
            get
            {
                return 0;
            }
        }

        /// <summary>
        /// Gets the en frame
        /// </summary>
        public override float EndAnimationTime
        {
            get
            {
                return this.Duration;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the animation is looping
        /// </summary>
        public override bool Loop
        {
            get
            {
                return this.loop;
            }

            set
            {
                this.loop = value;

                if (this.clipA != null)
                {
                    this.clipA.Loop = value;
                }

                if (this.clipB != null)
                {
                    this.clipB.Loop = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the lerp
        /// </summary>
        public float Lerp
        {
            get
            {
                return this.lerp;
            }

            set
            {
                this.lerp = MathHelper.Clamp(value, 0, 1);
            }
        }

        /////// <summary>
        /////// Gets the associated skeleton
        /////// </summary>
        ////public override Skeleton Skeleton
        ////{
        ////    get
        ////    {
        ////        return this.clipA.Skeleton;
        ////    }
        ////}

        /// <summary>
        /// Gets or sets the play time
        /// </summary>
        public override float PlayTime
        {
            get
            {
               return MathHelper.Lerp(this.clipA.PlayTime, this.clipB.PlayTime, this.lerp);
            }

            set
            {
                var phase = value / this.Duration;
                this.clipA.Phase = phase;
                this.clipB.Phase = phase;
            }
        }

        /// <summary>
        /// Gets the duration
        /// </summary>
        public override float Duration
        {
            get
            {
                return MathHelper.Lerp(this.clipA.Duration, this.clipB.Duration, this.lerp);
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
                this.clipB.PlaybackRate = value;
            }
        }

        /// <summary>
        /// Gets the frames per second of the clip
        /// </summary>
        public override float Framerate
        {
            get
            {
                return MathHelper.Lerp(this.clipA.Framerate, this.clipB.Framerate, this.lerp);
            }
        }
        #endregion

        #region Initialize

        /// <summary>
        /// Initializes a new instance of the <see cref="SynchronizedTransitionClip" /> class.
        /// </summary>
        /// <param name="clipA">The A clip</param>
        /// <param name="clipB">The B clip</param>
        /// <param name="loop">The animation is looping</param>
        /// <param name="playbackRate">Playback rate</param>
        public SynchronizedTransitionClip(AnimationBlendClip clipA, AnimationBlendClip clipB, bool loop = true, float playbackRate = 1)
            : base(clipA, clipB)
        {
            this.PlaybackRate = playbackRate;
            this.sample = new AnimationSample();

            this.Phase = this.clipA.Phase;
            this.Loop = loop;

            this.lerp = 0;
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// Gets the animation sample of this clip
        /// </summary>
        /// <returns>The animation sample</returns>
        protected override AnimationBlendClip UpdateBinaryClip()
        {
            this.clipA.Sample.Events.Clear();
            this.clipB.Sample.Events.Clear();

            if (this.lerp < 1.0)
            {
                this.clipA.FinalListenKeyframeEvents = this.FinalListenKeyframeEvents && ((1 - this.lerp) >= this.ListenAnimationThreshold);

                this.clipA.Phase = this.Phase;
                var newClipA = this.clipA.UpdateClip();
                this.sampleA = this.clipA.Sample;
                this.clipA = newClipA;
            }

            if (this.lerp > 0.0)
            {
                this.clipB.FinalListenKeyframeEvents = this.FinalListenKeyframeEvents && (this.lerp >= this.ListenAnimationThreshold);

                this.clipB.Phase = this.Phase;
                var newClipB = this.clipB.UpdateClip();
                this.sampleB = this.clipB.Sample;
                this.clipB = newClipB;
            }

            if (this.lerp < MathHelper.Epsilon)
            {
                this.sample = this.sampleA;
            }
            else if (this.lerp > UpperEpsilon)
            {
                this.sample = this.sampleB;
            }
            else
            {
                this.binarySample.Lerp(this.clipA.Sample, this.clipB.Sample, this.lerp);
                this.sample = this.binarySample;
            }

            return this;
        }
        #endregion
    }
}
