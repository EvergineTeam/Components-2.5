// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Using Statements
using System.Collections.Generic;
using System.Runtime.Serialization;
using WaveEngine.Common.Attributes;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Math;
using WaveEngine.Components.Graphics3D;
#endregion

namespace WaveEngine.Components.Primitives
{
    /// <summary>
    /// Bezier line primitive mesh component. To render this mesh use the <see cref="LineMeshRenderer3D"/> class.
    /// </summary>
    [DataContract]
    public class LineBezierMesh : LineMeshBase
    {
        [DataMember]
        private BezierTypes bezierType;

        [DataMember]
        private List<BezierPointInfo> bezierLinePoints;

        [DataMember]
        private int resolution;

        #region Properties

        /// <summary>
        /// Gets or sets the type of bezier curve.
        /// </summary>
        public BezierTypes Type
        {
            get
            {
                return this.bezierType;
            }

            set
            {
                this.bezierType = value;

                if (this.isInitialized)
                {
                    this.RefreshBezierPointsInfo();
                    this.RefreshMeshes();
                }
            }
        }

        /// <summary>
        /// Gets or sets a list with the points that defines the line.
        /// </summary>
        [RenderPropertyAsList(AddItemAction = nameof(CloneLastPoint), UpdateItemAction = nameof(RefreshItems), RemoveItemAction = nameof(RefreshItems))]
        public List<BezierPointInfo> LinePoints
        {
            get
            {
                return this.bezierLinePoints;
            }

            set
            {
                this.bezierLinePoints = value;

                if (this.isInitialized)
                {
                    this.RefreshBezierPointsInfo();
                    this.RefreshMeshes();
                }
            }
        }

        /// <summary>
        /// Gets or sets the resolution of each bezier segment
        /// </summary>
        [RenderPropertyAsInput(MinLimit = 3, MaxLimit = 50)]
        public int Resolution
        {
            get
            {
                return this.resolution;
            }

            set
            {
                if (this.resolution != value)
                {
                    this.resolution = value;

                    if (this.isInitialized)
                    {
                        this.RefreshMeshes();
                    }
                }
            }
        }

        #endregion

        /// <inheritdoc/>
        protected override void DefaultValues()
        {
            base.DefaultValues();

            this.bezierLinePoints = new List<BezierPointInfo>();
            this.bezierType = BezierTypes.Quadratic;
            this.resolution = 10;
            this.lineType = LineTypes.LineStrip;
        }

        /// <inheritdoc/>
        protected override void Initialize()
        {
            base.Initialize();

            this.RefreshBezierPointsInfo();
        }

        /// <inheritdoc/>
        protected override void RefreshMeshes()
        {
            if (this.bezierLinePoints == null || this.bezierLinePoints.Count < 2)
            {
                return;
            }

            this.linePoints.Clear();

            var firstPoint = this.bezierLinePoints[0];

            for (int i = 1; i < this.bezierLinePoints.Count; i++)
            {
                var secondPoint = this.bezierLinePoints[i];

                int r = i > 1 ? 1 : 0;
                for (; r <= this.resolution; r++)
                {
                    var t = (float)r / this.resolution;

                    var point = new LinePointInfo()
                    {
                        Color = Color.Lerp(ref firstPoint.Color, ref secondPoint.Color, t),
                        Thickness = MathHelper.Lerp(firstPoint.Thickness, secondPoint.Thickness, t)
                    };

                    if (this.bezierType == BezierTypes.Quadratic)
                    {
                        var p1 = secondPoint.Position + secondPoint.InboundHandle;
                        point.Position = this.CalculateQuadraticPoint(t, ref firstPoint.Position, ref p1, ref secondPoint.Position);
                    }
                    else if (this.bezierType == BezierTypes.Cubic)
                    {
                        var p1 = firstPoint.Position + firstPoint.OutboundHandle;
                        var p2 = secondPoint.Position + secondPoint.InboundHandle;
                        point.Position = this.CalculateCubicPoint(t, ref firstPoint.Position, ref p1, ref p2, ref secondPoint.Position);
                    }

                    this.linePoints.Add(point);
                }

                firstPoint = secondPoint;
            }

            base.RefreshMeshes();
        }

        private Vector3 CalculateQuadraticPoint(float t, ref Vector3 p0, ref Vector3 p1, ref Vector3 p2)
        {
            // B(t) = (1-t)^2 * P0 + 2 * (1-t) * t * P1 + t^2 * P2
            var u = 1 - t;
            return (u * u * p0) + (2 * u * t * p1) + (t * t * p2);
        }

        private Vector3 CalculateCubicPoint(float t, ref Vector3 p0, ref Vector3 p1, ref Vector3 p2, ref Vector3 p3)
        {
            // B(t) = (1-t)^3 * P0 + 3 * (1-t)^2 * t * P1 + 3 * (1-t) * t^2 * P2 + t^3 * P3
            var u = 1 - t;
            var uu = u * u;
            var tt = t * t;
            return (u * uu * p0) + (3 * uu * t * p1) + (3 * u * tt * p2) + (t * tt * p3);
        }

        private void RefreshBezierPointsInfo()
        {
            if (this.bezierLinePoints == null || this.bezierLinePoints.Count == 0)
            {
                return;
            }

            foreach (var pointInfo in this.bezierLinePoints)
            {
                pointInfo.IsFirstpoint = false;
                pointInfo.IsLastpoint = false;
                pointInfo.BezierType = this.bezierType;
            }

            this.bezierLinePoints[0].IsFirstpoint = true;
            this.bezierLinePoints[this.bezierLinePoints.Count - 1].IsLastpoint = true;
        }

        /// <summary>
        /// Updates last point added with the previous point
        /// </summary>
        /// <param name="point">Last point added to the list</param>
        public void CloneLastPoint(BezierPointInfo point)
        {
            if (this.bezierLinePoints.Count > 1)
            {
                var prev = this.bezierLinePoints[this.bezierLinePoints.Count - 2];
                prev.OutboundHandle = -prev.InboundHandle;

                point.Position = prev.Position + (2 * prev.OutboundHandle);
                point.InboundHandle = prev.InboundHandle;
                point.Color = prev.Color;
                point.Thickness = prev.Thickness;
            }
            else
            {
                point.Color = Color.White;
                point.Thickness = 0.1f;
            }

            this.RefreshBezierPointsInfo();
            this.RefreshMeshes();
        }

        /// <summary>
        /// Refresh mesh when a value from the list has changed
        /// </summary>
        /// <param name="point">point</param>
        public void RefreshItems(BezierPointInfo point)
        {
            this.RefreshBezierPointsInfo();
            this.RefreshMeshes();
        }
    }
}
