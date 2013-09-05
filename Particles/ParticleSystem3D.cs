#region File Description
//-----------------------------------------------------------------------------
// ParticleSystem3D
// Copyright © 2010 - 2013 Wave Coorporation. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Math;
using WaveEngine.Framework;
#endregion

namespace WaveEngine.Components.Particles
{
    /// <summary>
    /// Particle system class.
    /// </summary>
    public class ParticleSystem3D : Component
    {
        /// <summary>
        ///     Number of instances of this component created.
        /// </summary>
        private static int instances;

        /// <summary>
        /// How much random velocity to give each particle.
        /// </summary>
        public Vector3 RandomVelocity = Vector3.Zero;

        /// <summary>
        /// Direction and strength of the gravity effect.
        /// </summary>
        public Vector3 Gravity = Vector3.Zero;

        /// <summary>
        ///     Emiter shape.
        /// </summary>
        public Shape EmitterShape = Shape.Rectangle;
                
        /// <summary>
        ///     How much X, Y and Z axis velocity to give each particle.
        /// </summary>
        public Vector3 LocalVelocity = Vector3.Zero;

        /// <summary>
        /// The max color.
        /// </summary>
        public Color MaxColor = Color.White;

        /// <summary>
        /// The max life.
        /// </summary>
        public TimeSpan MaxLife = TimeSpan.FromSeconds(1);
        
        /// <summary>
        ///     Range of color for particle life.
        /// </summary>
        public List<Color> InterpolationColors;
        
        /// <summary>
        ///     Range of values controlling the particle color and alpha.
        /// </summary>
        public Color MinColor = Color.White;

        /// <summary>
        ///     How long these particles will last.
        /// </summary>
        public TimeSpan MinLife = TimeSpan.FromSeconds(1);

        /// <summary>
        ///     Size of emiter.
        /// </summary>
        public Vector2 EmitterSize = Vector2.Zero;

        /// <summary>
        /// The end delta scale.
        /// </summary>
        private float endDeltaScale = 1;

        /// <summary>
        ///     Maximun number of particles that can be displayed at one time.
        /// </summary>
        private int numParticles = 100;

        /// <summary>
        /// Name of the texture used by this instance.
        /// </summary>
        private string textureName = null;

        /// <summary>
        ///     Shape of the particles.
        /// </summary>
        public enum Shape
        {
            /// <summary>
            ///     Rectangle shape.
            /// </summary>
            Rectangle,

            /// <summary>
            ///     Fill rectangle shape.
            /// </summary>
            FillRectangle,

            /// <summary>
            ///     Circle shape.
            /// </summary>
            Circle,

            /// <summary>
            ///     Fill circle shape.
            /// </summary>
            FillCircle
        }

        /// <summary>
        /// Behavior of a particle when collides
        /// </summary>
        public enum ParticleCollisionBehavior
        {
            /// <summary>
            /// The particle dies
            /// </summary>
            Die = 0,

            /// <summary>
            /// The particle bounces
            /// </summary>
            Bounce
        }

        /// <summary>
        /// Flags of collision
        /// </summary>
        [Flags]
        public enum ParticleCollisionFlags
        {
            /// <summary>
            /// No collision.
            /// </summary>
            None = 0,

            /// <summary>
            /// Min X collision.
            /// </summary>
            MinX = 2,

            /// <summary>
            /// Max X Collision.
            /// </summary>
            MaxX = 4,

            /// <summary>
            /// Min Y collision.
            /// </summary>
            MinY = 8,

            /// <summary>
            /// Max Y collision.
            /// </summary>
            MaxY = 16,

            /// <summary>
            /// Min Z collision.
            /// </summary>
            MinZ = 32,

            /// <summary>
            /// Max Z collision.
            /// </summary>
            MaxZ = 64
        }

        /// <summary>
        /// If collisions are enabled.
        /// </summary>
        public ParticleCollisionFlags CollisionType = ParticleCollisionFlags.None;

        /// <summary>
        /// Behavior of the particles when collide.
        /// </summary>
        public ParticleCollisionBehavior CollisionBehavior;

        /// <summary>
        /// Responsiveness to bouncing when collision.
        /// </summary>
        public float Bounciness = 0;

        /// <summary>
        /// Collision Min X.
        /// </summary>
        public float CollisionMinX;

