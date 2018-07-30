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
    /// An animation track clip
    /// </summary>
    public class AnimationTrackClip : AnimationBlendClip
    {
        /// <summary>
        /// The current play time
        /// </summary>
        private float playTime;

        /// <summary>
        /// The playback rate
        /// </summary>
        private float playbackRate;

        /// <summary>
        /// The animation track
        /// </summary>
        private AnimationClip track;

        /// <summary>
        /// The start time
        /// </summary>
        private float startTime;

        /// <summary>
        /// The end time
        /// </summary>
        private float endTime;

        /// <summary>
        /// Plays a looping animation
        /// </summary>
        private bool loop;

        /// <summary>
        /// The current execution context
        /// </summary>
        private AnimationTrackContext context;

        /// <summary>
        /// Gets the duration
        /// </summary>
        private float duration;

        #region Properties

        /// <summary>
        /// Gets the animation track
        /// </summary>
        public AnimationClip Track
        {
            get
            {
                return this.track;
            }
        }

        /// <summary>
        /// Gets the start animation time
        /// </summary>
        public override float StartAnimationTime
        {
            get
            {
                return this.startTime;
            }
        }

        /// <summary>
        /// Gets the start animation time
        /// </summary>
        public override float EndAnimationTime
        {
            get
            {
                return this.endTime;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the animation has looping
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
            }
        }

        /// <summary>
        /// Gets or sets the play time
        /// </summary>
        public override float PlayTime
        {
            get
            {
                return this.playTime;
            }

            set
            {
                this.playTime = value;
            }
        }

        /// <summary>
        /// Gets the duration
        /// </summary>
        public override float Duration
        {
            get
            {
                return this.duration;
            }
        }

        /// <summary>
        /// Gets or sets the playback rate
        /// </summary>
        public override float PlaybackRate
        {
            get
            {
                return this.playbackRate;
            }

            set
            {
                this.playbackRate = value;
            }
        }

        /// <summary>
        /// Gets the frames per second of the clip
        /// </summary>
        public override float Framerate
        {
            get
            {
                return this.track.Framerate;
            }
        }
        #endregion

        #region Initialize

        /// <summary>
        /// Initializes a new instance of the <see cref="AnimationTrackClip" /> class.
        /// </summary>
        /// <param name="track">The animation track to reproduce</param>
        /// <param name="looping">Looping animation</param>
        /// <param name="playbackRate">The playback rate</param>
        /// <param name="startTime">The start time</param>
        /// <param name="endTime">The end time</param>
        public AnimationTrackClip(AnimationClip track, bool looping = false, float playbackRate = 1, float? startTime = null, float? endTime = null)
        {
            if (track == null)
            {
                throw new ArgumentNullException("track");
            }

            this.track = track;
            this.loop = looping;
            this.playbackRate = playbackRate;

            this.UpdateAnimationRange(startTime, endTime);

            this.track.Initialize();

            this.sample = new AnimationSample();
            this.context = new AnimationTrackContext();
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// Update the animation range
        /// </summary>
        /// <param name="startTime">The start time</param>
        /// <param name="endTime">The end time</param>
        public void UpdateAnimationRange(float? startTime, float? endTime)
        {
            if (this.track == null)
            {
                return;
            }

            this.startTime = startTime.HasValue ? startTime.Value : 0;
            this.endTime = endTime.HasValue ? endTime.Value : this.track.Duration;
            this.duration = this.endTime - this.startTime;
        }

        /// <summary>
        /// Gets the animation sample of this clip
        /// </summary>
        /// <returns>The animation sample</returns>
        public override AnimationBlendClip UpdateClip()
        {
            float time;

            if (this.duration > 0)
            {
                if (this.loop)
                {
                    time = ((this.playTime + this.duration) % this.duration) + this.startTime;
                    if (time < 0)
                    {
                        time += this.duration;
                    }
                }
                else
                {
                    time = Math.Min(this.playTime, this.endTime) + this.startTime;
                }
            }
            else
            {
                time = this.startTime;
            }

            if (!this.context.Time.HasValue || (Math.Abs(time - this.context.Time.Value) > MathHelper.Epsilon))
            {
                var phase = this.Phase;
                int loopCount = (int)phase;
                if (phase < 0)
                {
                    loopCount--;
                }

                this.FinalListenKeyframeEvents &= this.ListenKeyframeEvents;

                this.context.ListenKeyframeEvents = this.FinalListenKeyframeEvents;

                this.track.GetSample(time, this.sample, this.context, this.playbackRate, loopCount);

                this.FinalListenKeyframeEvents = true;
            }

            this.playTime += (float)(clock.ElapseTime.TotalSeconds * this.playbackRate);

            return this;
        }

        /// <summary>
        /// Initialize this clip
        /// </summary>
        internal override protected void InitializeClip()
        {
            if (this.context.Nodes != this.HierarchyMapping.Entities)
            {
                this.context.Nodes = this.HierarchyMapping.Entities;
                this.context.Initialize(this.track);
            }

            Array.Resize(ref this.sample.Poses, this.track.Channels.Count);

            if (this.track.Events.Count > 0)
            {
                this.sample.Events = new List<AnimationKeyframeEvent>();
            }

            for (int i = 0; i < this.track.Channels.Count; i++)
            {
                var channel = this.track.Channels[i];
                var channelContext = this.context.ChannelsContext[i];

                var animPose = default(AnimationSample.AnimationChannelPose);

                animPose.Key.Init(channelContext.TargetComponent, channel.PropertyUpdater);
                animPose.Channel = channel;
                animPose.Evaluator = channel?.Curve?.Evaluator;
                animPose.RefValue = new Lazy<object>(() =>
                {
                    return channel.PropertyUpdater.GetValue(channelContext.TargetComponent);
                });

                this.sample.Poses[i] = animPose;
            }
        }

        #endregion
    }
}
