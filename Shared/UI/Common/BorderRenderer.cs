#region File Description
//-----------------------------------------------------------------------------
// BorderRenderer
//
// Copyright © 2016 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using WaveEngine.Common.Graphics;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
#endregion

namespace WaveEngine.Components.UI
{
    /// <summary>
    /// Draw a simple border over controls
    /// </summary>
    [DataContract(Namespace = "WaveEngine.Components.UI")]
    public class BorderRenderer : DrawableGUI
    {
        /// <summary>
        /// The transform2D
        /// </summary>
        [RequiredComponent]
        public Transform2D Transform2D;

        /// <summary>
        /// Total number of instances.
        /// </summary>
        private static int instances;

        #region Properties
        /// <summary>
        /// Gets or sets the color.
        /// </summary>
        /// <value>
        /// The color.
        /// </value>
        [DataMember]
        public Color Color { get; set; }
        #endregion

        #region Initialize
        /// <summary>
        /// Initializes a new instance of the <see cref="BorderRenderer" /> class.
        /// </summary>
        public BorderRenderer()
            : this(Color.White)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BorderRenderer" /> class.
        /// </summary>
        /// <param name="color">The color.</param>
        public BorderRenderer(Color color)
            : this("BorderRenderer" + instances, DefaultLayers.GUI, color)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BorderRenderer" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="layerType">Type of the layer.</param>
        /// <param name="color">The color.</param>
        public BorderRenderer(string name, Type layerType, Color color)
            : base(name, layerType)
        {
            instances++;
            this.Color = color;
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
            this.layer.LineBatch2D.DrawRectangle(this.Transform2D.Rectangle, this.Color, this.Transform2D.DrawOrder);
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
        }
        #endregion
    }
}
