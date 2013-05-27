#region File Description
//-----------------------------------------------------------------------------
// CurveTimeline
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
    /// Base class for frames that use an interpolation bezier curve.
    /// </summary>
    public abstract class CurveTimeline : ITimeline
    {
        /// <summary>
        /// The LINEAR
        /// </summary>
        protected static float linear = 0;

        /// <summary>
        /// The STEPPED
        /// </summary>
        protected static float stepped = -1;

        /// <summary>
        /// The BEZIE r_ SEGMENTS
        /// </summary>
        protected static int bezierSegments = 10;

        /// <summary>
        /// The curves
        /// </summary>
        private float[] curves; // dfx, dfy, ddfx, ddfy, dddfx, dddfy, ...

        /// <summary>
        /// Gets the frame count.
        /// </summary>
        /// <value>
        /// The frame count.
        /// </value>
        public int FrameCount
        {
            get
            {
                return (this.curves.Length / 6) + 1;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CurveTimeline" /> class.
        /// </summary>
        /// <param name="frameCount">The frame count.</param>
        public CurveTimeline(int frameCount)
        {
            this.curves = new float[(frameCount - 1) * 6];
        }

        /// <summary>
        /// Sets the value(s) for the specified time.
        /// </summary>
        /// <param name="skeleton">The skeleton.</param>
        /// <param name="time">The time.</param>
        /// <param name="alpha">The alpha.</param>
        public abstract void Apply(Skeleton skeleton, float time, float alpha);

        /// <summary>
        /// Sets the linear.
        /// </summary>
        /// <param name="frameIndex">Index of the frame.</param>
        public void SetLinear(int frameIndex)
        {
            this.curves[frameIndex * 6] = linear;
        }

        /// <summary>
        /// Sets the stepped.
        /// </summary>
        /// <param name="frameIndex">Index of the frame.</param>
        public void SetStepped(int frameIndex)
        {
            this.curves[frameIndex * 6] = stepped;
        }

        /// <summary>
        /// Sets the control handle positions for an interpolation bezier curve used to transition from this keyframe to the next.
        /// </summary>
        /// <param name="frameIndex">Index of the frame.</param>
        /// <param name="cx1">cx1 is from 0 to 1, representing the percent of time between the two keyframes.</param>
        /// <param name="cy1">cy1 is the percent of the difference between the keyframe's values.</param>
        /// <param name="cx2">cx2 is from 0 to 1, representing the percent of time between the two keyframes..</param>
        /// <param name="cy2">cy2 is the percent of the difference between the keyframe's values.</param>
        public void SetCurve(int frameIndex, float cx1, float cy1, float cx2, float cy2)
        {
            float subdiv_step = 1f / bezierSegments;
            float subdiv_step2 = subdiv_step * subdiv_step;
            float subdiv_step3 = subdiv_step2 * subdiv_step;

            float pre1 = 3 * subdiv_step;
            float pre2 = 3 * subdiv_step2;
            float pre4 = 6 * subdiv_step2;
            float pre5 = 6 * subdiv_step3;

            float tmp1x = (-cx1 * 2) + cx2;
            float tmp1y = (-cy1 * 2) + cy2;
            float tmp2x = ((cx1 - cx2) * 3) + 1;
            float tmp2y = ((cy1 - cy2) * 3) + 1;

            int i = frameIndex * 6;

            float[] curves = this.curves;
            curves[i] = (cx1 * pre1) + (tmp1x * pre2) + (tmp2x * subdiv_step3);
            curves[i + 1] = (cy1 * pre1) + (tmp1y * pre2) + (tmp2y * subdiv_step3);
            curves[i + 2] = (tmp1x * pre4) + (tmp2x * pre5);
            curves[i + 3] = (tmp1y * pre4) + (tmp2y * pre5);
            curves[i + 4] = tmp2x * pre5;
            curves[i + 5] = tmp2y * pre5;
        }

        /// <summary>
        /// Gets the curve percent.
        /// </summary>
        /// <param name="frameIndex">Index of the frame.</param>
        /// <param name="percent">The percent.</param>
        /// <returns>Return percent.</returns>
        public float GetCurvePercent(int frameIndex, float percent)
        {
            int curveIndex = frameIndex * 6;
            float[] curves = this.curves;
            float dfx = curves[curveIndex];

            if (dfx == linear)
            {
                return percent;
            }

            if (dfx == stepped)
            {
                return 0;
            }

            float dfy = curves[curveIndex + 1];
            float ddfx = curves[curveIndex + 2];
            float ddfy = curves[curveIndex + 3];
            float dddfx = curves[curveIndex + 4];
            float dddfy = curves[curveIndex + 5];
            float x = dfx, y = dfy;
            int i = bezierSegments - 2;

            while (true)
            {
                if (x >= percent)
                {
                    float lastX = x - dfx;
                    float lastY = y - dfy;
                    return lastY + ((y - lastY) * (percent - lastX) / (x - lastX));
                }

                if (i == 0)
                {
                    break;
                }

                i--;
                dfx += ddfx;
                dfy += ddfy;
                ddfx += dddfx;
                ddfy += dddfy;
                x += dfx;
                y += dfy;
            }

            return y + ((1 - y) * (percent - x) / (1 - x)); // Last point is 1,1.
        }
    }
}
