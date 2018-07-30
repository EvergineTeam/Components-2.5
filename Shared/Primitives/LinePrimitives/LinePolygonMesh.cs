// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Using Statements
using System.Runtime.Serialization;
using WaveEngine.Common.Attributes;
using WaveEngine.Components.Graphics3D;
#endregion

namespace WaveEngine.Components.Primitives
{
    /// <summary>
    /// Arc primitive mesh component. To render this mesh use the <see cref="LineMeshRenderer3D"/> class.
    /// </summary>
    [DataContract]
    public class LinePolygonMesh : LineArcMeshBase
    {
        /// <summary>
        /// Gets or sets the number of vertices of the regular polygon
        /// </summary>
        [RenderPropertyAsInput(MinLimit = 3, MaxLimit = 50, Tooltip = "Number of vertices that defines the regular polygon")]
        public int Vertices
        {
            get
            {
                return this.tessellation;
            }

            set
            {
                if (this.tessellation != value)
                {
                    this.tessellation = value;

                    if (this.isInitialized)
                    {
                        this.RefreshMeshes();
                    }
                }
            }
        }

        /// <inheritdoc/>
        protected override void DefaultValues()
        {
            base.DefaultValues();
            this.tessellation = 3;
        }
    }
}
