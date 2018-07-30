// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Using Statements
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using WaveEngine.Common.Attributes;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Graphics.VertexFormats;
using WaveEngine.Common.Helpers;
using WaveEngine.Common.Math;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Services;
using WaveEngine.Materials;
#endregion

namespace WaveEngine.Components.Toolkit
{
    /// <summary>
    /// Renderer of the 3d text control
    /// </summary>
    [DataContract(Namespace = "WaveEngine.Components.Toolkit")]
    public class TextRenderer3D : Drawable3D
    {
        /// <summary>
        /// The text component
        /// </summary>
        [RequiredComponent]
        protected TextComponent textComponent;

        /// <summary>
        /// The entity's transform
        /// </summary>
        [RequiredComponent]
        protected Transform3D transform;

        /// <summary>
        /// The text material
        /// </summary>
        private StandardMaterial material;

        #region Properties

        /// <summary>
        /// Gets or sets the type of the layer.
        /// </summary>
        /// <value>
        /// The type of the layer.
        /// </value>
        [RenderPropertyAsLayer]
        [DataMember]
        public int LayerId;
        #endregion

        #region Initialize
        #endregion

        #region Public Methods

        /// <summary>
        /// Initializes default values
        /// </summary>
        protected override void DefaultValues()
        {
            base.DefaultValues();
            this.LayerId = DefaultLayers.Alpha;
        }

        /// <summary>
        /// Initializes the instance.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();

            this.material = new StandardMaterial() { DiffuseColor = Color.White, LightingEnabled = false, LayerId = this.LayerId };
        }

        /// <summary>
        /// Draws the text
        /// </summary>
        /// <param name="gameTime">The ellapsed gameTime</param>
        public override void Draw(TimeSpan gameTime)
        {
            if ((this.RenderManager == null) || (this.textComponent.SpriteFont == null))
            {
                return;
            }

            this.material.Diffuse1 = this.textComponent.SpriteFont.FontTexture;
            this.material.DiffuseColor = this.textComponent.Foreground;
            this.material.Alpha = this.textComponent.Alpha;

            if (this.material.LayerId != this.LayerId)
            {
                this.material.LayerId = this.LayerId;
            }

            var worldTransform = this.transform.WorldTransform;
            var scaleTransform = Matrix.CreateScale(new Vector3(0.1f));

            Matrix.Multiply(ref scaleTransform, ref worldTransform, out worldTransform);

            float zOrder = Vector3.DistanceSquared(this.RenderManager.CurrentDrawingCamera3D.Position, this.transform.Position);

            for (int i = 0; i < this.textComponent.MeshCount; i++)
            {
                var mesh = this.textComponent.Meshes[i];
                mesh.ZOrder = zOrder;

                this.RenderManager.DrawMesh(mesh, this.material, ref worldTransform);
            }
        }
        #endregion

        #region Private Methods

        /// <summary>
        /// Disposes the text control
        /// </summary>
        /// <param name="disposing">If it's disposing</param>
        protected override void Dispose(bool disposing)
        {
        }

        /// <summary>
        /// Draws the debug lines
        /// </summary>
        protected override void DrawDebugLines()
        {
            var lB = this.RenderManager.LineBatch3D;
            var color = this.textComponent.Foreground;

            var worldTransform = this.transform.WorldTransform;
            var scaleTransform = Matrix.CreateScale(new Vector3(0.1f));

            Matrix.Multiply(ref scaleTransform, ref worldTransform, out worldTransform);

            foreach (var c in this.textComponent.CharInfoList)
            {
                var p = c.Position;

                var pTL = Vector3.Transform(new Vector3(p.Left, p.Top, 0), worldTransform);
                var pTR = Vector3.Transform(new Vector3(p.Right, p.Top, 0), worldTransform);
                var pBL = Vector3.Transform(new Vector3(p.Left, p.Bottom, 0), worldTransform);
                var pBR = Vector3.Transform(new Vector3(p.Right, p.Bottom, 0), worldTransform);

                lB.DrawLine(pTL, pTR, color);
                lB.DrawLine(pTR, pBR, color);
                lB.DrawLine(pBR, pBL, color);
                lB.DrawLine(pBL, pTL, color);
            }
        }

        /// <summary>
        /// Refresh the bounding box of this drawable
        /// </summary>
        protected override void RefreshBoundingBox()
        {
            this.BoundingBox = this.textComponent.BoundingBox;

            if (this.BoundingBox.HasValue)
            {
                Matrix worldTransform = this.transform.WorldTransform;
                Matrix scaleTransform = Matrix.CreateScale(new Vector3(0.1f));
                Matrix.Multiply(ref scaleTransform, ref worldTransform, out worldTransform);

                var bbox = this.BoundingBox.Value;
                bbox.Transform(ref worldTransform);

                this.BoundingBox = bbox;
            }
        }
        #endregion
    }
}
