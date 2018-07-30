// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using WaveEngine.Common.Attributes;
using WaveEngine.Common.Helpers;
using WaveEngine.Common.Math;
using WaveEngine.Components.Graphics3D;
using WaveEngine.Framework;
using WaveEngine.Framework.Animation;
using WaveEngine.Framework.Diagnostic;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Graphics3D;
#endregion

namespace WaveEngine.Components.Animation
{
    /// <summary>
    ///     Behavior to controls the animations of a 3D model.
    /// </summary>
    /// <remarks>
    ///     Ideally this class should be used to hold all the animations related to a given 3D model.
    /// </remarks>
    [DataContract(Namespace = "WaveEngine.Components.Animation")]
    public class Animation3D : Behavior
    {
        /// <summary>
        /// The transform3D
        /// </summary>
        [RequiredComponent]
        private Transform3D transform3D = null;

        /// <summary>
        ///     Number of instances of this component created.
        /// </summary>
        private static int instances;

        /// <summary>
        /// The animation path.
        /// </summary>
        [DataMember]
        private string modelPath;

        /// <summary>
        /// The current animation.
        /// </summary>
        private string currentAnimation;

        /// <summary>
        /// The current animation clip
        /// </summary>
        private AnimationBlendClip clip;

        /// <summary>
        /// If the current animation is looped
        /// </summary>
        private bool loop;

        /// <summary>
        /// The internal animation.
        /// </summary>
        private InternalModel internalModel;

        /// <summary>
        /// Current animation track
        /// </summary>
        private AnimationClip currentAnimationTrack;

        /// <summary>
        /// The playback rate
        /// </summary>
        [DataMember]
        private float playbackRate;

        /// <summary>
        /// Needs update
        /// </summary>
        private bool needUpdate;

        /// <summary>
        /// An event fired when the animation is updated
        /// </summary>
        public event EventHandler<AnimationSample> AnimationUpdated;

        /// <summary>
        ///     Raised when a certain frame of an animation is played.
        /// </summary>
        public event EventHandler<AnimationKeyframeEvent> OnKeyFrameEvent;

        /// <summary>
        /// The hierarchy mapping
        /// </summary>
        private NodeHierarchyMapping hierarchyMapping;

        #region Properties

        /// <summary>
        /// Gets or sets the animation file path
        /// </summary>
        [RenderPropertyAsAsset(AssetType.Model)]
        public string ModelPath
        {
            get
            {
                return this.modelPath;
            }

            set
            {
                this.modelPath = value;

                if (this.isInitialized)
                {
                    this.RefreshAnimationAsset();
                }
            }
        }

        /// <summary>
        /// Gets the animation names.
        /// </summary>
        /// <value>
        /// The animation names.
        /// </value>
        [DontRenderProperty]
        public IEnumerable<string> AnimationNames
        {
            get
            {
                if (this.internalModel != null)
                {
                    return this.internalModel.Animations.Keys;
                }

                return new List<string>();
            }
        }

        /// <summary>
        /// Gets the current animation clip
        /// </summary>
        public AnimationBlendClip Clip => this.clip;

        /// <summary>
        ///     Gets or sets a value indicating whether the bounding box was refreshed.
        /// </summary>
        /// <value>
        ///     <c>true</c> if the bounding box was refreshed; otherwise, <c>false</c>.
        /// </value>
        [DontRenderProperty]
        public bool BoundingBoxRefreshed { get; set; }

