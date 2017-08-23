// Copyright © 2017 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Using Statements
using System;
using System.Runtime.Serialization;
using WaveEngine.Common.Attributes;
using WaveEngine.Common.Math;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
#endregion

namespace WaveEngine.Components.Toolkit
{
    /// <summary>
    /// Behavior that allows an entity to look to the camera or another entity
    /// </summary>
    [DataContract(Namespace = "WaveEngine.Components.Toolkit")]
    public class LookAtBehavior : Behavior
    {
        /// <summary>
        /// Enum for the axis
        /// </summary>
        public enum AxisEnum
        {
            /// <summary>
            /// X axis
            /// </summary>
            X,

            /// <summary>
            /// Y axis
            /// </summary>
            Y,

            /// <summary>
            ///  Z axis
            /// </summary>
            Z
        }

        /// <summary>
        /// Half PI
        /// </summary>
        private const float HalfPi = (float)(Math.PI * 0.5);

        /// <summary>
        /// PI value float
        /// </summary>
        private const float Pi = (float)Math.PI;

        /// <summary>
        /// The owner trasnform
        /// </summary>
        [RequiredComponent]
        protected Transform3D transform;

        /// <summary>
        /// If the entity look at the target rotating just in one axis.
        /// </summary>
        [DataMember]
        private bool axisConstraint;

        /// <summary>
        /// If the alignment is in local axis or absolute axis.
        /// </summary>
        [DataMember]
        private bool localOrientation;

        /// <summary>
        /// If the entity looks at the camera or another entity
        /// </summary>
        [DataMember]
        private bool lookAtEntity;

        /// <summary>
        /// The target entity path
        /// </summary>
        [DataMember]
        private string targetEntity;

        /// <summary>
        /// The target entity trasnform
        /// </summary>
        private Transform3D targetTransform;

        /// <summary>
        /// If the behavior strategy must be updated.
        /// </summary>
        private bool dirtyStrategy;

        /// <summary>
        /// If it's neccesary to update the target
        /// </summary>
        private bool dirtyTarget;

        /// <summary>
        /// The alignment strategy action
        /// </summary>
        private Action alignmentAction;

        /// <summary>
        /// The target position
        /// </summary>
        private Vector3 targetPosition;

