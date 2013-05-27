#region File Description
//-----------------------------------------------------------------------------
// SkeletonData
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
    /// SkeletonData class
    /// </summary>
    public class SkeletonData
    {
        /// <summary>
        /// The default skin
        /// </summary>
        public Skin DefaultSkin;

        #region Properties
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets the bones.
        /// </summary>
        /// <remarks>Ordered parents first.</remarks>
        /// <value>
        /// The bones.
        /// </value>
        public List<BoneData> Bones { get; private set; }

        /// <summary>
        /// Gets the slots.
        /// </summary>
        /// <remarks>// Bind pose draw order.</remarks>
        /// <value>
        /// The slots.
        /// </value>
        public List<SlotData> Slots { get; private set; } 

        /// <summary>
        /// Gets the skins.
        /// </summary>
        /// <value>
        /// The skins.
        /// </value>
        public List<Skin> Skins { get; private set; }

        /// <summary>
        /// Gets the animations.
        /// </summary>
        /// <value>
        /// The animations.
        /// </value>
        public List<Animation> Animations { get; private set; }
        #endregion

        #region Initialize
        /// <summary>
        /// Initializes a new instance of the <see cref="SkeletonData" /> class.
        /// </summary>
        public SkeletonData()
        {
            this.Bones = new List<BoneData>();
            this.Slots = new List<SlotData>();
            this.Skins = new List<Skin>();
            this.Animations = new List<Animation>();
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Adds the bone.
        /// </summary>
        /// <param name="bone">The bone.</param>
        /// <exception cref="System.ArgumentNullException">bone cannot be null.</exception>
        public void AddBone(BoneData bone)
        {
            if (bone == null)
            {
                throw new ArgumentNullException("bone cannot be null.");
            }

            this.Bones.Add(bone);
        }

        /// <summary>
        /// Finds the bone.
        /// </summary>
        /// <param name="boneName">Name of the bone.</param>
        /// <returns>May be null.</returns>
        /// <exception cref="System.ArgumentNullException">boneName cannot be null.</exception>
        public BoneData FindBone(string boneName)
        {
            if (boneName == null)
            {
                throw new ArgumentNullException("boneName cannot be null.");
            }

            for (int i = 0, n = this.Bones.Count; i < n; i++)
            {
                BoneData bone = this.Bones[i];
                if (bone.Name == boneName)
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
        /// <returns>-1 if the bone was not found. </returns>
        /// <exception cref="System.ArgumentNullException">boneName cannot be null.</exception>
        public int FindBoneIndex(string boneName)
        {
            if (boneName == null)
            {
                throw new ArgumentNullException("boneName cannot be null.");
            }

            for (int i = 0, n = this.Bones.Count; i < n; i++)
            {
                if (this.Bones[i].Name == boneName)
                {
                    return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// Adds the slot.
        /// </summary>
        /// <param name="slot">The slot.</param>
        /// <exception cref="System.ArgumentNullException">slot cannot be null.</exception>
        public void AddSlot(SlotData slot)
        {
            if (slot == null)
            {
                throw new ArgumentNullException("slot cannot be null.");
            }

            this.Slots.Add(slot);
        }

        /// <summary>
        /// Finds the slot.
        /// </summary>
        /// <param name="slotName">Name of the slot.</param>
        /// <returns>May be null. </returns>
        /// <exception cref="System.ArgumentNullException">slotName cannot be null.</exception>
        public SlotData FindSlot(string slotName)
        {
            if (slotName == null)
            {
                throw new ArgumentNullException("slotName cannot be null.");
            }

            for (int i = 0, n = this.Slots.Count; i < n; i++)
            {
                SlotData slot = this.Slots[i];
                if (slot.Name == slotName)
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
        /// <returns>-1 if the bone was not found.</returns>
        /// <exception cref="System.ArgumentNullException">slotName cannot be null.</exception>
        public int FindSlotIndex(string slotName)
        {
            if (slotName == null)
            {
                throw new ArgumentNullException("slotName cannot be null.");
            }

            for (int i = 0, n = this.Slots.Count; i < n; i++)
            {
                if (this.Slots[i].Name == slotName)
                {
                    return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// Adds the skin.
        /// </summary>
        /// <param name="skin">The skin.</param>
        /// <exception cref="System.ArgumentNullException">skin cannot be null.</exception>
        public void AddSkin(Skin skin)
        {
            if (skin == null)
            {
                throw new ArgumentNullException("skin cannot be null.");
            }

            this.Skins.Add(skin);
        }

        /// <summary>
        /// Finds the skin.
        /// </summary>
        /// <param name="skinName">Name of the skin.</param>
        /// <returns>May be null.</returns>
        /// <exception cref="System.ArgumentNullException">skinName cannot be null.</exception>
        public Skin FindSkin(string skinName)
        {
            if (skinName == null)
            {
                throw new ArgumentNullException("skinName cannot be null.");
            }

            foreach (Skin skin in this.Skins)
            {
                if (skin.Name == skinName)
                {
                    return skin;
                }
            }

            return null;
        }

        /// <summary>
        /// Adds the animation.
        /// </summary>
        /// <param name="animation">The animation.</param>
        /// <exception cref="System.ArgumentNullException">animation cannot be null.</exception>
        public void AddAnimation(Animation animation)
        {
            if (animation == null)
            {
                throw new ArgumentNullException("animation cannot be null.");
            }

            this.Animations.Add(animation);
        }

        /// <summary>
        /// Finds the animation.
        /// </summary>
        /// <param name="animationName">Name of the animation.</param>
        /// <returns>May be null.</returns>
        /// <exception cref="System.ArgumentNullException">animationName cannot be null.</exception>
        public Animation FindAnimation(string animationName)
        {
            if (animationName == null)
            {
                throw new ArgumentNullException("animationName cannot be null.");
            }

            for (int i = 0, n = this.Animations.Count; i < n; i++)
            {
                Animation animation = this.Animations[i];
                if (animation.Name == animationName)
                {
                    return animation;
                }
            }

            return null;
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return this.Name != null ? this.Name : base.ToString();
        }
        #endregion
    }
}
