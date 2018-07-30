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
    public class PartialBlendingClip : BinaryAnimationBlendClip
    {
        /// <summary>
        /// The jointName
        /// </summary>
        private string jointName;

        /// <summary>
        /// The max blend
        /// </summary>
        private float maxBlend;

        /// <summary>
        /// The amount of weight to increase
        /// </summary>
        private float jointBlendWeight;

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
        /// Gets or sets the play time
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
        /// Gets or sets the joint to be replaced
        /// </summary>
        public string ReplaceJointName
        {
            get
            {
                return this.jointName;
            }

            set
            {
                if (this.jointName != value)
                {
                    this.jointName = value;
                    this.RefreshWeights();
                }
            }
        }

        /// <summary>
        /// Gets or sets the amount of weight that a child will increase respect of its parent
        /// </summary>
        public float JointBlendWeight
        {
            get
            {
                return this.jointBlendWeight;
            }

            set
            {
                if (this.jointBlendWeight != value)
                {
                    this.jointBlendWeight = value;
                    this.RefreshWeights();
                }
            }
        }

        /// <summary>
        /// Gets or sets the max blend between the joints;
        /// </summary>
        public float MaxBlend
        {
            get
            {
                return this.maxBlend;
            }

            set
            {
                if (this.maxBlend != value)
                {
                    this.maxBlend = value;
                    this.RefreshWeights();
                }
            }
        }

        /// <summary>
        /// Gets or sets the joint blend weights
        /// </summary>
        public override float[] JointWeights
        {
            get
            {
                return this.jointWeights;
            }

            set
            {
                ////if (value != null && this.Skeleton != null)
                ////{
                ////    if (value.Length == this.Skeleton.JointCount)
                ////    {
                        this.jointWeights = value;

                        this.clipB.JointWeights = value;

                        var negativeWeights = new float[this.jointWeights.Length];
                        for (int i = 0; i < value.Length; i++)
                        {
                            negativeWeights[i] = 1 - value[i];
                        }

                        this.clipA.JointWeights = negativeWeights;
                ////    }
                ////}
            }
        }
        #endregion

        #region Initialize

        /// <summary>
        /// Initializes a new instance of the <see cref="PartialBlendingClip" /> class.
        /// </summary>
        /// <param name="clipA">The A clip</param>
        /// <param name="clipB">The B clip</param>
        /// <param name="replaceJoint">The joint</param>
        /// <param name="loop">The animation is looping</param>
        public PartialBlendingClip(AnimationBlendClip clipA, AnimationBlendClip clipB, string replaceJoint, bool loop = true)
            : base(clipA, clipB)
        {
            this.sample = new AnimationSample();
            this.Loop = loop;
            this.jointBlendWeight = 1;
            this.maxBlend = 1;
            this.jointName = replaceJoint;

            this.RefreshWeights();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PartialBlendingClip" /> class.
        /// </summary>
        /// <param name="clipA">The A clip</param>
        /// <param name="clipB">The B clip</param>
        /// <param name="jointWeights">The joint</param>
        /// <param name="loop">The animation is looping</param>
        public PartialBlendingClip(AnimationBlendClip clipA, AnimationBlendClip clipB, float[] jointWeights, bool loop = true)
            : base(clipA, clipB)
        {
            this.sample = new AnimationSample();
            this.Loop = loop;
            this.jointBlendWeight = 1;
            this.maxBlend = 1;
            this.jointName = null;

            this.JointWeights = jointWeights;
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// Gets the animation sample of this clip
        /// </summary>
        /// <returns>The animation sample</returns>
        public override AnimationBlendClip UpdateClip()
        {
            if (this.clipA != null)
            {
                var newClipA = this.clipA.UpdateClip();
                this.sampleA = this.clipA.Sample;
                this.clipA = newClipA;
            }

            if (this.clipB != null)
            {
                var newClipB = this.clipB.UpdateClip();
                this.sampleB = this.clipB.Sample;
                this.clipB = newClipB;
            }

            AnimationSample.Lerp(this.sampleA, this.sampleB, this.sample);

            this.ProcessKeyEvents();

            return this;
        }
        #endregion

        /// <summary>
        /// Refresh the blend weights
        /// </summary>
        private void RefreshWeights()
        {
            ////var skeleton = this.clipA.Skeleton;
            ////var weights = new float[skeleton.JointCount];

            ////if (!string.IsNullOrEmpty(this.jointName))
            ////{
            ////    this.jointIndex = skeleton.GetJointIndexByName(this.jointName);

            ////    if (this.jointIndex >= 0)
            ////    {
            ////        weights[this.jointIndex] = MathHelper.Clamp(this.JointBlendWeight, 0, this.MaxBlend);

            ////        for (int i = this.jointIndex + 1; i < weights.Length; i++)
            ////        {
            ////            int parentIndex = this.Skeleton.Joints[i].Parent;

            ////            float parentWeight = weights[parentIndex];
            ////            if (parentWeight > 0)
            ////            {
            ////                weights[i] = MathHelper.Clamp(parentWeight + this.JointBlendWeight, 0, this.MaxBlend);
            ////            }
            ////        }
            ////    }
            ////}

            ////this.JointWeights = weights;
        }
    }
}
