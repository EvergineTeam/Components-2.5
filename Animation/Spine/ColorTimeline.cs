#region File Description
//-----------------------------------------------------------------------------
// ColorTimeline
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
    /// ColorTimeLine class
    /// </summary>
    public class ColorTimeline : CurveTimeline
    {
        /// <summary>
        /// The lastFrameTime
        /// </summary>
        protected static int lastFrameTime = -5;

        /// <summary>
        /// The frameR
        /// </summary>
        protected static int frameR = 1;

        /// <summary>
        /// The frameG
        /// </summary>
        protected static int frameG = 2;

        /// <summary>
        /// the frameB
        /// </summary>
        protected static int frameB = 3;

        /// <summary>
        /// The frameA
        /// </summary>
        protected static int frameA = 4;

        #region Properties
        /// <summary>
        /// Gets or sets the index of the slot.
        /// </summary>
        /// <value>
        /// The index of the slot.
        /// </value>
        public int SlotIndex { get; set; }

        /// <summary>
        /// Gets the frames.
        /// </summary>
        /// <value>
        /// The frames.
        /// </value>
        public float[] Frames { get; private set; } // time, r, g, b, a, ...
        #endregion

        #region Initialize
        /// <summary>
        /// Initializes a new instance of the <see cref="ColorTimeline" /> class.
        /// </summary>
        /// <param name="frameCount">The frame count.</param>
        public ColorTimeline(int frameCount)
            : base(frameCount)
        {
            this.Frames = new float[frameCount * 5];
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Sets the time and value of the specified keyframe. 
        /// </summary>
        /// <param name="frameIndex">Index of the frame.</param>
        /// <param name="time">The time.</param>
        /// <param name="r">The r.</param>
        /// <param name="g">The g.</param>
        /// <param name="b">The b.</param>
        /// <param name="a">The a.</param>
        public void SetFrame(int frameIndex, float time, float r, float g, float b, float a)
        {
            frameIndex *= 5;
            this.Frames[frameIndex] = time;
            this.Frames[frameIndex + 1] = r;
            this.Frames[frameIndex + 2] = g;
            this.Frames[frameIndex + 3] = b;
            this.Frames[frameIndex + 4] = a;
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

            Slot slot = skeleton.Slots[this.SlotIndex];

            if (time >= frames[frames.Length - 5])
            {
                // Time is after last frame.
                int i = frames.Length - 1;
                slot.R = frames[i - 3];
                slot.G = frames[i - 2];
                slot.B = frames[i - 1];
                slot.A = frames[i];

                return;
            }

            // Interpolate between the last frame and the current frame.
            int frameIndex = Animation.BinarySearch(frames, time, 5);
            float lastFrameR = frames[frameIndex - 4];
            float lastFrameG = frames[frameIndex - 3];
            float lastFrameB = frames[frameIndex - 2];
            float lastFrameA = frames[frameIndex - 1];
            float frameTime = frames[frameIndex];
            float percent = 1 - ((time - frameTime) / (frames[(frameIndex + lastFrameTime)] - frameTime));
            percent = this.GetCurvePercent((frameIndex / 5) - 1, percent < 0 ? 0 : (percent > 1 ? 1 : percent));

            float r = lastFrameR + ((frames[frameIndex + frameR] - lastFrameR) * percent);
            float g = lastFrameG + ((frames[frameIndex + frameG] - lastFrameG) * percent);
            float b = lastFrameB + ((frames[frameIndex + frameB] - lastFrameB) * percent);
            float a = lastFrameA + ((frames[frameIndex + frameA] - lastFrameA) * percent);

            if (alpha < 1)
            {
                slot.R += (r - slot.R) * alpha;
                slot.G += (g - slot.G) * alpha;
                slot.B += (b - slot.B) * alpha;
                slot.A += (a - slot.A) * alpha;
            }
            else
            {
                slot.R = r;
                slot.G = g;
                slot.B = b;
                slot.A = a;
            }
        }
        #endregion
    }
}
