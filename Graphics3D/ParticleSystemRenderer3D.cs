#region File Description
//-----------------------------------------------------------------------------
// ParticleSystemRenderer3D
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

using WaveRandom = WaveEngine.Framework.Services.Random;

#if WINDOWS_PHONE7
using WaveEngine.Common.Helpers;
#endif
#endregion

namespace WaveEngine.Components.Graphics3D
{
    /// <summary>
    /// Renders a particle system on the screen.
    /// </summary>
    public class ParticleSystemRenderer3D : Drawable3D
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
        /// Particle system rendered.
        /// </summary>
        [RequiredComponent]
        public ParticleSystem3D System;

        /// <summary>
        /// Materials used rendering the particle system.
        /// </summary>
        [RequiredComponent]
        public MaterialsMap MaterialMap;

        /// <summary>
        /// Transform of the particle system emitter.
        /// </summary>
        [RequiredComponent]
        public Transform3D Transform;

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
        private DynamicVertexBuffer<VertexPositionColorTexture> vertexBuffer;

        /// <summary>
        /// The settings
        /// </summary>
        private ParticleSystem3D settings;

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
        /// The texcoord1
        /// </summary>
        private readonly Vector2 texcoord1 = new Vector2 { X = 0, Y = 0 };

        /// <summary>
        /// The texcoord2
        /// </summary>
        private readonly Vector2 texcoord2 = new Vector2 { X = 1, Y = 0 };

        /// <summary>
        /// The texcoord3
        /// </summary>
        private readonly Vector2 texcoord3 = new Vector2 { X = 1, Y = 1 };

        /// <summary>
        /// The texcoord4
        /// </summary>
        private readonly Vector2 texcoord4 = new Vector2 { X = 0, Y = 1 };

        /// <summary>
        /// The rotation matrix
        /// </summary>
        private Matrix rotationMatrix;

        /// <summary>
        /// The rotation vector
        /// </summary>
        private Vector3 rotationVector;

        #region Initialize

        /// <summary>
        /// Initializes a new instance of the <see cref="ParticleSystemRenderer3D" /> class.
        /// </summary>
        public ParticleSystemRenderer3D()
            : this("ParticleSystemRenderer" + instances)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ParticleSystemRenderer3D"/> class.
        /// </summary>
        /// <param name="name">Name of this instance.</param>
        public ParticleSystemRenderer3D(string name)
            : base(name)
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

            // Sets the timeFactor for updating velocities.
            float timeFactor = VELOCITYFACTOR * (float)gameTime.TotalMilliseconds;

