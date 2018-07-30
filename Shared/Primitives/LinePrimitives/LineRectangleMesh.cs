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
    /// Line primitive mesh. To render this mesh use the <see cref="LineMeshRenderer3D"/> class.
    /// </summary>
    [DataContract]
    public class LineRectangleMesh : LineMeshBase
    {
        [DataMember]
        private Vector2 origin;

        [DataMember]
        private float width;

        [DataMember]
        private float height;

        [DataMember]
        private float thickness;

        [DataMember]
        private Color color;

        #region Properties

        /// <summary>
        /// Gets or sets the origin (also known as pivot) from where the rectangle scales, rotates and translates.
        /// Its values are included in [0, 1] where (0, 0) indicates the top left corner.
        /// Such values are percentages where 1 means the 100% of the rectangle's width/height.
        /// </summary>
        [RenderProperty(Tooltip = "The origin (also known as pivot) from where the rectangle scales, rotates and translates. Its values are included in [0, 1] where (0, 0) indicates the top left corner. Such values are percentages where 1 means the 100% of the rectangle's width/height.")]
        public Vector2 Origin
        {
            get
            {
                return this.origin;
            }

            set
            {
                if (this.origin != value)
                {
                    this.origin = value;

                    if (this.isInitialized)
                    {
                        this.RefreshMeshes();
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the width of the rectangle to draw.
        /// </summary>
        [RenderProperty(Tooltip = "Rectangle width")]
        public float Width
        {
            get
            {
                return this.width;
            }

            set
            {
                if (this.width != value)
                {
                    this.width = value;

                    if (this.isInitialized)
                    {
                        this.RefreshMeshes();
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the height of the rectangle to draw.
        /// </summary>
        [RenderProperty(Tooltip = "Rectangle height")]
        public float Height
        {
            get
            {
                return this.height;
            }

            set
            {
                if (this.height != value)
                {
                    this.height = value;

                    if (this.isInitialized)
                    {
                        this.RefreshMeshes();
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the thickness of the rectangle's line to draw.
        /// </summary>
        [RenderPropertyAsFInput(MinLimit = 0, Tooltip = "Thickness of the rectangle's line")]
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
        [RenderProperty(Tooltip = "Color for the line, or tint color for the texture")]
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

        #endregion

        /// <inheritdoc/>
        protected override void DefaultValues()
        {
            base.DefaultValues();

            this.color = Color.White;
            this.origin = Vector2.Center;
            this.width = 1;
            this.height = 1;
            this.thickness = 0.1f;
            this.lineType = LineTypes.LineStrip;
            this.isLoop = true;
        }

        /// <summary>
        /// Refresh meshes method
        /// </summary>
        protected override void RefreshMeshes()
        {
            this.linePoints = new List<LinePointInfo>(4);

            var size = new Vector2(this.width, this.height);

            var tmp = this.origin * new Vector2(1, -1);

            this.linePoints.Add(this.GetLinePoint((Vector2.Zero - tmp) * size));
            this.linePoints.Add(this.GetLinePoint((Vector2.UnitX - tmp) * size));
            this.linePoints.Add(this.GetLinePoint((new Vector2(1, -1) - tmp) * size));
            this.linePoints.Add(this.GetLinePoint((-Vector2.UnitY - tmp) * size));

            base.RefreshMeshes();
        }

        private LinePointInfo GetLinePoint(Vector2 position)
        {
            return new LinePointInfo()
            {
                Position = position.ToVector3(0),
                Thickness = this.thickness,
                Color = this.color,
            };
        }
    }
}
