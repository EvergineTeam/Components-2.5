#region File Description
//-----------------------------------------------------------------------------
// ParticleSystem2D
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
    public class ParticleSystem2D : Component
    {
        /// <summary>
        /// Max number of particles.
        /// </summary>
        private const int MAXPARTICLES = 32000;

        /// <summary>
        ///     Number of instances of this component created.
        /// </summary>
        private static int instances;

        /// <summary>
        /// How much random velocity to give each particle.
        /// </summary>
        public Vector2 RandomVelocity = Vector2.Zero;

        /// <summary>
        /// Direction and strength of the gravity effect.
        /// </summary>
        public Vector2 Gravity = Vector2.Zero;

        /// <summary>
        /// Particles emitted per second.
        /// </summary>
        public float EmitRate = 0;

        /// <summary>
        ///     Emiter shape.
        /// </summary>
        public Shape EmitterShape = Shape.Rectangle;

        /// <summary>
        ///     How much X and Y axis velocity to give each particle.
        /// </summary>
        public Vector2 LocalVelocity = Vector2.Zero;

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
            /// Bottom collision.
            /// </summary>
            Bottom = 2,

            /// <summary>
            /// Top Collision.
            /// </summary>
            Top = 4,

            /// <summary>
            /// Left collision.
            /// </summary>
            Left = 8,

            /// <summary>
            /// Right collision.
            /// </summary>
            Right = 16
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
        /// Y Collision Bottom.
        /// </summary>
        public float CollisionBottom;

        /// <summary>
        /// Y Collision Top.
        /// </summary>
        public float CollisionTop;

        /// <summary>
        /// X Collision Left.
        /// </summary>
        public float CollisionLeft;

        /// <summary>
        /// X Collision Right.
        /// </summary>
        public float CollisionRight;

        /// <summary>
        /// Spread of a particle velocity when collides.
        /// </summary>
        public Vector2 CollisionSpread;

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
        ///     Gets or sets the variation of the initial angle of the particles.
        /// </summary>
        /// <value>
        ///     The variation of the initial angle of the particles.
        /// </value>
        public float InitialAngleVariation { get; set; }

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
        /// Gets or sets a value indicating whether the particles must be drawed in creation order.
        /// </summary>
        public bool SortEnabled { get; set; }

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
                if (value <= 0 || value > MAXPARTICLES)
                {
                    throw new InvalidOperationException("NumParticle > 0 and < " + MAXPARTICLES);
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
        /// Initializes a new instance of the <see cref="ParticleSystem2D" /> class.
        /// </summary>
        public ParticleSystem2D()
            : this("ParticleSystem2D" + instances++)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ParticleSystem2D" /> class.
        /// </summary>
        /// <param name="name">name of the instance</param>
        public ParticleSystem2D(string name)
            : base(name)
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