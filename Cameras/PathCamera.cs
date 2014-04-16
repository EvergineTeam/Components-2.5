#region File Description
//-----------------------------------------------------------------------------
// PathCamera
//
// Copyright © 2014 Wave Corporation
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using WaveEngine.Common.Math;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
#endregion

namespace WaveEngine.Components.Cameras
{
    /// <summary>
    /// PathCamera decorate class
    /// </summary>
    public class PathCamera : FixedCamera
    {
        #region Properties

        /// <summary>
        /// Gets or sets the state of the current.
        /// </summary>
        /// <value>
        /// The state of the current.
        /// </value>
        public PathCameraBehavior.State CurrentState
        {
            get
            {
                return this.entity.FindComponent<PathCameraBehavior>().CurrentState;
            }

            set
            {
                this.entity.FindComponent<PathCameraBehavior>().CurrentState = value;
            }
        }

        /// <summary>
        /// Gets or sets the speed.
        /// </summary>
        /// <value>
        /// The speed.
        /// </value>
        public float Speed
        {
            get
            {
                return this.entity.FindComponent<PathCameraBehavior>().Speed;
            }

            set
            {
                this.entity.FindComponent<PathCameraBehavior>().Speed = value;
            }
        }

        #endregion

        #region Initialize

        /// <summary>
        /// Initializes a new instance of the <see cref="PathCamera" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="position">The position.</param>
        /// <param name="lookAt">The look at.</param>
        /// <param name="cameraPointList">The camera point list.</param>
        public PathCamera(string name, Vector3 position, Vector3 lookAt, List<CameraPoint> cameraPointList)
            : base(name, position, lookAt)
        {
            this.entity.AddComponent(new PathCameraBehavior(cameraPointList));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PathCamera" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="position">The position.</param>
        /// <param name="lookAt">The look at.</param>
        /// <param name="cameraPointList">The camera point list.</param>
        /// <param name="pathFrames">The path frames.</param>
        public PathCamera(string name, Vector3 position, Vector3 lookAt, List<CameraPoint> cameraPointList, int pathFrames)
            : base(name, position, lookAt)
        {
            this.entity.AddComponent(new PathCameraBehavior(cameraPointList, pathFrames));
        }

        #endregion
    }
}
