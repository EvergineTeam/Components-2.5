#region File Description
//-----------------------------------------------------------------------------
// TouchGestures
//
// Copyright © 2014 Wave Corporation
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Input;
using WaveEngine.Common.Math;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Physics2D;
using WaveEngine.Framework.Services;
using WaveEngine.Framework.UI;
#endregion

namespace WaveEngine.Components.Gestures
{
    /// <summary>
    /// Enables an <see cref="Entity"/> to support touchs.
    /// It requires a <see cref="Collider2D"/> (usually, <see cref="RectangleCollider"/>) 
    /// and a <see cref="Transform2D"/>.
    /// Common events on touch scenarios are provided: pressed, released, etc.
    /// </summary>
    public class TouchGestures : Behavior, ITouchable
    {
        /// <summary>
        /// Required <see cref="Collider2D"/>.
        /// It provides a way to detect whether a touch hits the dessired area.
        /// </summary>
        [RequiredComponent(false)]
        public Collider2D Collider;

        /// <summary>
        /// Required <see cref="Transform2D"/>.
        /// It provides position information to generate touch events data.
        /// </summary>
        [RequiredComponent]
        public Transform2D Transform2D;

        /// <summary>
        /// The tap threshold
        /// </summary>
        private const short TapThreshold = 10;

        /// <summary>
        /// Number of instances of this component created.
        /// </summary>
        private static int instances;

        /// <summary>
        /// Valid gestures that will be recognized by this behavior.
        /// </summary>
        private SupportedGesture enabledGestures;

        /// <summary>
        /// Current toches list.
        /// </summary>
        private List<TouchLocation> currentTouches;

        /// <summary>
        /// Number of touches.
        /// </summary>
        private int numTouches;

        /// <summary>
        /// The current centroid.
        /// </summary>
        private Vector2 currentCentroid;

        /// <summary>
        /// The previous centroid
        /// </summary>
        private Vector2 previousCentroid;

        /// <summary>
        /// The current farthers1
        /// </summary>
        private Vector2 currentFarthers1;

        /// <summary>
        /// The current farthers2
        /// </summary>
        private Vector2 currentFarthers2;

        /// <summary>
        /// The previour farthers1
        /// </summary>
        private Vector2 previourFarthers1;

        /// <summary>
        /// The previous farthers2
        /// </summary>
        private Vector2 previousFarthers2;

        /// <summary>
        /// The gesture sample
        /// </summary>
        private GestureSample gestureSample;

        /// <summary>
        /// The translation
        /// </summary>
        private Vector2 translation;

        /// <summary>
        /// The state
        /// </summary>
        private GestureType state;

        /// <summary>
        /// The start tap position
        /// </summary>
        private Vector2 startTapPosition;

        /// <summary>
        /// The minimun scale
        /// </summary>
        private float minScale;

        /// <summary>
        /// The maximun scale
        /// </summary>
        private float maxScale;

        /// <summary>
        /// The touch order
        /// </summary>
        private int touchOrder;

        /// <summary>
        /// Touches must be projected using Camera2D
        /// </summary>
        private bool projectCamera;

        /// <summary>
        /// Occurs when there is a tap gesture.
        /// </summary>
        public event EventHandler<GestureEventArgs> TouchTap;

        /// <summary>
        /// Occurs when there is a pressed gesture.
        /// </summary>
        public event EventHandler<GestureEventArgs> TouchPressed;

        /// <summary>
        /// Occurs when there is a released gesture.
        /// </summary>
        public event EventHandler<GestureEventArgs> TouchReleased;

        /// <summary>
        /// Occurs when there is a moved gesture.
        /// </summary>
        public event EventHandler<GestureEventArgs> TouchMoved;

        /// <summary>
        /// Occurs when touch order is changed.
        /// </summary>
        public event EventHandler TouchOrdenChanged;

        #region Properties

