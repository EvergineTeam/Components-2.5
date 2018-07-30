// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using WaveEngine.Common.Attributes;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Math;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Managers;
#endregion

namespace WaveEngine.Components.Graphics2D
{
    /// <summary>
    /// Renders a <see cref="Sprite"/> on the screen.
    /// The owner <see cref="Entity"/> must contain the <see cref="Sprite"/> to be drawn, plus a <see cref="Transform2D"/>.
    /// </summary>
    [DataContract(Namespace = "WaveEngine.Components.Graphics2D")]
    public class SpriteRenderer : Drawable2D
    {
        /// <summary>
        /// Number of instances of this component created.
        /// </summary>
        private static int instances;

        /// <summary>
        /// Indicates when the slice cache must be refreshed
        /// </summary>
        private bool isCacheRefreshNeeded;

        /// <summary>
        /// Slice cache items
        /// </summary>
        private List<SliceCacheItem> sliceCacheItems;

        /// <summary>
        /// Required <see cref="Sprite"/>.
        /// It provides the in memory representation for a visual asset.
        /// </summary>
        [RequiredComponent(false)]
        public Sprite Sprite = null;

        /// <summary>
        /// Gets or sets the draw mode
        /// </summary>
        [DataMember]
        public SpriteDrawMode DrawMode { get; set; }

        #region Initialize

        /// <summary>
        /// Initializes a new instance of the <see cref="SpriteRenderer" /> class.
        /// </summary>
        public SpriteRenderer()
            : this(DefaultLayers.Alpha)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SpriteRenderer" /> class.
        /// </summary>
        /// <param name="layerId">
        /// Layer type (available at <see cref="DefaultLayers"/>).
        /// Example: new SpriteRenderer(DefaultLayers.Alpha)
        /// </param>
        public SpriteRenderer(int layerId)
            : base("SpriteRenderer" + instances++, layerId)
        {
            this.LayerId = layerId;
        }

        /// <summary>
        /// Sets the default values
        /// </summary>
        protected override void DefaultValues()
        {
            base.DefaultValues();

            this.DrawMode = SpriteDrawMode.Simple;
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
            if (this.layer == null)
            {
                return;
            }

            if ((this.Sprite.Texture != null || this.Sprite.Material != null)
                && this.Transform2D.GlobalOpacity > Drawable2D.Delta)
            {
                Color color = this.Sprite.TintColor * this.Transform2D.GlobalOpacity;

                switch (this.DrawMode)
                {
                    default:
                    case SpriteDrawMode.Simple:
                        this.DrawSimple(ref color);
                        break;
                    case SpriteDrawMode.Sliced:
                        this.DrawSlice(ref color);
                        break;
                }
            }
        }
        #endregion

        #region Private Methods

        /// <summary>
        /// Resolve entity dependencies
        /// </summary>
        protected override void ResolveDependencies()
        {
            base.ResolveDependencies();

            this.Transform2D.TransformChanged -= this.OnTransform2DOrSpritePropertyChanged;
            this.Transform2D.TransformChanged += this.OnTransform2DOrSpritePropertyChanged;

            this.Transform2D.OriginChanged -= this.OnTransform2DOrSpritePropertyChanged;
            this.Transform2D.OriginChanged += this.OnTransform2DOrSpritePropertyChanged;

            this.Transform2D.EffectChanged -= this.OnTransform2DOrSpritePropertyChanged;
            this.Transform2D.EffectChanged += this.OnTransform2DOrSpritePropertyChanged;

            this.Sprite.TextureChanged -= this.OnTransform2DOrSpritePropertyChanged;
            this.Sprite.TextureChanged += this.OnTransform2DOrSpritePropertyChanged;
        }

        /// <summary>
        /// Deletes the dependencies.
        /// </summary>
        protected override void DeleteDependencies()
        {
            this.Transform2D.TransformChanged -= this.OnTransform2DOrSpritePropertyChanged;
            this.Transform2D.OriginChanged -= this.OnTransform2DOrSpritePropertyChanged;
            this.Transform2D.EffectChanged -= this.OnTransform2DOrSpritePropertyChanged;
            this.Sprite.TextureChanged -= this.OnTransform2DOrSpritePropertyChanged;

            base.DeleteDependencies();
        }

