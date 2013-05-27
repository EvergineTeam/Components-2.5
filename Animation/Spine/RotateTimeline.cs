#region File Description
//-----------------------------------------------------------------------------
// RotateTimeline
//
// Copyright (c) 2013, Esoteric Software
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions are met:
// 
// 1. Redistributions of source code must retain the above copyright notice, this
//    list of conditions and the following disclaimer.
// 2. Redistributions in binary form must reproduce the above copyright notice,
//    this list of conditions and the following disclaimer in the documentation
//    and/or other materials provided with the distribution.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR
// ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
// (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
// LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
// ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
// (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
// SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
//-----------------------------------------------------------------------------
#endregion

namespace WaveEngine.Components.Animation.Spine
{
    /// <summary>
    /// RotateTimeLine class
    /// </summary>
    public class RotateTimeline : CurveTimeline
    {
        /// <summary>
        /// The lastFrameTime
        /// </summary>
        protected static int lastFrameTime = -2;

        /// <summary>
        /// The frameValue
        /// </summary>
        protected static int frameValue = 1;

        #region Properties
        /// <summary>
        /// Gets or sets the index of the bone.
        /// </summary>
        /// <value>
        /// The index of the bone.
        /// </value>
        public int BoneIndex { get; set; }

        /// <summary>
        /// Gets the frames.
        /// </summary>
        /// <value>
        /// The frames.
        /// </value>
        public float[] Frames { get; private set; }
        #endregion

        #region Initialize
        /// <summary>
        /// Initializes a new instance of the <see cref="RotateTimeline" /> class.
        /// </summary>
        /// <param name="frameCount">The frame count.</param>
        public RotateTimeline(int frameCount)
            : base(frameCount)
        {
            this.Frames = new float[frameCount * 2];
        }

        #endregion

        #region Public Methods
        /// <summary>
        /// Sets the time and value of the specified keyframe.
        /// </summary>
        /// <param name="frameIndex">Index of the frame.</param>
        /// <param name="time">The time.</param>
        /// <param name="angle">The angle.</param>
        public void SetFrame(int frameIndex, float time, float angle)
        {
            frameIndex *= 2;
            this.Frames[frameIndex] = time;
            this.Frames[frameIndex + 1] = angle;
        }

        /// <summary>
        /// Sets the value(s) for the specified time.
        /// </summary>
        /// <param name="skeleton">The skeleton.</param>
        /// <param name="time">The time.</param>
        /// <param name="alpha">The alpha.</param>
        public override void Apply(Skeleton skeleton, float time, float alpha)
        {
            float[] frames = this.Frames;

            if (time < frames[0])
            {
                // Time is before first frame.
                return;
            }

            Bone bone = skeleton.Bones[this.BoneIndex];

            float amount;

            if (time >= frames[frames.Length - 2])
            {
                // Time is after last frame.
                amount = bone.Data.Rotation + frames[frames.Length - 1] - bone.Rotation;

                while (amount > 180)
                {
                    amount -= 360;
                }

                while (amount < -180)
                {
                    amount += 360;
                }

                bone.Rotation += amount * alpha;

                return;
            }

            // Interpolate between the last frame and the current frame.
            int frameIndex = Animation.BinarySearch(frames, time, 2);
            float lastFrameValue = frames[frameIndex - 1];
            float frameTime = frames[frameIndex];
            float percent = 1 - ((time - frameTime) / (frames[frameIndex + lastFrameTime] - frameTime));
            percent = this.GetCurvePercent((frameIndex / 2) - 1, percent < 0 ? 0 : (percent > 1 ? 1 : percent));

            amount = frames[frameIndex + frameValue] - lastFrameValue;
            while (amount > 180)
            {
                amount -= 360;
            }

            while (amount < -180)
            {
                amount += 360;
            }

            amount = bone.Data.Rotation + (lastFrameValue + (amount * percent)) - bone.Rotation;

            while (amount > 180)
            {
                amount -= 360;
            }

            while (amount < -180)
            {
                amount += 360;
            }

            bone.Rotation += amount * alpha;
        }
        #endregion
    }
}
