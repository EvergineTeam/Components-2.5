#region File Description
//-----------------------------------------------------------------------------
// ParticleSystem3D
// Copyright © 2017 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using WaveEngine.Common.Attributes;
using WaveEngine.Common.Attributes.Converters;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Math;
using WaveEngine.Framework;
#endregion

namespace WaveEngine.Components.Particles
{
    /// <summary>
    /// Particle system class.
    /// </summary>
    [DataContract(Namespace = "WaveEngine.Components.Graphics3D")]

    public class ParticleSystem3D : Component
    {
        /// <summary>
        /// Gets or sets the Number of instances of this component created.
        /// </summary>
        private static int instances;

        /// <summary>
        /// The end delta scale.
        /// </summary>
        [DataMember]
        private float endDeltaScale = 1;

        /// <summary>
        ///     Maximun number of particles that can be displayed at one time.
        /// </summary>
        [DataMember]
        private int numParticles = 100;

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

        #region Properties

        /// <summary>
        ///     Gets or sets a value indicating whether this <see cref="ParticleSystem3D" /> is enabled.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is enabled; otherwise, <c>false</c>.
        /// </value>
        [DataMember]
        public bool Emit { get; set; }

        /// <summary>
        ///     Gets or sets the number of particles.
        /// </summary>
        /// <value>
        ///     The num particles.
        /// </value>
        [RenderPropertyAsInput(MinLimit = 1, MaxLimit = 32000)]
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
        ///     Gets or sets the Emiter shape.
        /// </summary>
        [DataMember]
        public Shape EmitterShape { get; set; }

        /// <summary>
        ///     Gets or sets the Size of emiter.
        /// </summary>
        [DataMember]
        public Vector2 EmitterSize { get; set; }

        /// <summary>
        /// Gets or sets the Particles emitted per second.
        /// </summary>
        [DataMember]
        public float EmitRate { get; set; }

        /// <summary>
        ///     Gets or sets the How much X, Y and Z axis velocity to give each particle.
        /// </summary>
        [DataMember]
        public Vector3 LocalVelocity { get; set; }

        /// <summary>
        /// Gets or sets the How much random velocity to give each particle.
        /// </summary>
        [DataMember]
        public Vector3 RandomVelocity { get; set; }

        /// <summary>
        /// Gets or sets the Direction and strength of the gravity effect.
        /// </summary>
        [DataMember]
        public Vector3 Gravity { get; set; }

        /// <summary>
        ///     Gets or sets the initial angle of the particles.
        /// </summary>
        /// <value>
        ///     The initial angle of the particles.
        /// </value>
        [DataMember]
        public float InitialAngle { get; set; }

        /// <summary>
        ///     Gets or sets the minimum rotation speed of the particles.
        /// </summary>
        /// <value>
        ///     The minimum rotation speed of the particles.
        /// </value>
        [DataMember]
        [RenderProperty(typeof(FloatRadianToDegreeConverter))]
        public float MinRotateSpeed { get; set; }

        /// <summary>
        /// Gets or sets the maximum rotation speed of the particles.
        /// </summary>
        /// <value>
        /// The maximum rotation speed of the particles.
        /// </value>
        [DataMember]
        [RenderProperty(typeof(FloatRadianToDegreeConverter))]
        public float MaxRotateSpeed { get; set; }

        /// <summary>
        ///     Gets or sets the minimum size of the particles.
        /// </summary>
        /// <value>
        ///     The minimum size of the particles.
        /// </value>
        [DataMember]
        public float MinSize { get; set; }

        /// <summary>
        ///     Gets or sets the maximum size of the particles.
        /// </summary>
        /// <value>
        ///     The maximum size of the particles.
        /// </value>
        [DataMember]
        public float MaxSize { get; set; }

        /// <summary>
        ///     Gets or sets the end delta scale.
        /// </summary>
        /// <value>
        ///     The end delta scale.
        /// </value>
        [RenderPropertyAsFInput(0, float.MaxValue)]
        [DataMember]
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
        ///     Gets or sets the How long these particles will last. (In seconds)
        /// </summary>
        [DataMember]
        public float MinLife { get; set; }

        /// <summary>
        /// Gets or sets the The max life. (In seconds)
        /// </summary>
        [DataMember]
        public float MaxLife { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether linear color interpolation is enabled.
        /// </summary>
        /// <value>
        ///     <c>true</c> if linear color interpolation is enabled; otherwise, <c>false</c>.
        /// </value>
        [DataMember]
        [RenderProperty(Tag = 1)]
        public bool LinearColorEnabled { get; set; }

        /// <summary>
        ///     Gets or sets the Range of color for particle life.
        /// </summary>
        [DataMember]
        [RenderProperty(AttatchToTag = 1, AttachToValue = true)]
        public List<Color> InterpolationColors { get; set; }

        /// <summary>
        ///     Gets or sets the Range of values controlling the particle color and alpha.
        /// </summary>
        [DataMember]
        public Color MinColor { get; set; }

        /// <summary>
        /// Gets or sets the The max color.
        /// </summary>
        [DataMember]
        public Color MaxColor { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether alpha is enabled.
        /// </summary>
        /// <value>
        ///     <c>true</c> if alpha is enabled; otherwise, <c>false</c>.
        /// </value>
        [DataMember]
        public bool AlphaEnabled { get; set; }

        /// <summary>
        /// Gets or sets Collision Type
        /// </summary>
        [DataMember]
        public ParticleCollisionFlags CollisionType { get; set; }

        /// <summary>
        /// Gets or sets Collision Minimum
        /// </summary>
        [DataMember]
        public Vector3 CollisionMin { get; set; }

        /// <summary>
        /// Gets or sets Collision Maximum
        /// </summary>
        [DataMember]
        public Vector3 CollisionMax { get; set; }

        /// <summary>
        /// Gets or sets Collision Spread
        /// </summary>
        [DataMember]
        public Vector3 CollisionSpread { get; set; }

        /// <summary>
        /// Gets or sets Collision Behavior
        /// </summary>
        [DataMember]
        public ParticleCollisionBehavior CollisionBehavior { get; set; }

        /// <summary>
        /// Gets or sets Bounciness
        /// </summary>
        [DataMember]
        public float Bounciness { get; set; }

        #endregion

        #region Initialize

        /// <summary>
        ///     Initializes a new instance of the <see cref="ParticleSystem3D" /> class.
        /// </summary>
        public ParticleSystem3D()
            : base("ParticleSystem" + instances++)
        {
        }

        /// <summary>
        /// Set default values
        /// </summary>
        protected override void DefaultValues()
        {
            base.DefaultValues();

            this.AlphaEnabled = true;
            this.NumParticles = 100;
            this.Emit = true;
            this.MinSize = 0.1f;
            this.MaxSize = 0.5f;
            this.LocalVelocity = new Vector3(0, 0.1f, 0);
            this.RandomVelocity = new Vector3(0.1f);
            this.EmitterShape = Shape.Rectangle;
            this.MaxColor = Color.White;
            this.MinLife = 0.2f;
            this.MaxLife = 1.0f;
            this.EmitterSize = Vector2.One;
        }
        #endregion
    }
}