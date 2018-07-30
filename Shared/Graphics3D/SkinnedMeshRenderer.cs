// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using WaveEngine.Common.Attributes;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Math;
using WaveEngine.Common.Shared.Graphics;
using WaveEngine.Framework;
using WaveEngine.Framework.Diagnostic;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Services;
#endregion

namespace WaveEngine.Components.Graphics3D
{
    /// <summary>
    /// Renders an skinned mesh on the screen.
    /// </summary>
    [DataContract(Namespace = "WaveEngine.Components.Graphics3D")]
    public class SkinnedMeshRenderer : Drawable3D
    {
        /// <summary>
        /// Number of instances of this component created.
        /// </summary>
        private static int instances;

        /// <summary>
        /// <see cref="MeshComponent"/> to render.
        /// </summary>
        [RequiredComponent(false)]
        public MeshComponent ModelMesh;

        /// <summary>
        /// Materials used rendering the <see cref="MeshComponent"/>.
        /// </summary>
        public MaterialComponent[] Materials;

        /// <summary>
        /// Transform of the <see cref="MeshRenderer"/>.
        /// </summary>
        [RequiredComponent]
        public Transform3D Transform;

        /// <summary>
        /// Wether this instance has been disposed.
        /// </summary>
        private bool disposed;

        /// <summary>
        /// The skin
        /// </summary>
        private SkinContent skin;

        /// <summary>
        /// The mesh content
        /// </summary>
        private MeshContent meshContent;

        /// <summary>
        /// The root bone inverse bind pose
        /// </summary>
        private Matrix rootInverseBindPose;

        /// <summary>
        /// The root joint
        /// </summary>
        [DataMember]
        private string rootJointPath;

        [DataMember]
        private string[] jointPaths;

        /// <summary>
        /// The root joint path
        /// </summary>
        private Entity rootJoint;

        /// <summary>
        /// The root joint is changed
        /// </summary>
        private bool rootJointChanged;

        /// <summary>
        /// The root joint trasnform
        /// </summary>
        private Transform3D rootJointTransform;

        /// <summary>
        /// The mesh content
        /// </summary>
        private SkinnedMesh[] meshes;

        /// <summary>
        /// The joint list
        /// </summary>
        private Transform3D[] joints;

        /// <summary>
        /// The cached transform update
        /// </summary>
        private int[] cachedTransformUpdate;

        /// <summary>
        /// The skin matrices
        /// </summary>
        private Matrix[] skinMatrices;

        /// <summary>
        /// The skin should be skinned
        /// </summary>
        private bool shouldSkinMeshes;

        /// <summary>
        /// The skin quality
        /// </summary>
        private SkinQuality quality;

        /// <summary>
        /// The morph tarngets weights
        /// </summary>
        [DataMember]
        private float[] morphTargetWeights;

        /// <summary>
        /// The morph tarngets weights
        /// </summary>
        private float morphTargetWeightsHash;

        /// <summary>
        /// Gets or sets the morph target weights
        /// </summary>
        public float[] MorphTargetWeights
        {
            get
            {
                return this.morphTargetWeights;
            }

            set
            {
                this.morphTargetWeights = value;
            }
        }

        /// <summary>
        /// Gets or sets the root joint
        /// </summary>
        [RenderPropertyAsEntity(new string[] { "WaveEngine.Framework.Transform3D" })]
        public string RootJointPath
        {
            get
            {
                return this.rootJointPath;
            }

            set
            {
                this.rootJointPath = value;

                if (this.isInitialized)
                {
                    this.RefreshRootJointPath();
                }
            }
        }

        /// <summary>
        /// Gets or sets the joint paths array
        /// </summary>
        public string[] JointPaths
        {
            get
            {
                return this.jointPaths;
            }

            set
            {
                this.jointPaths = value;
                if (this.isInitialized)
                {
                    this.RefreshJointPaths();
                }
            }
        }

        /// <summary>
        /// Gets or sets the root joint
        /// </summary>
        public Entity RootJoint
        {
            get
            {
                return this.rootJoint;
            }

            set
            {
                this.rootJoint = value;
                this.rootJointChanged = true;
                if (this.isInitialized)
                {
                    this.RefreshRootJoint();
                }
            }
        }

        /// <summary>
        /// Gets or sets the joints used to skin this mesh.
        /// </summary>
        [DontRenderProperty]
        public Transform3D[] Joints
        {
            get
            {
                return this.joints;
            }

            set
            {
                this.joints = value;
                this.jointPaths = this.joints?.Select(j => j.Owner.EntityPath).ToArray();
                this.RefreshCahedTransformUpdate();
            }
        }