        /// <summary>
        /// Gets or sets the touch order.
        /// Value within [0, Int32.MaxValue] where 0 means the farthest (i.e., the last to receive the touch gesture) 
        /// and bigger values come near increasing the chance to receive the input.
        /// NOTE: It is required to have set <see cref="ManualTouchOrder"/> to <c>true</c> in order the engine not to override
        /// this value. See <see cref="ManualTouchOrder"/> for a more detailed information.
        /// </summary>
        /// <value>
        /// The touch order.
        /// </value>
        public int TouchOrder
        {
            get
            {
                return this.touchOrder;
            }

            set
            {
                this.touchOrder = value;
                if (this.TouchOrdenChanged != null)
                {
                    this.TouchOrdenChanged(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Gets or sets the minimun scale.
        /// Value within [0, float.MaxValue] which is understood as the minimun scale applicable to required 
        /// <see cref="Transform2D"/> when <see cref="SupportedGesture.Scale"/> is enabled through 
        /// <see cref="TouchGestures.EnabledGestures"/>.
        /// </summary>
        /// <value>
        /// The minimun scale.
        /// </value>
        public float MinScale
        {
            get
            {
                return this.minScale;
            }

            set
            {
                this.minScale = value;
            }
        }

        /// <summary>
        /// Gets or sets the maximun scale.
        /// Value within [0, float.MaxValue] which is understood as the maximun scale applicable to required 
        /// <see cref="Transform2D"/> when <see cref="SupportedGesture.Scale"/> is enabled through 
        /// <see cref="TouchGestures.EnabledGestures"/>.
        /// </summary>
        /// <value>
        /// The maximun scale.
        /// </value>
        public float MaxScale
        {
            get
            {
                return this.maxScale;
            }

            set
            {
                this.maxScale = value;
            }
        }

        /// <summary>
        /// Gets or sets which gestures are enabled.
        /// See <see cref="SupportedGesture"/> for available options.
        /// Such values can be set through bit masks, enabling more than once at the same time, for example:
        /// EnabledGestures = SupportedGestures.Translation | SupportedGestures.Rotation
        /// </summary>
        /// <value>
        /// The enabled gestures.
        /// </value>
        public SupportedGesture EnabledGestures
        {
            get
            {
                return this.enabledGestures;
            }

            set
            {
                this.enabledGestures = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether using <see cref="TouchOrder"/>,
        /// or a different order gathered using both the <see pref="Transform2D.DrawOrder"/> and
        /// the <see cref="Layer"/> used. Such calcs are performed during the call to 
        /// <see cref="UpdateTouchOrder"/>.
        /// </summary>
        /// <value>
        ///   <c>true</c> if using <see cref="TouchOrder"/>; otherwise, <c>false</c>.
        /// </value>
        public bool ManualTouchOrder
        {
            get;
            set;
        }

        #endregion

        #region Initialize

        /// <summary>
        /// Initializes a new instance of the <see cref="TouchGestures"/> class.
        /// By default, scale is within [0.1, 5], the delta scale is set to 1 and there is no 
        /// supported gesture.
        /// </summary>
        /// <param name="projectCamera">Indicates if the touches will be processed using Cameras</param>
        public TouchGestures(bool projectCamera = true)
            : base("TouchGestures" + instances++)
        {
            this.currentTouches = new List<TouchLocation>();
            this.minScale = 0.1f;
            this.maxScale = 5f;
            this.gestureSample = new GestureSample { DeltaScale = 1 };
            this.enabledGestures = SupportedGesture.None;
            this.projectCamera = projectCamera;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Determines whether the required <see cref="Collider2D"/> contains the passed point.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <returns>
        ///   <c>true</c> if the point is contained; otherwise, <c>false</c>.
        /// </returns>
        public bool Contains(Vector2 point)
        {
            if (!Owner.IsActive || !Owner.IsVisible)
            {
                return false;
            }

            if (!this.projectCamera)
            {
                return this.Collider.Contain(point);
            }
            else
            {
                Ray ray;

                foreach (Camera2D camera in this.RenderManager.Camera2DList)
                {
                    camera.CalculateRay(ref point, out ray);

                    if (this.Collider.Intersects(ref ray))
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        /// <summary>
        /// Adds a <see cref="TouchLocation"/> to the current active touches.
        /// </summary>
        /// <param name="touch">The touch location.</param>
        /// <param name="isNew">Whether such touch must be considered as new.</param>
        public void AddTouch(TouchLocation touch, bool isNew)
        {
            touch.IsNew = isNew;

            if (!this.projectCamera)
            {
                this.currentTouches.Add(touch);
            }
            else
            {
                float drawOrder = this.Transform2D.DrawOrder;
                Ray ray;
                Vector3 pointWorld;
                Vector2 worldPoint2D;

                foreach (Camera2D camera in this.RenderManager.Camera2DList)
                {
                    camera.CalculateRay(ref touch.Position, out ray);
                    ray.IntersectionZPlane(drawOrder, out pointWorld);
                    pointWorld.ToVector2(out worldPoint2D);
                    touch.Position = worldPoint2D;
                    this.currentTouches.Add(touch);
                }
            }
        }

        /// <summary>
        /// Gets the current <see cref="GestureSample"/>.
        /// </summary>
        /// <returns>The current <see cref="GestureSample"/></returns>
        public GestureSample ReadGesture()
        {
            return this.gestureSample;
        }

        /// <summary>
        /// If and only if <see cref="ManualTouchOrder"/> is set to <c>false</c> (by default it is)
        /// the touch order is calculated based on both <see pref="Transform2D.DrawOrder"/> and
        /// the <see cref="Layer"/> used.
        /// </summary>
        public void UpdateTouchOrder()
        {
            if (!this.ManualTouchOrder && this.Owner != null)
            {
                int order = 0;
                int numberOfLevels = 100;

                Drawable2D drawable2D = this.Owner.FindComponent<Drawable2D>(false);
                if (drawable2D != null)
                {
                    Type layer = drawable2D.LayerType;
                    int index = RenderManager.GetLayerIndex(layer);

                    if (index != -1)
                    {
                        order = index * numberOfLevels;
                    }
                }

                if (this.Transform2D != null)
                {
                    order += (int)((1 - this.Transform2D.DrawOrder) * numberOfLevels);
                }

                this.TouchOrder = order;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Resolves the dependencies needed for this instance to work.
        /// </summary>
        protected override void ResolveDependencies()
        {
            base.ResolveDependencies();

            this.UpdateTouchOrder();

            this.Owner.FindComponent<Transform2D>().PropertyChanged += this.DrawOrderPropertyChanged;

            if (this.Owner.Scene.IsInitialized)
            {
                var touchManager = WaveServices.TouchPanel;
                touchManager.Subscribe(this);
            }
        }

        /// <summary>
        /// Draws the order property changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs" /> instance containing the event data.</param>
        private void DrawOrderPropertyChanged(object sender, ref DependencyPropertyChangedEventArgs e)
        {
            ////    if (e.Property == WaveEngine.Framework.Graphics.Transform2D.DrawOrderProperty)
            ////    {
            ////        this.UpdateTouchOrder();
            ////    }
        }

        /// <summary>
        /// Updates this behavior.
        /// </summary>
        /// <param name="gameTime">The elapsed game time.</param>
        protected override void Update(TimeSpan gameTime)
        {
            switch (this.state)
            {
                case GestureType.None:
                    this.NoneState();
                    break;

                case GestureType.Pressed:
                    this.PressedState();
                    break;

                case GestureType.Tap:
                    this.TapState();
                    break;

                case GestureType.Stopped:
                case GestureType.Drag:
                    this.StoppedAndDragState();
                    break;

                case GestureType.Free:
                    this.FreeState();
                    break;
            }

            this.UpdatePreviousValues();
        }

        /// <summary>
        /// Handle the NoneState
        /// </summary>
        private void NoneState()
        {
            if (this.currentTouches.Count > 0)
            {
                TouchLocation touch = this.currentTouches[0];
                this.currentCentroid = touch.Position;

                this.gestureSample = new GestureSample();
                this.gestureSample.IsNew = touch.IsNew;
                this.gestureSample.DeltaScale = 1;
                this.gestureSample.Type = GestureType.Pressed;
                this.gestureSample.Position = this.currentCentroid;

                this.InvokeEvent(this.TouchPressed, this.gestureSample);

                this.startTapPosition = touch.Position;
                this.state = GestureType.Pressed;
            }
        }

        /// <summary>
        /// Handle the PressedState
        /// </summary>
        private void PressedState()
        {
            if (this.currentTouches.Count == 0)
            {
                this.gestureSample.Type = GestureType.Tap;
                this.state = GestureType.Tap;
            }
            else if (this.currentTouches.Count > 1)
            {
                this.gestureSample.Type = GestureType.Free;
                this.state = GestureType.Free;
            }
            else if (this.IsMovingOverThreshold())
            {
                this.gestureSample.Type = GestureType.Drag;
                this.state = GestureType.Drag;
            }
        }

        /// <summary>
        /// Handle the TapState
        /// </summary>
        private void TapState()
        {
            this.InvokeEvent(this.TouchReleased, this.gestureSample);

            this.InvokeEvent(this.TouchTap, this.gestureSample);

            this.gestureSample.Type = GestureType.None;
            this.state = GestureType.None;
        }

        /// <summary>
        /// Handle the StoppedAndDragState
        /// </summary>
        private void StoppedAndDragState()
        {
            if (this.currentTouches.Count == 0)
            {
                this.InvokeEvent(this.TouchReleased, this.gestureSample);

                this.gestureSample.Type = GestureType.None;
                this.state = GestureType.None;
            }
            else
            {
                if (this.currentTouches.Count > 1)
                {
                    this.gestureSample.Type = GestureType.Free;
                    this.state = GestureType.Free;
                }
                else
                {
                    if (this.IsMoving())
                    {
                        this.state = GestureType.Drag;

                        this.gestureSample.Position = this.currentCentroid;

                        this.gestureSample.DeltaTranslation = this.translation;

                        // Update entity translation
                        if (this.IsGestureSupported(SupportedGesture.Translation))
                        {
                            this.Transform2D.X += this.gestureSample.DeltaTranslation.X;
                            this.Transform2D.Y += this.gestureSample.DeltaTranslation.Y;
                        }

                        this.gestureSample.Type = GestureType.Drag;

                        // Lanzamos el evento si alguien está subscrito.
                        this.InvokeEvent(this.TouchMoved, this.gestureSample);
                    }
                    else
                    {
                        this.gestureSample.Type = GestureType.Stopped;
                        this.state = GestureType.Stopped;
                        this.gestureSample.DeltaTranslation = Vector2.Zero;
                    }
                }
            }
        }

        /// <summary>
        /// Handle the FreeState
        /// </summary>
        private void FreeState()
        {
            if (this.currentTouches.Count < 2)
            {
                if (this.currentTouches.Count == 1)
                {
                    this.currentCentroid = this.currentTouches[0].Position;
                    this.previousCentroid = this.currentCentroid;
                }

                this.numTouches = 0;
                this.gestureSample.Type = GestureType.Drag;
                this.state = GestureType.Drag;
            }
            else
            {
                // Calculamos el centroid
                this.currentCentroid = this.ComputeCentroid(this.currentTouches);

                // Calculamos los puntos más lejanos
                if (this.currentTouches.Count == 2)
                {
                    this.currentFarthers1 = this.currentTouches[0].Position;
                    this.currentFarthers2 = this.currentTouches[1].Position;
                }
                else
                {
                    this.FindTwoFarthestTouches(this.currentTouches, ref this.currentFarthers1, ref this.currentFarthers2);
                }

                // Comprobar si se ha producido un cambio en el número de touches
                if (this.numTouches != this.currentTouches.Count)
                {
                    this.numTouches = this.currentTouches.Count;
                }
                else
                {
                    // Actualizamos el centroid
                    this.gestureSample.Position = this.currentCentroid;

                    // Calcula el delta translation
                    if (this.IsGestureSupported(SupportedGesture.Translation))
                    {
                        this.gestureSample.DeltaTranslation = this.currentCentroid - this.previousCentroid;

                        // Update entity translation
                        this.Transform2D.X += this.gestureSample.DeltaTranslation.X;
                        this.Transform2D.Y += this.gestureSample.DeltaTranslation.Y;
                    }

                    // (Común a Rotation & Scale) Calcular los dos puntos más alejados
                    if (this.IsGestureSupported(SupportedGesture.Rotation) || this.IsGestureSupported(SupportedGesture.Scale))
                    {
                        Vector2 currentVector = this.currentFarthers2 - this.currentFarthers1;
                        Vector2 previousVector = this.previousFarthers2 - this.previourFarthers1;

                        // Calcula el deta Scale
                        if (this.IsGestureSupported(SupportedGesture.Scale))
                        {
                            float currentLength = currentVector.Length();
                            float previousLength = previousVector.Length();
                            this.gestureSample.DiffScale = currentLength - previousLength;

                            float scale = currentLength / previousLength;

                            if (scale != 1 && !float.IsNaN(scale) && !float.IsInfinity(scale) && scale > 0.01f)
                            {
                                this.gestureSample.DeltaScale = scale;

                                // Update entity scale
                                float newScale = this.Transform2D.XScale + (this.gestureSample.DeltaScale - 1);
                                if (newScale > this.minScale && newScale < this.maxScale)
                                {
                                    this.Transform2D.XScale += this.gestureSample.DeltaScale - 1;
                                    this.Transform2D.YScale += this.gestureSample.DeltaScale - 1;
                                }
                            }
                            else
                            {
                                this.gestureSample.DeltaScale = 1;
                            }
                        }

                        // Calcula el delta rotation
                        if (this.IsGestureSupported(SupportedGesture.Rotation))
                        {
                            float angle = Vector2.ComputeAngle(ref currentVector, ref previousVector);

                            if (angle != 0 && !float.IsNaN(angle) && !float.IsInfinity(angle))
                            {
                                this.gestureSample.DeltaAngle = angle;

                                // Update entity rotation
                                this.Transform2D.Rotation += this.gestureSample.DeltaAngle;
                            }
                            else
                            {
                                this.gestureSample.DeltaAngle = 0;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Invokes the event.
        /// </summary>
        /// <param name="touchEvent">The touch event.</param>
        /// <param name="sample">The gesture sample.</param>
        private void InvokeEvent(EventHandler<GestureEventArgs> touchEvent, GestureSample sample)
        {
            if (touchEvent != null)
            {
                touchEvent(this.Owner, new GestureEventArgs(sample));
            }
        }

        /// <summary>
        /// Updates the previous values.
        /// </summary>
        private void UpdatePreviousValues()
        {
            // Update Previous Values
            this.previousCentroid = this.currentCentroid;
            this.previourFarthers1 = this.currentFarthers1;
            this.previousFarthers2 = this.currentFarthers2;

            // Clean last touchList
            this.currentTouches.Clear();
        }

        /// <summary>
        /// Determines whether [is moving over threshold].
        /// </summary>
        /// <returns>
        ///   <c>true</c> if [is moving over threshold]; otherwise, <c>false</c>.
        /// </returns>
        private bool IsMovingOverThreshold()
        {
            this.currentCentroid = this.currentTouches[0].Position;
            float dis;
            Vector2.Distance(ref this.currentCentroid, ref this.startTapPosition, out dis);

            return dis > TapThreshold;
        }

        /// <summary>
        /// Determines whether this instance is moving.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if this instance is moving; otherwise, <c>false</c>.
        /// </returns>
        private bool IsMoving()
        {
            this.currentCentroid = this.currentTouches[0].Position;
            this.translation = this.currentCentroid - this.previousCentroid;

            return this.translation.X.Distinct(0) || this.translation.Y.Distinct(0);
        }

        /// <summary>
        /// Determines whether a type of gesture is supported.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>
        ///   <c>true</c> if the type of gesture is supported; otherwise, <c>false</c>.
        /// </returns>
        private bool IsGestureSupported(SupportedGesture type)
        {
            return (this.enabledGestures & type) == type;
        }

        /// <summary>
        /// Computes the centroid of a group of touches.
        /// </summary>
        /// <param name="touches">The touches.</param>
        /// <returns>The centroid position.</returns>
        private Vector2 ComputeCentroid(List<TouchLocation> touches)
        {
            if (touches.Count < 1)
            {
                return Vector2.Zero;
            }

            float x = 0;
            float y = 0;

            for (int i = 0; i < touches.Count; i++)
            {
                TouchLocation touch = touches[i];
                x = x + touch.Position.X;
                y = y + touch.Position.Y;
            }

            return new Vector2(x / touches.Count, y / touches.Count);
        }

        /// <summary>
        /// Finds the two farthest touches in a group of touches.
        /// </summary>
        /// <param name="touches">The touches.</param>
        /// <param name="point1">The first point.</param>
        /// <param name="point2">The second point.</param>
        private void FindTwoFarthestTouches(List<TouchLocation> touches, ref Vector2 point1, ref Vector2 point2)
        {
            float maxdist = 1;

            for (int i = 0; i < touches.Count; i++)
            {
                Vector2 iTouch = touches[i].Position;

                for (int j = 0; j < touches.Count; j++)
                {
                    Vector2 jTouch = touches[j].Position;

                    float dis = (float)Math.Sqrt(Math.Pow(jTouch.X - iTouch.X, 2) +
                                                 Math.Pow(jTouch.Y - iTouch.Y, 2));

                    if (maxdist < dis)
                    {
                        maxdist = dis;
                        point1 = iTouch;
                        point2 = jTouch;
                    }
                }
            }
        }

        #endregion
    }
}
