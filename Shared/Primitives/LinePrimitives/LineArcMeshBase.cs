// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Using Statements
using System;
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
    /// Arc primitive mesh component. To render this mesh use the <see cref="LineMeshRenderer3D"/> class.
    /// </summary>
    [DataContract]
    public abstract class LineArcMeshBase : LineMeshBase
    {
        /// <summary>
        /// The radius
        /// </summary>
        [DataMember]
        protected Vector2 radius;

        /// <summary>
        /// Thickness of the lines of the polygon
        /// </summary>
        [DataMember]
        protected float thickness;

        /// <summary>
        /// Tint color for the texture
        /// </summary>
        [DataMember]
        protected Color color;

        /// <summary>
        /// The angle
        /// </summary>
        [DataMember]
        protected float angle;

        /// <summary>
        /// The tessellation
        /// </summary>
        [DataMember]
        protected int tessellation;

        /// <summary>
        /// Gets or sets the radius
        /// </summary>
        [RenderProperty(Tooltip = "Polygon radius")]
        public Vector2 Radius
        {
            get
            {
                return this.radius;
            }

            set
            {
                if (this.radius != value)
                {
                    this.radius = value;

                    if (this.isInitialized)
                    {
                        this.RefreshMeshes();
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the line thickness
        /// </summary>
        [RenderPropertyAsFInput(MinLimit = 0, Tooltip = "Line thickness")]
        public float Thickness
        {
            get
            {
                return this.thickness;
            }

            set
            {
                if (this.thickness != value)
                {
                    this.thickness = value;

                    if (this.isInitialized)
                    {
                        this.RefreshMeshes();
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the color for the line, or tint color for the texture
        /// </summary>
        [RenderProperty(
            CustomPropertyName = "Tint Color",
            Tooltip = "Color for the line, or tint color for the texture")]
        public Color Color
        {
            get
            {
                return this.color;
            }

            set
            {
                if (this.color != value)
                {
                    this.color = value;

                    if (this.isInitialized)
                    {
                        this.RefreshMeshes();
                    }
                }
            }
        }

        /// <inheritdoc/>
        protected override void DefaultValues()
        {
            base.DefaultValues();

            this.lineType = LineTypes.LineStrip;
            this.radius = Vector2.One;
            this.thickness = 0.1f;
            this.color = Color.White;
            this.angle = MathHelper.TwoPi;
            this.tessellation = 16;
        }

        /// <summary>
        /// Refresh meshes method
        /// </summary>
        protected override void RefreshMeshes()
        {
            var isCircle = Math.Abs(this.angle - MathHelper.TwoPi) < MathHelper.Epsilon;
            var iterations = isCircle ? this.tessellation - 1 : this.tessellation;
            this.isLoop = isCircle;

            this.linePoints = new List<LinePointInfo>(iterations);

            for (int i = 0; i <= iterations; i++)
            {
                float percent = i / (float)this.tessellation;
                float angleStep = percent * this.angle;

                float dx = (float)Math.Cos(angleStep);
                float dy = (float)Math.Sin(angleStep);

                var radiusDirection = new Vector2(-dx, dy);

                var arcPosition = radiusDirection * this.radius;

                this.linePoints.Add(new LinePointInfo()
                {
                    Position = arcPosition.ToVector3(0),
                    Thickness = this.thickness,
                    Color = this.color
                });
            }

            base.RefreshMeshes();
        }
    }
}
