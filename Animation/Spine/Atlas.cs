#region File Description
//-----------------------------------------------------------------------------
// Atlas
//
// Copyright (c) 2013, Esoteric Software
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions are met:
// 
// 1. Redistributions of source code must retain the above copyright notice, this
//    list of conditions and the following disclaimer.
// 2. Redistributions in binary form must reproduce the above copyright notice,
//    this list of conditions and the following disclaimer in the documentation
//    and/or other materials provided with the distribution.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR
// ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
// (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
// LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
// ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
// (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
// SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using System.IO;
using WaveEngine.Framework.Services;
#endregion

namespace WaveEngine.Components.Animation.Spine
{
    /// <summary>
    /// Atlas class
    /// </summary>
    public class Atlas
    {
        /// <summary>
        /// The pages
        /// </summary>
        public readonly List<AtlasPage> Pages = new List<AtlasPage>();

        /// <summary>
        /// The regions
        /// </summary>
        private List<AtlasRegion> regions = new List<AtlasRegion>();

        /// <summary>
        /// The texture loader
        /// </summary>
        private ITextureLoader textureLoader;

        #region Initialize
        /// <summary>
        /// Initializes a new instance of the <see cref="Atlas" /> class.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="textureLoader">The texture loader.</param>
        /// <exception cref="System.Exception">Error reading atlas file:  + path</exception>
        public Atlas(string path, ITextureLoader textureLoader)
        {
            Stream stream = WaveServices.Storage.OpenContentFile(path);
            using (StreamReader reader = new StreamReader(stream))
            {
                try
                {
                    this.Load(reader, Path.GetDirectoryName(path), textureLoader);
                }
                catch (Exception ex)
                {
                    throw new Exception("Error reading atlas file: " + path, ex);
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Atlas" /> class.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="dir">The dir.</param>
        /// <param name="textureLoader">The texture loader.</param>
        public Atlas(TextReader reader, string dir, ITextureLoader textureLoader)
        {
            this.Load(reader, dir, textureLoader);
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// Returns the first region found with the specified name. This method uses string comparison to find the region, so the result 
        /// should be cached rather than calling this method multiple times.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>The region, or null.</returns>
        public AtlasRegion FindRegion(string name)
        {
            for (int i = 0, n = this.regions.Count; i < n; i++)
            {
                if (this.regions[i].Name == name)
                {
                    return this.regions[i];
                }
            }

            return null;
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        public void Dispose()
        {
            for (int i = 0, n = this.Pages.Count; i < n; i++)
            {
                this.textureLoader.Unload(this.Pages[i].Texture);
            }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Loads the specified reader.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="imagesDir">The images dir.</param>
        /// <param name="textureLoader">The texture loader.</param>
        /// <exception cref="System.ArgumentNullException">textureLoader cannot be null.</exception>
        private void Load(TextReader reader, string imagesDir, ITextureLoader textureLoader)
        {
            if (textureLoader == null)
            {
                throw new ArgumentNullException("textureLoader cannot be null.");
            }

            this.textureLoader = textureLoader;

            string[] tuple = new string[4];
            AtlasPage page = null;

            while (true)
            {
                string line = reader.ReadLine();

                if (line == null)
                {
                    break;
                }

                if (line.Trim().Length == 0)
                {
                    page = null;
                }
                else if (page == null)
                {
                    page = new AtlasPage();
                    page.Name = line;

                    page.Format = (Format)Enum.Parse(typeof(Format), this.ReadValue(reader), false);

                    this.ReadTuple(reader, tuple);

                    page.MinFilter = (TextureFilter)Enum.Parse(typeof(TextureFilter), tuple[0], true);
                    page.MagFilter = (TextureFilter)Enum.Parse(typeof(TextureFilter), tuple[1], true);

                    string direction = this.ReadValue(reader);
                    page.UWrap = TextureWrap.ClampToEdge;
                    page.VWrap = TextureWrap.ClampToEdge;
                    if (direction == "x")
                    {
                        page.UWrap = TextureWrap.Repeat;
                    }
                    else if (direction == "y")
                    {
                        page.VWrap = TextureWrap.Repeat;
                    }
                    else if (direction == "xy")
                    {
                        page.UWrap = page.VWrap = TextureWrap.Repeat;
                    }

                    textureLoader.Load(page, Path.Combine(imagesDir, line));

                    this.Pages.Add(page);
                }
                else
                {
                    AtlasRegion region = new AtlasRegion();
                    region.Name = line;
                    region.Page = page;

                    region.Rotate = bool.Parse(this.ReadValue(reader));

                    this.ReadTuple(reader, tuple);
                    int x = int.Parse(tuple[0]);
                    int y = int.Parse(tuple[1]);

                    this.ReadTuple(reader, tuple);
                    int width = int.Parse(tuple[0]);
                    int height = int.Parse(tuple[1]);

                    region.U = x / (float)page.Width;
                    region.V = y / (float)page.Height;

                    if (region.Rotate)
                    {
                        region.U2 = (x + height) / (float)page.Width;
                        region.V2 = (y + width) / (float)page.Height;
                    }
                    else
                    {
                        region.U2 = (x + width) / (float)page.Width;
                        region.V2 = (y + height) / (float)page.Height;
                    }

                    region.X = x;
                    region.Y = y;
                    region.Width = Math.Abs(width);
                    region.Height = Math.Abs(height);

                    if (this.ReadTuple(reader, tuple) == 4)
                    { // split is optional
                        region.Splits = new int[] { int.Parse(tuple[0]), int.Parse(tuple[1]), int.Parse(tuple[2]), int.Parse(tuple[3]) };

                        if (this.ReadTuple(reader, tuple) == 4)
                        { // pad is optional, but only present with splits
                            region.Pads = new int[] { int.Parse(tuple[0]), int.Parse(tuple[1]), int.Parse(tuple[2]), int.Parse(tuple[3]) };

                            this.ReadTuple(reader, tuple);
                        }
                    }

                    region.OriginalWidth = int.Parse(tuple[0]);
                    region.OriginalHeight = int.Parse(tuple[1]);

                    this.ReadTuple(reader, tuple);
                    region.OffsetX = int.Parse(tuple[0]);
                    region.OffsetY = int.Parse(tuple[1]);

                    region.Index = int.Parse(this.ReadValue(reader));

                    this.regions.Add(region);
                }
            }
        }

        /// <summary>
        /// Reads the value.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <returns>Return a string value.</returns>
        /// <exception cref="System.Exception">Invalid line:  + line</exception>
        private string ReadValue(TextReader reader)
        {
            string line = reader.ReadLine();
            int colon = line.IndexOf(':');

            if (colon == -1)
            {
                throw new Exception("Invalid line: " + line);
            }

            return line.Substring(colon + 1).Trim();
        }

        /// <summary>
        /// Returns the number of tuple values read (2 or 4).
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="tuple">The tuple.</param>
        /// <returns>Return value.</returns>
        /// <exception cref="System.Exception">Invalid line:  + line</exception>
        private int ReadTuple(TextReader reader, string[] tuple)
        {
            string line = reader.ReadLine();
            int colon = line.IndexOf(':');
            if (colon == -1)
            {
                throw new Exception("Invalid line: " + line);
            }

            int i = 0, lastMatch = colon + 1;

            for (; i < 3; i++)
            {
                int comma = line.IndexOf(',', lastMatch);

                if (comma == -1)
                {
                    if (i == 0)
                    {
                        throw new Exception("Invalid line: " + line);
                    }

                    break;
                }

                tuple[i] = line.Substring(lastMatch, comma - lastMatch).Trim();
                lastMatch = comma + 1;
            }

            tuple[i] = line.Substring(lastMatch).Trim();

            return i + 1;
        }
        #endregion
    }
}
