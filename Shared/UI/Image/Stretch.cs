// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Using Statements

#endregion

namespace WaveEngine.Components.UI
{
    /// <summary>
    /// Stretch for UI Image
    /// </summary>
    public enum Stretch
    {
        /// <summary>
        /// The image is not stretched to fill the output area. If the image is larger
        /// than the output area, the image is drawn to the output area, clipping what
        /// does not fit.
        /// </summary>
        None = 0,

        /// <summary>
        /// The image is scaled to fit the output area. Because the image height and width
        /// are scaled independently, the original aspect ratio of the image might not be
        /// preserved. That is, the image might be warped in order to completely fill the
        /// output container.
        /// </summary>
        Fill = 1,

        /// <summary>
        /// The image is scaled so that it fits completely within the output area. The
        /// image's aspect ratio is preserved.
        /// </summary>
        Uniform = 2,

        /// <summary>
        /// The image is scaled so that it completely fills the output area while preserving
        /// the image's original aspect ratio.
        /// </summary>
        UniformToFill = 3
    }
}
