#region File Description
//-----------------------------------------------------------------------------
// StaticResources
//
// Copyright © 2010 - 2013 Wave Coorporation. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Services; 
#endregion

namespace WaveEngine.Components.Resources
{
    /// <summary>
    /// Access to Embebed resources
    /// </summary>
    public static class StaticResources
    {
        /// <summary>
        /// Default font path (embebed resource)
        /// </summary>
#if WINDOWS
        private static readonly string defaultFontResourcePath = "WaveEngine.Components.Resources.DefaultFont.wpk";
#endif
#if WINDOWS_PHONE
        private static readonly string defaultFontResourcePath = "WaveEngineWP.Components.Resources.DefaultFont.wpk";
#endif
#if METRO
        private static readonly string defaultFontResourcePath = "WaveEngineMetro.Components.Resources.DefaultFont.wpk";
#endif
#if OUYA
        private static readonly string defaultFontResourcePath = "WaveEngineOUYA.Components.Resources.DefaultFont.wpk";
#elif ANDROID
        private static readonly string defaultFontResourcePath = "WaveEngineAndroid.Components.Resources.DefaultFont.wpk";
#elif IOS
        private static readonly string defaultFontResourcePath = "WaveEngineiOS.Components.Resources.DefaultFont.wpk";
#elif MAC
		private static readonly string defaultFontResourcePath = "WaveEngineMac.Components.Resources.DefaultFont.wpk";
#elif LINUX
		private static readonly string defaultFontResourcePath = "WaveEngineLinux.Components.Resources.DefaultFont.wpk";
#endif

        /// <summary>
        /// The default sprite font
        /// </summary>
        private static SpriteFont defaultSpriteFont;

        /// <summary>
        /// Gets the default sprite font.
        /// </summary>
        /// <value>
        /// The default sprite font.
        /// </value>
        public static SpriteFont DefaultSpriteFont
        {
            get
            {
                if (defaultSpriteFont == null)
                {
                    Stream defaultFontStream = null;
#if METRO || PSSuite
                    Assembly assembly = typeof(StaticResources).GetTypeInfo().Assembly;
                    defaultFontStream = assembly.GetManifestResourceStream(defaultFontResourcePath);
#else
                    System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
                    defaultFontStream = assembly.GetManifestResourceStream(defaultFontResourcePath);
#endif
                    defaultSpriteFont = WaveServices.Assets.Global.LoadAsset<SpriteFont>(defaultFontResourcePath, defaultFontStream);
                }

                return defaultSpriteFont;
            }
        }
    }
}