        /// <summary>
        ///     Gets or sets the current active animation.
        /// </summary>
        /// <value>
        ///     The current animation.
        /// </value>
        [DataMember]
        [RenderPropertyAsSelector("AnimationNames")]
        public string CurrentAnimation
        {
            get
            {
                return this.currentAnimation;
            }

            set
            {
                if (this.currentAnimation != value)
                {
                    this.currentAnimation = value;

                    if (this.PlayAutomatically)
                    {
                        this.PlayAnimation(this.currentAnimation);
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether current active animation is looping.
        /// </summary>
        /// <value>
        ///     <c>true</c> if the animation is looping; otherwise, <c>false</c>.
        /// </value>
        [DontRenderProperty]
        public bool Loop
        {
            get
            {
                return this.loop;
            }

            set
            {
                if (this.loop != value)
                {
                    this.loop = value;

                    if (this.clip != null)
                    {
                        this.clip.Loop = value;
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the state of the current active animation.
        /// </summary>
        [DontRenderProperty]
        public AnimationState State { get; protected set; }

        /// <summary>
        /// Gets the current animation track
        /// </summary>
        public AnimationClip CurrentAnimationTrack => this.currentAnimationTrack;

        /// <summary>
        /// Gets or sets the current frame of the animation.
        /// </summary>
        [DontRenderProperty]
        public float Frame
        {
            get
            {
                var duration = this.Duration;

                if ((this.clip != null) && (duration > 0))
                {
                    float fps = this.clip.Framerate;

                    float frameF;

                    frameF = (this.PlayTime + duration) % duration;
                    if (frameF < 0)
                    {
                        frameF += this.Duration;
                    }

                    ////frameF += this.StartAnimationTime;

                    return frameF * fps;
                }
                else
                {
                    return 0;
                }
            }

            set
            {
                if (this.clip != null)
                {
                    float fps = this.clip.Framerate;

                    this.PlayTime = value / fps;
                }
            }
        }

        /// <summary>
        /// Gets or sets the current animation time.
        /// </summary>
        [DontRenderProperty]
        public float PlayTime
        {
            get
            {
                return (this.clip != null) ? this.clip.PlayTime : 0;
            }

            set
            {
                if (this.clip != null)
                {
                    this.clip.PlayTime = value;
                    this.needUpdate = true;
                }
            }
        }

        /// <summary>
        /// Gets the start animation time.
        /// </summary>
        public float StartAnimationTime
        {
            get
            {
                var trackClip = this.clip as AnimationTrackClip;
                return (trackClip != null) ? trackClip.StartAnimationTime : 0;
            }
        }

        /// <summary>
        /// Gets the end animation time.
        /// </summary>
        public float EndAnimationTime
        {
            get
            {
                var trackClip = this.clip as AnimationTrackClip;
                return (trackClip != null) ? trackClip.EndAnimationTime : this.Duration;
            }
        }

        /// <summary>
        /// Gets the duration time.
        /// </summary>
        public float Duration => (this.clip != null) ? this.clip.Duration : 0;

        /// <summary>
        ///     Gets or sets the playback rate.
        /// </summary>
        /// <value>
        ///     The speed of the animation.
        /// </value>
        public float PlaybackRate
        {
            get
            {
                return (this.clip != null) ? this.clip.PlaybackRate : this.playbackRate;
            }

            set
            {
                this.playbackRate = value;
                if (this.clip != null)
                {
                    this.clip.PlaybackRate = this.playbackRate;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the animation is played automatically when the CurrentAnimation
        /// </summary>
        /// <value>
        ///   <c>true</c> if [play automatically]; otherwise, <c>false</c>.
        /// </value>
        [DataMember]
        public bool PlayAutomatically { get; set; }

        /// <summary>
        ///     Gets the animation data.
        /// </summary>
        [DontRenderProperty]
        public InternalModel InternalModel => this.internalModel;

        /// <summary>
        /// Gets or sets a value indicating whether the root motion will be applied to this entity.
        /// </summary>
        [DataMember]
        public bool ApplyRootMotion { get; set; }
        #endregion

        #region Initialize

        /// <summary>
        /// Initializes a new instance of the <see cref="Animation3D"/> class.
        /// </summary>
        public Animation3D()
            : base("Animation3D" + instances)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Animation3D"/> class.
        /// </summary>
        /// <param name="animationPath">
        /// The path to the animation data.
        /// </param>
        public Animation3D(string animationPath)
            : this("Animation3D" + instances, animationPath)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Animation3D"/> class.
        /// </summary>
        /// <param name="name">
        /// The name of this behavior.
        /// </param>
        /// <param name="animationPath">
        /// The path to the animation data.
        /// </param>
        public Animation3D(string name, string animationPath)
            : base(name)
        {
            if (string.IsNullOrEmpty(animationPath))
            {
                throw new NullReferenceException("AnimationPath cannot be null.");
            }

            this.modelPath = animationPath;
        }

        /// <summary>
        /// The default values
        /// </summary>
        protected override void DefaultValues()
        {
            base.DefaultValues();

            this.playbackRate = 1;
            this.PlayTime = 0;
            this.Loop = true;
            this.ApplyRootMotion = true;

            instances++;
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// Gets the duration of an animation.
        /// </summary>
        /// <param name="animation">
        /// The animation name.
        /// </param>
        /// <returns>
        /// The duration of the animation
        /// </returns>
        public double GetDuration(string animation)
        {
            AnimationClip track;
            if (this.internalModel.Animations.TryGetValue(animation, out track))
            {
                return track.Duration;
            }
            else
            {
                return 0.0;
            }
        }

        /// <summary>
        /// Plays the animation between the specified frames.
        /// </summary>
        /// <param name="name">The name of the animation.</param>
        /// <param name="loop">Looping animation</param>
        /// <param name="transitionTime">The transition time</param>
        /// <param name="playbackRate">The playback rate</param>
        /// <param name="startTime">The frame where the animation starts playing.</param>
        /// <param name="endTime">The last frame of the animation to play.</param>
        public void PlayAnimation(string name, bool loop = true, float transitionTime = 0, float playbackRate = 1, float? startTime = null, float? endTime = null)
        {
            AnimationClip track;
            if (this.InternalModel != null && this.InternalModel.Animations.TryGetValue(name, out track))
            {
                this.BoundingBoxRefreshed = true;
                this.Loop = loop;
                this.State = AnimationState.Playing;
                this.playbackRate = playbackRate;

                if (transitionTime <= 0 || this.clip == null)
                {
                    this.clip = new AnimationTrackClip(track, loop, playbackRate, startTime, endTime);
                }
                else
                {
                    var clipB = new AnimationTrackClip(track, loop, playbackRate, startTime, endTime);
                    this.clip = new TransitionClip(this.clip, clipB, transitionTime);
                }

                this.clip.BaseInitializeClip(this.hierarchyMapping);
            }
        }

        /// <summary>
        /// Plays the animation between the specified frames.
        /// </summary>
        /// <param name="clip">The animation clip</param>
        /// <param name="transitionTime">The transition time</param>
        public void PlayAnimation(AnimationBlendClip clip, float transitionTime = 0)
        {
            this.BoundingBoxRefreshed = true;
            this.State = AnimationState.Playing;

            if (transitionTime <= 0 || this.clip == null)
            {
                this.clip = clip;
            }
            else
            {
                var clipA = this.clip;
                var clipB = clip;
                this.clip = new TransitionClip(clipA, clipB, transitionTime);
            }

            this.clip.BaseInitializeClip(this.hierarchyMapping);
        }

        /// <summary>
        /// Plays the animation between the specified frames.
        /// </summary>
        /// <param name="name">
        /// The name of the animation.
        /// </param>
        /// <param name="startTime">
        /// The frame where the animation starts playing.
        /// </param>
        /// <param name="endTime">
        /// The last frame of the animation to play.
        /// </param>
        /// <param name="loop">
        /// if set to <c>true</c> loop the animation.
        /// </param>
        /// <param name="playbackRate">
        /// the playback rate.
        /// </param>
        public void PlayAnimation(string name, int? startTime, int? endTime, bool loop = true, float playbackRate = 1)
        {
            AnimationClip track;
            if (this.InternalModel != null && this.InternalModel.Animations.TryGetValue(name, out track))
            {
                this.BoundingBoxRefreshed = true;
                this.State = AnimationState.Playing;
                this.playbackRate = playbackRate;

                var start = startTime ?? 0;
                var end = endTime ?? this.currentAnimationTrack.Duration;
                System.Diagnostics.Debug.WriteLine("Play " + track.Name + " " + track.Duration + " start: " + start + " end: " + end);
                this.clip = new AnimationTrackClip(track, loop, playbackRate, start, end);

                this.clip.BaseInitializeClip(this.hierarchyMapping);
            }
        }

        /////// <summary>
        /////// Plays the animation up to a given frame.
        /////// </summary>
        /////// <param name="name">
        /////// The name of the animation.
        /////// </param>
        /////// <param name="from">
        /////// The frame where the animation starts playing.
        /////// </param>
        /////// <param name="destFrame">
        /////// The last frame of the animation to play.
        /////// </param>
        /////// <param name="loop">
        /////// if set to <c>true</c> loop the animation.
        /////// </param>
        ////public override void PlayToFrame(string name, int? from, int? destFrame, bool loop = true)
        ////{
        ////    this.CurrentAnimation = name;

        ////    if (from.HasValue)
        ////    {
        ////        this.lastFrame = from.Value;
        ////    }
        ////    else if (this.State == AnimationState.Playing)
        ////    {
        ////        this.totalAnimTime = TimeSpan.Zero;
        ////        this.lastFrame = this.frame;
        ////    }

        ////    this.BoundingBoxRefreshed = true;
        ////    this.Loop = loop;
        ////    this.State = AnimationState.Playing;
        ////    this.targetFrame = destFrame.Value;

        ////    if (destFrame < this.lastFrame)
        ////    {
        ////        this.Backwards = true;
        ////    }
        ////    else
        ////    {
        ////        this.Backwards = false;
        ////    }
        ////}

        /// <summary>
        /// Resume the animation.
        /// </summary>
        public void ResumeAnimation()
        {
            this.State = AnimationState.Playing;
        }

        /// <summary>
        /// Stops the animation.
        /// </summary>
        public void StopAnimation()
        {
            this.State = AnimationState.Stopped;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Performs further custom initialization for this instance.
        /// </summary>
        protected override void Initialize()
        {
            ////this.Loop = false;
            this.RefreshAnimationAsset();
        }

        /// <summary>
        /// Refresh animation asset
        /// </summary>
        private void RefreshAnimationAsset()
        {
            if (this.internalModel != null && !string.IsNullOrEmpty(this.internalModel.AssetPath))
            {
                this.Assets.UnloadAsset(this.internalModel.AssetPath);
                this.internalModel = null;
            }

            if (!string.IsNullOrEmpty(this.modelPath))
            {
                this.internalModel = this.Assets.LoadAsset<InternalModel>(this.modelPath);

                if (this.internalModel != null)
                {
                    this.hierarchyMapping = new NodeHierarchyMapping(this.internalModel, this.Owner);
                }

                if (string.IsNullOrEmpty(this.CurrentAnimation))
                {
                    this.CurrentAnimation = this.internalModel.Animations.Keys.ToArray()[0];
                }

                this.RefreshAnimationTrack();
            }

            if (this.PlayAutomatically && !string.IsNullOrEmpty(this.CurrentAnimation))
            {
                this.PlayAnimation(this.CurrentAnimation, this.Loop);
            }
        }

        /// <summary>
        /// Updates the animation.
        /// </summary>
        /// <param name="gameTime">
        /// The game time.
        /// </param>
        protected override void Update(TimeSpan gameTime)
        {
            if (this.State == AnimationState.Playing || this.needUpdate)
            {
                ////Timers.BeginAveragedTimer("Update Clip");
                this.clip.FinalListenKeyframeEvents = this.OnKeyFrameEvent != null;
                this.clip = this.clip.UpdateClip();
                ////Timers.EndAveragedTimer("Update Clip");

                var sample = this.clip.Sample;

                sample?.ApplyPose();

                if (this.ApplyRootMotion)
                {
                    ////Labels.Add("PositionOffset", sample.PositionOffset);
                    ////Labels.Add("RotationOffset", Quaternion.ToEuler(sample.RotationOffset));
                    this.transform3D.LocalOrientation *= sample.RotationOffset;
                    var positionOffset = Vector3.TransformNormal(sample.PositionOffset, this.transform3D.LocalTransform);
                    this.transform3D.LocalPosition += positionOffset;
                    ////this.RenderManager.LineBatch3D.DrawAxis(this.transform3D.WorldTransform, 0.5f);
                }

                if (this.OnKeyFrameEvent != null)
                {
                    for (int i = 0; i < sample.Events.Count; i++)
                    {
                        this.OnKeyFrameEvent(this, sample.Events[i]);
                    }
                }

                this.AnimationUpdated?.Invoke(this, sample);
                this.needUpdate = false;
            }
        }

        /// <summary>
        /// The update num frames.
        /// </summary>
        private void RefreshAnimationTrack()
        {
            if (this.internalModel != null && this.internalModel.Animations.Count > 0)
            {
                this.internalModel.Animations.TryGetValue(this.CurrentAnimation, out this.currentAnimationTrack);
            }
        }
        #endregion
    }
}
