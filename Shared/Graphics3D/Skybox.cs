// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Using Statements
using System;
using System.Collections.Generic;
using WaveEngine.Common.Graphics;
using WaveEngine.Common;
using WaveEngine.Common.Math;
using WaveEngine.Framework.Graphics;
using WaveEngine.Materials;
using WaveEngine.Common.Graphics.VertexFormats;
using System.Runtime.Serialization;
using WaveEngine.Common.Attributes;
using WaveEngine.Common.Attributes.Converters;

#endregion

namespace WaveEngine.Framework.Graphics
{
    /// <summary>
    /// This class represent a Skybox.
    /// </summary>
    [DataContract(Namespace = "WaveEngine.Framework.Graphics")]
    public class Skybox : Drawable3D
    {
        /// <summary>
        /// The instances
        /// </summary>
        private static int instances;

        /// <summary>
        /// The camera3D
        /// </summary>
        [RequiredComponent(false)]
        [DontRenderProperty]
        public Camera3D Camera3D;

        /// <summary>
        /// The material
        /// </summary>
        private SkyboxMaterial material;

        /// <summary>
        /// The cubemap texture
        /// </summary>
        private string cubemapPath;

        /// <summary>
        /// The cube mesh
        /// </summary>
        private Mesh cubeMesh;

        /// <summary>
        /// The disposed
        /// </summary>
        private bool disposed;

        /// <summary>
        /// Rotation in euler angles
        /// </summary>
        private Vector3 rotation;

        /// <summary>
        /// Rotation expressed in quaternion
        /// </summary>
        private Quaternion orientation;

        /// <summary>
        /// Cached rotation transform
        /// </summary>
        private Matrix cachedRotation;

        #region Properties

        /// <summary>
        /// Gets or sets the SkyBox cubemap texture
        /// </summary>
        [DataMember]
        [RenderPropertyAsAsset(AssetType.Cubemap)]
        public string CubemapPath
        {
            get
            {
                return this.cubemapPath;
            }

            set
            {
                this.cubemapPath = value;
                if (this.isInitialized)
                {
                    this.RefreshCubemapMaterial();
                }
            }
        }

        /// <summary>
        /// Gets or sets the SkyBox cube rotation expressed in Euler angles (yaw, pitch, roll)
        /// </summary>
        [RenderProperty(typeof(Vector3RadianToDegreeConverter))]

        public Vector3 Rotation
        {
            get
            {
                return this.rotation;
            }

            set
            {
                this.rotation = value;
                Quaternion.CreateFromYawPitchRoll(value.X, value.Y, value.Z, out this.orientation);
                Matrix.CreateFromQuaternion(ref this.orientation, out this.cachedRotation);
            }
        }

        /// <summary>
        /// Gets or sets the SkyBox cube orientation expressed in quaternion.
        /// </summary>
        [DontRenderProperty]
        [DataMember]
        public Quaternion Orientation
        {
            get
            {
                return this.orientation;
            }

            set
            {
                this.orientation = value;
                Vector3.FromQuaternion(ref this.orientation, out this.rotation);
                Matrix.CreateFromQuaternion(ref this.orientation, out this.cachedRotation);
            }
        }
        #endregion

        #region Initialize

        /// <summary>
        /// Initializes a new instance of the <see cref="Skybox" /> class.
        /// </summary>
        public Skybox()
            : base("Skybox" + instances)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Skybox" /> class.
        /// </summary>
        /// <param name="cubemapTexture">The cubemap texture.</param>
        public Skybox(string cubemapTexture)
            : base("Skybox" + instances)
        {
            this.cubemapPath = cubemapTexture;
        }

        /// <summary>
        /// The default values.
        /// </summary>
        protected override void DefaultValues()
        {
            base.DefaultValues();
            instances++;
            this.Rotation = Vector3.Zero;
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// Performs further custom initialization for this instance.
        /// </summary>
        /// <exception cref="System.ObjectDisposedException">Skybox disposed.</exception>
        protected override void Initialize()
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException("Skybox");
            }

            base.Initialize();

            Vector3[] normals =
            {
                new Vector3(0, 0, 1),
                new Vector3(0, 0, -1),
                new Vector3(1, 0, 0),
                new Vector3(-1, 0, 0),
                new Vector3(0, 1, 0),
                new Vector3(0, -1, 0)
            };

