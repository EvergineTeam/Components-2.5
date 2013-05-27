#region File Description
//-----------------------------------------------------------------------------
// AtlasAttachmentLoader
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
    /// AtlasAttachementLoader class
    /// </summary>
    public class AtlasAttachmentLoader : IAttachmentLoader
    {
        /// <summary>
        /// The atlas
        /// </summary>
        private Atlas atlas;

        #region Initialize
        /// <summary>
        /// Initializes a new instance of the <see cref="AtlasAttachmentLoader" /> class.
        /// </summary>
        /// <param name="atlas">The atlas.</param>
        /// <exception cref="System.ArgumentNullException">atlas cannot be null.</exception>
        public AtlasAttachmentLoader(Atlas atlas)
        {
            if (atlas == null)
            {
                throw new ArgumentNullException("atlas cannot be null.");
            }

            this.atlas = atlas;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// New Attachment.
        /// </summary>
        /// <param name="skin">The skin.</param>
        /// <param name="type">The attachmentType.</param>
        /// <param name="name">The name.</param>
        /// <returns>May be null to not load any attachment.</returns>
        /// <exception cref="System.Exception">Region not found in atlas:  + name +  ( + type + )</exception>
        public Attachment NewAttachment(Skin skin, AttachmentType type, string name)
        {
            if (type == AttachmentType.region)
            {
                AtlasRegion region = this.atlas.FindRegion(name);
                if (region == null)
                {
                    throw new Exception("Region not found in atlas: " + name + " (" + type + ")");
                }

                RegionAttachment attachment = new RegionAttachment(name);
                attachment.Texture = region.Page.Texture;
                attachment.SetUVs(region.U, region.V, region.U2, region.V2, region.Rotate);
                attachment.RegionOffsetX = region.OffsetX;
                attachment.RegionOffsetY = region.OffsetY;
                attachment.RegionWidth = region.Width;
                attachment.RegionHeight = region.Height;
                attachment.RegionOriginalWidth = region.OriginalWidth;
                attachment.RegionOriginalHeight = region.OriginalHeight;

                return attachment;
            }
            else
            {
                throw new Exception("Unknown attachment type: " + type);
            }
        }
        #endregion
    }
}