        /// <summary>
        /// Occurs when transform matrix changes
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event args.</param>
        private void OnTransform2DOrSpritePropertyChanged(object sender, EventArgs e)
        {
            this.isCacheRefreshNeeded = true;
        }

        /// <summary>
        /// Draws the sprite using the Simple drawing mode
        /// </summary>
        /// <param name="color">The color</param>
        private void DrawSimple(ref Color color)
        {
            Matrix spriteMatrix = this.Transform2D.WorldTransform;
            Vector2 origin = this.Transform2D.Origin;

            if (this.Sprite.Material == null)
            {
                this.layer.SpriteBatch.Draw(
                    this.Sprite.Texture,
                    this.Sprite.SourceRectangle,
                    ref color,
                    ref origin,
                    this.Transform2D.Effect,
                    ref spriteMatrix,
                    this.Transform2D.DrawOrder);
            }
            else
            {
                var rectangle = this.Transform2D.Rectangle;
                this.layer.SpriteBatch.Draw(
                    this.Sprite.Material,
                    rectangle,
                    this.Sprite.SourceRectangle,
                    ref color,
                    ref origin,
                    this.Transform2D.Effect,
                    ref spriteMatrix,
                    this.Transform2D.DrawOrder);
            }
        }

        /// <summary>
        /// Draws the sprite using the Slice drawing mode
        /// </summary>
        /// <param name="color">The color</param>
        private void DrawSlice(ref Color color)
        {
            if (this.isCacheRefreshNeeded || this.sliceCacheItems == null)
            {
                this.RefreshSliceCache();
                this.isCacheRefreshNeeded = false;
            }

            var origin = Vector2.Center;
            foreach (var item in this.sliceCacheItems)
            {
                var finalColor = this.RenderManager.ShouldDrawFlag(DebugLinesFlags.DebugAlphaOpacity) ? color * item.DebugTintColor : color;

                this.layer.SpriteBatch.Draw(
                this.Sprite.Texture,
                item.SourceRectangle,
                ref finalColor,
                ref origin,
                SpriteEffects.None,
                ref item.WorldMatrix,
                this.Transform2D.DrawOrder);
            }
        }

