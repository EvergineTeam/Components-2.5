// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Using Statements
using System.Runtime.Serialization;
using WaveEngine.Common.Attributes;
using WaveEngine.Common.Attributes.Converters;
using WaveEngine.Common.Math;
using WaveEngine.Components.Graphics3D;
#endregion

namespace WaveEngine.Components.Primitives
{
    /// <summary>
    /// Arc primitive mesh component. To render this mesh use the <see cref="LineMeshRenderer3D"/> class.
    /// </summary>
    [DataContract]
    public class LineArcMesh : LineArcMeshBase
    {
        /// <summary>
        /// Gets or sets the angle of the arc in degrees
        /// </summary>
        [RenderPropertyAsFInput(typeof(FloatRadianToDegreeConverter), MinLimit = 0, MaxLimit = 360, Tooltip = "Angle of the arc in degrees")]
        public float Angle
        {
            get
            {
                return this.angle;
            }

            set
            {
                if (this.angle != value)
                {
                    this.angle = value;

                    if (this.isInitialized)
                    {
                        this.RefreshMeshes();
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets yhe number of iterations used for generating the line mesh
        /// </summary>
        [RenderPropertyAsInput(MinLimit = 3, MaxLimit = 50, Tooltip = "The number of iterations used for generating the line mesh")]
        public int Tessellation
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
    }
}
