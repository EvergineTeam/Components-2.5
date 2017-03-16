#region File Description
//-----------------------------------------------------------------------------
// FixedCamera2D
//
// Copyright © 2017 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Math;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Managers;
using WaveEngine.Framework.Services;
#endregion

namespace WaveEngine.Components.Cameras
{
    /// <summary>
    /// FixedCamera 2D decorate class
    /// </summary>
    [DataContract(Namespace = "WaveEngine.Components.Cameras")]
    public class FixedCamera2D : BaseDecorator
    {
        #region Properties

        /// <summary>
        /// Gets or sets the filed of view.
        /// </summary>
        /// <value>
        /// The filed of view.
        /// </value>
        public float FieldOfView
        {
            get
            {
                return this.entity.FindComponent<Camera2D>().FieldOfView;
            }

            set
            {
                this.entity.FindComponent<Camera2D>().FieldOfView = value;
            }
        }

        /// <summary>
        /// Gets or sets the vanishing point of the Camera 2D. It indicates the point of the screen where the perspective is focused. 
        /// Its values are included in [0, 1] where (0, 0) indicates the top left corner.
        /// Such values are percentages where 1 means the 100% of the rectangle's width/height.
        /// </summary>
        /// <remarks>The default value is [0.5f, 0.5f]</remarks>
        public Vector2 VanishingPoint
        {
            get
            {
                return this.entity.FindComponent<Camera2D>().VanishingPoint;
            }

            set
            {
                this.entity.FindComponent<Camera2D>().VanishingPoint = value;
            }
        }

        /// <summary>
        /// Gets or sets the far plane.
        /// </summary>
        /// <value>
        /// The far plane.
        /// </value>
        public float FarPlane
        {
            get
            {
                return this.entity.FindComponent<Camera2D>().FarPlane;
            }

            set
            {
                this.entity.FindComponent<Camera2D>().FarPlane = value;
            }
        }

        /// <summary>
        /// Gets or sets the near plane.
        /// </summary>
        /// <value>
        /// The near plane.
        /// </value>
        public float NearPlane
        {
            get
            {
                return this.entity.FindComponent<Camera2D>().NearPlane;
            }

            set
            {
                this.entity.FindComponent<Camera2D>().NearPlane = value;
            }
        }

        /// <summary>
        /// Gets or sets the camera order.
        /// </summary>
        /// <value>
        /// The near plane.
        /// </value>
        public float CameraOrder
        {
            get
            {
                return this.entity.FindComponent<Camera2D>().CameraOrder;
            }

            set
            {
                this.entity.FindComponent<Camera2D>().CameraOrder = value;
            }
        }

        /// <summary>
        /// Gets or sets the RenderTarget associated to the camera.
        /// </summary>
        /// <value>
        /// The render target.
        /// </value>
        public RenderTarget RenderTarget
        {
            get
            {
                return this.entity.FindComponent<Camera2D>().RenderTarget;
            }

            set
            {
                this.entity.FindComponent<Camera2D>().RenderTarget = value;
            }
        }

        /// <summary>
        /// Gets or sets Clear flags used for clean FrameBuffer, stencilbuffer and Zbuffer.
        /// </summary>
        /// <value>
        /// The clear flags.
        /// </value>
        /// <exception cref="System.ObjectDisposedException">RenderManager has been disposed.</exception>
        public ClearFlags ClearFlags
        {
            get
            {
                return this.entity.FindComponent<Camera2D>().ClearFlags;
            }

            set
            {
                this.entity.FindComponent<Camera2D>().ClearFlags = value;
            }
        }

        /// <summary>
        /// Gets or sets the color of the background.
        /// </summary>
        /// <value>
        /// The background color of the camera if it was setted, or the RenderManager default background color.
        /// </value>
        public Color BackgroundColor
        {
            get
            {
                return this.entity.FindComponent<Camera2D>().BackgroundColor;
            }

            set
            {
                this.entity.FindComponent<Camera2D>().BackgroundColor = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the camera is active.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the camera is active; otherwise, <c>false</c>.
        /// </value>
        public bool IsActive
        {
            get
            {
                return this.entity.FindComponent<Camera2D>().IsActive;
            }

            set
            {
                this.entity.FindComponent<Camera2D>().IsActive = value;
            }
        }

        /// <summary>
        /// Gets the layer mask.
        /// </summary>
        /// <value>
        /// The layer mask.
        /// </value>
        public IDictionary<Type, bool> LayerMask
        {
            get
            {
                return this.entity.FindComponent<Camera2D>().LayerMask;
            }
        }
        
        /// <summary>
        /// Gets or sets the custom virtual screen.
        /// </summary>
        public VirtualScreenManager CustomVirtualScreen
        {
            get
            {
                return this.entity.FindComponent<Camera2D>().CustomVirtualScreen;
            }

            set
            {
                this.entity.FindComponent<Camera2D>().CustomVirtualScreen = value;
            }
        }
        #endregion

        #region Initialize

        /// <summary>
        /// Initializes a new instance of the <see cref="FixedCamera2D" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public FixedCamera2D(string name)
        {
            this.InitEntity(name);
            this.entity.FindComponent<Camera2D>().CenterScreen();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FixedCamera2D" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="position">The camera position</param>
        public FixedCamera2D(string name, Vector2 position)
        {
            this.InitEntity(name);

            Vector3 position3D = position.ToVector3(-10);
            this.entity.FindComponent<Transform3D>(false).Position = position3D;
        }
        #endregion

        /// <summary>
        /// Initialize entity
        /// </summary>
        /// <param name="name">The name.</param>
        private void InitEntity(string name)
        {
            this.entity = new Entity(name)
            .AddComponent(new Transform2D())
            .AddComponent(new Camera2D());            
        }
    }
}