            Vector2[] texCoord =
            {
                new Vector2(1, 1), new Vector2(0, 1), new Vector2(0, 0), new Vector2(1, 0),
                new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1),
                new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1), new Vector2(0, 0),
                new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1), new Vector2(0, 0),
                new Vector2(0, 1), new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1),
                new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1), new Vector2(0, 0),
            };

            float sizeOverTwo = 0.5f;

            ushort[] indices = new ushort[36];
            VertexPositionTexture[] vertices = new VertexPositionTexture[24];

            ushort currentIndice = 0;
            ushort currentVertex = 0;

            // Create each face in turn.
            for (int i = 0, j = 0; i < normals.Length; i++, j += 4)
            {
                Vector3 normal = normals[i];

                // Get two vectors perpendicular to the face normal and to each other.
                Vector3 side1 = new Vector3(normal.Y, normal.Z, normal.X);
                Vector3 side2 = Vector3.Cross(normal, side1);

                // Six indices (two triangles) per face.
                indices[currentIndice++] = (ushort)(currentVertex + 0);
                indices[currentIndice++] = (ushort)(currentVertex + 1);
                indices[currentIndice++] = (ushort)(currentVertex + 3);

                indices[currentIndice++] = (ushort)(currentVertex + 1);
                indices[currentIndice++] = (ushort)(currentVertex + 2);
                indices[currentIndice++] = (ushort)(currentVertex + 3);

                // 1   2
                // 0   3

                // Four vertices per face.
                vertices[currentVertex].Position = (normal - side1 - side2) * sizeOverTwo;
                vertices[currentVertex++].TexCoord = texCoord[j + 0];
                vertices[currentVertex].Position = (normal - side1 + side2) * sizeOverTwo;
                vertices[currentVertex++].TexCoord = texCoord[j + 1];
                vertices[currentVertex].Position = (normal + side1 + side2) * sizeOverTwo;
                vertices[currentVertex++].TexCoord = texCoord[j + 2];
                vertices[currentVertex].Position = (normal + side1 - side2) * sizeOverTwo;
                vertices[currentVertex++].TexCoord = texCoord[j + 3];
            }

            VertexBuffer vertexBuffer = new VertexBuffer(VertexPositionTexture.VertexFormat);
            vertexBuffer.SetData(vertices, 24);

            IndexBuffer indexBuffer = new IndexBuffer(indices);

            // create the quad
            this.cubeMesh = new Mesh(0, 24, 0, 12, vertexBuffer, indexBuffer, PrimitiveType.TriangleList)
            {
                DisableBatch = true
            };

            this.RefreshCubemapMaterial();
        }

        /// <summary>
        /// Allows to perform custom drawing.
        /// </summary>
        /// <param name="gameTime">The elapsed game time.</param>
        /// <remarks>
        /// This method will only be called if all the following points are true:
        /// <list type="bullet"><item><description>The entity passes the culling test.</description></item><item><description>The parent of the owner <see cref="Entity" /> of the <see cref="Drawable" /> cascades its visibility to its children and it is visible.</description></item><item><description>The <see cref="Drawable" /> is active.</description></item><item><description>The owner <see cref="Entity" /> of the <see cref="Drawable" /> is active and visible.</description></item></list>
        /// </remarks>
        public override void Draw(TimeSpan gameTime)
        {
            if (this.material == null)
            {
                return;
            }

            if (this.Camera3D == this.RenderManager.CurrentDrawingCamera3D)
            {
                var camera = this.Camera3D;
                Matrix scale = Matrix.CreateScale((camera.NearPlane + camera.FarPlane) * 0.5f);
                Matrix position = Matrix.CreateTranslation(camera.Position);
                Matrix worldTransform = Matrix.Multiply(Matrix.Multiply(scale, this.cachedRotation), position);

                this.RenderManager.DrawMesh(this.cubeMesh, this.material, ref worldTransform);
            }
        }
        #endregion

        #region Private Methods

        /// <summary>
        /// Refresh cubemap material
        /// </summary>
        private void RefreshCubemapMaterial()
        {
            if (this.material != null)
            {
                this.material.Unload(this.Assets);
                this.material = null;
            }

            if (!string.IsNullOrEmpty(this.cubemapPath))
            {
                this.material = new SkyboxMaterial(this.cubemapPath);
                this.material.Initialize(this.Assets);
            }
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    if (this.isInitialized)
                    {
                        this.GraphicsDevice.DestroyVertexBuffer(this.cubeMesh.VertexBuffer);
                        this.GraphicsDevice.DestroyIndexBuffer(this.cubeMesh.IndexBuffer);

                        if (!string.IsNullOrEmpty(this.cubemapPath))
                        {
                            this.Assets.UnloadAsset(this.cubemapPath);
                        }
                    }

                    this.disposed = true;
                }
            }
        }

        #endregion
    }
}
