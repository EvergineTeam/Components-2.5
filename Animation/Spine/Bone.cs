#region File Description
//-----------------------------------------------------------------------------
// Bone
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
    /// Bone class
    /// </summary>
    public class Bone
    {
        /// <summary>
        /// The y down
        /// </summary>
        public static bool YDown;

        #region Properties
        /// <summary>
        /// Gets the data.
        /// </summary>
        /// <value>
        /// The data.
        /// </value>
        public BoneData Data { get; private set; }

        /// <summary>
        /// Gets the parent.
        /// </summary>
        /// <value>
        /// The parent.
        /// </value>
        public Bone Parent { get; private set; }

        /// <summary>
        /// Gets or sets the X.
        /// </summary>
        /// <value>
        /// The X.
        /// </value>
        public float X { get; set; }

        /// <summary>
        /// Gets or sets the Y.
        /// </summary>
        /// <value>
        /// The Y.
        /// </value>
        public float Y { get; set; }

        /// <summary>
        /// Gets or sets the rotation.
        /// </summary>
        /// <value>
        /// The rotation.
        /// </value>
        public float Rotation { get; set; }

        /// <summary>
        /// Gets or sets the offset rotation. This rotation is added to the animation rotation.
        /// </summary>
        /// <value>
        /// The offset rotation.
        /// </value>
        public float OffsetRotation { get; set; }

        /// <summary>
        /// Gets or sets the scale X.
        /// </summary>
        /// <value>
        /// The scale X.
        /// </value>
        public float ScaleX { get; set; }

        /// <summary>
        /// Gets or sets the scale Y.
        /// </summary>
        /// <value>
        /// The scale Y.
        /// </value>
        public float ScaleY { get; set; }

        /// <summary>
        /// Gets the M00.
        /// </summary>
        /// <value>
        /// The M00.
        /// </value>
        public float M00 { get; private set; }

        /// <summary>
        /// Gets the M01.
        /// </summary>
        /// <value>
        /// The M01.
        /// </value>
        public float M01 { get; private set; }

        /// <summary>
        /// Gets the M10.
        /// </summary>
        /// <value>
        /// The M10.
        /// </value>
        public float M10 { get; private set; }

        /// <summary>
        /// Gets the M11.
        /// </summary>
        /// <value>
        /// The M11.
        /// </value>
        public float M11 { get; private set; }

        /// <summary>
        /// Gets the world X.
        /// </summary>
        /// <value>
        /// The world X.
        /// </value>
        public float WorldX { get; private set; }

        /// <summary>
        /// Gets the world Y.
        /// </summary>
        /// <value>
        /// The world Y.
        /// </value>
        public float WorldY { get; private set; }

        /// <summary>
        /// Gets the world rotation.
        /// </summary>
        /// <value>
        /// The world rotation.
        /// </value>
        public float WorldRotation { get; private set; }

        /// <summary>
        /// Gets the world scale X.
        /// </summary>
        /// <value>
        /// The world scale X.
        /// </value>
        public float WorldScaleX { get; private set; }

        /// <summary>
        /// Gets the world scale Y.
        /// </summary>
        /// <value>
        /// The world scale Y.
        /// </value>
        public float WorldScaleY { get; private set; }
        #endregion

        #region Initialize
        /// <summary>
        /// Initializes a new instance of the <see cref="Bone" /> class.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="parent">May be null.</param>
        /// <exception cref="System.ArgumentNullException">data cannot be null.</exception>
        public Bone(BoneData data, Bone parent)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data cannot be null.");
            }

            this.Data = data;
            this.Parent = parent;
            this.SetToBindPose();
            this.OffsetRotation = 0;
        }

        #endregion

        #region Public Methods
        /// <summary>
        ///  Computes the world SRT using the parent bone and the local SRT. 
        /// </summary>
        /// <param name="flipX">if set to <c>true</c> [flip X].</param>
        /// <param name="flipY">if set to <c>true</c> [flip Y].</param>
        public void UpdateWorldTransform(bool flipX, bool flipY)
        {
            Bone parent = this.Parent;
            if (parent != null)
            {
                this.WorldX = (this.X * parent.M00) + (this.Y * parent.M01) + parent.WorldX;
                this.WorldY = (this.X * parent.M10) + (this.Y * parent.M11) + parent.WorldY;
                this.WorldScaleX = parent.WorldScaleX * this.ScaleX;
                this.WorldScaleY = parent.WorldScaleY * this.ScaleY;
                this.WorldRotation = parent.WorldRotation + this.Rotation + this.OffsetRotation;
            }
            else
            {
                this.WorldX = this.X;
                this.WorldY = this.Y;
                this.WorldScaleX = this.ScaleX;
                this.WorldScaleY = this.ScaleY;
                this.WorldRotation = this.Rotation + this.OffsetRotation;
            }

            float radians = this.WorldRotation * (float)Math.PI / 180;
            float cos = (float)Math.Cos(radians);
            float sin = (float)Math.Sin(radians);
            this.M00 = cos * this.WorldScaleX;
            this.M10 = -sin * this.WorldScaleX;
            this.M01 = -sin * this.WorldScaleY;
            this.M11 = -cos * this.WorldScaleY;

            if (flipX)
            {
                this.M00 = -this.M00;
                this.M01 = -this.M01;
            }

            if (flipY)
            {
                this.M10 = -this.M10;
                this.M11 = -this.M11;
            }

            if (YDown)
            {
                this.M10 = -this.M10;
                this.M11 = -this.M11;
            }
        }

        /// <summary>
        /// Sets to bind pose.
        /// </summary>
        public void SetToBindPose()
        {
            BoneData data = this.Data;
            this.X = data.X;
            this.Y = data.Y;
            this.Rotation = data.Rotation;
            this.ScaleX = data.ScaleX;
            this.ScaleY = data.ScaleY;
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
        #endregion
    }
}
