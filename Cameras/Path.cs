#region File Description
//-----------------------------------------------------------------------------
// Path
// Copyright © 2014 Wave Corporation
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Usings Statements
using System;
using System.Collections.Generic;
using WaveEngine.Common.Math;
#endregion

namespace WaveEngine.Components.Cameras
{
    /// <summary>
    /// A collection of <see cref="CameraPoint" />s.
    /// </summary>
    internal sealed class Path
    {
        // REVIEW: esta variable no deberia existir, se deberia pasar la lista de points del constructor al BuildPath
        // Precalculated key frames

        /// <summary>
        /// Whether this path should loop.
        /// </summary>
        public bool LoopEnabled;

        /// <summary>
        /// The cam index.
        /// </summary>
        private int camIndex;

        /// <summary>
        /// The current path points.
        /// </summary>
        private List<CameraPoint> currentPathPoints;

        // Current calculated point

        /// <summary>
        /// The current point.
        /// </summary>
        public CameraPoint CurrentPoint;

        /// <summary>
        /// The key frames.
        /// </summary>
        private List<CameraPoint> keyFrames;

        // REVIEW: esta variable no deberia existir, se deberia pasar del constructor al BuildPath
        // Number of interpolated frames between each camera point

        /// <summary>
        /// The path steps.
        /// </summary>
        private int pathSteps;

        // Current interpolated point index
        #region Initialize

        /// <summary>
        /// Initializes a new instance of the <see cref="Path"/> class.
        /// </summary>
        /// <param name="cameraPoints">
        /// The camera positions.
        /// </param>
        /// <exception cref="System.ArgumentException">
        /// If cameraPoints is null or empty.
        /// </exception>
        public Path(List<CameraPoint> cameraPoints)
        {
            if (cameraPoints == null || cameraPoints.Count == 0)
            {
                throw new ArgumentException("cameraPoints can't be null or empty");
            }

            this.Initialize(cameraPoints, cameraPoints.Count * 20);

            // REVIEW: esto esta bien de verdad? No deberia ser 20 a secas?
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Path"/> class.
        /// </summary>
        /// <param name="cameraPoints">
        /// The camera positions.
        /// </param>
        /// <param name="interpoledPointsPerCameraPosition">
        /// The number of points to interpolate between each camera position.
        /// </param>
        /// <exception cref="System.ArgumentException">
        /// If cameraPoints is null or empty.
        /// </exception>
        public Path(List<CameraPoint> cameraPoints, int interpoledPointsPerCameraPosition)
        {
            if (cameraPoints == null || cameraPoints.Count == 0)
            {
                throw new ArgumentException("cameraPoints can't be null or empty");
            }

            this.Initialize(cameraPoints, interpoledPointsPerCameraPosition);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Get the next point of the path.
        /// </summary>
        /// <param name="milliseconds">
        /// The milliseconds to advance in the path.
        /// </param>
        /// <returns>
        /// The next camera point in the path.
        /// </returns>
        public bool Evaluate(int milliseconds)
        {
            bool result = true;

            if (this.LoopEnabled)
            {
                this.camIndex = (this.camIndex + milliseconds) % this.currentPathPoints.Count;
            }
            else
            {
                this.camIndex = this.camIndex + milliseconds;
            }

            if (this.camIndex >= this.currentPathPoints.Count)
            {
                result = false;
            }
            else
            {
                this.CurrentPoint = this.currentPathPoints[this.camIndex];
            }

            return result;
        }

        /// <summary>
        ///     Restarts the path.
        /// </summary>
        public void ResetIndex()
        {
            this.camIndex = 0;
        }

        #endregion

        #region Private Methods
        // REVIEW: este metodo y el siguiente se deberian fusionar

        /// <summary>
        /// The build path.
        /// </summary>
        /// <param name="pointsPerElement">
        /// The points per element.
        /// </param>
        private void BuildPath(int pointsPerElement)
        {
            this.pathSteps = pointsPerElement;
            this.BuildPath();
        }

        /// <summary>
        ///     Constructs an interpolated path based on the Key Frames.
        ///     Options are Linear or Cubic interpolation
        /// </summary>
        private void BuildPath()
        {
            this.currentPathPoints.Clear();

            // a point on a cubic spline is affected by every other point
            // so we need to build the cubic equations for each control point
            // by looking at the entire curve. This is what calculateCubicSpline does
            var looks = new Vector3[this.keyFrames.Count];
            var positions = new Vector3[this.keyFrames.Count];
            var ups = new Vector3[this.keyFrames.Count];

            for (int i = 0; i < this.keyFrames.Count; i++)
            {
                looks[i] = this.keyFrames[i].LookAt;
                positions[i] = this.keyFrames[i].Position;
                ups[i] = this.keyFrames[i].Up;
            }

            int count = this.keyFrames.Count - 1;
            Spline[] posCubic;
            Spline.CalculateCubicSpline(ref count, ref positions, out posCubic);
            Spline[] lookCubic;
            Spline.CalculateCubicSpline(ref count, ref looks, out lookCubic);
            Spline[] upCubic;
            Spline.CalculateCubicSpline(ref count, ref ups, out upCubic);

            for (int i = 0; i < this.keyFrames.Count - 1; i++)
            {
                for (int j = 0; j < this.pathSteps; j++)
                {
                    float k = j / (float)(this.pathSteps - 1);

                    Vector3 center = posCubic[i].GetPointOnSpline(k);
                    Vector3 up = upCubic[i].GetPointOnSpline(k);
                    Vector3 look = lookCubic[i].GetPointOnSpline(k);

                    var cam = new CameraPoint(ref center, ref look, ref up);
                    this.currentPathPoints.Add(cam);
                }
            }
        }

        /// <summary>
        /// Initialize the path context
        /// </summary>
        /// <param name="cameraPoints">
        /// A few camera points to interpolate
        /// </param>
        /// <param name="pointsPerElement">
        /// The points per camera point to calculate
        /// </param>
        private void Initialize(List<CameraPoint> cameraPoints, int pointsPerElement)
        {
            this.keyFrames = cameraPoints;
            this.currentPathPoints = new List<CameraPoint>();
            this.BuildPath(pointsPerElement);
            this.LoopEnabled = true;
        }

        #endregion
    }
}