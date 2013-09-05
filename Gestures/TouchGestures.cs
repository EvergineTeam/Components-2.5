#region File Description
//-----------------------------------------------------------------------------
// TouchGestures
//
// Copyright © 2010 - 2013 Wave Coorporation. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
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
    /// Allows a control to receive touch input.
    /// </summary>
    public class TouchGestures : Behavior, ITouchable
    {
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
        /// Handle to collider2D.
        /// </summary>
        [RequiredComponent(false)]
        public Collider2D Collider;

        /// <summary>
        /// The transform2D
        /// </summary>
        [RequiredComponent]
        public Transform2D Transform2D;

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
        /// The touch manager
        /// </summary>
        private TouchPanel touchManager;

        /// <summary>
        /// The start tap position
        /// </summary>
        private Vector2 startTapPosition;

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
        /// Occurs when [touch orden changed].
        /// </summary>
        public event EventHandler TouchOrdenChanged;

        #region Properties

        /// <summary>
        /// Gets or sets the touch order.
        /// </summary>
        /// <value>
        /// The touch order.
        /// </value>
        /// <remarks>
        /// "0" is Back and "> 0" Near
        /// </remarks>
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
        /// Gets or sets the min scale.
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
        /// Gets or sets the max scale.
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
        /// </summary>
        /// <value>
        /// The enabled gestures.
        /// </value>
        /// <exception cref="System.ArgumentException">If value is set to SupportedGesture.None.</exception>
        public SupportedGesture EnabledGestures
        {
            get
            {
                return this.enabledGestures;
            }

            set
            {
                // Commented as I (Marcos) don't understand the reason to not allow None
                ////if (value == SupportedGesture.None)
                ////{
                ////    throw new ArgumentException("EnablesGestures can not be None");
                ////}

                this.enabledGestures = value;
            }
        }
        #endregion

        #region Initialize
        /// <summary>
        /// Initializes a new instance of the <see cref="TouchGestures"/> class.
        /// </summary>
        public TouchGestures()
            : base("TouchGestures" + instances++)
        {
            this.currentTouches = new List<TouchLocation>();
            this.minScale = 0.1f;
            this.maxScale = 5f;
            this.gestureSample = new GestureSample { DeltaScale = 1 };
            this.enabledGestures = SupportedGesture.None;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Determines whether [contains] [the specified point].
        /// </summary>
        /// <param name="point">The point.</param>
        /// <returns>
        ///   <c>true</c> if [contains] [the specified point]; otherwise, <c>false</c>.
        /// </returns>
        public bool Contains(Vector2 point)
        {
            if (!Owner.IsActive || !Owner.IsVisible)
            {
                return false;
            }

            return this.Collider.Contain(point);
        }

        /// <summary>
        /// Adds a touch location to the current active touches.
        /// </summary>
        /// <param name="touch">The touch location.</param>
        /// <param name="isNew">if set to <c>true</c> [is new].</param>
        public void AddTouch(TouchLocation touch, bool isNew)
        {
            touch.IsNew = isNew;
            this.currentTouches.Add(touch);
        }

        /// <summary>
        /// Gets the current gesture.
        /// </summary>
        /// <returns>The current gesture</returns>
        public GestureSample ReadGesture()
        {
            return this.gestureSample;
        }
        #endregion

        #region Private Methods

        /// <summary>
        /// Resolves the dependencies needed for this instance to work.
        /// </summary>
        protected override void ResolveDependencies()
        {
            base.ResolveDependencies();

            if (this.Owner != null)
            {
                int order = 0;
                int numberOfLevels = 100;

                Drawable2D drawable2D = this.Owner.FindComponentOfType<Drawable2D>();
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

            this.touchManager = WaveServices.TouchPanel;
            this.touchManager.Subscribe(this);
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
                        int v = (int)this.gestureSample.Type;

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
