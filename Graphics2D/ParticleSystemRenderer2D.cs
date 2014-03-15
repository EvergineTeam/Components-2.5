#region File Description
//-----------------------------------------------------------------------------
// ParticleSystemRenderer2D
//
// Copyright © 2010 - 2013 Wave Coorporation. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements

using System;

using WaveEngine.Common.Graphics;
using WaveEngine.Common.Graphics.VertexFormats;
using WaveEngine.Common.Math;
using WaveEngine.Components.Particles;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Services;
using System.Diagnostics.CodeAnalysis;
using WaveEngine.Materials;
using System.Diagnostics;

using WaveRandom = WaveEngine.Framework.Services.Random;

#if WINDOWS_PHONE
using WaveEngine.Common.Helpers;
#endif
#endregion

namespace WaveEngine.Components.Graphics2D
{
    /// <summary>
    /// Renders a 2D particle system on the screen.
    /// </summary>
    public class ParticleSystemRenderer2D : Drawable2D
    {
        /// <summary>
        /// Velocity adjust of the particles.
        /// </summary>
        private const float VELOCITYFACTOR = 60f / 1000f;

        /// <summary>
        /// The init time multipler
        /// </summary>
        private const short InitTimeMultipler = 10;

        /// <summary>
        /// Number of instances of this component created.
        /// </summary>
        private static int instances;

        /// <summary>
        /// The texcoord1
        /// </summary>
        private static readonly Vector2 TEXCOORD1 = new Vector2 { X = 0, Y = 0 };

        /// <summary>
        /// The texcoord2
        /// </summary>
        private static readonly Vector2 TEXCOORD2 = new Vector2 { X = 1, Y = 0 };

        /// <summary>
        /// The texcoord3
        /// </summary>
        private static readonly Vector2 TEXCOORD3 = new Vector2 { X = 1, Y = 1 };

        /// <summary>
        /// The texcoord4
        /// </summary>
        private static readonly Vector2 TEXCOORD4 = new Vector2 { X = 0, Y = 1 };

        /// <summary>
        /// Scale of the viewport.
        /// </summary>
        private Vector2 viewportScale;

        /// <summary>
        /// Translation of the viewport.
        /// </summary>
        private Vector2 viewportTranslate;

        /// <summary>
        /// If the viewport is enabled.
        /// </summary>
        private bool viewportEnabled;

        /// <summary>
        /// Particle system rendered.
        /// </summary>
        [RequiredComponent]
        public ParticleSystem2D System;

        /// <summary>
        /// Materials used rendering the particle system.
        /// </summary>
        [RequiredComponent]
        public Material2D Material;

        /// <summary>
        /// Transform of the particle system emitter.
        /// </summary>
        [RequiredComponent]
        public Transform2D Transform;

        /// <summary>
        /// The random
        /// </summary>
        private WaveRandom random;

        /// <summary>
        /// The alive particles
        /// </summary>
        private int aliveParticles;

        /// <summary>
        /// The num particles
        /// </summary>
        private int numParticles;

        /// <summary>
        /// The num vertices
        /// </summary>
        private int numVertices;

        /// <summary>
        /// The num indices
        /// </summary>
        private int numIndices;

        /// <summary>
        /// The num primitives
        /// </summary>
        private int numPrimitives;

        /// <summary>
        /// The particles
        /// </summary>
        private Particle[] particles;

        /// <summary>
        /// If the current frame a particle was emited.
        /// </summary>
        private bool emitedParticle;

        /// <summary>
        /// The vertices
        /// </summary>
        private VertexPositionColorTexture[] vertices;

        /// <summary>
        /// The index buffer
        /// </summary>
        private IndexBuffer indexBuffer;

        /// <summary>
        /// The vertex buffer
        /// </summary>
        private DynamicVertexBuffer vertexBuffer;

        /// <summary>
        /// The settings
        /// </summary>
        private ParticleSystem2D settings;

        /// <summary>
        /// The internal enabled
        /// </summary>
        private bool internalEnabled;

        /// <summary>
        /// Wether this instance has been disposed.
        /// </summary>
        private bool disposed;

        /// <summary>
        /// The vertex1
        /// </summary>
        private Vector3 vertex1 = new Vector3 { X = -1, Y = 1, Z = 0 };

        /// <summary>
        /// The vertex2
        /// </summary>
        private Vector3 vertex2 = new Vector3 { X = 1, Y = 1, Z = 0 };

