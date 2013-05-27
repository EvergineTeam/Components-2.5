#region File Description
//-----------------------------------------------------------------------------
// Skeleton
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

#region Usings Statements
using System;
using System.Collections.Generic;
#endregion

namespace WaveEngine.Components.Animation.Spine
{
    /// <summary>
    /// Skeleton class
    /// </summary>
    public class Skeleton
    {
        #region Properties
        /// <summary>
        /// Gets the data.
        /// </summary>
        /// <value>
        /// The data.
        /// </value>
        public SkeletonData Data { get; private set; }

        /// <summary>
        /// Gets the bones.
        /// </summary>
        /// <value>
        /// The bones.
        /// </value>
        public List<Bone> Bones { get; private set; }

        /// <summary>
        /// Gets the slots.
        /// </summary>
        /// <value>
        /// The slots.
        /// </value>
        public List<Slot> Slots { get; private set; }

        /// <summary>
        /// Gets the draw order.
        /// </summary>
        /// <value>
        /// The draw order.
        /// </value>
        public List<Slot> DrawOrder { get; private set; }

        /// <summary>
        /// Gets or sets the skin.
        /// </summary>
        /// <value>
        /// The skin.
        /// </value>
        public Skin Skin { get; set; }

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
        /// Gets or sets the time.
        /// </summary>
        /// <value>
        /// The time.
        /// </value>
        public float Time { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [flip X].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [flip X]; otherwise, <c>false</c>.
        /// </value>
        public bool FlipX { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [flip Y].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [flip Y]; otherwise, <c>false</c>.
        /// </value>
        public bool FlipY { get; set; }

        /// <summary>
        /// Gets the root bone.
        /// </summary>
        /// <value>
        /// The root bone.
        /// </value>
        public Bone RootBone
        {
            get
            {
                return this.Bones.Count == 0 ? null : this.Bones[0];
            }
        }
        #endregion

        #region Initialize
        /// <summary>
        /// Initializes a new instance of the <see cref="Skeleton" /> class.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <exception cref="System.ArgumentNullException">data cannot be null.</exception>
        public Skeleton(SkeletonData data)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data cannot be null.");
            }

            this.Data = data;

            this.Bones = new List<Bone>(this.Data.Bones.Count);
            foreach (BoneData boneData in this.Data.Bones)
            {
                Bone parent = boneData.Parent == null ? null : this.Bones[this.Data.Bones.IndexOf(boneData.Parent)];
                this.Bones.Add(new Bone(boneData, parent));
            }

            this.Slots = new List<Slot>(this.Data.Slots.Count);
            this.DrawOrder = new List<Slot>(this.Data.Slots.Count);
            foreach (SlotData slotData in this.Data.Slots)
            {
                Bone bone = this.Bones[this.Data.Bones.IndexOf(slotData.BoneData)];
                Slot slot = new Slot(slotData, this, bone);
                this.Slots.Add(slot);
                this.DrawOrder.Add(slot);
            }

            this.R = 1;
            this.G = 1;
            this.B = 1;
            this.A = 1;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Updates the world transform for each bone.
        /// </summary>
        public void UpdateWorldTransform()
        {
            bool flipX = this.FlipX;
            bool flipY = this.FlipY;
            List<Bone> bones = this.Bones;
            for (int i = 0, n = bones.Count; i < n; i++)
            {
                bones[i].UpdateWorldTransform(flipX, flipY);
            }
        }

        /// <summary>
        ///  Sets the bones and slots to their bind pose values.
        /// </summary>
        public void SetToBindPose()
        {
            this.SetBonesToBindPose();
            this.SetSlotsToBindPose();
        }

        /// <summary>
        /// Sets the bones to bind pose.
        /// </summary>
        public void SetBonesToBindPose()
        {
            List<Bone> bones = this.Bones;
            for (int i = 0, n = bones.Count; i < n; i++)
            {
                bones[i].SetToBindPose();
            }
        }

        /// <summary>
        /// Sets the slots to bind pose.
        /// </summary>
        public void SetSlotsToBindPose()
        {
            List<Slot> slots = this.Slots;
            for (int i = 0, n = slots.Count; i < n; i++)
            {
                slots[i].SetToBindPose(i);
            }
        }

        /// <summary>
        /// Finds the bone.
        /// </summary>
        /// <param name="boneName">Name of the bone.</param>
        /// <returns>May be null.</returns>
        /// <exception cref="System.ArgumentNullException">boneName cannot be null.</exception>
        public Bone FindBone(string boneName)
        {
            if (boneName == null)
            {
                throw new ArgumentNullException("boneName cannot be null.");
            }

            List<Bone> bones = this.Bones;
            for (int i = 0, n = bones.Count; i < n; i++)
            {
                Bone bone = bones[i];
                if (bone.Data.Name == boneName)
                {
                    return bone;
                }
            }

            return null;
        }

        /// <summary>
        /// Finds the index of the bone.
        /// </summary>
        /// <param name="boneName">Name of the bone.</param>
        /// <returns>-1 if the bone was not found.</returns>
        /// <exception cref="System.ArgumentNullException">boneName cannot be null.</exception>
        public int FindBoneIndex(string boneName)
        {
            if (boneName == null)
            {
                throw new ArgumentNullException("boneName cannot be null.");
            }

            List<Bone> bones = this.Bones;
            for (int i = 0, n = bones.Count; i < n; i++)
            {
                if (bones[i].Data.Name == boneName)
                {
                    return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// Finds the slot.
        /// </summary>
        /// <param name="slotName">Name of the slot.</param>
        /// <returns>my be null.</returns>
        /// <exception cref="System.ArgumentNullException">slotName cannot be null.</exception>
        public Slot FindSlot(string slotName)
        {
            if (slotName == null)
            {
                throw new ArgumentNullException("slotName cannot be null.");
            }

            List<Slot> slots = this.Slots;
            for (int i = 0, n = slots.Count; i < n; i++)
            {
                Slot slot = slots[i];
                if (slot.Data.Name == slotName)
                {
                    return slot;
                }
            }

            return null;
        }

        /// <summary>
        /// Finds the index of the slot.
        /// </summary>
        /// <param name="slotName">Name of the slot.</param>
        /// <returns>-1 if the bone was not found. </returns>
        /// <exception cref="System.ArgumentNullException">slotName cannot be null.</exception>
        public int FindSlotIndex(string slotName)
        {
            if (slotName == null)
            {
                throw new ArgumentNullException("slotName cannot be null.");
            }

            List<Slot> slots = this.Slots;
            for (int i = 0, n = slots.Count; i < n; i++)
            {
                if (slots[i].Data.Name.Equals(slotName))
                {
                    return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// Sets a skin by name.
        /// </summary>
        /// <param name="skinName">Name of the skin.</param>
        /// <exception cref="System.ArgumentException">Skin not found:  + skinName</exception>
        public void SetSkin(string skinName)
        {
            Skin skin = this.Data.FindSkin(skinName);
            if (skin == null)
            {
                throw new ArgumentException("Skin not found: " + skinName);
            }

            this.SetSkin(skin);
        }

        /// <summary>
        ///  Sets the skin used to look up attachments not found in the  <see cref="SkeletonData" /> class Attachments 
        ///  from the new skin are attached if the corresponding attachment from the old skin was attached.
        /// </summary>
        /// <param name="newSkin">The new skin. (may be null)</param>
        public void SetSkin(Skin newSkin)
        {
            if (Skin != null && newSkin != null)
            {
                newSkin.AttachAll(this, Skin);
            }

            Skin = newSkin;
        }

        /// <summary>
        /// Gets the attachment.
        /// </summary>
        /// <param name="slotName">Name of the slot.</param>
        /// <param name="attachmentName">Name of the attachment.</param>
        /// <returns>May be null.</returns>
        public Attachment GetAttachment(string slotName, string attachmentName)
        {
            return this.GetAttachment(this.Data.FindSlotIndex(slotName), attachmentName);
        }

        /// <summary>
        /// Gets the attachment.
        /// </summary>
        /// <param name="slotIndex">Index of the slot.</param>
        /// <param name="attachmentName">Name of the attachment.(may be null)</param>
        /// <returns>Return attachment.</returns>
        /// <exception cref="System.ArgumentNullException">attachmentName cannot be null.</exception>
        public Attachment GetAttachment(int slotIndex, string attachmentName)
        {
            if (attachmentName == null)
            {
                throw new ArgumentNullException("attachmentName cannot be null.");
            }

            if (Skin != null)
            {
                Attachment attachment = Skin.GetAttachment(slotIndex, attachmentName);
                if (attachment != null)
                {
                    return attachment;
                }
            }

            if (this.Data.DefaultSkin != null)
            {
                return this.Data.DefaultSkin.GetAttachment(slotIndex, attachmentName);
            }

            return null;
        }

        /// <summary>
        /// Sets the attachment.
        /// </summary>
        /// <param name="slotName">Name of the slot.</param>
        /// <param name="attachmentName">May be null.</param>
        /// <exception cref="System.ArgumentNullException">slotName cannot be null.</exception>
        /// <exception cref="System.Exception">Slot not found:  + slotName</exception>
        public void SetAttachment(string slotName, string attachmentName)
        {
            if (slotName == null)
            {
                throw new ArgumentNullException("slotName cannot be null.");
            }

            List<Slot> slots = this.Slots;
            for (int i = 0, n = slots.Count; i < n; i++)
            {
                Slot slot = slots[i];
                if (slot.Data.Name == slotName)
                {
                    Attachment attachment = null;
                    if (attachmentName != null)
                    {
                        attachment = this.GetAttachment(i, attachmentName);
                        if (attachment == null)
                        {
                            throw new ArgumentNullException("Attachment not found: " + attachmentName + ", for slot: " + slotName);
                        }
                    }

                    slot.Attachment = attachment;

                    return;
                }
            }

            throw new Exception("Slot not found: " + slotName);
        }

        /// <summary>
        /// Updates the specified delta.
        /// </summary>
        /// <param name="delta">The delta.</param>
        public void Update(float delta)
        {
            this.Time += delta;
        }
        #endregion
    }
}
