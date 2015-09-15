#region File Description
//-----------------------------------------------------------------------------
// TextControlRenderer
//
// Copyright © 2015 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Runtime.Serialization;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Math;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
#endregion

namespace WaveEngine.Components.UI
{
    /// <summary>
    /// The text block renderer component.
    /// </summary>
    [DataContract(Namespace = "WaveEngine.Components.UI")]
    public class TextControlRenderer : DrawableGUI
    {
        /// <summary>
        /// Total number of instances.
        /// </summary>
        private static int instances;

        /// <summary>
        /// The transform2 D
        /// </summary>
        [RequiredComponent]
        public Transform2D Transform2D;

        /// <summary>
        /// The text block
        /// </summary>
        [RequiredComponent]
        public TextControl TextBlock;

        /// <summary>
        /// The position
        /// </summary>
        private Vector2 position;

        /// <summary>
        /// The scale
        /// </summary>
        private Vector2 scale;

        /// <summary>
        /// The origin
        /// </summary>
        private Vector2 origin;

        #region Initialize

        /// <summary>
        /// Initializes a new instance of the <see cref="TextControlRenderer" /> class.
        /// </summary>
        public TextControlRenderer()
            : this(DefaultLayers.GUI)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TextControlRenderer" /> class.
        /// </summary>
        /// <param name="layerType">Type of the layer.</param>
        public TextControlRenderer(Type layerType)
            : this("TextControlRenderer" + instances, layerType)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TextControlRenderer" /> class.
        /// </summary>
        /// <param name="name">Name of this instance.</param>
        /// <param name="layerType">Type of the layer.</param>
        public TextControlRenderer(string name, Type layerType)
            : base(name, layerType)
        {
            instances++;
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
            if (this.Transform2D.GlobalOpacity > Drawable2D.Delta)
            {
                this.position.X = this.Transform2D.Rectangle.X + this.Transform2D.X;
                this.position.Y = this.Transform2D.Rectangle.Y + this.Transform2D.Y;
                this.scale.X = (this.Transform2D.Rectangle.Width / this.TextBlock.Width) * this.Transform2D.XScale;
                this.scale.Y = (this.Transform2D.Rectangle.Height / this.TextBlock.Height) * this.Transform2D.YScale;

                Vector2 transformOrigin = this.Transform2D.Origin;
                this.origin.X = transformOrigin.X * this.TextBlock.Width;
                this.origin.Y = transformOrigin.Y * this.TextBlock.Height;

                float opacity = this.RenderManager.DebugLines ? DebugAlpha : this.Transform2D.GlobalOpacity;

                Vector2 aux;
                for (int i = 0; i < this.TextBlock.LinesInfo.Count; i++)
                {
                    aux = this.position;
                    aux.X = this.position.X + (this.TextBlock.LinesInfo[i].AlignmentOffsetX * this.Transform2D.XScale);

                    for (int j = 0; j < this.TextBlock.LinesInfo[i].SubTextList.Count; j++)
                    {
                        this.layer.SpriteBatch.DrawStringVM(
                            this.TextBlock.SpriteFont,
                            this.TextBlock.LinesInfo[i].SubTextList[j].Text,
                            aux,
                            this.TextBlock.LinesInfo[i].SubTextList[j].Color * opacity,
                            this.Transform2D.Rotation,
                            this.origin,
                            this.scale,
                            this.Transform2D.Effect,
                            this.Transform2D.DrawOrder);

                        aux.X = aux.X + this.TextBlock.LinesInfo[i].SubTextList[j].Size.X;
                    }

                    this.position.Y = this.position.Y + ((this.TextBlock.FontHeight + this.TextBlock.LineSpacing) * this.Transform2D.YScale);
                }
            }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
        }

        /// <summary>
        /// Helper method that draws debug lines.
        /// </summary>
        /// <remarks>
        /// This method will only work on debug mode and if RenderManager.DebugLines /&gt;
        /// is set to <c>true</c>.
        /// </remarks>
        protected override void DrawDebugLines()
        {
            base.DrawDebugLines();

            // Rectangle Layout pass
            RenderManager.LineBatch2D.DrawRectangleVM(this.Transform2D.Rectangle, Color.Blue, this.Transform2D.DrawOrder);

            // Origin
            RenderManager.LineBatch2D.DrawPointVM(this.Transform2D.Rectangle.Location + this.Transform2D.Origin, 10f, Color.Red, this.Transform2D.DrawOrder);
        }
        #endregion
    }
}