        /// <summary>
        /// The vertex3
        /// </summary>
        private Vector3 vertex3 = new Vector3 { X = 1, Y = -1, Z = 0 };

        /// <summary>
        /// The vertex4
        /// </summary>
        private Vector3 vertex4 = new Vector3 { X = -1, Y = -1, Z = 0 };

        /// <summary>
        /// The rotation matrix
        /// </summary>
        private Matrix rotationMatrix;

        /// <summary>
        /// The rotation vector
        /// </summary>
        private Vector3 rotationVector;

        /// <summary>
        /// Time passed between 2 particles.
        /// </summary>
        private float emitLapse;

        /// <summary>
        /// Remainder time of the prev frame.
        /// </summary>
        private float emitRemainder;

        #region Initialize

        /// <summary>
        /// Initializes a new instance of the <see cref="ParticleSystemRenderer2D" /> class.
        /// </summary>
        public ParticleSystemRenderer2D()
            : this("ParticleSystemRenderer2D" + instances, DefaultLayers.Alpha)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ParticleSystemRenderer2D"/> class.
        /// </summary>
        /// <param name="name">Name of this instance.</param>
        /// <param name="layerType">Type of the layer.</param>
        public ParticleSystemRenderer2D(string name, Type layerType)
            : base(name, layerType)
        {
            instances++;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Draws the particle system.
        /// </summary>
        /// <param name="gameTime">The elapsed game time.</param>
        public override void Draw(TimeSpan gameTime)
        {
            // Optimización del estado inactivo
            if (!this.internalEnabled && !this.settings.Emit)
            {
                return;
            }

            this.viewportEnabled = WaveServices.ViewportManager.IsActivated;
            if (this.viewportEnabled)
            {
                this.viewportTranslate.X = WaveServices.ViewportManager.TranslateX(0);
                this.viewportTranslate.Y = WaveServices.ViewportManager.TranslateY(0);
                this.viewportScale.X = WaveServices.ViewportManager.RatioX;
                this.viewportScale.Y = WaveServices.ViewportManager.RatioY;
            }

            // Sets the timeFactor for updating velocities.
            float timeFactor = VELOCITYFACTOR * (float)gameTime.TotalMilliseconds;

            this.aliveParticles = 0;

            // Sorts the array according age.
            if (this.settings.SortEnabled && this.emitedParticle)
            {
                Array.Sort(
                    this.particles,
                    delegate(Particle p1, Particle p2)
                    {
                        if (p1.StartTime != p2.StartTime)
                        {
                            return p1.StartTime.CompareTo(p2.StartTime);
                        }
                        else
                        {
                            return p1.Life.CompareTo(p2.Life);
                        }
                    });
            }

            this.emitedParticle = false;

            bool infiniteCreation = this.settings.EmitRate == 0;
            int particlesToCreate = 0;

            if (!infiniteCreation)
            {
                float particlesToCreateF = ((float)gameTime.TotalMilliseconds / this.emitLapse) + this.emitRemainder;
                particlesToCreate = (int)particlesToCreateF;
                this.emitRemainder = particlesToCreateF - particlesToCreate;
            }

            // Recorremos todas las partículas
            for (int i = 0, j = 0; i < this.numParticles; i++)
            {
                Particle p = this.particles[i];

                // Update particle
                // Si la partícula está viva seguimos consumiendo su vida
                if (p.Alive)
                {
                    p.Life = p.Life - gameTime.TotalMilliseconds;
                }

                // Si la partícula ha llegado al final de su vida
                if (p.Life < 0)
                {
                    // Si permitimos la emisión de nuevas partículas y debe resetear sus valores
                    if (this.settings.Emit && (infiniteCreation || (particlesToCreate > 0)))
                    {
                        this.ResetParticle(ref p);
                        particlesToCreate--;
                    }
                    else
                    {
                        // Si la particula no está viva
                        p.Alive = false;
                        p.Size = 0;
                    }
                }
                else if (!this.Owner.Scene.IsPaused)
                {
                    // Si la particula está viva y visible
                    // p.Position
                    p.Velocity.X = p.Velocity.X + (timeFactor * this.settings.Gravity.X);
                    p.Velocity.Y = p.Velocity.Y + (timeFactor * this.settings.Gravity.Y);

                    // Calculate the rotation with the transform
                    Vector3.TransformNormal(ref p.Velocity, ref this.rotationMatrix, out this.rotationVector);

                    p.Position.X = p.Position.X + (timeFactor * this.rotationVector.X);
                    p.Position.Y = p.Position.Y + (timeFactor * this.rotationVector.Y);

                    if (this.settings.CollisionType != ParticleSystem2D.ParticleCollisionFlags.None)
                    {
                        // Checks vertical collisions
                        if (this.settings.CollisionType.HasFlag(ParticleSystem2D.ParticleCollisionFlags.Bottom) && (p.Position.Y > this.settings.CollisionBottom))
                        {
                            switch (this.settings.CollisionBehavior)
                            {
                                case ParticleSystem2D.ParticleCollisionBehavior.Die:
                                    // Kills the particle
                                    p.Alive = false;
                                    p.Size = 0;
                                    p.Life = -1;
                                    break;
                                case ParticleSystem2D.ParticleCollisionBehavior.Bounce:
                                    // Mirrors the position and velocity with the bounciness factor.
                                    float bounciness = this.settings.Bounciness;
                                    p.Position.Y = this.settings.CollisionBottom - (bounciness * (p.Position.Y - this.settings.CollisionBottom));

                                    // Applies the collision spread to the velocity
                                    if (this.settings.CollisionSpread != Vector2.Zero)
                                    {
                                        p.Velocity.X = bounciness * (p.Velocity.X + (this.settings.CollisionSpread.X * (((float)this.random.NextDouble() * 2) - 1)));
                                        p.Velocity.Y = -bounciness * (p.Velocity.Y + (this.settings.CollisionSpread.Y * (((float)this.random.NextDouble() * 2) - 1)));
                                    }
                                    else
                                    {
                                        p.Velocity.X = bounciness * p.Velocity.X;
                                        p.Velocity.Y = -bounciness * p.Velocity.Y;
                                    }

                                    break;
                                default:
                                    // Do nothing
                                    break;
                            }
                        }
                        else if (this.settings.CollisionType.HasFlag(ParticleSystem2D.ParticleCollisionFlags.Top) && (p.Position.Y < this.settings.CollisionTop))
                        {
                            switch (this.settings.CollisionBehavior)
                            {
                                case ParticleSystem2D.ParticleCollisionBehavior.Die:
                                    // Kills the particle
                                    p.Alive = false;
                                    p.Size = 0;
                                    p.Life = -1;
                                    break;
                                case ParticleSystem2D.ParticleCollisionBehavior.Bounce:
                                    // Mirrors the position and velocity with the bounciness factor.
                                    float bounciness = this.settings.Bounciness;
                                    p.Position.Y = this.settings.CollisionTop + (bounciness * (this.settings.CollisionTop - p.Position.Y));

                                    // Applies the collision spread to the velocity
                                    if (this.settings.CollisionSpread != Vector2.Zero)
                                    {
                                        p.Velocity.X = bounciness * (p.Velocity.X + (this.settings.CollisionSpread.X * (((float)this.random.NextDouble() * 2) - 1)));
                                        p.Velocity.Y = -bounciness * (p.Velocity.Y + (this.settings.CollisionSpread.Y * (((float)this.random.NextDouble() * 2) - 1)));
                                    }
                                    else
                                    {
                                        p.Velocity.X = bounciness * p.Velocity.X;
                                        p.Velocity.Y = -bounciness * p.Velocity.Y;
                                    }

                                    break;
                                default:
                                    // Do nothing
                                    break;
                            }
                        }

                        // Checks horizontal collisions
                        if (this.settings.CollisionType.HasFlag(ParticleSystem2D.ParticleCollisionFlags.Right) && (p.Position.X > this.settings.CollisionRight))
                        {
                            switch (this.settings.CollisionBehavior)
                            {
                                case ParticleSystem2D.ParticleCollisionBehavior.Die:
                                    // Kills the particle
                                    p.Alive = false;
                                    p.Size = 0;
                                    p.Life = -1;
                                    break;
                                case ParticleSystem2D.ParticleCollisionBehavior.Bounce:
                                    // Mirrors the position and velocity with the bounciness factor.
                                    float bounciness = this.settings.Bounciness;
                                    p.Position.X = this.settings.CollisionRight - (bounciness * (p.Position.X - this.settings.CollisionRight));

                                    // Applies the collision spread to the velocity
                                    if (this.settings.CollisionSpread != Vector2.Zero)
                                    {
                                        p.Velocity.X = -bounciness * (p.Velocity.X + (this.settings.CollisionSpread.X * (((float)this.random.NextDouble() * 2) - 1)));
                                        p.Velocity.Y = bounciness * (p.Velocity.Y + (this.settings.CollisionSpread.Y * (((float)this.random.NextDouble() * 2) - 1)));
                                    }
                                    else
                                    {
                                        p.Velocity.X = -bounciness * p.Velocity.X;
                                        p.Velocity.Y = bounciness * p.Velocity.Y;
                                    }

                                    break;
                                default:
                                    // Do nothing
                                    break;
                            }
                        }
                        else if (this.settings.CollisionType.HasFlag(ParticleSystem2D.ParticleCollisionFlags.Left) && (p.Position.X < this.settings.CollisionLeft))
                        {
                            switch (this.settings.CollisionBehavior)
                            {
                                case ParticleSystem2D.ParticleCollisionBehavior.Die:
                                    // Kills the particle
                                    p.Alive = false;
                                    p.Size = 0;
                                    p.Life = -1;
                                    break;
                                case ParticleSystem2D.ParticleCollisionBehavior.Bounce:
                                    // Mirrors the position and velocity with the bounciness factor.
                                    float bounciness = this.settings.Bounciness;
                                    p.Position.X = this.settings.CollisionLeft + (bounciness * (this.settings.CollisionLeft - p.Position.X));

                                    // Applies the collision spread to the velocity
                                    if (this.settings.CollisionSpread != Vector2.Zero)
                                    {
                                        p.Velocity.X = -bounciness * (p.Velocity.X + (this.settings.CollisionSpread.X * (((float)this.random.NextDouble() * 2) - 1)));
                                        p.Velocity.Y = bounciness * (p.Velocity.Y + (this.settings.CollisionSpread.Y * (((float)this.random.NextDouble() * 2) - 1)));
                                    }
                                    else
                                    {
                                        p.Velocity.X = -bounciness * p.Velocity.X;
                                        p.Velocity.Y = bounciness * p.Velocity.Y;
                                    }

                                    break;
                                default:
                                    // Do nothing
                                    break;
                            }
                        }
                    }

                    // Rotate
                    p.Angle = p.Angle + (timeFactor * p.VelocityRotation);

                    // Color
                    if (this.settings.LinearColorEnabled && p.TimeLife != 0 && this.settings.InterpolationColors != null && this.settings.InterpolationColors.Count > 0)
                    {
                        // Num destiny colors
                        short count = (short)this.settings.InterpolationColors.Count;

                        // Life time of one color
                        double timeColorAge = p.TimeLife / count;

                        // destiny Color
                        short destinyIndex = (short)(p.Life / timeColorAge);

                        // Age between 1 and 0 for current color
                        double currentColorAge = (p.Life - (timeColorAge * destinyIndex)) / timeColorAge;

                        // Iterpolation color
                        if (currentColorAge > 0)
                        {
                            float amount = 1 - (float)currentColorAge;

                            short index = (short)((count - 1) - destinyIndex);
                            if (index != p.CurrentIndex)
                            {
                                p.CurrentColor = p.Color;
                                p.CurrentIndex = index;
                            }

                            Color destinyColor = this.settings.InterpolationColors[p.CurrentIndex];

                            p.Color = Color.Lerp(ref p.CurrentColor, ref destinyColor, amount);
                        }
                    }

                    if (this.settings.AlphaEnabled && p.TimeLife.Distinct(0))
                    {
                        double age = p.Life / p.TimeLife;
                        double alpha = Math.Pow(age, 0.333333);
                        p.Color.A = (byte)(255 * age);
                        p.Color.R = (byte)(p.Color.R * age);
                        p.Color.G = (byte)(p.Color.G * age);
                        p.Color.B = (byte)(p.Color.B * age);
                    }
                }

                // Update Vertex Buffer
                Matrix world = this.CalculateLocalWorld(ref p);

                Vector3.Transform(ref this.vertex1, ref world, out this.vertices[j].Position);
                this.vertices[j++].Color = p.Color;

                Vector3.Transform(ref this.vertex2, ref world, out this.vertices[j].Position);
                this.vertices[j++].Color = p.Color;

                Vector3.Transform(ref this.vertex3, ref world, out this.vertices[j].Position);
                this.vertices[j++].Color = p.Color;

                Vector3.Transform(ref this.vertex4, ref world, out this.vertices[j].Position);
                this.vertices[j++].Color = p.Color;

                // Si la partícula está viva la contamos
                if (p.Alive)
                {
                    this.aliveParticles++;
                }
            }

            this.internalEnabled = this.aliveParticles > 0;

            base.Draw(gameTime);
        }
        #endregion

        #region Private Methods

        /// <summary>
        /// Resolve the dependencies
        /// </summary>
        protected override void ResolveDependencies()
        {
            base.ResolveDependencies();

            // If the entity is initialized, we need to update the current particle system
            if (this.isInitialized)
            {
                this.LoadParticleSystem();
            }
        }

        /// <summary>
        /// Performs further custom initialization for this instance.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();

            this.LoadParticleSystem();
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    this.GraphicsDevice.DestroyIndexBuffer(this.indexBuffer);
                    this.GraphicsDevice.DestroyVertexBuffer(this.vertexBuffer);
                    this.disposed = true;
                }
            }
        }

        /// <summary>
        /// Set the current settings to the particle system attached
        /// </summary>
        private void LoadParticleSystem()
        {
            this.random = WaveServices.Random;

            if (this.indexBuffer != null)
            {
                this.GraphicsDevice.DestroyIndexBuffer(this.indexBuffer);
            }

            if (this.vertexBuffer != null)
            {
                this.GraphicsDevice.DestroyVertexBuffer(this.vertexBuffer);
            }

            this.settings = this.System;
            this.numParticles = this.System.NumParticles;
            this.numPrimitives = this.numParticles * 2;
            this.numVertices = this.numParticles * 4;
            this.numIndices = this.numParticles * 6;
            this.particles = new Particle[this.numParticles];

            // Sets the time passed between 2 particles creation.
            this.emitLapse = (this.settings.EmitRate > 0) ? 1000 / this.settings.EmitRate : 0;

            // Create Indexbuffer
            ushort[] indices = new ushort[this.numIndices];

            for (int i = 0; i < this.numParticles; i++)
            {
                indices[(i * 6) + 0] = (ushort)((i * 4) + 0);
                indices[(i * 6) + 1] = (ushort)((i * 4) + 2);
                indices[(i * 6) + 2] = (ushort)((i * 4) + 1);

                indices[(i * 6) + 3] = (ushort)((i * 4) + 0);
                indices[(i * 6) + 4] = (ushort)((i * 4) + 3);
                indices[(i * 6) + 5] = (ushort)((i * 4) + 2);
            }

            this.indexBuffer = new IndexBuffer(indices);
            this.GraphicsDevice.BindIndexBuffer(this.indexBuffer);

            // Initialize Particles
            for (int i = 0; i < this.numParticles; i++)
            {
                double life = (this.settings.EmitRate > 0) ? -1 : TimeSpan.FromMilliseconds(this.random.NextDouble() * (this.numParticles * InitTimeMultipler)).TotalMilliseconds;

                this.particles[i] = new Particle()
                {
                    Alive = true,
                    Life = life
                };
            }

            this.vertices = new VertexPositionColorTexture[this.numVertices];

            // Initializes the coordinate textures of the vertices
            for (int i = 0; i < this.numVertices; i++)
            {
                this.vertices[i++].TexCoord = TEXCOORD1;
                this.vertices[i++].TexCoord = TEXCOORD2;
                this.vertices[i++].TexCoord = TEXCOORD3;
                this.vertices[i].TexCoord = TEXCOORD4;
            }

            this.vertexBuffer = new DynamicVertexBuffer(VertexPositionColorTexture.VertexFormat);
            this.vertexBuffer.SetData(this.vertices, this.numVertices);
            this.GraphicsDevice.BindVertexBuffer(this.vertexBuffer);
        }

        /// <summary>
        /// Resets the particle.
        /// </summary>
        /// <param name="p">The p.</param>
        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1126:PrefixCallsCorrectly", Justification = "Reviewed. Suppression is OK here.")]
        private void ResetParticle(ref Particle p)
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException("ParticleSystemRenderer");
            }

            this.emitedParticle = true;

            p.Alive = true;

            if (this.settings.MinLife != this.settings.MaxLife)
            {
                TimeSpan time = TimeSpan.FromMilliseconds(MathHelper.Lerp(
                                                                        (float)this.settings.MinLife.TotalMilliseconds,
                                                                        (float)this.settings.MaxLife.TotalMilliseconds,
                                                                        (float)this.random.NextDouble()));
                p.TimeLife = time.TotalMilliseconds;
            }
            else
            {
                p.TimeLife = this.settings.MinLife.TotalMilliseconds;
            }

            p.Life = p.TimeLife;

            // Velocity
            if (this.settings.RandomVelocity != Vector2.Zero)
            {
                p.Velocity.X = this.settings.LocalVelocity.X + (this.settings.RandomVelocity.X * (((float)this.random.NextDouble() * 2) - 1));
                p.Velocity.Y = this.settings.LocalVelocity.Y + (this.settings.RandomVelocity.Y * (((float)this.random.NextDouble() * 2) - 1));
            }
            else
            {
                p.Velocity.X = this.settings.LocalVelocity.X;
                p.Velocity.Y = this.settings.LocalVelocity.Y;
            }

            p.Position.X = this.Transform.X;
            p.Position.Y = this.Transform.Y;

            p.StartTime = DateTime.Now.Ticks;

            if (this.settings.EmitterSize != Vector2.Zero)
            {
                switch (this.settings.EmitterShape)
                {
                    case ParticleSystem2D.Shape.Circle:
                        {
                            float radius = this.settings.EmitterSize.X > this.settings.EmitterSize.Y ? (this.settings.EmitterSize.X / 2) : (this.settings.EmitterSize.Y / 2);
                            double angle = this.random.NextDouble() * MathHelper.TwoPi;
                            float x = (float)Math.Cos(angle);
                            float y = (float)Math.Sin(angle);

                            p.Position.X = p.Position.X + (x * radius);
                            p.Position.Y = p.Position.Y + (y * radius);

                            break;
                        }

                    case ParticleSystem2D.Shape.FillCircle:
                        {
                            float rnd0 = ((float)this.random.NextDouble() * 2) - 1;
                            float rnd1 = ((float)this.random.NextDouble() * 2) - 1;
                            float radius = this.settings.EmitterSize.X > this.settings.EmitterSize.Y ? (this.settings.EmitterSize.X / 2) : (this.settings.EmitterSize.Y / 2);
                            double angle = this.random.NextDouble() * MathHelper.TwoPi;
                            float x = (float)Math.Cos(angle);
                            float y = (float)Math.Sin(angle);

                            p.Position.X = p.Position.X + (x * radius * rnd0);
                            p.Position.Y = p.Position.Y + (y * radius * rnd1);

                            break;
                        }

                    case ParticleSystem2D.Shape.Rectangle:
                        {
                            int c = this.random.Next(4);
                            float rnd0 = ((float)this.random.NextDouble() * 2) - 1;
                            float xside = this.settings.EmitterSize.X / 2;
                            float yside = this.settings.EmitterSize.Y / 2;

                            switch (c)
                            {
                                case 0:
                                    p.Position.X = p.Position.X + xside;
                                    p.Position.Y = p.Position.Y + (yside * rnd0);
                                    break;
                                case 1:
                                    p.Position.X = p.Position.X - xside;
                                    p.Position.Y = p.Position.Y + (yside * rnd0);
                                    break;
                                case 2:
                                    p.Position.X = p.Position.X + (xside * rnd0);
                                    p.Position.Y = p.Position.Y + yside;
                                    break;
                                case 3:
                                default:
                                    p.Position.X = p.Position.X + (xside * rnd0);
                                    p.Position.Y = p.Position.Y - yside;
                                    break;
                            }

                            break;
                        }

                    case ParticleSystem2D.Shape.FillRectangle:
                        {
                            float rnd0 = ((float)this.random.NextDouble() * 2) - 1;
                            float rnd1 = ((float)this.random.NextDouble() * 2) - 1;

                            p.Position.X = p.Position.X + ((this.settings.EmitterSize.X / 2) * rnd0);
                            p.Position.Y = p.Position.Y + ((this.settings.EmitterSize.Y / 2) * rnd1);
                            break;
                        }

                    default:
                        {
                            throw new ArgumentException("Invalid particleSystem shape");
                        }
                }
            }

            // Initial Angle
            if (this.settings.InitialAngle.Distinct(0) || this.settings.InitialAngleVariation.Distinct(0))
            {
                float randomAngle = this.settings.InitialAngleVariation * (((float)this.random.NextDouble() * 2) - 1);
                p.Angle = this.settings.InitialAngle + randomAngle;
            }

            // Velocity Rotation
            if (this.settings.MinRotateSpeed.Distinct(this.settings.MaxRotateSpeed))
            {
                p.VelocityRotation = MathHelper.Lerp(this.settings.MinRotateSpeed, this.settings.MaxRotateSpeed, (float)this.random.NextDouble());
            }
            else
            {
                p.VelocityRotation = this.settings.MinRotateSpeed;
            }

            // Size
            if (this.settings.MinSize.Distinct(this.settings.MaxSize))
            {
                p.Size = MathHelper.Lerp(this.settings.MinSize, this.settings.MaxSize, (float)this.random.NextDouble());
            }
            else
            {
                p.Size = this.settings.MinSize;
            }

            // Color
            if (this.settings.MinColor != this.settings.MaxColor)
            {
                p.CurrentColor = Color.Lerp(ref this.settings.MinColor, ref this.settings.MaxColor, 1 - (float)this.random.NextDouble());
            }
            else
            {
                p.CurrentColor = this.settings.MinColor;
            }

            p.Color = p.CurrentColor;

            Matrix.CreateFromYawPitchRoll(0, 0, this.Transform.Rotation, out this.rotationMatrix);
        }

        /// <summary>
        /// Calculates the local world.
        /// </summary>
        /// <param name="p">The p.</param>
        /// <returns>World matrix.</returns>
        /// <exception cref="ObjectDisposedException">ParticleSystemRenderer has been disposed.</exception>
        private Matrix CalculateLocalWorld(ref Particle p)
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException("ParticleSystemRenderer");
            }

            ////Matrix matrix = Matrix.CreateRotationZ(p.Angle)
            ////                * Matrix.CreateScale(p.Size)
            ////                * Matrix.CreateTranslation(p.Position);

            float cos = (float)Math.Cos(p.Angle);
            float sin = (float)Math.Sin(p.Angle);

            float scale;

            if (this.settings.EndDeltaScale.Equal(1))
            {
                scale = p.Size;
            }
            else
            {
                float age = 1 - (float)(p.Life / p.TimeLife);
                scale = p.Size * (1 + ((this.settings.EndDeltaScale - 1) * age));
            }

            float scaleCos = scale * cos;
            float scaleSin = scale * sin;

            Matrix matrix;

            if (!this.viewportEnabled)
            {
                matrix.M11 = scaleCos;
                matrix.M12 = scaleSin;
                matrix.M13 = 0;
                matrix.M14 = 0;
                matrix.M21 = -scaleSin;
                matrix.M22 = scaleCos;
                matrix.M23 = 0;
                matrix.M24 = 0;
                matrix.M31 = 0f;
                matrix.M32 = 0f;
                matrix.M33 = 1f;
                matrix.M34 = 0f;
                matrix.M41 = p.Position.X;
                matrix.M42 = p.Position.Y;
                matrix.M43 = 0;
                matrix.M44 = 1f;
            }
            else
            {
                matrix.M11 = scaleCos * this.viewportScale.X;
                matrix.M12 = scaleSin * this.viewportScale.Y;
                matrix.M13 = 0;
                matrix.M14 = 0;
                matrix.M21 = -scaleSin * this.viewportScale.X;
                matrix.M22 = scaleCos * this.viewportScale.Y;
                matrix.M23 = 0;
                matrix.M24 = 0;
                matrix.M31 = 0f;
                matrix.M32 = 0f;
                matrix.M33 = 1f;
                matrix.M34 = 0f;
                matrix.M41 = (p.Position.X * this.viewportScale.X) + this.viewportTranslate.X;
                matrix.M42 = (p.Position.Y * this.viewportScale.Y) + this.viewportTranslate.Y;
                matrix.M43 = 0;
                matrix.M44 = 1f;
            }

            return matrix;
        }

        /// <summary>
        /// Draws the basic unit.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        protected override void DrawBasicUnit(int parameter)
        {
            if (this.internalEnabled)
            {
                this.Material.Material.Apply(this.RenderManager);

                this.vertexBuffer.SetData(this.vertices, this.numVertices);
                this.GraphicsDevice.BindVertexBuffer(this.vertexBuffer);
                this.GraphicsDevice.DrawVertexBuffer(
                                          this.numVertices,
                                          this.numPrimitives,
                                          PrimitiveType.TriangleList,
                                          this.vertexBuffer,
                                          this.indexBuffer);
            }
        }
        #endregion
    }
}
