// Copyright © 2017 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Using Statements
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Math;
#endregion

namespace WaveEngine.Components.Particles
{
    /// <summary>
    /// Class that holds the information of a particle.
    /// </summary>
    internal class Particle
    {
        /// <summary>
        /// Whether the particle is alive.
        /// </summary>
        public bool Alive;

        /// <summary>
        /// Time of birth.
        /// </summary>
        public long StartTime;

        /// <summary>
        /// Size of the particle.
        /// </summary>
        public float Size;

        /// <summary>
        /// Position of the particle.
        /// </summary>
        public Vector3 Position;

        /// <summary>
        /// Velocity of the particle.
        /// </summary>
        public Vector3 Velocity;

        /// <summary>
        /// Angle of the particle.
        /// </summary>
        public float Angle;

        /// <summary>
        /// Rotation velocity of the particle.
        /// </summary>
        public float VelocityRotation;

        /// <summary>
        /// Color of the particle.
        /// </summary>
        public Color Color;

        /// <summary>
        /// Current color of the particle if the color is interpolated.
        /// </summary>
        public Color CurrentColor;

        /// <summary>
        /// Current index of the particle color if the color is based on a list of possible colors.
        /// </summary>
        public short CurrentIndex;

        /// <summary>
        /// Total life time of the particle.
        /// </summary>
        public double TimeLife;

        /// <summary>
        /// Current life left in the particle.
        /// </summary>
        public double Life;
    }
}
