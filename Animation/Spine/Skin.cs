#region File Description
//-----------------------------------------------------------------------------
// Skin
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
    /// Stores attachments by slot index and attachment name.
    /// </summary>
    public class Skin
    {
        /// <summary>
        /// The attachments
        /// </summary>
        private Dictionary<KeyValuePair<int, string>, Attachment> attachments = new Dictionary<KeyValuePair<int, string>, Attachment>();

        #region Properties
        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; private set; }
        #endregion

        #region Initialize
        /// <summary>
        /// Initializes a new instance of the <see cref="Skin" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <exception cref="System.ArgumentNullException">name cannot be null.</exception>
        public Skin(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name cannot be null.");
            }

            this.Name = name;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Adds the attachment.
        /// </summary>
        /// <param name="slotIndex">Index of the slot.</param>
        /// <param name="name">The name.</param>
        /// <param name="attachment">The attachment.</param>
        /// <exception cref="System.ArgumentNullException">attachment cannot be null.</exception>
        public void AddAttachment(int slotIndex, string name, Attachment attachment)
        {
            if (attachment == null)
            {
                throw new ArgumentNullException("attachment cannot be null.");
            }

            this.attachments.Add(new KeyValuePair<int, string>(slotIndex, name), attachment);
        }

        /// <summary>
        /// Gets the attachment.
        /// </summary>
        /// <param name="slotIndex">Index of the slot.</param>
        /// <param name="name">The name.</param>
        /// <returns>May be null.</returns>
        public Attachment GetAttachment(int slotIndex, string name)
        {
            Attachment attachment;
            this.attachments.TryGetValue(new KeyValuePair<int, string>(slotIndex, name), out attachment);

            return attachment;
        }

        /// <summary>
        /// Finds the names for slot.
        /// </summary>
        /// <param name="slotIndex">Index of the slot.</param>
        /// <param name="names">The names.</param>
        /// <exception cref="System.ArgumentNullException">names cannot be null.</exception>
        public void FindNamesForSlot(int slotIndex, List<string> names)
        {
            if (names == null)
            {
                throw new ArgumentNullException("names cannot be null.");
            }

            foreach (KeyValuePair<int, string> key in this.attachments.Keys)
            {
                if (key.Key == slotIndex)
                {
                    names.Add(key.Value);
                }
            }
        }

        /// <summary>
        /// Finds the attachments for slot.
        /// </summary>
        /// <param name="slotIndex">Index of the slot.</param>
        /// <param name="attachments">The attachments.</param>
        /// <exception cref="System.ArgumentNullException">attachments cannot be null.</exception>
        public void FindAttachmentsForSlot(int slotIndex, List<Attachment> attachments)
        {
            if (attachments == null)
            {
                throw new ArgumentNullException("attachments cannot be null.");
            }

            foreach (KeyValuePair<KeyValuePair<int, string>, Attachment> entry in this.attachments)
            {
                if (entry.Key.Key == slotIndex)
                {
                    attachments.Add(entry.Value);
                }
            }
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return this.Name;
        }

        /// <summary>
        /// Attach all attachments from this skin if the corresponding attachment from the old skin is currently attached.
        /// </summary>
        /// <param name="skeleton">The skeleton.</param>
        /// <param name="oldSkin">The old skin.</param>
        internal void AttachAll(Skeleton skeleton, Skin oldSkin)
        {
            foreach (KeyValuePair<KeyValuePair<int, string>, Attachment> entry in oldSkin.attachments)
            {
                int slotIndex = entry.Key.Key;
                Slot slot = skeleton.Slots[slotIndex];

                if (slot.Attachment == entry.Value)
                {
                    Attachment attachment = this.GetAttachment(slotIndex, entry.Key.Value);
                    
                    if (attachment != null)
                    {
                        slot.Attachment = attachment;
                    }
                }
            }
        }
        #endregion
    }
}