        /// <summary>
        /// Refresh the slice cache
        /// </summary>
        private void RefreshSliceCache()
        {
            if (this.sliceCacheItems == null)
            {
                this.sliceCacheItems = new List<SliceCacheItem>();
            }

            this.sliceCacheItems.Clear();

            var texture2D = this.Sprite.Texture as Texture2D;

            if (texture2D == null)
            {
                return;
            }

            var hasHorizontalAnchors = texture2D.HorizontalScalableAnchors?.Length > 0;
            var hasVerticalAnchors = texture2D.VerticalScalableAnchors?.Length > 0;
            var horizontalScalableAnchors = hasHorizontalAnchors ? texture2D.HorizontalScalableAnchors : new Point[] { new Point(0, texture2D.Width) };
            var verticalScalableAnchors = hasVerticalAnchors ? texture2D.VerticalScalableAnchors : new Point[] { new Point(0, texture2D.Height) };

            var flippedOrigin = this.Transform2D.Origin;
            flippedOrigin.X = this.Transform2D.Effect.HasFlag(SpriteEffects.FlipHorizontally) ? 1 - flippedOrigin.X : flippedOrigin.X;
            flippedOrigin.Y = this.Transform2D.Effect.HasFlag(SpriteEffects.FlipVertically) ? 1 - flippedOrigin.Y : flippedOrigin.Y;

            var spriteTotalSize = Vector2.Abs(this.Transform2D.Scale) * new Vector2(texture2D.Width, texture2D.Height);
            var spriteOriginCoord = (flippedOrigin * spriteTotalSize).ToVector3(0);
            var totalScalableArea = new Vector2(horizontalScalableAnchors.Sum(a => a.Y), verticalScalableAnchors.Sum(a => a.Y));

            var stretchScale = new Vector3(
                        (spriteTotalSize.X - (texture2D.Width - totalScalableArea.X)) / totalScalableArea.X,
                        (spriteTotalSize.Y - (texture2D.Height - totalScalableArea.Y)) / totalScalableArea.Y,
                        1);

            var effectMultiplier = new Vector3(
                        this.Transform2D.Effect.HasFlag(SpriteEffects.FlipHorizontally) ? -1 : 1,
                        this.Transform2D.Effect.HasFlag(SpriteEffects.FlipVertically) ? -1 : 1,
                        1);

            var worldMatrix = this.Transform2D.WorldTransform;

            // Horizontal loop
            var hAreaEnumerator = horizontalScalableAnchors.GetEnumerator();
            var currentHArea = Point.Zero;
            int hStretchAcumm = 0;
            for (int hOffset = 0; hOffset < texture2D.Width;)
            {
                int hWidth = hOffset;
                bool hStretchActive = currentHArea != Point.Zero;
                if (hStretchActive)
                {
                    hStretchActive = true;
                    hWidth = currentHArea.Y;
                    currentHArea = Point.Zero;
                }
                else if (hAreaEnumerator.MoveNext())
                {
                    currentHArea = (Point)hAreaEnumerator.Current;
                    hWidth = currentHArea.X - hOffset;
                }
                else
                {
                    hWidth = texture2D.Width - hOffset;
                }

                // Vertical loop
                var vAreaEnumerator = verticalScalableAnchors.GetEnumerator();
                var currentVArea = Point.Zero;
                int vStretchAcumm = 0;
                for (int vOffset = 0; vOffset < texture2D.Height;)
                {
                    int vHeight = vOffset;
                    bool vStretchActive = currentVArea != Point.Zero;
                    if (vStretchActive)
                    {
                        vHeight = currentVArea.Y;
                        currentVArea = Point.Zero;
                    }
                    else if (vAreaEnumerator.MoveNext())
                    {
                        currentVArea = (Point)vAreaEnumerator.Current;
                        vHeight = currentVArea.X - vOffset;
                    }
                    else
                    {
                        vHeight = texture2D.Height - vOffset;
                    }

                    var scalableCenter = Vector3.Zero;
                    var scale = effectMultiplier;

                    if (hStretchActive)
                    {
                        scalableCenter.X = (hOffset - hStretchAcumm) + (((hWidth * 0.5f) + hStretchAcumm) * stretchScale.X);
                        scale.X *= stretchScale.X;
                    }
                    else
                    {
                        scalableCenter.X = (hOffset - hStretchAcumm) + (hWidth * 0.5f) + (hStretchAcumm * stretchScale.X);
                    }

                    if (vStretchActive)
                    {
                        scalableCenter.Y = (vOffset - vStretchAcumm) + (((vHeight * 0.5f) + vStretchAcumm) * stretchScale.Y);
                        scale.Y *= stretchScale.Y;
                    }
                    else
                    {
                        scalableCenter.Y = (vOffset - vStretchAcumm) + (vHeight * 0.5f) + (vStretchAcumm * stretchScale.Y);
                    }

                    var originOffset = (spriteOriginCoord - scalableCenter) * effectMultiplier;
                    originOffset = Vector3.Transform(originOffset, worldMatrix.Orientation);
                    var matrix = Matrix.CreateFromTRS(worldMatrix.Translation - originOffset, worldMatrix.Orientation, scale);
                    var scolor = vStretchActive ? (hStretchActive ? Color.Pink : Color.LightGreen) : (hStretchActive ? Color.LightGreen : Color.White);

                    this.sliceCacheItems.Add(new SliceCacheItem()
                    {
                        SourceRectangle = new Rectangle(hOffset, vOffset, hWidth, vHeight),
                        DebugTintColor = scolor,
                        WorldMatrix = matrix
                    });

                    vStretchAcumm += vStretchActive ? vHeight : 0;
                    vOffset += vHeight;
                }

                hStretchAcumm += hStretchActive ? hWidth : 0;
                hOffset += hWidth;
            }
        }

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
