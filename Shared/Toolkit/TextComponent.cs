#region File Description
//-----------------------------------------------------------------------------
// TextComponent
//
// Copyright © 2016 Wave Coorporation. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using WaveEngine.Common.Attributes;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Graphics.VertexFormats;
using WaveEngine.Common.Math;
using WaveEngine.Components.UI;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Resources;
using WaveEngine.Framework.Services;
#endregion

namespace WaveEngine.Components.Toolkit
{
    /// <summary>
    /// Component with the 3D text information
    /// </summary>
    [DataContract(Namespace = "WaveEngine.Components.Toolkit")]
    public class TextComponent : BaseModel
    {
        /// <summary>
        /// The info of a text character
        /// </summary>
        internal struct TextCharInfo
        {
            /// <summary>
            /// The source on the spritefont
            /// </summary>
            public Rectangle SourceRectangle;

            /// <summary>
            /// The position rectangle
            /// </summary>
            public RectangleF Position;

            /// <summary>
            /// The character
            /// </summary>
            public char Character;
        }

        #region fields

        /// <summary>
        /// The max number of characters per mesh
        /// </summary>
        private const int MAXCHARS = 256;

        /// <summary>
        /// The vertexbuffer length
        /// </summary>
        private const int BUFFERLENGTH = MAXCHARS * CHARVERTICES;

        /// <summary>
        /// Vertices per character
        /// </summary>
        private const int CHARVERTICES = 4;

        /// <summary>
        /// Triangles per section
        /// </summary>
        private const int CHARTRIANGLES = 2;

        /// <summary>
        /// Indices per character
        /// </summary>
        private const int CHARINDICES = 6;

        /// <summary>
        /// The vertex buffer data
        /// </summary>
        private VertexPositionNormalTexture[] vertexBufferData;

        /// <summary>
        /// The index buffer
        /// </summary>
        private IndexBuffer indexBuffer;

        /// <summary>
        /// Array of the characters info
        /// </summary>
        private List<TextCharInfo> charInfoList;

        /// <summary>
        /// The mesh list.
        /// </summary>
        private List<Mesh> meshes;

        /// <summary>
        /// The text to show
        /// </summary>
        [DataMember]
        private string text;

        /// <summary>
        /// The font path
        /// </summary>
        [DataMember]
        private string fontPath;

        /// <summary>
        /// The text foreground color
        /// </summary>
        [DataMember]
        private Color foreground;

        /// <summary>
        /// The text alignment
        /// </summary>
        [DataMember]
        private TextAlignment textAlignment;

        /// <summary>
        /// The text origin alignment
        /// </summary>
        [DataMember]
        private Vector2 origin;

        /// <summary>
        /// The text scale
        /// </summary>
        [DataMember]
        private Vector2 scale;

        /// <summary>
        /// The layer type of the text
        /// </summary>
        ////[DataMember]
        private Type layerType;

        /// <summary>
        /// The sprite font
        /// </summary>
        private SpriteFont spriteFont;

        /// <summary>
        /// The line spacing
        /// </summary>
        [DataMember]
        private float lineSpacing;

        /// <summary>
        /// If the text must wrapp
        /// </summary>
        [DataMember]
        private bool textWrapping;

        /// <summary>
        /// The with of the text control
        /// </summary>
        [DataMember]
        private float width;

        /// <summary>
        /// If the text component material needs an update
        /// </summary>
        internal bool MaterialDirty;

        /// <summary>
        /// Text offset
        /// </summary>
        private Vector2 textOffset;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the text of the control
        /// </summary>
        [RenderPropertyAsTextBox]
        public string Text
        {
            get
            {
                return this.text;
            }

            set
            {
                if (this.text != value)
                {
                    this.text = value;

                    this.RefreshText();
                }
            }
        }

        /// <summary>
        /// Gets the meshes
        /// </summary>
        public List<Mesh> Meshes
        {
            get
            {
                return this.meshes;
            }
        }

        /// <summary>
        /// Gets the char info list.
        /// </summary>
        internal List<TextCharInfo> CharInfoList
        {
            get
            {
                return this.charInfoList;
            }
        }

        /// <summary>
        /// Gets the lines info
        /// </summary>
        [DontRenderProperty]
        public List<LineInfo> LinesInfo { get; private set; }

