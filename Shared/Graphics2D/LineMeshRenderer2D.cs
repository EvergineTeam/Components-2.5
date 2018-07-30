// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Using Statements
using System;
using System.Runtime.Serialization;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Math;
using WaveEngine.Components.Primitives;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
#endregion

namespace WaveEngine.Components.Graphics2D
{
    /// <summary>
    /// Renders a line mesh on the screen.
    /// </summary>
    [DataContract(Namespace = "WaveEngine.Components.Graphics2D")]
    public class LineMeshRenderer2D : Drawable2D
    {
        /// <summary>
        /// Whether this instance has been disposed.
        /// </summary>
        private bool disposed;

        /// <summary>
        /// <see cref="LineMeshBase"/> to render.
        /// </summary>
        [RequiredComponent(false)]
        protected LineMeshBase lineMesh;

        /// <summary>
        /// Initializes a new instance of the <see cref="LineMeshRenderer2D"/> class.
        /// </summary>
        public LineMeshRenderer2D()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LineMeshRenderer2D" /> class.
        /// </summary>
        /// <param name="layerId">Id of the layer.</param>
        protected LineMeshRenderer2D(int layerId)
            : base(layerId)
        {
        }

        /// <inheritdoc/>
        protected override void ResolveDependencies()
        {
            base.ResolveDependencies();

            this.lineMesh.is2DMode = true;
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

            var worldTransform = this.GetWorldTransform();

            var drawOrder = this.Transform2D.DrawOrder;

            if (this.lineMesh.lineType == LineTypes.LineList)
            {
                var pos1 = Vector2.Transform(this.lineMesh.linePoints[0].Position.ToVector2(), worldTransform);
                for (int i = 0; i < this.lineMesh.linePoints.Count - 1; i += 2)
                {
                    var pos2 = Vector2.Transform(this.lineMesh.linePoints[i + 1].Position.ToVector2(), worldTransform);
                    this.RenderManager.LineBatch2D.DrawLine(pos1, pos2, this.lineMesh.linePoints[i].Color, drawOrder);

                    pos1 = pos2;
                }
            }
            else
            {
                var pos1 = Vector2.Transform(this.lineMesh.linePoints[0].Position.ToVector2(), worldTransform);
                for (int i = 0; i < this.lineMesh.linePoints.Count - 1; i++)
                {
                    var pos2 = Vector2.Transform(this.lineMesh.linePoints[i + 1].Position.ToVector2(), worldTransform);
                    this.RenderManager.LineBatch2D.DrawLine(pos1, pos2, this.lineMesh.linePoints[i].Color, drawOrder);

                    pos1 = pos2;
                }

                var lineBezierMesh = this.lineMesh as LineBezierMesh;
                if (lineBezierMesh != null)
                {
                    foreach (var point in lineBezierMesh.LinePoints)
                    {
                        var position = Vector2.Transform(point.Position.ToVector2(), worldTransform);

                        if (point.HasInboundHandleVisible())
                        {
                            var inboundHandle = Vector2.Transform((point.Position + point.InboundHandle).ToVector2(), worldTransform);
                            this.RenderManager.LineBatch2D.DrawPoint(inboundHandle, 5, Color.Yellow, drawOrder);
                            this.RenderManager.LineBatch2D.DrawLine(inboundHandle, position, Color.Black, drawOrder);
                        }

                        if (point.HasOutboundHandleVisible())
                        {
                            var outboundHandle = Vector2.Transform((point.Position + point.OutboundHandle).ToVector2(), worldTransform);
                            this.RenderManager.LineBatch2D.DrawPoint(outboundHandle, 5, Color.Yellow, drawOrder);
                            this.RenderManager.LineBatch2D.DrawLine(outboundHandle, position, Color.Black, drawOrder);
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

            this.lineMesh.Material.LayerId = this.LayerId;

            var meshGroup = this.lineMesh.Meshes;
            if (meshGroup != null)
            {
                var worldTransform = this.GetWorldTransform();

                for (int i = 0; i < meshGroup.Count; i++)
                {
                    var currentMesh = meshGroup[i];
                    this.RenderManager.DrawMesh(currentMesh, this.lineMesh.Material, ref worldTransform);
                }
            }
        }

        /// <inheritdoc />
        protected override void RefreshBoundingBox()
        {
            var rect = this.Transform2D.Rectangle;
            var origin = this.Transform2D.Origin;

            rect.X = -origin.X * rect.Width;
            rect.Y = -origin.Y * rect.Height;

            BoundingBox boundingBox = new Common.Math.BoundingBox();
            boundingBox.Min.X = rect.Left;
            boundingBox.Min.Y = rect.Top;
            boundingBox.Max.X = rect.Right;
            boundingBox.Max.Y = rect.Bottom;

            if (!this.lineMesh.UseWorldSpace)
            {
                var world = this.Transform2D.WorldTransform;
                boundingBox.Transform(ref world);
            }

            this.BoundingBox = boundingBox;
        }

        /// <inheritdoc />
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

        private Matrix GetWorldTransform()
        {
            var origin = this.Transform2D.Origin;
            var rectangle = this.Transform2D.Rectangle;
            var worldTransform = Matrix.CreateTranslation(-rectangle.X - (rectangle.Width * origin.X), -rectangle.Y - (rectangle.Height * origin.Y), 0);

            if (!this.lineMesh.UseWorldSpace)
            {
                worldTransform *= this.Transform2D.WorldTransform;
            }

            return worldTransform;
        }
    }
}
