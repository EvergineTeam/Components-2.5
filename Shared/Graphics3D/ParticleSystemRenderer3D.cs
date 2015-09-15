#region File Description
//-----------------------------------------------------------------------------
// ParticleSystemRenderer3D
//
// Copyright © 2015 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements

using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Graphics.VertexFormats;
using WaveEngine.Common.Math;
using WaveEngine.Components.Particles;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Services;
using WaveRandom = WaveEngine.Framework.Services.Random;

#if WINDOWS_PHONE
using WaveEngine.Common.Helpers;
#endif
#endregion

namespace WaveEngine.Components.Graphics3D
{
    /// <summary>
    /// Renders a particle system on the screen.
    /// </summary>
    [DataContract(Namespace = "WaveEngine.Components.Graphics3D")]
    public class ParticleSystemRenderer3D : Drawable3D
    {
        /// <summary>
        /// Velocity adjust of the particles.
        /// </summary>
        private const float VELOCITYFACTOR = 60f;

        /// <summary>
        /// The init time multipler
        /// </summary>
        private const float InitTimeMultipler = 10 / 1000.0f;

        /// <summary>
        /// The renderer local world
        /// </summary>
        private static Matrix localWorld = Matrix.Identity;

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
        public MaterialsMap MaterialsMap;

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
        private VertexPositionNormalColorTexture[] vertices;

        /// <summary>
        /// Particle mesh
        /// </summary>
        private Mesh mesh;

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
        private static Vector3 vertex1 = new Vector3 { X = 1, Y = -1, Z = 0 };

        /// <summary>
        /// The vertex2
        /// </summary>
        private static Vector3 vertex2 = new Vector3 { X = -1, Y = -1, Z = 0 };

        /// <summary>
        /// The normal
        /// </summary>
        private static Vector3 normal = new Vector3 { X = 0, Y = 0, Z = 1 };

        /// <summary>
        /// The vertex3
        /// </summary>
        private static Vector3 vertex3 = new Vector3 { X = -1, Y = 1, Z = 0 };

        /// <summary>
        /// The vertex4
        /// </summary>
        private static Vector3 vertex4 = new Vector3 { X = 1, Y = 1, Z = 0 };

        /// <summary>
        /// The texcoord1
        /// </summary>
        private static Vector2 texcoord1 = new Vector2 { X = 0, Y = 1 };

        /// <summary>
        /// The texcoord2
        /// </summary>
        private static Vector2 texcoord2 = new Vector2 { X = 1, Y = 1 };

        /// <summary>
        /// The texcoord3
        /// </summary>
        private static Vector2 texcoord3 = new Vector2 { X = 1, Y = 0 };

        /// <summary>
        /// The texcoord4
        /// </summary>
        private static Vector2 texcoord4 = new Vector2 { X = 0, Y = 0 };

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
            if (this.System.NumParticles == 0
                || (!this.internalEnabled && !this.settings.Emit))
            {
                return;
            }

            if (this.numParticles != this.System.NumParticles)
            {
                this.LoadParticleSystem();
            }

