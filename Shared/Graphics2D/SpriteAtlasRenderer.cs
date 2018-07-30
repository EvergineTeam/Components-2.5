// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Using Statements
using System;
using System.Runtime.Serialization;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Math;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
#endregion

namespace WaveEngine.Components.Graphics2D
{
    /// <summary>
    /// Renders a Image contained in a <see cref="SpriteAtlas"/> on the screen.
    /// </summary>
    [DataContract(Namespace = "WaveEngine.Components.Graphics2D")]
    public class SpriteAtlasRenderer : Drawable2D
    {
        /// <summary>
        /// Number of instances of this component created.
        /// </summary>
        private static int instances;

        /// <summary>
        /// <see cref="SpriteAtlas"/> to render.
        /// </summary>
        [RequiredComponent(false)]
        public SpriteAtlas Sprite;

        #region Initialize

        /// <summary>
        /// Initializes a new instance of the <see cref="SpriteAtlasRenderer" /> class.
        /// </summary>
        public SpriteAtlasRenderer()
            : this(DefaultLayers.Alpha)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SpriteAtlasRenderer" /> class.
        /// </summary>
        /// <param name="layerId">Type of the layer.</param>
        public SpriteAtlasRenderer(int layerId)
            : base("SpriteAtlasRenderer" + instances++, layerId)
        {
            this.LayerId = layerId;
        }
        #endregion

        #region Public Methods

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
            if (this.Sprite.SpriteSheet != null &&
                this.Sprite.SpriteSheet.Texture != null &&
                this.Transform2D.GlobalOpacity > Drawable2D.Delta)
            {
                Color color = this.Sprite.TintColor * this.Transform2D.GlobalOpacity;
                Vector2 origin = this.Transform2D.Origin;

                Matrix spriteMatrix = this.Transform2D.WorldTransform;
                this.layer.SpriteBatch.Draw(
                    this.Sprite.SpriteSheet.Texture,
                    this.Sprite.SourceRectangle,
                    ref color,
                    ref origin,
                    this.Transform2D.Effect,
                    ref spriteMatrix,
                    this.Transform2D.DrawOrder);
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
            base.Dispose(disposing);
            if (disposing && this.Sprite != null)
            {
                this.Sprite.Dispose();
            }
        }

        #endregion
    }
}
