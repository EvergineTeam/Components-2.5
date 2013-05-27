#region File Description
//-----------------------------------------------------------------------------
// TranslateTimeline
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
    /// TranslateTimeLine class
    /// </summary>
    public class TranslateTimeline : CurveTimeline
    {
        /// <summary>
        /// The LAST_FRAME_TIME
        /// </summary>
        protected static int lastFrameTime = -3;

        /// <summary>
        /// The FRAME_X
        /// </summary>
        protected static int frameX = 1;

        /// <summary>
        /// The FRAME_Y
        /// </summary>
        protected static int frameY = 2;

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
        public float[] Frames { get; private set; } // time, value, value, ...
        #endregion

        #region Initialize
        /// <summary>
        /// Initializes a new instance of the <see cref="TranslateTimeline" /> class.
        /// </summary>
        /// <param name="frameCount">The frame count.</param>
        public TranslateTimeline(int frameCount)
            : base(frameCount)
        {
            this.Frames = new float[frameCount * 3];
        }

        #endregion

        #region Public Methods
        /// <summary>
        /// Sets the time and value of the specified keyframe. 
        /// </summary>
        /// <param name="frameIndex">Index of the frame.</param>
        /// <param name="time">The time.</param>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        public void SetFrame(int frameIndex, float time, float x, float y)
        {
            frameIndex *= 3;
            this.Frames[frameIndex] = time;
            this.Frames[frameIndex + 1] = x;
            this.Frames[frameIndex + 2] = y;
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

            if (time >= frames[frames.Length - 3])
            {
                // Time is after last frame.
                bone.X += (bone.Data.X + frames[frames.Length - 2] - bone.X) * alpha;
                bone.Y += (bone.Data.Y + frames[frames.Length - 1] - bone.Y) * alpha;
                return;
            }

            // Interpolate between the last frame and the current frame.
            int frameIndex = Animation.BinarySearch(frames, time, 3);
            float lastFrameX = frames[frameIndex - 2];
            float lastFrameY = frames[frameIndex - 1];
            float frameTime = frames[frameIndex];
            float percent = 1 - ((time - frameTime) / (frames[frameIndex + lastFrameTime] - frameTime));
            percent = this.GetCurvePercent((frameIndex / 3) - 1, percent < 0 ? 0 : (percent > 1 ? 1 : percent));

            bone.X += (bone.Data.X + lastFrameX + ((frames[frameIndex + frameX] - lastFrameX) * percent) - bone.X) * alpha;
            bone.Y += (bone.Data.Y + lastFrameY + ((frames[frameIndex + frameY] - lastFrameY) * percent) - bone.Y) * alpha;
        }
        #endregion
    }
}
