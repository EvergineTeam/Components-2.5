#region File Description
//-----------------------------------------------------------------------------
// TexturePackerGenericXml
// Copyright © 2010 - 2013 Wave Coorporation. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using WaveEngine.Common.Math;
using WaveEngine.Framework.Services;
#endregion

namespace WaveEngine.Components.Animation
{
    /// <summary>
    /// Texture Packer's Generic XML sprite sheet loader.
    /// </summary>
    public class TexturePackerGenericXml : ISpriteSheetLoader
    {
        /// <summary>
        /// Parses the passed XML looking for frame info.
        /// </summary>
        /// <param name="path">Path to the XML.</param>
        /// <returns>Array of <see cref="Rectangle"/>.</returns>
        public Rectangle[] Parse(string path)
        {
            var xml = XDocument.Load(WaveServices.Storage.OpenContentFile(path));
            
            // <TextureAtlas imagePath="TimRunningSpriteSheet.png" width="1024" height="1024">
            //     <sprite n="slice25_25.png" x="426" y="2" w="119" h="129"/>
            //     <sprite n="slice26_26.png" x="304" y="405" w="118" h="130"/>
            //     [...]
            // </TextureAtlas>
            // NOTE: Width & height seem unuseful currently, so they aren't parsed
            var frames = from sprite in xml.Descendants("sprite")
                         select new Rectangle
                         {
                             X = int.Parse(sprite.Attribute("x").Value),
                             Y = int.Parse(sprite.Attribute("y").Value),
                             Width = int.Parse(sprite.Attribute("w").Value),
                             Height = int.Parse(sprite.Attribute("h").Value)
                         };

            return frames.ToArray();
        }
    }
}
