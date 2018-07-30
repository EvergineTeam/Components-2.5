// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Using Statements
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using WaveEngine.Common.Attributes;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Math;
using WaveEngine.Framework;
#endregion

namespace WaveEngine.Components.Particles
{
    /// <summary>
    /// Particle system class.
    /// </summary>
    [DataContract(Namespace = "WaveEngine.Components.Particles")]
    public class ParticleSystem2D : Component
    {
        /// <summary>
        /// Max number of particles.
        /// </summary>
        private const int MAXPARTICLES = 32000;

        /// <summary>
        /// Number of instances of this component created.
        /// </summary>
        private static int instances;

        /// <summary>
        /// The end delta scale.
        /// </summary>
        private float endDeltaScale = 1;

        /// <summary>
        ///     Maximun number of particles that can be displayed at one time.
        /// </summary>
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
            FillCircle,

            /// <summary>
            ///     Fill box shape.
            /// </summary>
            FillBox,
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
        /// Y Collision Bottom.
        /// </summary>
        [DataMember]
        public float CollisionBottom;

        /// <summary>
        /// Y Collision Top.
        /// </summary>
        [DataMember]
        public float CollisionTop;

        /// <summary>
        /// X Collision Left.
        /// </summary>
        [DataMember]
        public float CollisionLeft;

        /// <summary>
        /// X Collision Right.
        /// </summary>
        [DataMember]
        public float CollisionRight;

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
        [DataMember]
        [RenderPropertyAsInput(MinLimit = 1, MaxLimit = MAXPARTICLES)]
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
        /// Gets or sets Emitter Shape
        /// </summary>
        [DataMember]
        public Shape EmitterShape { get; set; }

        /// <summary>
        /// Gets or sets Emitter Size
        /// </summary>
        [DataMember]
        public Vector3 EmitterSize { get; set; }

        /// <summary>
        /// Gets or sets Emit Rate
        /// </summary>
        [DataMember]
        public float EmitRate { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the particles must be drawed in creation order.
        /// </summary>
        [DataMember]
        public bool SortEnabled { get; set; }

        /// <summary>
        /// Gets or sets Local Velocity
        /// </summary>
        [DataMember]
        public Vector2 LocalVelocity { get; set; }

        /// <summary>
        /// Gets or sets Ramdom Velocity
        /// </summary>
        [DataMember]
        public Vector2 RandomVelocity { get; set; }

        /// <summary>
        /// Gets or sets Gravity
        /// </summary>
        [DataMember]
        public Vector2 Gravity { get; set; }

        /// <summary>
        ///     Gets or sets the initial angle of the particles.
        /// </summary>
        /// <value>
        ///     The initial angle of the particles.
        /// </value>
        [DataMember]
        public float InitialAngle { get; set; }

        /// <summary>
        ///     Gets or sets the variation of the initial angle of the particles.
        /// </summary>
        /// <value>
        ///     The variation of the initial angle of the particles.
        /// </value>
        [DataMember]
        public float InitialAngleVariation { get; set; }

        /// <summary>
        ///     Gets or sets the minimum rotation speed of the particles.
        /// </summary>
        /// <value>
        ///     The minimum rotation speed of the particles.
        /// </value>
        [DataMember]
        public float MinRotateSpeed { get; set; }

        /// <summary>
        /// Gets or sets the maximum rotation speed of the particles.
        /// </summary>
        /// <value>
        /// The maximum rotation speed of the particles.
        /// </value>
        [DataMember]
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
        [RenderPropertyAsFInput(MinLimit = 0, MaxLimit = float.MaxValue)]
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
        ///     Gets or sets a value indicating whether linear color interpolation is enabled.
        /// </summary>
        /// <value>
        ///     <c>true</c> if linear color interpolation is enabled; otherwise, <c>false</c>.
        /// </value>
        [DataMember]
        [RenderProperty(Tag = 1)]
        public bool LinearColorEnabled { get; set; }

        /// <summary>
        /// Gets or sets Interpolation Colors
        /// </summary>
        [DataMember]
        [RenderProperty(AttatchToTag = 1, AttachToValue = true)]
        public List<Color> InterpolationColors { get; set; }

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
        /// Gets or sets Collision Spread
        /// </summary>
        [DataMember]
        public Vector2 CollisionSpread { get; set; }

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
        }

        /// <summary>
        /// Set default values
        /// </summary>
        protected override void DefaultValues()
        {
            base.DefaultValues();

            this.AlphaEnabled = true;
            this.Emit = true;
            this.MinSize = 20;
            this.MaxSize = 50;
            this.MaxRotateSpeed = 0;
            this.MinRotateSpeed = 0;
            this.LinearColorEnabled = false;
            this.InitialAngle = 0;
            this.LocalVelocity = Vector2.Zero;
            this.RandomVelocity = new Vector2(0.5f);
            this.Gravity = Vector2.Zero;
            this.EmitRate = 0;
            this.EmitterShape = Shape.Rectangle;
            this.MaxColor = Color.White;
            this.MinLife = 1;
            this.MaxLife = 2;
            this.MinColor = Color.White;
        }

        #endregion
    }
}