            if (!this.Owner.Scene.IsPaused)
            {
                // Sets the timeFactor for updating velocities.
                float timeFactor = VELOCITYFACTOR * (float)gameTime.TotalSeconds;

                this.aliveParticles = 0;

                // Sets the time passed between 2 particles creation.
                this.emitLapse = (this.settings.EmitRate > 0) ? 1 / this.settings.EmitRate : 0;

                bool infiniteCreation = this.settings.EmitRate == 0;
                int particlesToCreate = 0;
                if (!infiniteCreation)
                {
                    float particlesToCreateF = ((float)gameTime.TotalSeconds / this.emitLapse) + this.emitRemainder;
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
                        p.Life = p.Life - gameTime.TotalSeconds;
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
                    else
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
                            if (this.settings.CollisionType.HasFlag(ParticleSystem3D.ParticleCollisionFlags.MaxY) && (p.Position.Y > this.settings.CollisionMax.Y))
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
                                        p.Position.Y = this.settings.CollisionMax.Y - (bounciness * (p.Position.Y - this.settings.CollisionMax.Y));

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
                            else if (this.settings.CollisionType.HasFlag(ParticleSystem3D.ParticleCollisionFlags.MinY) && (p.Position.Y < this.settings.CollisionMin.Y))
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
                                        p.Position.Y = this.settings.CollisionMin.Y + (bounciness * (this.settings.CollisionMin.Y - p.Position.Y));

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
                            if (this.settings.CollisionType.HasFlag(ParticleSystem3D.ParticleCollisionFlags.MaxX) && (p.Position.X > this.settings.CollisionMax.X))
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
                                        p.Position.X = this.settings.CollisionMax.X - (bounciness * (p.Position.X - this.settings.CollisionMax.X));

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
                            else if (this.settings.CollisionType.HasFlag(ParticleSystem3D.ParticleCollisionFlags.MinX) && (p.Position.X < this.settings.CollisionMin.X))
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
                                        p.Position.X = this.settings.CollisionMin.X + (bounciness * (this.settings.CollisionMin.X - p.Position.X));

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
                            if (this.settings.CollisionType.HasFlag(ParticleSystem3D.ParticleCollisionFlags.MaxZ) && (p.Position.Z > this.settings.CollisionMax.Z))
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
                                        p.Position.Z = this.settings.CollisionMax.Z - (bounciness * (p.Position.Z - this.settings.CollisionMax.Z));

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
                            else if (this.settings.CollisionType.HasFlag(ParticleSystem3D.ParticleCollisionFlags.MinZ) && (p.Position.Z < this.settings.CollisionMin.Z))
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
                                        p.Position.Z = this.settings.CollisionMin.Z + (bounciness * (this.settings.CollisionMin.Z - p.Position.Z));

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
                        if (this.settings.LinearColorEnabled && p.TimeLife != 0 && this.settings.InterpolationColors != null && this.settings.InterpolationColors.Count > 1)
                        {
                            // Num destiny colors
                            int count = this.settings.InterpolationColors.Count;

                            // Current lerp
                            double lerp = 1 - (p.Life / p.TimeLife);

                            // Life time of one color
                            double colorPeriod = 1 / (count - 1.0);

                            // destiny Color
                            int sourceIndex = (int)(lerp / colorPeriod);

                            sourceIndex = Math.Max(0, Math.Min(count - 2, sourceIndex));

                            double currentLerp = (lerp - (sourceIndex * colorPeriod)) / colorPeriod;

                            Color sourceColor = this.settings.InterpolationColors[sourceIndex];
                            Color destinyColor = this.settings.InterpolationColors[sourceIndex + 1];

                            p.Color = Color.Lerp(ref sourceColor, ref destinyColor, (float)currentLerp);
                        }
                        else
                        {
                            p.Color = p.CurrentColor;
                        }

                        if (this.settings.AlphaEnabled && p.TimeLife.Distinct(0))
                        {
                            double age = p.Life / p.TimeLife;
                            p.Color *= (float)age;
                        }
                    }

                    // Update Vertex Buffer
                    Matrix world = this.CalculateLocalWorld(ref p);

                    Vector3 transformedNormal;

                    Vector3.Transform(ref normal, ref world, out transformedNormal);

                    Vector3.Transform(ref vertex1, ref world, out this.vertices[j].Position);
                    this.vertices[j].Color = p.Color;
                    this.vertices[j].Normal = transformedNormal;
                    this.vertices[j++].TexCoord = texcoord1;

                    Vector3.Transform(ref vertex2, ref world, out this.vertices[j].Position);
                    this.vertices[j].Color = p.Color;
                    this.vertices[j].Normal = transformedNormal;
                    this.vertices[j++].TexCoord = texcoord2;

                    Vector3.Transform(ref vertex3, ref world, out this.vertices[j].Position);
                    this.vertices[j].Color = p.Color;
                    this.vertices[j].Normal = transformedNormal;
                    this.vertices[j++].TexCoord = texcoord3;

                    Vector3.Transform(ref vertex4, ref world, out this.vertices[j].Position);
                    this.vertices[j].Color = p.Color;
                    this.vertices[j].Normal = transformedNormal;
                    this.vertices[j++].TexCoord = texcoord4;

                    ////Vector3 pos1 = this.vertices[j - 4].Position;
                    ////Vector3 pos2 = this.vertices[j - 3].Position;
                    ////Vector3 pos3 = this.vertices[j - 2].Position;
                    ////Vector3 pos4 = this.vertices[j - 1].Position;

                    ////this.RenderManager.LineBatch3D.DrawLine(pos1, pos2, p.Color);
                    ////this.RenderManager.LineBatch3D.DrawLine(pos2, pos3, p.Color);
                    ////this.RenderManager.LineBatch3D.DrawLine(pos3, pos4, p.Color);
                    ////this.RenderManager.LineBatch3D.DrawLine(pos4, pos1, p.Color);

                    // Si la partícula está viva la contamos
                    if (p.Alive)
                    {
                        this.aliveParticles++;
                    }
                }

