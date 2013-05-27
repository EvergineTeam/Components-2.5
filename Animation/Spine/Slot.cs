#region File Description
//-----------------------------------------------------------------------------
// Slot
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
    /// Slot class
    /// </summary>
    public class Slot
    {
        /// <summary>
        /// The attachment
        /// </summary>
        private Attachment attachment;

        /// <summary>
        /// The attachment time
        /// </summary>
        private float attachmentTime;

        #region Properties
        /// <summary>
        /// Gets the data.
        /// </summary>
        /// <value>
        /// The data.
        /// </value>
        public SlotData Data { get; private set; }

        /// <summary>
        /// Gets the bone.
        /// </summary>
        /// <value>
        /// The bone.
        /// </value>
        public Bone Bone { get; private set; }

        /// <summary>
        /// Gets the skeleton.
        /// </summary>
        /// <value>
        /// The skeleton.
        /// </value>
        public Skeleton Skeleton { get; private set; }

        /// <summary>
        /// Gets or sets the R.
        /// </summary>
        /// <value>
        /// The R.
        /// </value>
        public float R { get; set; }

        /// <summary>
        /// Gets or sets the G.
        /// </summary>
        /// <value>
        /// The G.
        /// </value>
        public float G { get; set; }

        /// <summary>
        /// Gets or sets the B.
        /// </summary>
        /// <value>
        /// The B.
        /// </value>
        public float B { get; set; }

        /// <summary>
        /// Gets or sets the A.
        /// </summary>
        /// <value>
        /// The A.
        /// </value>
        public float A { get; set; }

        /// <summary>
        /// Gets or sets the attachment.
        /// </summary>
        /// <value>
        /// The attachment.
        /// </value>
        public Attachment Attachment
        {
            get
            {
                return this.attachment;
            }

            set
            {
                this.attachment = value;
                this.attachmentTime = Skeleton.Time;
            }
        }

        /// <summary>
        /// Gets or sets the attachment time.
        /// </summary>
        /// <value>
        /// The attachment time.
        /// </value>
        public float AttachmentTime
        {
            get
            {
                return Skeleton.Time - this.attachmentTime;
            }

            set
            {
                this.attachmentTime = Skeleton.Time - value;
            }
        }
        #endregion

        #region Initialize
        /// <summary>
        /// Initializes a new instance of the <see cref="Slot" /> class.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="skeleton">The skeleton.</param>
        /// <param name="bone">The bone.</param>
        /// <exception cref="System.ArgumentNullException">data cannot be null.</exception>
        public Slot(SlotData data, Skeleton skeleton, Bone bone)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data cannot be null.");
            }

            if (skeleton == null)
            {
                throw new ArgumentNullException("skeleton cannot be null.");
            }

            if (bone == null)
            {
                throw new ArgumentNullException("bone cannot be null.");
            }

            this.Data = data;
            Skeleton = skeleton;
            Bone = bone;
            this.SetToBindPose();
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// Sets to bind pose.
        /// </summary>
        public void SetToBindPose()
        {
            this.SetToBindPose(Skeleton.Data.Slots.IndexOf(this.Data));
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return this.Data.Name;
        }

        /// <summary>
        /// Sets to bind pose.
        /// </summary>
        /// <param name="slotIndex">Index of the slot.</param>
        internal void SetToBindPose(int slotIndex)
        {
            this.R = this.Data.R;
            this.G = this.Data.G;
            this.B = this.Data.B;
            this.A = this.Data.A;
            Attachment = this.Data.AttachmentName == null ? null : Skeleton.GetAttachment(slotIndex, this.Data.AttachmentName);
        }
        #endregion
    }
}
