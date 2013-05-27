#region File Description
//-----------------------------------------------------------------------------
// AttachmentTimeline
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
#endregion

namespace WaveEngine.Components.Animation.Spine
{
    /// <summary>
    /// AttachmentTimeLine class
    /// </summary>
    public class AttachmentTimeline : ITimeline
    {
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
        public float[] Frames { get; private set; }

        /// <summary>
        /// Gets the attachment names.
        /// </summary>
        /// <value>
        /// The attachment names.
        /// </value>
        public string[] AttachmentNames { get; private set; }

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
                return this.Frames.Length;
            }
        }
        #endregion

        #region Initialize
        /// <summary>
        /// Initializes a new instance of the <see cref="AttachmentTimeline" /> class.
        /// </summary>
        /// <param name="frameCount">The frame count.</param>
        public AttachmentTimeline(int frameCount)
        {
            this.Frames = new float[frameCount];
            this.AttachmentNames = new string[frameCount];
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Sets the time and value of the specified keyframe. 
        /// </summary>
        /// <param name="frameIndex">Index of the frame.</param>
        /// <param name="time">The time.</param>
        /// <param name="attachmentName">Name of the attachment.</param>
        public void SetFrame(int frameIndex, float time, string attachmentName)
        {
            this.Frames[frameIndex] = time;
            this.AttachmentNames[frameIndex] = attachmentName;
        }

        /// <summary>
        /// Sets the value(s) for the specified time.
        /// </summary>
        /// <param name="skeleton">The skeleton.</param>
        /// <param name="time">The time.</param>
        /// <param name="alpha">The alpha.</param>
        public void Apply(Skeleton skeleton, float time, float alpha)
        {
            float[] frames = this.Frames;
            if (time < frames[0])
            {
                // Time is before first frame.
                return; 
            }

            int frameIndex;
            if (time >= frames[frames.Length - 1])
            {
                // Time is after last frame.
                frameIndex = frames.Length - 1;
            }
            else
            {
                frameIndex = Animation.BinarySearch(frames, time, 1) - 1;
            }

            string attachmentName = this.AttachmentNames[frameIndex];
            skeleton.Slots[this.SlotIndex].Attachment = attachmentName == null ? null : skeleton.GetAttachment(this.SlotIndex, attachmentName);
        }
        #endregion
    }
}
