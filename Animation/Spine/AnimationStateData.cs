#region File Description
//-----------------------------------------------------------------------------
// AnimationStateData
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
    /// AnimationStateData class
    /// </summary>
    public class AnimationStateData
    {
        /// <summary>
        /// Gets the skeleton data.
        /// </summary>
        /// <value>
        /// The skeleton data.
        /// </value>
        public SkeletonData SkeletonData { get; private set; }

        /// <summary>
        /// The animation to mix time
        /// </summary>
        private Dictionary<KeyValuePair<Animation, Animation>, float> animationToMixTime = new Dictionary<KeyValuePair<Animation, Animation>, float>();

        #region Initialize
        /// <summary>
        /// Initializes a new instance of the <see cref="AnimationStateData" /> class.
        /// </summary>
        /// <param name="skeletonData">The skeleton data.</param>
        public AnimationStateData(SkeletonData skeletonData)
        {
            SkeletonData = skeletonData;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Sets the mix.
        /// </summary>
        /// <param name="fromName">From name.</param>
        /// <param name="toName">To name.</param>
        /// <param name="duration">The duration.</param>
        /// <exception cref="System.ArgumentException">Animation not found:  + fromName</exception>
        public void SetMix(string fromName, string toName, float duration)
        {
            Animation from = SkeletonData.FindAnimation(fromName);
            if (from == null)
            {
                throw new ArgumentException("Animation not found: " + fromName);
            }

            Animation to = SkeletonData.FindAnimation(toName);
            if (to == null)
            {
                throw new ArgumentException("Animation not found: " + toName);
            }

            this.SetMix(from, to, duration);
        }

        /// <summary>
        /// Sets the mix.
        /// </summary>
        /// <param name="from">Animation from.</param>
        /// <param name="to">Animation To.</param>
        /// <param name="duration">The duration.</param>
        /// <exception cref="System.ArgumentNullException">from cannot be null.</exception>
        public void SetMix(Animation from, Animation to, float duration)
        {
            if (from == null)
            {
                throw new ArgumentNullException("from cannot be null.");
            }

            if (to == null)
            {
                throw new ArgumentNullException("to cannot be null.");
            }

            KeyValuePair<Animation, Animation> key = new KeyValuePair<Animation, Animation>(from, to);
            this.animationToMixTime.Remove(key);

            if (duration > 0)
            {
                this.animationToMixTime.Add(key, duration);
            }
        }

        /// <summary>
        /// Gets the mix.
        /// </summary>
        /// <param name="from">Animation From.</param>
        /// <param name="to">Animation To.</param>
        /// <returns>return mix result.</returns>
        public float GetMix(Animation from, Animation to)
        {
            KeyValuePair<Animation, Animation> key = new KeyValuePair<Animation, Animation>(from, to);
            float duration;
            this.animationToMixTime.TryGetValue(key, out duration);

            return duration;
        }
        #endregion
    }
}
