#region File Description
//-----------------------------------------------------------------------------
// CameraPoint
//
// Copyright © 2016 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System.Runtime.Serialization;
using WaveEngine.Common.Math;
#endregion

namespace WaveEngine.Components.Cameras
{
    /// <summary>
    /// Struct to define camera points in space.
    /// </summary>
    [DataContract(Namespace = "WaveEngine.Components.Cameras")]
    public struct CameraPoint
    {
        /// <summary>
        /// The position of the point.
        /// </summary>
        [DataMember]
        public Vector3 Position;

        /// <summary>
        /// The up vector.
        /// </summary>
        [DataMember]
        public Vector3 Up;

        /// <summary>
        /// The look at vector.
        /// </summary>
        [DataMember]
        public Vector3 LookAt;

        #region Initialize
        /// <summary>
        /// Initializes a new instance of the <see cref="CameraPoint" /> struct.
        /// </summary>
        /// <param name="position">The position of the point.</param>
        /// <param name="look">The look at vector.</param>
        /// <param name="up">The up vector.</param>
        public CameraPoint(Vector3 position, Vector3 look, Vector3 up)
        {
            this.Position = position;
            this.LookAt = look;
            this.Up = up;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CameraPoint" /> struct.
        /// </summary>
        /// <param name="position">The position of the point.</param>
        /// <param name="look">The look at vector.</param>
        /// <param name="up">The up vector.</param>
        public CameraPoint(ref Vector3 position, ref Vector3 look, ref Vector3 up)
        {
            this.Position = position;
            this.LookAt = look;
            this.Up = up;
        }
        #endregion
    }
}
