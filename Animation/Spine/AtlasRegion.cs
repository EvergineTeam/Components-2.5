#region File Description
//-----------------------------------------------------------------------------
// AtlasRegion
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
    /// AtlasRegion class
    /// </summary>
    public class AtlasRegion
    {
        /// <summary>
        /// The page
        /// </summary>
        public AtlasPage Page;

        /// <summary>
        /// The name
        /// </summary>
        public string Name;

        /// <summary>
        /// The rectabgke
        /// </summary>
        public int X, Y, Width, Height;

        /// <summary>
        /// The texcoord
        /// </summary>
        public float U, V, U2, V2;

        /// <summary>
        /// The offsets
        /// </summary>
        public float OffsetX, OffsetY;

        /// <summary>
        /// The original width and original height
        /// </summary>
        public int OriginalWidth, OriginalHeight;

        /// <summary>
        /// The index
        /// </summary>
        public int Index;

        /// <summary>
        /// The rotate
        /// </summary>
        public bool Rotate;

        /// <summary>
        /// The splits
        /// </summary>
        public int[] Splits;

        /// <summary>
        /// The pads
        /// </summary>
        public int[] Pads;
    }
}