        /// <summary>
        /// Collision Max X.
        /// </summary>
        public float CollisionMaxX;

        /// <summary>
        /// Collision Min Y.
        /// </summary>
        public float CollisionMinY;

        /// <summary>
        /// Collision Max Y.
        /// </summary>
        public float CollisionMaxY;

        /// <summary>
        /// Collision Min Z.
        /// </summary>
        public float CollisionMinZ;

        /// <summary>
        /// Collision Max Z.
        /// </summary>
        public float CollisionMaxZ;

        /// <summary>
        /// Spread of a particle velocity when collides.
        /// </summary>
        public Vector3 CollisionSpread;

        #region Properties

        /// <summary>
        ///     Gets or sets a value indicating whether alpha is enabled.
        /// </summary>
        /// <value>
        ///     <c>true</c> if alpha is enabled; otherwise, <c>false</c>.
        /// </value>
        public bool AlphaEnabled { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether this <see cref="ParticleSystem3D" /> is enabled.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is enabled; otherwise, <c>false</c>.
        /// </value>
        public bool Emit { get; set; }

        /// <summary>
        ///     Gets or sets the end delta scale.
        /// </summary>
        /// <value>
        ///     The end delta scale.
        /// </value>
        public float EndDeltaScale
        {
            get
            {
                return this.endDeltaScale;
            }

            set
            {
                if (value < 0)
                {
                    throw new InvalidOperationException("EndDeltaScale can not be less than 0");
                }

                this.endDeltaScale = value;
            }
        }

        /// <summary>
        ///     Gets or sets the initial angle of the particles.
        /// </summary>
        /// <value>
        ///     The initial angle of the particles.
        /// </value>
        public float InitialAngle { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether linear color interpolation is enabled.
        /// </summary>
        /// <value>
        ///     <c>true</c> if linear color interpolation is enabled; otherwise, <c>false</c>.
        /// </value>
        public bool LinearColorEnabled { get; set; }

        /// <summary>
        /// Gets or sets the maximum rotation speed of the particles.
        /// </summary>
        /// <value>
        /// The maximum rotation speed of the particles.
        /// </value>
        public float MaxRotateSpeed { get; set; }

        /// <summary>
        ///     Gets or sets the maximum size of the particles.
        /// </summary>
        /// <value>
        ///     The maximum size of the particles.
        /// </value>
        public float MaxSize { get; set; }

        /// <summary>
        ///     Gets or sets the minimum rotation speed of the particles.
        /// </summary>
        /// <value>
        ///     The minimum rotation speed of the particles.
        /// </value>
        public float MinRotateSpeed { get; set; }

        /// <summary>
        ///     Gets or sets the minimum size of the particles.
        /// </summary>
        /// <value>
        ///     The minimum size of the particles.
        /// </value>
        public float MinSize { get; set; }

        /// <summary>
        ///     Gets or sets the number of particles.
        /// </summary>
        /// <value>
        ///     The num particles.
        /// </value>
        public int NumParticles
        {
            get
            {
                return this.numParticles;
            }

            set
            {
                if (value <= 0 || value > 32000)
                {
                    throw new InvalidOperationException("NumParticle > 0 and < 32000");
                }

                this.numParticles = value;
            }
        }

        /// <summary>
        /// Gets or sets the name of the texture used for the particles.
        /// </summary>
        /// <value>
        /// The name of the texture used for the particles.
        /// </value>
        /// <exception cref="System.InvalidOperationException">TextureName can not be Empty</exception>
        public string TextureName
        {
            get
            {
                return this.textureName;
            }

            set
            {
                if (value == string.Empty)
                {
                    throw new InvalidOperationException("TextureName can not be Empty");
                }

                this.textureName = value;
            }
        }

        #endregion

        #region Initialize

        /// <summary>
        ///     Initializes a new instance of the <see cref="ParticleSystem3D" /> class.
        /// </summary>
        public ParticleSystem3D()
            : base("ParticleSystem" + instances++)
        {
            this.AlphaEnabled = true;
            this.Emit = true;
            this.MinSize = 1;
            this.MaxSize = 1;
            this.MaxRotateSpeed = 0;
            this.MinRotateSpeed = 0;
            this.LinearColorEnabled = false;
            this.InitialAngle = 0;
        }

        #endregion
    }
}