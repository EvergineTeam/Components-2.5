#region File Description
//-----------------------------------------------------------------------------
// GestureSample
//
// Copyright © 2015 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using WaveEngine.Common.Math;
#endregion

namespace WaveEngine.Components.Gestures
{
    /// <summary>
    /// Struct that holds the information of a gesture.
    /// </summary>
    public struct GestureSample
    {
        /// <summary>
        /// Position of the gesture.
        /// </summary>
        public Vector2 Position;

        /// <summary>
        /// Translation vector between the latest and the current point
        /// </summary>
        public Vector2 DeltaTranslation;
        
        /// <summary>
        /// Angle of the rotation between the latest and the current point
        /// </summary>
        public float DeltaAngle;
        
        /// <summary>
        /// Scale of the gesture
        /// </summary>
        public float DeltaScale;
        
        /// <summary>
        /// Scale difference between the latest and the current point
        /// </summary>
        public float DiffScale;

        /// <summary>
        /// Type of gesture.
        /// </summary>
        public GestureType Type;

        /// <summary>
        /// If the point is new or it previously existing
        /// </summary>
        public bool IsNew;
    }
}