        /// <summary>
        /// Gets the line width
        /// </summary>
        [DontRenderProperty]
        public float LineWidth { get; private set; }

        /// <summary>
        /// Gets the font height
        /// </summary>
        [DontRenderProperty]
        public float FontHeight { get; private set; }

        /// <summary>
        /// Gets or sets the font path of the control
        /// </summary>
        [RenderPropertyAsAsset(AssetType.Font)]
        public string FontPath
        {
            get
            {
                return this.fontPath;
            }

            set
            {
                ////if (this.fontPath != value)
                ////{
                this.fontPath = value;

                if (this.isInitialized)
                {
                    this.UpdateSpriteFont();
                    this.RefreshText();
                    this.MaterialDirty = true;
                }
                ////}
            }
        }

        /// <summary>
        /// Gets the actual width
        /// </summary>
        public float ActualWidth { get; private set; }

        /// <summary>
        /// Gets the actual height
        /// </summary>
        public float ActualHeight { get; private set; }

        /// <summary>
        /// Gets the control sprite font
        /// </summary>
        public SpriteFont SpriteFont
        {
            get
            {
                return this.spriteFont;
            }
        }

        /// <summary>
        /// Gets or sets the text alignment
        /// </summary>
        public TextAlignment TextAlignment
        {
            get
            {
                return this.textAlignment;
            }

            set
            {
                if (this.textAlignment != value)
                {
                    this.textAlignment = value;

                    if (this.isInitialized)
                    {
                        this.RefreshText();
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the foreground color of the text control
        /// </summary>
        public Color Foreground
        {
            get
            {
                return this.foreground;
            }

            set
            {
                if (this.foreground != value)
                {
                    this.foreground = value;
                    this.MaterialDirty = true;
                }
            }
        }

        /// <summary>
        /// Gets or sets the type of the layer.
        /// </summary>
        /// <value>
        /// The type of the layer.
        /// </value>            
        [RenderPropertyAsLayer]
        public Type LayerType
        {
            get
            {
                return this.layerType;
            }

            set
            {
                if (this.layerType != value)
                {
                    this.layerType = value;
                    this.MaterialDirty = true;
                }
            }
        }

        /// <summary>
        /// Gets or sets the line spacing
        /// </summary>
        public float LineSpacing
        {
            get
            {
                return this.lineSpacing;
            }

            set
            {
                if (this.lineSpacing != value)
                {
                    this.lineSpacing = value;

                    if (this.isInitialized)
                    {
                        this.RefreshText();
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the text scale
        /// </summary>
        public Vector2 TextScale
        {
            get
            {
                return this.scale;
            }

            set
            {
                if (this.scale != value)
                {
                    this.scale = value;

                    if (this.isInitialized)
                    {
                        this.RefreshText();
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the text must be wrapped
        /// </summary>
        [RenderProperty(Tag = 1)]
        public bool TextWrapping
        {
            get
            {
                return this.textWrapping;
            }

            set
            {
                if (this.textWrapping != value)
                {
                    this.textWrapping = value;

                    if (this.isInitialized)
                    {
                        this.RefreshText();
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the width of the text control
        /// </summary>
        [RenderProperty(AttatchToTag = 1, AttachToValue = true)]
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
                        this.RefreshText();
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the origin of the text control
        /// </summary>
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
                        this.RefreshText();
                    }
                }
            }
        }

        /// <summary>
        /// Gets the text offset
        /// </summary>
        internal Vector2 TextOffset
        {
            get
            {
                return this.textOffset;
            }
        }

        /// <summary>
        /// Gets the number of meshes of the model
        /// </summary>
        public override int MeshCount
        {
            get
            {
                return this.meshes.Count;
            }
        }

        #endregion

        #region Initialize

        /// <summary>
        /// Sets the default values of the component
        /// </summary>
        protected override void DefaultValues()
        {
            base.DefaultValues();
            this.origin = new Vector2(0.5f);
            this.foreground = Color.White;
            this.layerType = DefaultLayers.Alpha;
            this.LinesInfo = new List<LineInfo>();
            this.textWrapping = false;
            this.width = 300;
            this.TextScale = Vector2.One;

            this.charInfoList = new List<TextCharInfo>();
            this.meshes = new List<Mesh>();
        }
        #endregion

        #region Public Methods
        #endregion

        #region Private Methods

        /// <summary>
        /// Initializes the component
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();

            this.vertexBufferData = new VertexPositionNormalTexture[BUFFERLENGTH];

            this.CreateInfexBuffer();

            this.UpdateSpriteFont();

            this.RefreshText();
            this.MaterialDirty = true;
        }

        /// <summary>
        /// Updates the sprite font
        /// </summary>
        private void UpdateSpriteFont()
        {
            SpriteFont font = null;

            try
            {
                if (!string.IsNullOrEmpty(this.fontPath))
                {
                    font = this.Assets.LoadAsset<SpriteFont>(this.fontPath);
                }
                else
                {
                    font = StaticResources.DefaultSpriteFont;
                }
            }
            catch (Exception)
            {
                font = null;
            }

            this.spriteFont = font;
        }

        /// <summary>
        /// Refreshes the text
        /// </summary>
        private void RefreshText()
        {
            this.RemoveAll();

            if (!string.IsNullOrEmpty(this.Text))
            {
                this.RefreshSize();
                this.RefreshLines();
                this.RefreshMeshes();
                this.RefreshBoundingBox();
            }
        }

        /// <summary>
        /// Updates the lines info of the text component
        /// </summary>
        private void RefreshSize()
        {
            this.LinesInfo.Clear();

            if (!string.IsNullOrEmpty(this.text) && this.SpriteFont != null)
            {
                this.FontHeight = this.SpriteFont.MeasureString("A").Y;

                // Filters
                var handledText = this.text;
                handledText = handledText.Replace("\r\n", " /n ");
                handledText = handledText.Replace("\n", " /n ");
                ////this.text = this.text.Replace("/n", "/n");
                handledText = handledText.Replace("\t", "  ");

                string[] words = handledText.Split(' ');
                var stringBuilder = new StringBuilder();
                float maxWidth = 0;

                var scaledWidth = this.width / this.TextScale.X;
                var scaledLineSpacing = this.lineSpacing / this.TextScale.Y;

                int i = 0;
                while (i < words.Length)
                {
                    do
                    {
                        if (words[i].Equals("/n"))
                        {
                            i++;
                            break;
                        }

                        string nextString = string.Format("{0}{1} ", stringBuilder, words[i]);
                        float lineSize = this.SpriteFont.MeasureString(nextString).X;
                        if (this.textWrapping && (stringBuilder.Length != 0 && lineSize > scaledWidth))
                        {
                            break;
                        }

                        stringBuilder.Append(words[i]);
                        stringBuilder.Append(" ");
                        i++;
                    }
                    while (i < words.Length);

                    string text = stringBuilder.ToString();
                    Vector2 size = this.SpriteFont.MeasureString(text);
                    ////float offsetX = this.CalculateAlignmentOffset(size);
                    this.LinesInfo.Add(new LineInfo(text, this.Foreground, size, 0));
                    stringBuilder.Length = 0;

                    if (size.X > maxWidth)
                    {
                        maxWidth = size.X;
                    }
                }

                this.ActualWidth = maxWidth;

                this.ActualHeight = this.LinesInfo.Count * (this.FontHeight + scaledLineSpacing);

                this.textOffset.X = -this.ActualWidth * this.origin.X;
                this.textOffset.Y = this.ActualHeight * this.origin.Y;

                for (int j = 0; j < this.LinesInfo.Count; j++)
                {
                    var line = this.LinesInfo[j];
                    this.CalculateAlignmentOffset(ref line);
                    this.LinesInfo[j] = line;
                }
            }
        }

        /// <summary>
        /// Calculates the alignment offset.
        /// </summary>
        /// <param name="line">The line info.</param>
        public void CalculateAlignmentOffset(ref LineInfo line)
        {
            float offsetX = 0;
            Vector2 size = line.Size;

            var width = this.textWrapping ? this.width / this.TextScale.X : this.ActualWidth;

            switch (this.textAlignment)
            {
                case TextAlignment.Left:
                    break;
                case TextAlignment.Right:

                    if (size.X < width)
                    {
                        offsetX = width - size.X;
                    }

                    break;
                case TextAlignment.Center:

                    if (size.X < width)
                    {
                        offsetX = (width - size.X) * 0.5f;
                    }

                    break;
            }

            line.AlignmentOffsetX = offsetX;
        }

        /// <summary>
        /// Updates the size of the index buffer.
        /// </summary>
        private void CreateInfexBuffer()
        {
            int numIndices = MAXCHARS * CHARINDICES;

            ushort[] indices = new ushort[numIndices];
            for (int j = 0, v = 0; j < indices.Length; j += CHARINDICES, v += CHARVERTICES)
            {
                indices[j + 0] = (ushort)v; // 0
                indices[j + 1] = (ushort)(v + 2); // 2
                indices[j + 2] = (ushort)(v + 1); // 1
                indices[j + 3] = (ushort)(v + 1); // 1
                indices[j + 4] = (ushort)(v + 2); // 2
                indices[j + 5] = (ushort)(v + 3); // 3
            }

            this.indexBuffer = new IndexBuffer(indices);
            this.RenderManager.GraphicsDevice.BindIndexBuffer(this.indexBuffer);
        }

        /// <summary>
        /// Updates the text lines
        /// </summary>
        private void RefreshLines()
        {
            this.charInfoList.Clear();

            if (this.spriteFont == null)
            {
                return;
            }

            Vector2 aux = Vector2.Zero;
            var linesInfo = this.LinesInfo;

            for (int i = 0; i < linesInfo.Count; i++)
            {
                aux.X = this.LinesInfo[i].AlignmentOffsetX;

                for (int j = 0; j < this.LinesInfo[i].SubTextList.Count; j++)
                {
                    var subText = linesInfo[i].SubTextList[0];
                    this.AddLine(
                        subText.Text,
                        aux,
                        subText.Color);

                    aux.X += linesInfo[i].SubTextList[j].Size.X;
                }

                aux.Y -= this.FontHeight + (this.lineSpacing / this.TextScale.Y);
            }
        }

        /// <summary>
        /// Updates the vertex buffer of the text
        /// </summary>
        /// <param name="text">The text position</param>
        /// <param name="initOffset">The line offset</param>
        /// <param name="color">The text color</param>
        private void AddLine(string text, Vector2 initOffset, Color color)
        {
            var spriteFont = this.SpriteFont;
            var textSize = spriteFont.MeasureString(text);
            Vector2 accumulatedPosition = Vector2.Zero;
            bool flag = true;

            accumulatedPosition.Y = textSize.Y - spriteFont.LineSpacing; // * scale.Y;

            for (int i = 0; i < text.Length; i++)
            {
                char character = text[i];

                int indexForCharacter = spriteFont.GetIndexForCharacter(character);
                Vector3 kerning = spriteFont.Kerning[indexForCharacter];

                if (flag)
                {
                    kerning.X = Math.Max(kerning.X, 0f);
                }
                else
                {
                    accumulatedPosition.X += spriteFont.Spacing;
                }

                accumulatedPosition.X += kerning.X;
                Rectangle glyphRectangle = spriteFont.Glyphs[indexForCharacter];
                Rectangle cropRectangle = spriteFont.Cropping[indexForCharacter];

                cropRectangle.Y = -glyphRectangle.Height - cropRectangle.Y;

                Vector2 position = accumulatedPosition;
                position.X += cropRectangle.X;
                position.Y += cropRectangle.Y;

                position += initOffset + this.TextOffset;

                this.AddGlyph(character, position, glyphRectangle);
                flag = false;
                accumulatedPosition.X += kerning.Y + kerning.Z;
            }
        }

        /// <summary>
        /// Adds a glyph to the vertex buffer
        /// </summary>
        /// <param name="character">The glyph character</param>
        /// <param name="position">The glyph position</param>
        /// <param name="sourceRectangle">The glyph rectangle</param>
        private void AddGlyph(char character, Vector2 position, Rectangle sourceRectangle)
        {
            var scale = this.TextScale;
            var width = sourceRectangle.Width * scale.X;
            var height = sourceRectangle.Height * scale.Y;
            var x = position.X * scale.X;
            var y = position.Y * scale.Y;

            var charInfo = new TextCharInfo()
            {
                Character = character,
                SourceRectangle = sourceRectangle,
                Position = new RectangleF(x, y, width, height)
            };

            this.charInfoList.Add(charInfo);
        }

        /// <summary>
        /// Refreshes the meshes
        /// </summary>
        private void RefreshMeshes()
        {
            int totalChars = this.charInfoList.Count;

            if (totalChars == 0)
            {
                return;
            }

            ////this.normalLines.Clear();

            int index = 0;
            int remainingVertices = totalChars * CHARVERTICES;
            int bufferLength = 0;
            Vector3 normal = Vector3.UnitZ;
            float tW = this.SpriteFont.FontTexture.Width;
            float tH = this.SpriteFont.FontTexture.Height;

            for (int charNum = 0; charNum < totalChars; charNum++)
            {
                var charInfo = this.charInfoList[charNum];
                var p = charInfo.Position;
                var s = charInfo.SourceRectangle;

                // Builds the vertex information.
                // Front Vertex
                this.vertexBufferData[index].Position = new Vector3(p.Left, p.Top, 0);
                this.vertexBufferData[index].Normal = normal;
                this.vertexBufferData[index].TexCoord = new Vector2(s.Left / tW, s.Bottom / tH);

                this.vertexBufferData[index + 1].Position = new Vector3(p.Right, p.Top, 0);
                this.vertexBufferData[index + 1].Normal = normal;
                this.vertexBufferData[index + 1].TexCoord = new Vector2(s.Right / tW, s.Bottom / tH);

                this.vertexBufferData[index + 2].Position = new Vector3(p.Left, p.Bottom, 0);
                this.vertexBufferData[index + 2].Normal = normal;
                this.vertexBufferData[index + 2].TexCoord = new Vector2(s.Left / tW, s.Top / tH);

                this.vertexBufferData[index + 3].Position = new Vector3(p.Right, p.Bottom, 0);
                this.vertexBufferData[index + 3].Normal = normal;
                this.vertexBufferData[index + 3].TexCoord = new Vector2(s.Right / tW, s.Top / tH);

                index += CHARVERTICES;
                remainingVertices -= CHARVERTICES;
                bufferLength += CHARVERTICES;

                if ((index >= BUFFERLENGTH) || (remainingVertices <= 0))
                {
                    Mesh mesh = this.CreateMesh();
                    mesh.VertexBuffer.SetData(this.vertexBufferData, BUFFERLENGTH, 0);
                    mesh.NumVertices = bufferLength;
                    mesh.NumPrimitives = (bufferLength * CHARTRIANGLES) / CHARVERTICES;

                    this.RenderManager.GraphicsDevice.BindVertexBuffer(mesh.VertexBuffer);

                    this.meshes.Add(mesh);

                    mesh.VertexBuffer.FreePointer();

                    index = 0;
                    bufferLength = 0;
                }
            }
        }

        /// <summary>
        /// Creates the mesh.
        /// </summary>
        /// <returns>The mesh</returns>
        private Mesh CreateMesh()
        {
            return new Mesh(
                0,
                BUFFERLENGTH,
                0,
                MAXCHARS * CHARTRIANGLES,
                new DynamicVertexBuffer(VertexPositionNormalTexture.VertexFormat),
                this.indexBuffer,
                PrimitiveType.TriangleList)
            {
                DisableBatch = true
            };
        }

        /// <summary>
        /// Removes all.
        /// </summary>
        public void RemoveAll()
        {
            foreach (var mesh in this.meshes)
            {
                this.RenderManager.GraphicsDevice.DestroyVertexBuffer(mesh.VertexBuffer);
            }

            this.charInfoList.Clear();
            this.meshes.Clear();
        }

        /// <summary>
        /// Gets the collition info.
        /// </summary>
        /// <returns>Vertex array.</returns>
        public override Vector3[] GetVertices()
        {
            return null;
        }

        /// <summary>
        /// The get indices
        /// </summary>
        /// <returns>Indices array</returns>
        public override int[] GetIndices()
        {
            return null;
        }

        /// <summary>
        /// Updates the bonding box
        /// </summary>
        private void RefreshBoundingBox()
        {
            BoundingBox boundingBox;
            boundingBox.Min = new Vector3(this.ActualWidth + this.textOffset.X, this.ActualHeight + this.textOffset.Y, 0);
            boundingBox.Max = boundingBox.Min + new Vector3(this.ActualWidth, this.ActualHeight, 0);

            this.BoundingBoxRefreshed = true;
        }
        #endregion
    }
}