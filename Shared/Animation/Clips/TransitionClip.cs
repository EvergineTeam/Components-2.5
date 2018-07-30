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
    /// A transition between two clips
    /// </summary>
    public class TransitionClip : BinaryAnimationBlendClip
    {
        /// <summary>
        /// The epsilon upper
        /// </summary>
        private const float UpperEpsilon = 1 - MathHelper.Epsilon;

        /// <summary>
        /// The duration
        /// </summary>
        private float duration;

        /// <summary>
        /// The playback rate
        /// </summary>
        private float playbackRate;

        /// <summary>
        /// The play time
        /// </summary>
        private float playTime;

        #region Properties

        /// <summary>
        /// Gets or sets the playback rate
        /// </summary>
        public override float PlaybackRate
        {
            get
            {
                return this.clipB.PlaybackRate;
            }

            set
            {
                this.playbackRate = Math.Abs(value);
                this.clipB.PlaybackRate = value;
            }
        }

        /// <summary>
        /// Gets the start frame
        /// </summary>
        public override float StartAnimationTime
        {
            get
            {
                return this.clipB.StartAnimationTime;
            }
        }

        /// <summary>
        /// Gets the en frame
        /// </summary>
        public override float EndAnimationTime
        {
            get
            {
                return this.clipB.EndAnimationTime;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the animation is looping
        /// </summary>
        public override bool Loop
        {
            get
            {
                return this.clipB.Loop;
            }

            set
            {
                this.clipB.Loop = value;
            }
        }

        /// <summary>
        /// Gets or sets the play time
        /// </summary>
        public override float PlayTime
        {
            get
            {
                return this.clipB.PlayTime;
            }

            set
            {
                this.clipB.PlayTime = value;
            }
        }

        /// <summary>
        /// Gets the duration
        /// </summary>
        public override float Duration
        {
            get
            {
                return this.clipB.Duration;
            }
        }

        /// <summary>
        /// Gets the frames per second of the clip
        /// </summary>
        public override float Framerate
        {
            get
            {
                return this.clipB.Framerate;
            }
        }
        #endregion

        #region Initialize

        /// <summary>
        /// Initializes a new instance of the <see cref="TransitionClip" /> class.
        /// </summary>
        /// <param name="clipA">The A clip</param>
        /// <param name="clipB">The B clip</param>
        /// <param name="duration">The transition duration</param>
        /// <param name="playbackRate">Playback rate</param>
        public TransitionClip(AnimationBlendClip clipA, AnimationBlendClip clipB, float duration, float playbackRate = 1)
            : base(clipA, clipB)
        {
            this.duration = duration;
            this.playbackRate = playbackRate;
            this.sample = new AnimationSample();
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// Gets the animation sample of this clip
        /// </summary>
        /// <returns>The animation sample</returns>
        protected override AnimationBlendClip UpdateBinaryClip()
        {
            this.playTime += (float)(clock.ElapseTime.TotalSeconds * this.playbackRate);
            var phase = this.playTime / this.duration;

            float lerp = Math.Max(Math.Min(phase, 1), 0);

            if (this.clipA != null)
            {
                this.clipA.Sample.Events.Clear();
                this.clipA.FinalListenKeyframeEvents = this.FinalListenKeyframeEvents && ((1 - lerp) >= this.ListenAnimationThreshold);
                var newClipA = this.clipA.UpdateClip();
                this.sampleA = this.clipA.Sample;
                this.clipA = newClipA;
            }

            if (this.clipB != null)
            {
                this.clipB.Sample.Events.Clear();
                this.clipB.FinalListenKeyframeEvents = this.FinalListenKeyframeEvents && (lerp >= this.ListenAnimationThreshold);
                var newClipB = this.clipB.UpdateClip();
                this.sampleB = this.clipB.Sample;
                this.clipB = newClipB;
            }

            if (lerp < MathHelper.Epsilon)
            {
                this.sample = this.sampleA;
            }
            else if (lerp > UpperEpsilon)
            {
                this.sample = this.sampleB;
            }
            else
            {
                this.binarySample.Lerp(this.clipA.Sample, this.clipB.Sample, lerp);
                this.sample = this.binarySample;
            }

            if (lerp == 1.0f)
            {
                return this.clipB;
            }
            else
            {
                return this;
            }
        }
        #endregion
    }
}