        /// <summary>
        /// Gets or sets a value indicating the axis of the entity to be aligned
        /// </summary>
        [DataMember]
        [RenderProperty(Tooltip = "The axis of the entity used for the alignment")]
        public AxisEnum Axis { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the alignment is flipped
        /// </summary>
        [RenderProperty(Tooltip = "Flips the alignment")]
        [DataMember]
        public bool Flip { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the entity look at the target rotating just in one axis.
        /// </summary>
        [RenderProperty(Tag = 1, CustomPropertyName = "Axis Constraint", Tooltip = "If true, the orientation is restricted to just one axis.")]
        public bool AxisConstraint
        {
            get
            {
                return this.axisConstraint;
            }

            set
            {
                this.axisConstraint = value;
                this.dirtyStrategy = true;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating the rotating axis of the alignment
        /// </summary>
        [DataMember]
        [RenderProperty(AttatchToTag = 1, AttachToValue = true, CustomPropertyName = "Orientation Axis", Tooltip = "The constraint axis used by the entity to orientate to its target")]
        public AxisEnum OrientationAxis { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the alignment is in local axis or absolute axis.
        /// </summary>
        [RenderProperty(AttatchToTag = 1, AttachToValue = true, CustomPropertyName = "Is Local Oriented", Tooltip = "If true, the constraint alignment is made using local orientations")]
        public bool IsLocalOriented
        {
            get
            {
                return this.localOrientation;
            }

            set
            {
                this.localOrientation = value;
                this.dirtyStrategy = true;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the entity looks to another entity, instead of the camera
        /// </summary>
        [RenderProperty(Tag = 2, CustomPropertyName = "Look At Entity", Tooltip = "If true, the entity looks at another entity. If false, it looks to the camera")]
        public bool LookAtEntity
        {
            get
            {
                return this.lookAtEntity;
            }

            set
            {
                this.lookAtEntity = value;
                this.dirtyTarget = true;
            }
        }

        /// <summary>
        /// Gets or sets the target entity path
        /// </summary>
        [RenderPropertyAsEntity(new string[] { "WaveEngine.Framework.Graphics.Transform3D" }, AttatchToTag = 2, AttachToValue = true, CustomPropertyName = "Target Entity", Tooltip = "The target entity")]
        public string TargetEntity
        {
            get
            {
                return this.targetEntity;
            }

            set
            {
                this.targetEntity = value;
                this.dirtyTarget = true;
            }
        }

        /// <summary>
        /// Sets the default values of the behavior
        /// </summary>
        protected override void DefaultValues()
        {
            base.DefaultValues();

            this.Axis = AxisEnum.Z;
            this.AxisConstraint = false;
            ////this.targetEntity = null;
        }

        /// <summary>
        /// Initializes the component
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();

            this.dirtyStrategy = true;
            this.dirtyTarget = true;
        }

        /// <summary>
        /// Updates the behavior
        /// </summary>
        /// <param name="gameTime">The elapsed game time</param>
        protected override void Update(TimeSpan gameTime)
        {
            if (this.dirtyTarget)
            {
                this.UpdateTarget();
                this.dirtyTarget = false;
            }

            if (this.dirtyStrategy)
            {
                this.UpdateStrategy();
                this.dirtyStrategy = false;
            }

            if (this.targetTransform != null)
            {
                this.targetPosition = this.targetTransform.Position;
            }
            else
            {
                this.targetPosition = this.RenderManager.ActiveCamera3D.Position;
            }

            this.alignmentAction();

            this.ApplyAxis();
        }

        /// <summary>
        /// Updates the target
        /// </summary>
        private void UpdateTarget()
        {
            if (this.lookAtEntity && this.targetEntity != null)
            {
                var target = this.EntityManager.Find(this.targetEntity, this.Owner);
                if (target != null)
                {
                    this.targetTransform = target.FindComponent<Transform3D>();
                }
                else
                {
                    this.targetTransform = null;
                }
            }
            else
            {
                this.targetTransform = null;
            }
        }

        /// <summary>
        /// Updates the align strategy
        /// </summary>
        private void UpdateStrategy()
        {
            if (this.axisConstraint)
            {
                if (this.localOrientation)
                {
                    this.alignmentAction = this.LocalAxisAlignment;
                }
                else
                {
                    this.alignmentAction = this.GlobalAxisAlignment;
                }
            }
            else
            {
                this.alignmentAction = this.LookAtAlignment;
            }
        }

        /// <summary>
        /// Look to the target directly
        /// </summary>
        private void LookAtAlignment()
        {
            this.transform.LookAt(this.targetPosition, Vector3.UnitY);
        }

        /// <summary>
        /// Applies the axis orientation
        /// </summary>
        private void ApplyAxis()
        {
            Quaternion offsetQ;

            switch (this.Axis)
            {
                case AxisEnum.X:

                    if (this.Flip)
                    {
                        Quaternion.CreateFromYawPitchRoll(-HalfPi, 0, 0, out offsetQ);
                    }
                    else
                    {
                        Quaternion.CreateFromYawPitchRoll(HalfPi, 0, 0, out offsetQ);
                    }

                    break;
                case AxisEnum.Y:

                    if (this.Flip)
                    {
                        Quaternion.CreateFromYawPitchRoll(0, -HalfPi, -Pi, out offsetQ);
                    }
                    else
                    {
                        Quaternion.CreateFromYawPitchRoll(0, HalfPi, Pi, out offsetQ);
                    }

                    break;
                case AxisEnum.Z:

                    if (this.Flip)
                    {
                        return;
                    }
                    else
                    {
                        Quaternion.CreateFromYawPitchRoll(Pi, 0, 0, out offsetQ);
                    }

                    break;
                default:
                    return;
            }

            this.transform.Orientation = Quaternion.Concatenate(offsetQ, this.transform.Orientation);
        }

        /// <summary>
        /// Look to the target using global axis
        /// </summary>
        private void GlobalAxisAlignment()
        {
            switch (this.OrientationAxis)
            {
                case AxisEnum.X:
                    this.targetPosition.X = this.transform.Position.X;
                    break;
                case AxisEnum.Y:
                    this.targetPosition.Y = this.transform.Position.Y;
                    break;
                case AxisEnum.Z:
                    this.targetPosition.Z = this.transform.Position.Z;
                    break;
                default:
                    break;
            }

            var up = Vector3.UnitY;

            Quaternion orientation;

            var position = this.transform.Position - this.targetPosition;

            Quaternion.CreateFromLookAt(ref position, ref up, out orientation);

            this.transform.Orientation = orientation;
        }

        /// <summary>
        /// Alignment based in local axis
        /// </summary>
        private void LocalAxisAlignment()
        {
            var localTargetPosition = -Vector3.Transform(this.targetPosition, this.transform.WorldToLocalTransform);

            switch (this.OrientationAxis)
            {
                case AxisEnum.X:
                    localTargetPosition.X = 0;
                    break;
                case AxisEnum.Y:
                    localTargetPosition.Y = 0;
                    break;
                case AxisEnum.Z:
                    localTargetPosition.Z = 0;
                    break;
                default:
                    break;
            }

            var up = Vector3.UnitY;

            Quaternion orientation;

            Quaternion.CreateFromLookAt(ref localTargetPosition, ref up, out orientation);

            this.transform.LocalOrientation = orientation;
        }
    }
}
