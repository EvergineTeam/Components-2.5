// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using WaveEngine.Common.Attributes;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Math;
using WaveEngine.Components.Primitives;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
#endregion

namespace WaveEngine.Components.Graphics3D
{
    /// <summary>
    /// Renders a line mesh on the screen.
    /// </summary>
    [DataContract(Namespace = "WaveEngine.Components.Graphics3D")]
    public class LineMeshRenderer3D : Drawable3D
    {
        /// <summary>
        /// Number of instances of this component created.
        /// </summary>
        private static int instances;

        /// <summary>
        /// Whether this instance has been disposed.
        /// </summary>
        private bool disposed;

        /// <summary>
        /// Whether id is the layer.
        /// </summary>
        [DataMember]
        private int layerId;

        /// <summary>
        /// The <see cref="Transform3D"/>.
        /// </summary>
        [RequiredComponent]
        protected Transform3D transform;

        /// <summary>
        /// <see cref="LineMeshBase"/> to render.
        /// </summary>
        [RequiredComponent(false)]
        protected LineMeshBase lineMesh;

        /// <summary>
        /// Gets or sets the type of the layer.
        /// </summary>
        /// <value>
        /// The type of the layer.
        /// </value>
        [RenderPropertyAsLayer]
        public int LayerId
        {
            get
            {
                return this.layerId;
            }

            set
            {
                if (this.layerId != value)
                {
                    this.layerId = value;

                    if (this.lineMesh != null)
                    {
                        this.lineMesh.Material.LayerId = this.layerId;
                    }
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LineMeshRenderer3D"/> class.
        /// </summary>
        public LineMeshRenderer3D()
            : base("LineRenderer3D" + instances++)
        {
        }

        /// <inheritdoc/>
        protected override void DefaultValues()
        {
            base.DefaultValues();
            this.layerId = DefaultLayers.Alpha;
        }

        /// <inheritdoc/>
        protected override void ResolveDependencies()
        {
            base.ResolveDependencies();

            this.lineMesh.Material.LayerId = this.layerId;
        }

        /// <inheritdoc/>
        protected override void DrawDebugLines()
        {
            base.DrawDebugLines();

            if (this.lineMesh.InternalModel == null ||
                this.lineMesh.linePoints == null)
            {
                return;
            }

            var worldTransform = this.lineMesh.UseWorldSpace ? Matrix.Identity : this.transform.WorldTransform;

            if (this.lineMesh.lineType == LineTypes.LineList)
            {
                var pos1 = Vector3.Transform(this.lineMesh.linePoints[0].Position, worldTransform);
                for (int i = 0; i < this.lineMesh.linePoints.Count - 1; i += 2)
                {
                    var pos2 = Vector3.Transform(this.lineMesh.linePoints[i + 1].Position, worldTransform);
                    this.RenderManager.LineBatch3D.DrawLine(pos1, pos2, this.lineMesh.linePoints[i].Color);

                    pos1 = pos2;
                }
            }
            else
            {
                var pos1 = this.lineMesh.linePoints[0].Position;

                if (!this.lineMesh.UseWorldSpace)
                {
                    pos1 = Vector3.Transform(pos1, worldTransform);
                }

                for (int i = 0; i < this.lineMesh.linePoints.Count - 1; i++)
                {
                    var pos2 = this.lineMesh.linePoints[i + 1].Position;

                    if (!this.lineMesh.UseWorldSpace)
                    {
                        pos2 = Vector3.Transform(pos2, worldTransform);
                    }

                    this.RenderManager.LineBatch3D.DrawLine(pos1, pos2, this.lineMesh.linePoints[i].Color);

                    pos1 = pos2;
                }

                var lineBezierMesh = this.lineMesh as LineBezierMesh;
                if (lineBezierMesh != null)
                {
                    foreach (var point in lineBezierMesh.LinePoints)
                    {
                        var position = Vector3.Transform(point.Position, worldTransform);

                        if (point.HasInboundHandleVisible())
                        {
                            var inboundHandle = Vector3.Transform(point.Position + point.InboundHandle, worldTransform);
                            this.RenderManager.LineBatch3D.DrawPoint(inboundHandle, 0.1f, Color.Yellow);
                            this.RenderManager.LineBatch3D.DrawLine(inboundHandle, position, Color.Black);
                        }

                        if (point.HasOutboundHandleVisible())
                        {
                            var outboundHandle = Vector3.Transform(point.Position + point.OutboundHandle, worldTransform);
                            this.RenderManager.LineBatch3D.DrawPoint(outboundHandle, 0.1f, Color.Yellow);
                            this.RenderManager.LineBatch3D.DrawLine(outboundHandle, position, Color.Black);
                        }
                    }
                }
            }
        }

        /// <inheritdoc/>
        public override void Draw(TimeSpan gameTime)
        {
            if (this.lineMesh.InternalModel == null ||
                this.lineMesh.Material == null)
            {
                return;
            }

            float zOrder;
            if (this.BoundingBox.HasValue)
            {
                zOrder = Vector3.DistanceSquared(this.RenderManager.CurrentDrawingCamera3D.Position, this.BoundingBox.Value.Center);
            }
            else
            {
                zOrder = Vector3.DistanceSquared(this.RenderManager.CurrentDrawingCamera3D.Position, this.transform.Position);
            }

            List<Mesh> meshGroup = this.lineMesh.Meshes;
            if (meshGroup != null)
            {
                var transform = this.lineMesh.UseWorldSpace ? Matrix.Identity : this.transform.WorldTransform;

                for (int i = 0; i < meshGroup.Count; i++)
                {
                    Mesh currentMesh = meshGroup[i];
                    currentMesh.ZOrder = zOrder;

                    this.RenderManager.DrawMesh(currentMesh, this.lineMesh.Material, ref transform, this.Owner.IsFinalStatic);
                }
            }
        }

        /// <summary>
        /// Refresh the bounding box of this drawable
        /// </summary>
        protected override void RefreshBoundingBox()
        {
            if (this.lineMesh != null && this.lineMesh.BoundingBox.HasValue)
            {
                var bbox = this.lineMesh.BoundingBox.Value;

                if (!this.lineMesh.UseWorldSpace)
                {
                    var world = this.transform.WorldTransform;
                    bbox.Transform(ref world);
                }

                this.BoundingBox = bbox;
            }
            else
            {
                this.BoundingBox = null;
            }
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    this.disposed = true;
                }
            }
        }
    }
}