        /// <summary>
        /// Gets or sets the skin quality
        /// </summary>
        [DataMember]
        public SkinQuality Quality
        {
            get
            {
                return this.quality;
            }

            set
            {
                if (this.quality != value)
                {
                    this.quality = value;
                    this.shouldSkinMeshes = true;
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SkinnedMeshRenderer"/> class.
        /// </summary>
        public SkinnedMeshRenderer()
            : base("SkinnedMeshRenderer" + instances++)
        {
        }

        /// <summary>
        /// Sets the default values
        /// </summary>
        protected override void DefaultValues()
        {
            base.DefaultValues();
            this.Quality = SkinQuality._4Joints;
            this.rootJointChanged = true;
        }

        /// <summary>
        /// Resolve dependencies
        /// </summary>
        protected override void ResolveDependencies()
        {
            base.ResolveDependencies();

            this.Materials = this.Owner.FindComponents<MaterialComponent>()
                                       .ToArray();
        }

        /// <summary>
        /// Initialize model renderer
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();

            this.ModelMesh.Refreshed -= this.OnModelRefreshed;
            this.ModelMesh.Refreshed += this.OnModelRefreshed;

            this.RefreshRootJointPath();
            this.RefreshRootJoint();
            this.RefreshJointPaths();

            this.Owner.EntityInitialized += this.Owner_EntityInitialized;
        }

        /// <summary>
        /// Draws the model.
        /// </summary>
        /// <param name="gameTime">The elapsed game time.</param>
        public override void Draw(TimeSpan gameTime)
        {
            if (this.meshes == null)
            {
                return;
            }

            float zOrder = Vector3.DistanceSquared(this.RenderManager.CurrentDrawingCamera3D.Position, this.rootJointTransform.Position);

            this.shouldSkinMeshes |= this.RefreshSkinMatrices();
            this.shouldSkinMeshes |= this.ShouldRefreshMorph();

            var worldTransform = this.rootJointTransform.WorldTransform;

            for (int i = 0; i < this.meshes.Length; i++)
            {
                var currentMesh = this.meshes[i] as SkinnedMesh;

                if (this.shouldSkinMeshes)
                {
                    Timers.BeginAveragedTimer("Skin_" + this.Owner.Name);
                    currentMesh.SetBones(this.skinMatrices, this.Quality, this.morphTargetWeights, true, true);
                    Timers.EndAveragedTimer("Skin_" + this.Owner.Name);

                    this.GraphicsDevice.BindVertexBuffer(currentMesh.VertexBuffer);
                }

                Material currentMaterial = null;
                if (this.Materials?.Length > 0 && this.ModelMesh?.InternalModel?.Materials?.Count > currentMesh.MaterialIndex)
                {
                    string fileMaterialName = this.ModelMesh.InternalModel.Materials[currentMesh.MaterialIndex];

                    var materialComponent = (from m in this.Materials where m.AsignedTo == fileMaterialName select m).FirstOrDefault();
                    if (materialComponent != null)
                    {
                        currentMaterial = materialComponent.Material;
                    }
                    else
                    {
                        currentMaterial = this.Materials[0].Material;
                    }
                }

                if (currentMaterial != null)
                {
                    currentMesh.ZOrder = zOrder;

                    // Draw mesh
                    this.RenderManager.DrawMesh(currentMesh, currentMaterial, worldTransform, this.Owner.IsFinalStatic);
                }
            }

            this.shouldSkinMeshes = false;
            this.rootJointChanged = false;
        }

        private void Owner_EntityInitialized(object sender, EventArgs e)
        {
            this.RefreshModel();
            this.Owner.EntityInitialized -= this.Owner_EntityInitialized;
        }

        /// <summary>
        /// If this mesh should be morph
        /// </summary>
        /// <returns>If the mesh morph must be refreshed</returns>
        private bool ShouldRefreshMorph()
        {
            bool shouldRefreshMorph = false;
            if (this.morphTargetWeights != null)
            {
                float hash = 0;
                for (int i = 0; i < this.morphTargetWeights.Length; i++)
                {
                    hash += (i * 10) + this.morphTargetWeights[i];
                }

                shouldRefreshMorph = hash != this.morphTargetWeightsHash;
                this.morphTargetWeightsHash = hash;
            }

            return shouldRefreshMorph;
        }

        /// <summary>
        /// Refresh joints
        /// </summary>
        private void RefreshJointPaths()
        {
            if (this.jointPaths != null)
            {
                Array.Resize(ref this.joints, this.jointPaths.Length);
                for (int i = 0; i < this.jointPaths.Length; i++)
                {
                    this.joints[i] = this.EntityManager.Find(this.jointPaths[i], this.Owner)?.FindComponent<Transform3D>();
                }

                this.RefreshCahedTransformUpdate();
            }
            else
            {
                this.joints = null;
            }

            this.shouldSkinMeshes = true;
        }

        /// <summary>
        /// Refresh the skin matrices
        /// </summary>
        /// <returns>True if the matrices has changed</returns>
        private bool RefreshSkinMatrices()
        {
            if (this.joints == null)
            {
                return false;
            }

            Matrix jointWorldTransform;
            int updateCounter;
            bool changed = false;

            Matrix rootInverse = this.rootJointTransform.WorldInverseTransform;

            Matrix tmp;

            for (int i = 0; i < this.joints.Length; i++)
            {
                var joint = this.joints[i];

                if (joint == null)
                {
                    continue;
                }

                updateCounter = joint.UpdateCounter;
                jointWorldTransform = joint.WorldTransform;
                if (!this.rootJointChanged && this.cachedTransformUpdate[i] == updateCounter)
                {
                    continue;
                }

                changed = true;
                this.cachedTransformUpdate[i] = updateCounter;
                Matrix.Multiply(ref this.skin.Joints[i].InverseBindPose, ref jointWorldTransform, out tmp);
                Matrix.Multiply(ref tmp, ref rootInverse, out this.skinMatrices[i]);
            }

            return changed;
        }

        /// <summary>
        /// Refresh the bounding box of this drawable
        /// </summary>
        protected override void RefreshBoundingBox()
        {
            if (this.ModelMesh != null && this.ModelMesh.BoundingBox.HasValue)
            {
                var bbox = this.ModelMesh.BoundingBox.Value;
                bbox.Transform(this.rootInverseBindPose * this.rootJointTransform.WorldTransform);
                this.BoundingBox = bbox;
            }
            else
            {
                this.BoundingBox = null;
            }
        }

        /// <summary>
        /// On model refreshed
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">The args</param>
        private void OnModelRefreshed(object sender, EventArgs e)
        {
            this.RefreshModel();
        }

        private void RefreshRootJointPath()
        {
            if (!string.IsNullOrEmpty(this.rootJointPath) && this.rootJoint == null)
            {
                this.RootJoint = this.EntityManager.Find(this.rootJointPath, this.Owner);
            }
            else
            {
                this.RootJoint = this.Owner;
            }
        }

        private void RefreshRootJoint()
        {
            if (this.Owner.IsDisposed)
            {
                return;
            }

            if (this.rootJoint == null)
            {
                this.rootJoint = this.Owner;
                this.rootJointTransform = this.Transform;
                this.rootJointPath = null;
            }
            else
            {
                this.rootJointTransform = this.rootJoint.FindComponent<Transform3D>();
            }

            this.shouldSkinMeshes = true;
        }

        /// <summary>
        /// The refresh model
        /// </summary>
        private void RefreshModel()
        {
            if (this.ModelMesh.InternalModel == null)
            {
                return;
            }

            this.meshContent = this.ModelMesh.MeshContent;
            this.meshes = this.ModelMesh.MeshContent?.MeshParts?.Select(mp => (mp as SkinnedMesh)?.Clone()).ToArray();

            this.rootInverseBindPose = Matrix.Identity;

            if (this.meshContent.Skin >= 0)
            {
                this.skin = this.ModelMesh.InternalModel?.Skins[this.meshContent.Skin];

                for (int i = 0; i < this.skin.Joints.Length; i++)
                {
                    if (this.skin.RootJoint == this.skin.Joints[i].NodeId)
                    {
                        this.rootInverseBindPose = this.skin.Joints[i].InverseBindPose;
                        break;
                    }
                }

                Array.Resize(ref this.skinMatrices, this.skin.Joints.Length);
                for (int i = 0; i < this.skinMatrices.Length; i++)
                {
                    this.skinMatrices[i] = Matrix.Identity;
                }
            }

            this.shouldSkinMeshes = true;
            this.rootJointChanged = true;
        }

        /// <summary>
        /// Rsolve joint entity
        /// </summary>
        /// <param name="jointId">The joint Id</param>
        /// <returns>The joint transform</returns>
        private Transform3D ResolveJoint(int jointId)
        {
            int nodeId = this.skin.Joints[jointId].NodeId;
            NodeContent jointNode = this.ModelMesh.InternalModel.Nodes[nodeId];

            if (jointNode.Name == this.rootJoint.Name)
            {
                return this.rootJointTransform;
            }
            else
            {
                return this.rootJoint.FindChild(jointNode.Name, true)?.FindComponent<Transform3D>();
            }
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
                    this.disposed = true;
                    this.ModelMesh.Refreshed -= this.OnModelRefreshed;

                    for (int i = 0; i < this.meshes?.Length; i++)
                    {
                        var mesh = this.meshes[i];
                        if (mesh != null)
                        {
                            this.GraphicsDevice.DestroyIndexBuffer(mesh.IndexBuffer);
                            this.GraphicsDevice.DestroyVertexBuffer(mesh.VertexBuffer);
                        }
                    }

                    this.meshes = null;
                }
            }
        }

        /// <summary>
        /// Refresh cached transforms update
        /// </summary>
        private void RefreshCahedTransformUpdate()
        {
            Array.Resize(ref this.cachedTransformUpdate, this.joints.Length);
            for (int i = 0; i < this.cachedTransformUpdate.Length; i++)
            {
                this.cachedTransformUpdate[i] = -1;
            }
        }
    }
}
