#region File Description
//-----------------------------------------------------------------------------
// ScaleTimeline
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
    /// ScaleTimeLine class
    /// </summary>
    public class ScaleTimeline : TranslateTimeline
    {
        #region Initialize
        /// <summary>
        /// Initializes a new instance of the <see cref="ScaleTimeline" /> class.
        /// </summary>
        /// <param name="frameCount">The frame count.</param>
        public ScaleTimeline(int frameCount)
            : base(frameCount)
        {
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Sets the value(s) for the specified time.
        /// </summary>
        /// <param name="skeleton">The skeleton.</param>
        /// <param name="time">The time.</param>
        /// <param name="alpha">The alpha.</param>
        public override void Apply(Skeleton skeleton, float time, float alpha)
        {
            float[] frames = Frames;
            if (time < frames[0])
            {
                // Time is before first frame.
                return; 
            }

            Bone bone = skeleton.Bones[BoneIndex];
            if (time >= frames[frames.Length - 3])
            { 
                // Time is after last frame.
                bone.ScaleX += (bone.Data.ScaleX - 1 + frames[frames.Length - 2] - bone.ScaleX) * alpha;
                bone.ScaleY += (bone.Data.ScaleY - 1 + frames[frames.Length - 1] - bone.ScaleY) * alpha;

                return;
            }

            // Interpolate between the last frame and the current frame.
            int frameIndex = Animation.BinarySearch(frames, time, 3);
            float lastFrameX = frames[frameIndex - 2];
            float lastFrameY = frames[frameIndex - 1];
            float frameTime = frames[frameIndex];
            float percent = 1 - ((time - frameTime) / (frames[frameIndex + lastFrameTime] - frameTime));
            percent = this.GetCurvePercent((frameIndex / 3) - 1, percent < 0 ? 0 : (percent > 1 ? 1 : percent));

            bone.ScaleX += (bone.Data.ScaleX - 1 + lastFrameX + ((frames[frameIndex + TranslateTimeline.frameX] - lastFrameX) * percent) - bone.ScaleX) * alpha;
            bone.ScaleY += (bone.Data.ScaleY - 1 + lastFrameY + ((frames[frameIndex + TranslateTimeline.frameY] - lastFrameY) * percent) - bone.ScaleY) * alpha;
        }
        #endregion
    }
}