                this.internalEnabled = this.aliveParticles > 0;

                if (this.internalEnabled)
                {
                    float zOrder = Vector3.DistanceSquared(this.RenderManager.CurrentDrawingCamera3D.Position, this.Transform.Position);

                    this.mesh.ZOrder = zOrder;
                    this.mesh.VertexBuffer.SetData(this.vertices, this.numVertices);
                    this.GraphicsDevice.BindVertexBuffer(this.mesh.VertexBuffer);

                    this.RenderManager.DrawMesh(this.mesh, this.MaterialsMap.DefaultMaterial, ref localWorld, false);
                }
            }
            else
            {
                // if the scene is paused, draw the previous mesh
                this.RenderManager.DrawMesh(this.mesh, this.MaterialsMap.DefaultMaterial, ref localWorld, false);
            }
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
                    this.GraphicsDevice.DestroyIndexBuffer(this.mesh.IndexBuffer);
                    this.GraphicsDevice.DestroyVertexBuffer(this.mesh.VertexBuffer);
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

            if (this.mesh != null)
            {
                if (this.mesh.IndexBuffer != null)
                {
                    this.GraphicsDevice.DestroyIndexBuffer(this.mesh.IndexBuffer);
                }

                if (this.mesh.VertexBuffer != null)
                {
                    this.GraphicsDevice.DestroyVertexBuffer(this.mesh.VertexBuffer);
                }

                this.mesh = null;
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

            IndexBuffer indexBuffer = new IndexBuffer(indices);

            // Initialize Particles
            for (int i = 0; i < this.numParticles; i++)
            {
                double life = (this.settings.EmitRate > 0) ? -1 : TimeSpan.FromSeconds(this.random.NextDouble() * (this.numParticles * InitTimeMultipler)).TotalSeconds;

                this.particles[i] = new Particle()
                {
                    Alive = true,
                    Life = life
                };
            }

            this.vertices = new VertexPositionNormalColorTexture[this.numVertices];
            DynamicVertexBuffer vertexBuffer = new DynamicVertexBuffer(VertexPositionNormalColorTexture.VertexFormat);
            vertexBuffer.SetData(this.vertices, this.numVertices);

            this.mesh = new Mesh(0, vertexBuffer.VertexCount, 0, indexBuffer.IndexCount / 3, vertexBuffer, indexBuffer, PrimitiveType.TriangleList)
            {
                DisableBatch = true
            };
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

            p.Alive = true;

            // Life
            if (this.settings.MinLife != this.settings.MaxLife)
            {
                float pLife = MathHelper.Lerp(
                                                                        this.settings.MinLife,
                                                                        this.settings.MaxLife,
                                                                        (float)this.random.NextDouble());
                p.TimeLife = pLife;
            }
            else
            {
                p.TimeLife = this.settings.MinLife;
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

            p.Position = this.Transform.Position;

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
                p.CurrentColor = Color.Lerp(this.settings.MinColor, this.settings.MaxColor, 1 - (float)this.random.NextDouble());
            }
            else
            {
                p.CurrentColor = this.settings.MinColor;
            }

            p.Color = p.CurrentColor;

            Matrix.CreateFromYawPitchRoll(this.Transform.Rotation.Y, this.Transform.Rotation.X, this.Transform.Rotation.Z, out this.rotationMatrix);
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

            Vector3 cameraPosition = this.RenderManager.CurrentDrawingCamera3D.Position;
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

        #endregion
    }
}