            this.aliveParticles = 0;

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
                    if (this.settings.Emit)
                    {
                        this.ResetParticle(ref p);
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
                    p.Velocity.Z = p.Velocity.Z + (timeFactor * this.settings.Gravity.Z);

                    // Calculate the rotation with the transform
                    Vector3.TransformNormal(ref p.Velocity, ref this.rotationMatrix, out this.rotationVector);

                    p.Position.X = p.Position.X + (timeFactor * this.rotationVector.X);
                    p.Position.Y = p.Position.Y + (timeFactor * this.rotationVector.Y);
                    p.Position.Z = p.Position.Z + (timeFactor * this.rotationVector.Z);

                    if (this.settings.CollisionType != ParticleSystem3D.ParticleCollisionFlags.None)
                    {
                        // Checks vertical collisions
                        if (this.settings.CollisionType.HasFlag(ParticleSystem3D.ParticleCollisionFlags.MaxY) && (p.Position.Y > this.settings.CollisionMaxY))
                        {
                            switch (this.settings.CollisionBehavior)
                            {
                                case ParticleSystem3D.ParticleCollisionBehavior.Die:
                                    // Kills the particle
                                    p.Alive = false;
                                    p.Size = 0;
                                    p.Life = -1;
                                    break;
                                case ParticleSystem3D.ParticleCollisionBehavior.Bounce:
                                    // Mirrors the position and velocity with the bounciness factor.
                                    float bounciness = this.settings.Bounciness;
                                    p.Position.Y = this.settings.CollisionMaxY - (bounciness * (p.Position.Y - this.settings.CollisionMaxY));

                                    // Applies the collision spread to the velocity
                                    if (this.settings.CollisionSpread != Vector3.Zero)
                                    {
                                        p.Velocity.X = bounciness * (p.Velocity.X + (this.settings.CollisionSpread.X * (((float)this.random.NextDouble() * 2) - 1)));
                                        p.Velocity.Y = -bounciness * (p.Velocity.Y + (this.settings.CollisionSpread.Y * (((float)this.random.NextDouble() * 2) - 1)));
                                        p.Velocity.Z = bounciness * (p.Velocity.Z + (this.settings.CollisionSpread.Z * (((float)this.random.NextDouble() * 2) - 1)));
                                    }
                                    else
                                    {
                                        p.Velocity.X = bounciness * p.Velocity.X;
                                        p.Velocity.Y = -bounciness * p.Velocity.Y;
                                        p.Velocity.Z = bounciness * p.Velocity.Z;
                                    }

                                    break;
                                default:
                                    // Do nothing
                                    break;
                            }
                        }
                        else if (this.settings.CollisionType.HasFlag(ParticleSystem3D.ParticleCollisionFlags.MinY) && (p.Position.Y < this.settings.CollisionMinY))
                        {
                            switch (this.settings.CollisionBehavior)
                            {
                                case ParticleSystem3D.ParticleCollisionBehavior.Die:
                                    // Kills the particle
                                    p.Alive = false;
                                    p.Size = 0;
                                    p.Life = -1;
                                    break;
                                case ParticleSystem3D.ParticleCollisionBehavior.Bounce:
                                    // Mirrors the position and velocity with the bounciness factor.
                                    float bounciness = this.settings.Bounciness;
                                    p.Position.Y = this.settings.CollisionMinY + (bounciness * (this.settings.CollisionMinY - p.Position.Y));

                                    // Applies the collision spread to the velocity
                                    if (this.settings.CollisionSpread != Vector3.Zero)
                                    {
                                        p.Velocity.X = bounciness * (p.Velocity.X + (this.settings.CollisionSpread.X * (((float)this.random.NextDouble() * 2) - 1)));
                                        p.Velocity.Y = -bounciness * (p.Velocity.Y + (this.settings.CollisionSpread.Y * (((float)this.random.NextDouble() * 2) - 1)));
                                        p.Velocity.Z = bounciness * (p.Velocity.Z + (this.settings.CollisionSpread.Z * (((float)this.random.NextDouble() * 2) - 1)));
                                    }
                                    else
                                    {
                                        p.Velocity.X = bounciness * p.Velocity.X;
                                        p.Velocity.Y = -bounciness * p.Velocity.Y;
                                        p.Velocity.Z = -bounciness * p.Velocity.Z;
                                    }

                                    break;
                                default:
                                    // Do nothing
                                    break;
                            }
                        }

                        // Checks horizontal collisions
                        if (this.settings.CollisionType.HasFlag(ParticleSystem3D.ParticleCollisionFlags.MaxX) && (p.Position.X > this.settings.CollisionMaxX))
                        {
                            switch (this.settings.CollisionBehavior)
                            {
                                case ParticleSystem3D.ParticleCollisionBehavior.Die:
                                    // Kills the particle
                                    p.Alive = false;
                                    p.Size = 0;
                                    p.Life = -1;
                                    break;
                                case ParticleSystem3D.ParticleCollisionBehavior.Bounce:
                                    // Mirrors the position and velocity with the bounciness factor.
                                    float bounciness = this.settings.Bounciness;
                                    p.Position.X = this.settings.CollisionMaxX - (bounciness * (p.Position.X - this.settings.CollisionMaxX));

                                    // Applies the collision spread to the velocity
                                    if (this.settings.CollisionSpread != Vector3.Zero)
                                    {
                                        p.Velocity.X = -bounciness * (p.Velocity.X + (this.settings.CollisionSpread.X * (((float)this.random.NextDouble() * 2) - 1)));
                                        p.Velocity.Y = bounciness * (p.Velocity.Y + (this.settings.CollisionSpread.Y * (((float)this.random.NextDouble() * 2) - 1)));
                                        p.Velocity.Z = bounciness * (p.Velocity.Z + (this.settings.CollisionSpread.Z * (((float)this.random.NextDouble() * 2) - 1)));
                                    }
                                    else
                                    {
                                        p.Velocity.X = -bounciness * p.Velocity.X;
                                        p.Velocity.Y = bounciness * p.Velocity.Y;
                                        p.Velocity.Z = bounciness * p.Velocity.Z;
                                    }

                                    break;
                                default:
                                    // Do nothing
                                    break;
                            }
                        }
                        else if (this.settings.CollisionType.HasFlag(ParticleSystem3D.ParticleCollisionFlags.MinX) && (p.Position.X < this.settings.CollisionMinX))
                        {
                            switch (this.settings.CollisionBehavior)
                            {
                                case ParticleSystem3D.ParticleCollisionBehavior.Die:
                                    // Kills the particle
                                    p.Alive = false;
                                    p.Size = 0;
                                    p.Life = -1;
                                    break;
                                case ParticleSystem3D.ParticleCollisionBehavior.Bounce:
                                    // Mirrors the position and velocity with the bounciness factor.
                                    float bounciness = this.settings.Bounciness;
                                    p.Position.X = this.settings.CollisionMinX + (bounciness * (this.settings.CollisionMinX - p.Position.X));

                                    // Applies the collision spread to the velocity
                                    if (this.settings.CollisionSpread != Vector3.Zero)
                                    {
                                        p.Velocity.X = -bounciness * (p.Velocity.X + (this.settings.CollisionSpread.X * (((float)this.random.NextDouble() * 2) - 1)));
                                        p.Velocity.Y = bounciness * (p.Velocity.Y + (this.settings.CollisionSpread.Y * (((float)this.random.NextDouble() * 2) - 1)));
                                        p.Velocity.Z = bounciness * (p.Velocity.Z + (this.settings.CollisionSpread.Z * (((float)this.random.NextDouble() * 2) - 1)));
                                    }
                                    else
                                    {
                                        p.Velocity.X = -bounciness * p.Velocity.X;
                                        p.Velocity.Y = bounciness * p.Velocity.Y;
                                        p.Velocity.Z = bounciness * p.Velocity.Z;
                                    }

                                    break;
                                default:
                                    // Do nothing
                                    break;
                            }
                        }

                        // Checks depth collisions
                        if (this.settings.CollisionType.HasFlag(ParticleSystem3D.ParticleCollisionFlags.MaxZ) && (p.Position.Z > this.settings.CollisionMaxZ))
                        {
                            switch (this.settings.CollisionBehavior)
                            {
                                case ParticleSystem3D.ParticleCollisionBehavior.Die:
                                    // Kills the particle
                                    p.Alive = false;
                                    p.Size = 0;
                                    p.Life = -1;
                                    break;
                                case ParticleSystem3D.ParticleCollisionBehavior.Bounce:
                                    // Mirrors the position and velocity with the bounciness factor.
                                    float bounciness = this.settings.Bounciness;
                                    p.Position.Z = this.settings.CollisionMaxZ - (bounciness * (p.Position.Z - this.settings.CollisionMaxZ));

                                    // Applies the collision spread to the velocity
                                    if (this.settings.CollisionSpread != Vector3.Zero)
                                    {
                                        p.Velocity.X = bounciness * (p.Velocity.X + (this.settings.CollisionSpread.X * (((float)this.random.NextDouble() * 2) - 1)));
                                        p.Velocity.Y = bounciness * (p.Velocity.Y + (this.settings.CollisionSpread.Y * (((float)this.random.NextDouble() * 2) - 1)));
                                        p.Velocity.Z = -bounciness * (p.Velocity.Z + (this.settings.CollisionSpread.Z * (((float)this.random.NextDouble() * 2) - 1)));
                                    }
                                    else
                                    {
                                        p.Velocity.X = bounciness * p.Velocity.X;
                                        p.Velocity.Y = bounciness * p.Velocity.Y;
                                        p.Velocity.Z = -bounciness * p.Velocity.Z;
                                    }

                                    break;
                                default:
                                    // Do nothing
                                    break;
                            }
                        }
                        else if (this.settings.CollisionType.HasFlag(ParticleSystem3D.ParticleCollisionFlags.MinZ) && (p.Position.Z < this.settings.CollisionMinZ))
                        {
                            switch (this.settings.CollisionBehavior)
                            {
                                case ParticleSystem3D.ParticleCollisionBehavior.Die:
                                    // Kills the particle
                                    p.Alive = false;
                                    p.Size = 0;
                                    p.Life = -1;
                                    break;
                                case ParticleSystem3D.ParticleCollisionBehavior.Bounce:
                                    // Mirrors the position and velocity with the bounciness factor.
                                    float bounciness = this.settings.Bounciness;
                                    p.Position.Z = this.settings.CollisionMinZ + (bounciness * (this.settings.CollisionMinZ - p.Position.Z));

                                    // Applies the collision spread to the velocity
                                    if (this.settings.CollisionSpread != Vector3.Zero)
                                    {
                                        p.Velocity.X = bounciness * (p.Velocity.X + (this.settings.CollisionSpread.X * (((float)this.random.NextDouble() * 2) - 1)));
                                        p.Velocity.Y = bounciness * (p.Velocity.Y + (this.settings.CollisionSpread.Y * (((float)this.random.NextDouble() * 2) - 1)));
                                        p.Velocity.Z = -bounciness * (p.Velocity.Z + (this.settings.CollisionSpread.Z * (((float)this.random.NextDouble() * 2) - 1)));
                                    }
                                    else
                                    {
                                        p.Velocity.X = bounciness * p.Velocity.X;
                                        p.Velocity.Y = bounciness * p.Velocity.Y;
                                        p.Velocity.Z = -bounciness * p.Velocity.Z;
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
                        p.Color.A = (byte)(255 * Math.Pow(age, 0.333333f));
                    }
                }

                // Update Vertex Buffer
                Matrix world = this.CalculateLocalWorld(ref p);

                Vector3.Transform(ref this.vertex1, ref world, out this.vertices[j].Position);
                this.vertices[j].Color = p.Color;
                this.vertices[j++].TexCoord = this.texcoord1;

                Vector3.Transform(ref this.vertex2, ref world, out this.vertices[j].Position);
                this.vertices[j].Color = p.Color;
                this.vertices[j++].TexCoord = this.texcoord2;

                Vector3.Transform(ref this.vertex3, ref world, out this.vertices[j].Position);
                this.vertices[j].Color = p.Color;
                this.vertices[j++].TexCoord = this.texcoord3;

                Vector3.Transform(ref this.vertex4, ref world, out this.vertices[j].Position);
                this.vertices[j].Color = p.Color;
                this.vertices[j++].TexCoord = this.texcoord4;

                // Si la partícula está viva la contamos
                if (p.Alive)
                {
                    this.aliveParticles++;
                }
            }

            this.internalEnabled = this.aliveParticles > 0;

            Layer layer = this.RenderManager.FindLayer(this.MaterialMap.DefaultMaterial.LayerType);

            layer.AddDrawable(0, this);
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
                this.particles[i] = new Particle()
                {
                    Alive = true,
                    Life = TimeSpan.FromMilliseconds(this.random.NextDouble() * (this.numParticles * InitTimeMultipler)).TotalMilliseconds
                };
            }

            this.vertices = new VertexPositionColorTexture[this.numVertices];
            this.vertexBuffer = new DynamicVertexBuffer<VertexPositionColorTexture>(VertexPositionColorTexture.VertexFormat);
            this.vertexBuffer.SetData(this.numVertices, this.vertices);
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

            if (!p.Alive)
            {
                p.Alive = true;
                p.Life = TimeSpan.FromMilliseconds(this.random.NextDouble() * (this.numParticles * InitTimeMultipler)).TotalMilliseconds;
                p.Position.X = 0;
                p.Position.Y = 0;
                p.Position.Z = 0;
                p.Velocity.X = 0;
                p.Velocity.Y = 0;
                p.Velocity.Z = 0;
                p.VelocityRotation = 0;
                p.Angle = 0;
                p.Size = 0;
                p.TimeLife = 0;
            }
            else
            {
                // Life
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
                if (this.settings.RandomVelocity != Vector3.Zero)
                {
                    p.Velocity = this.settings.LocalVelocity;

                    p.Velocity.X = p.Velocity.X + (this.settings.RandomVelocity.X * (((float)this.random.NextDouble() * 2) - 1));
                    p.Velocity.Y = p.Velocity.Y + (this.settings.RandomVelocity.Y * (((float)this.random.NextDouble() * 2) - 1));
                    p.Velocity.Z = p.Velocity.Z + (this.settings.RandomVelocity.Z * (((float)this.random.NextDouble() * 2) - 1));
                }
                else
                {
                    p.Velocity = this.settings.LocalVelocity;
                }

                p.Position = this.Transform.LocalWorld.Translation;

                if (this.settings.EmitterSize != Vector2.Zero)
                {
                    switch (this.settings.EmitterShape)
                    {
                        case ParticleSystem3D.Shape.Circle:
                            {
                                float radius = this.settings.EmitterSize.X > this.settings.EmitterSize.Y ? (this.settings.EmitterSize.X / 2) : (this.settings.EmitterSize.Y / 2);
                                double angle = this.random.NextDouble() * MathHelper.TwoPi;
                                float x = (float)Math.Cos(angle);
                                float y = (float)Math.Sin(angle);

                                p.Position.X = p.Position.X + (x * radius);
                                p.Position.Z = p.Position.Z + (y * radius);

                                break;
                            }

                        case ParticleSystem3D.Shape.FillCircle:
                            {
                                float rnd0 = ((float)this.random.NextDouble() * 2) - 1;
                                float rnd1 = ((float)this.random.NextDouble() * 2) - 1;
                                float radius = this.settings.EmitterSize.X > this.settings.EmitterSize.Y ? (this.settings.EmitterSize.X / 2) : (this.settings.EmitterSize.Y / 2);
                                double angle = this.random.NextDouble() * MathHelper.TwoPi;
                                float x = (float)Math.Cos(angle);
                                float y = (float)Math.Sin(angle);

                                p.Position.X = p.Position.X + (x * radius * rnd0);
                                p.Position.Z = p.Position.Z + (y * radius * rnd1);

                                break;
                            }

                        case ParticleSystem3D.Shape.Rectangle:
                            {
                                int c = this.random.Next(4);
                                float rnd0 = ((float)this.random.NextDouble() * 2) - 1;
                                float xside = this.settings.EmitterSize.X / 2;
                                float yside = this.settings.EmitterSize.Y / 2;

                                switch (c)
                                {
                                    case 0:
                                        p.Position.X = p.Position.X + xside;
                                        p.Position.Z = p.Position.Z + (yside * rnd0);
                                        break;
                                    case 1:
                                        p.Position.X = p.Position.X - xside;
                                        p.Position.Z = p.Position.Z + (yside * rnd0);
                                        break;
                                    case 2:
                                        p.Position.X = p.Position.X + (xside * rnd0);
                                        p.Position.Z = p.Position.Z + yside;
                                        break;
                                    case 3:
                                    default:
                                        p.Position.X = p.Position.X + (xside * rnd0);
                                        p.Position.Z = p.Position.Z - yside;
                                        break;
                                }

                                break;
                            }

                        case ParticleSystem3D.Shape.FillRectangle:
                            {
                                float rnd0 = ((float)this.random.NextDouble() * 2) - 1;
                                float rnd1 = ((float)this.random.NextDouble() * 2) - 1;

                                p.Position.X = p.Position.X + ((this.settings.EmitterSize.X / 2) * rnd0);
                                p.Position.Z = p.Position.Z + ((this.settings.EmitterSize.Y / 2) * rnd1);
                                break;
                            }

                        default:
                            {
                                throw new ArgumentException("Invalid particleSystem shape");
                            }
                    }
                }

                // Initial Angle
                if (this.settings.InitialAngle.Distinct(0))
                {
                    float randomAngle = ((float)this.random.NextDouble() * 2) - 1;
                    p.Angle = this.settings.InitialAngle * randomAngle;
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

                Matrix.CreateFromYawPitchRoll(this.Transform.Rotation.X, this.Transform.Rotation.Y, this.Transform.Rotation.Z, out this.rotationMatrix);
            }
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

            // Optimización del código
            // Matrix matrix = Matrix.CreateRotationZ(p.Angle)
            //               * Matrix.CreateScale(p.Size)
            //                 *Matrix.CreateBillboard(p.Position, RenderManager.Camera.Position, Vector3.Up, null);
            Matrix matrix;

            float cos = (float)Math.Cos(p.Angle);
            float sin = (float)Math.Sin(p.Angle);

            float scale = 0;

            if (this.settings.EndDeltaScale.Equal(1))
            {
                scale = p.Size;
            }
            else
            {
                float age = 1 - (float)(p.Life / p.TimeLife);
                scale = p.Size * (1 + ((this.settings.EndDeltaScale - 1) * age));
            }

            float cosscale = cos * scale;
            float sinscale = sin * scale;
            float negsinscale = -sin * scale;

            Vector3 cameraPosition = this.RenderManager.Camera.Position;
            Vector3 vector;
            Vector3 vector2;
            Vector3 vector3;
            vector.X = p.Position.X - cameraPosition.X;
            vector.Y = p.Position.Y - cameraPosition.Y;
            vector.Z = p.Position.Z - cameraPosition.Z;
            float num = vector.LengthSquared();
            if (num < 0.0001f)
            {
                vector = Vector3.Forward;
            }
            else
            {
                Vector3.Multiply(ref vector, (float)(1f / ((float)Math.Sqrt((double)num))), out vector);
            }

            ////Vector3 cameraUpVector = Vector3.Up;
            // Vector3.Cross(ref cameraUpVector, ref vector, out vector3);
            vector3.X = vector.Z;
            vector3.Y = 0;
            vector3.Z = -vector.X;

            vector3.Normalize();

            // Vector3.Cross(ref vector, ref vector3, out vector2);
            vector2.X = (vector.Y * vector3.Z) - (vector.Z * vector3.Y);
            vector2.Y = (vector.Z * vector3.X) - (vector.X * vector3.Z);
            vector2.Z = (vector.X * vector3.Y) - (vector.Y * vector3.X);

            matrix.M11 = (cosscale * vector3.X) + (sinscale * vector2.X);
            matrix.M12 = (cosscale * vector3.Y) + (sinscale * vector2.Y);
            matrix.M13 = (cosscale * vector3.Z) + (sinscale * vector2.Z);
            matrix.M14 = 0f;
            matrix.M21 = (negsinscale * vector3.X) + (cosscale * vector2.X);
            matrix.M22 = (negsinscale * vector3.Y) + (cosscale * vector2.Y);
            matrix.M23 = (negsinscale * vector3.Z) + (cosscale * vector2.Z);
            matrix.M24 = 0f;
            matrix.M31 = scale * vector.X;
            matrix.M32 = scale * vector.Y;
            matrix.M33 = scale * vector.Z;
            matrix.M34 = 0f;
            matrix.M41 = p.Position.X;
            matrix.M42 = p.Position.Y;
            matrix.M43 = p.Position.Z;
            matrix.M44 = 1f;

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
                this.MaterialMap.DefaultMaterial.Matrices.World = Matrix.Identity;
                this.MaterialMap.DefaultMaterial.Apply(this.RenderManager);

                this.vertexBuffer.SetData(this.numVertices, this.vertices);
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
