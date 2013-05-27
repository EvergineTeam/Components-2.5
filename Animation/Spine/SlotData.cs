#region File Description
//-----------------------------------------------------------------------------
// SlotData
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
    /// SlotData class.
    /// </summary>
    public class SlotData
    {
        #region Properties
        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the bone data.
        /// </summary>
        /// <value>
        /// The bone data.
        /// </value>
        public BoneData BoneData { get; private set; }

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
        /// Gets or sets the name of the attachment.
        /// </summary>
        /// <value>
        /// The name of the attachment.
        /// </value>
        public string AttachmentName { get; set; }
        #endregion

        #region Initialize
        /// <summary>
        /// Initializes a new instance of the <see cref="SlotData" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="boneData">The bone data.</param>
        /// <exception cref="System.ArgumentNullException">name cannot be null.</exception>
        public SlotData(string name, BoneData boneData)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name cannot be null.");
            }

            if (boneData == null)
            {
                throw new ArgumentNullException("boneData cannot be null.");
            }

            this.Name = name;
            this.BoneData = boneData;
            this.R = 1;
            this.G = 1;
            this.B = 1;
            this.A = 1;
        }
        #endregion

        #region Public Methods
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
        #endregion
    }
}
