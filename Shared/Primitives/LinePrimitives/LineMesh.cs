// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Using Statements
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using WaveEngine.Common.Attributes;
using WaveEngine.Common.Graphics;
using WaveEngine.Components.Graphics3D;
#endregion

namespace WaveEngine.Components.Primitives
{
    /// <summary>
    /// Line primitive mesh. To render this mesh use the <see cref="LineMeshRenderer3D"/> class.
    /// </summary>
    [DataContract]
    public class LineMesh : LineMeshBase
    {
        #region Properties

        /// <summary>
        /// Gets or sets a list with the points that defines the line.
        /// </summary>
        [DataMember]
        [RenderPropertyAsList(
            CustomPropertyName = "Line Points",
            Tooltip = "List with the points that defines the line",
            AddItemAction = nameof(CloneLastPoint),
            UpdateItemAction = nameof(Refresh),
            RemoveItemAction = nameof(Refresh))]
        public List<LinePointInfo> LinePoints
        {
            get
            {
                return this.linePoints;
            }

            set
            {
                this.linePoints = value;

                if (this.isInitialized)
                {
                    this.RefreshMeshes();
                }
            }
        }

        /// <summary>
        /// Gets or sets the type of the line to draw.
        /// </summary>
        [RenderProperty(
            CustomPropertyName = "Line Type",
            Tooltip = "The type of the line to draw. Use LineStrip to compose the line connecting the dots. Use LineList to compose the line with isolated, straight line segments")]
        public LineTypes LineType
        {
            get
            {
                return this.lineType;
            }

            set
            {
                if (this.lineType != value)
                {
                    this.lineType = value;

                    if (this.isInitialized)
                    {
                        this.RefreshMeshes();
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the first point of the list is appended with the last one.
        /// </summary>
        [RenderProperty(
            CustomPropertyName = "Is Loop",
            Tooltip = "Enable this to connect the first and last positions of the line. This forms a closed loop.")]
        public bool IsLoop
        {
            get
            {
                return this.isLoop;
            }

            set
            {
                if (this.isLoop != value)
                {
                    this.isLoop = value;

                    if (this.isInitialized)
                    {
                        this.RefreshMeshes();
                    }
                }
            }
        }
        #endregion

        /// <summary>
        /// Updates last point added with the previous point
        /// </summary>
        /// <param name="point">Last point added to the list</param>
        public void CloneLastPoint(LinePointInfo point)
        {
            if (this.linePoints.Count == 1)
            {
                var prev = this.linePoints.ElementAt(0);
                point.Color = Color.White;
                point.Thickness = 1;
            }
            else
            {
                var prev = this.linePoints.ElementAt(this.linePoints.Count - 2);
                point.Color = prev.Color;
                point.Position = prev.Position;
                point.Thickness = prev.Thickness;
            }

            this.RefreshMeshes();
        }

        /// <summary>
        /// Refresh mesh when a value from the list has changed
        /// </summary>
        /// <param name="point">point</param>
        public void Refresh(LinePointInfo point)
        {
            this.RefreshMeshes();
        }
    }
}
