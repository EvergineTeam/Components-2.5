// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Using Statements
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Math;
using WaveEngine.Components.Graphics2D;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using System.Reflection;
using WaveEngine.Common.Attributes;
using WaveEngine.Common.Helpers;
#endregion

namespace WaveEngine.Components.Shared.Graphics3D
{
    /// <summary>
    /// Renders a <see cref="Billboard"/> on the screen.
    /// The owner <see cref="Entity"/> must contain the <see cref="Billboard"/> to be drawn, plus a <see cref="Transform3D"/>.
    /// </summary>
    [DataContract(Namespace = "WaveEngine.Components.Shared.Graphic3D")]
    public class BillboardRenderer : Drawable3D
    {
        /// <summary>
        /// Number of instances of this component created.
        /// </summary>
        private static int instances;

        /// <summary>
        /// Required <see cref="Transform3D"/>.
        /// It provides where to draw the <see cref="Billboard"/>, which rotation to apply and which scale.
        /// </summary>
        [RequiredComponent]
        public Billboard Billboard = null;

        /// <summary>
        /// Required <see cref="Transform3D"/>.
        /// It provides where to draw the <see cref="Billboard"/>, which rotation to apply and which scale.
        /// </summary>
        [RequiredComponent]
        public Transform3D Transform3D = null;

        /// <summary>
        /// The layer type
        /// </summary>
        [DataMember]
        private int layerId;

        /// <summary>
        /// The layer
        /// </summary>
        private RenderLayer layer;

        #region Properties

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
                this.layerId = value;

                if (this.RenderManager != null)
                {
                    this.layer = this.RenderManager.FindLayer(this.layerId);
                }
            }
        }
        #endregion

        #region Initialize

        /// <summary>
        /// Initializes a new instance of the <see cref="BillboardRenderer" /> class.
        /// </summary>
        public BillboardRenderer()
            : this(DefaultLayers.Alpha)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BillboardRenderer" /> class.
        /// </summary>
        /// <param name="layerId">
        /// Layer type (available at <see cref="DefaultLayers"/>).
        /// Example: new SpriteRenderer(DefaultLayers.Alpha)
        /// </param>
        public BillboardRenderer(int layerId)
            : base("BillboardRenderer" + instances++)
        {
            this.LayerId = layerId;
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// Performs further custom initialization for this instance.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
            this.LayerId = this.layerId;
        }

        /// <summary>
        /// Allows to perform custom drawing.
        /// </summary>
        /// <param name="gameTime">The elapsed game time.</param>
        /// <remarks>
        /// This method will only be called if all the following points are true:
        /// <list type="bullet">
        /// <item>
        /// <description>The parent of the owner <see cref="Entity" /> of the <see cref="Drawable" /> cascades its visibility to its children and it is visible.</description>
        /// </item>
        /// <item>
        /// <description>The <see cref="Drawable" /> is active.</description>
        /// </item>
        /// <item>
        /// <description>The owner <see cref="Entity" /> of the <see cref="Drawable" /> is active and visible.</description>
        /// </item>
        /// </list>
        /// </remarks>
        public override void Draw(TimeSpan gameTime)
        {
            if (this.Billboard == null || this.Billboard.Texture == null || this.layer == null)
            {
                return;
            }

            Vector2 size = this.Billboard.Size * this.Transform3D.Scale.ToVector2();

            if (this.Billboard.BillboardType == BillboardType.PointOrientation)
            {
                this.layer.BillboardBatch.DrawBillboard(
                    this.Billboard.Texture,
                    this.Transform3D.Position,
                    this.Billboard.Rotation,
                    size,
                    this.Billboard.Origin,
                    this.Billboard.TintColor);
            }
            else
            {
                Vector3 axis = Vector3.Transform(Vector3.Up, this.Transform3D.Orientation);
                axis = this.Transform3D.WorldTransform.Up;

                this.layer.BillboardBatch.DrawBillboard(
                    this.Billboard.Texture,
                    this.Transform3D.Position,
                    this.Billboard.Rotation,
                    size,
                    this.Billboard.Origin,
                    this.Billboard.TintColor,
                    this.Transform3D.WorldTransform.Up);
            }
        }
        #endregion

        #region Private Methods

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && this.Billboard != null)
            {
                this.Billboard.Dispose();
            }
        }
        #endregion
    }
}
