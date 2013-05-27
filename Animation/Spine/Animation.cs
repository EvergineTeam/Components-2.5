#region File Description
//-----------------------------------------------------------------------------
// Animation
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

#region Using Statements
using System;
using System.Collections.Generic;
#endregion

namespace WaveEngine.Components.Animation.Spine
{
    /// <summary>
    /// Animation class
    /// </summary>
    public class Animation
    {
        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; private set; }

        /// <summary>
        /// Gets or sets the timelines.
        /// </summary>
        /// <value>
        /// The timelines.
        /// </value>
        public List<ITimeline> Timelines { get; set; }

        /// <summary>
        /// Gets or sets the duration.
        /// </summary>
        /// <value>
        /// The duration.
        /// </value>
        public float Duration { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Animation" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="timelines">The timelines.</param>
        /// <param name="duration">The duration.</param>
        /// <exception cref="System.ArgumentNullException">name cannot be null.</exception>
        public Animation(string name, List<ITimeline> timelines, float duration)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name cannot be null.");
            }

            if (timelines == null)
            {
                throw new ArgumentNullException("timelines cannot be null.");
            }

            this.Name = name;
            this.Timelines = timelines;
            this.Duration = duration;
        }

        /// <summary>
        /// Poses the skeleton at the specified time for this animation.
        /// </summary>
        /// <param name="skeleton">The skeleton.</param>
        /// <param name="time">The time.</param>
        /// <param name="loop">if set to <c>true</c> [loop].</param>
        /// <exception cref="System.ArgumentNullException">skeleton cannot be null.</exception>
        public void Apply(Skeleton skeleton, float time, bool loop)
        {
            if (skeleton == null)
            {
                throw new ArgumentNullException("skeleton cannot be null.");
            }

            if (loop && this.Duration != 0)
            {
                time %= this.Duration;
            }

            List<ITimeline> timelines = this.Timelines;
            for (int i = 0, n = timelines.Count; i < n; i++)
            {
                timelines[i].Apply(skeleton, time, 1);
            }
        }

        /// <summary>
        /// Poses the skeleton at the specified time for this animation mixed with the current pose.
        /// </summary>
        /// <param name="skeleton">The skeleton.</param>
        /// <param name="time">The time.</param>
        /// <param name="loop">if set to <c>true</c> [loop].</param>
        /// <param name="alpha">The amount of this animation that affects the current pose.</param>
        /// <exception cref="System.ArgumentNullException">skeleton cannot be null.</exception>
        public void Mix(Skeleton skeleton, float time, bool loop, float alpha)
        {
            if (skeleton == null)
            {
                throw new ArgumentNullException("skeleton cannot be null.");
            }

            if (loop && this.Duration != 0)
            {
                time %= this.Duration;
            }

            List<ITimeline> timelines = this.Timelines;
            for (int i = 0, n = timelines.Count; i < n; i++)
            {
                timelines[i].Apply(skeleton, time, alpha);
            }
        }

        /// <summary>
        /// Binaries the search.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <param name="target">After the first and before the last entry.</param>
        /// <param name="step">The step.</param>
        /// <returns>search result.</returns>
        internal static int BinarySearch(float[] values, float target, int step)
        {
            int low = 0;
            int high = (values.Length / step) - 2;
            if (high == 0)
            {
                return step;
            }

            int current = (int)((uint)high >> 1);

            while (true)
            {
                if (values[(current + 1) * step] <= target)
                {
                    low = current + 1;
                }
                else
                {
                    high = current;
                }

                if (low == high)
                {
                    return (low + 1) * step;
                }

                current = (int)((uint)(low + high) >> 1);
            }
        }

        /// <summary>
        /// Linears the search.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <param name="target">The target.</param>
        /// <param name="step">The step.</param>
        /// <returns>Search result.</returns>
        internal static int LinearSearch(float[] values, float target, int step)
        {
            for (int i = 0, last = values.Length - step; i <= last; i += step)
            {
                if (values[i] > target)
                {
                    return i;
                }
            }

            return -1;
        }
    }
}
