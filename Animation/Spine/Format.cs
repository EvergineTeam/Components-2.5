#region File Description
//-----------------------------------------------------------------------------
// Format
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
    /// Format enum
    /// </summary>
    public enum Format
    {
        /// <summary>
        /// The alpha
        /// </summary>
        Alpha,

        /// <summary>
        /// The intensity
        /// </summary>
        Intensity,

        /// <summary>
        /// The luminance alpha
        /// </summary>
        LuminanceAlpha,

        /// <summary>
        /// The RG B565
        /// </summary>
        RGB565,

        /// <summary>
        /// The RGB a4444
        /// </summary>
        RGBA4444,

        /// <summary>
        /// The RG B888
        /// </summary>
        RGB888,

        /// <summary>
        /// The RGB a8888
        /// </summary>
        RGBA8888
    }
}
