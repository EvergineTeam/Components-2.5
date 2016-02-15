#region File Description
//-----------------------------------------------------------------------------
// SkinnedModelRenderer
// Copyright © 2016 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Math;
using WaveEngine.Components.Animation;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Services;
using WaveEngine.Materials;

#endregion

namespace WaveEngine.Components.Graphics3D
{
    /// <summary>
    /// Renders an animated model.
    /// </summary>
    [DataContract(Namespace = "WaveEngine.Components.Graphics3D")]
    public class SkinnedModelRenderer : Drawable3D
    {
        /// <summary>
        /// The quality.
        /// </summary>
        private const short Quality = 4;

        /// <summary>
        ///     Number of instances of this component created.
        /// </summary>
        private static int instances;

        /// <summary>
        ///     Specific animation to render.
        /// </summary>
        [RequiredComponent]
        public Animation3D Animation;

        /// <summary>
        ///     Materials used rendering the animated model.
        /// </summary>
        [RequiredComponent]
        public MaterialsMap MaterialMap;

        /// <summary>
        ///     Animated model to render.
        /// </summary>
        [RequiredComponent(false)]
        public SkinnedModel Model;

        /// <summary>
        ///     Transform of the animated model.
        /// </summary>
        [RequiredComponent]
        public Transform3D Transform;

        /// <summary>
        /// The bone names.
        /// </summary>
        private Dictionary<string, int> boneNames;

        /// <summary>
        /// The mesh materials
        /// </summary>
        private Material[] meshMaterials;

        /// <summary>
        /// The bone transforms.
        /// </summary>
        private Matrix[] boneTransforms;

        /// <summary>
        ///     Wether this instance has been disposed.
        /// </summary>
        private bool disposed;

        /// <summary>
        /// The last key frame.
        /// </summary>
        private int lastKeyFrame = -1;

        /// <summary>
        /// The line color.
        /// </summary>
        private Color lineColor;

        /// <summary>
        /// The lod diff distance.
        /// </summary>
        private float lodDiffDistance;

        /// <summary>
        /// The lod max distance.
        /// </summary>
        private float lodMaxDistance;

        /// <summary>
        /// The lod min distance.
        /// </summary>
        private float lodMinDistance;

        /// <summary>
        /// The pass update.
        /// </summary>
        private int passUpdate;

        /// <summary>
        /// The skin transforms.
        /// </summary>
        private Matrix[] skinTransforms;

        /// <summary>
        /// The skinned model.
        /// </summary>
        private InternalSkinnedModel skinnedModel;

        /// <summary>
        /// The update lod.
        /// </summary>
        private bool updateLod;

        /// <summary>
        /// need to update vertex buffer
        /// </summary>
        private bool[] updateVertexBuffer;

        /// <summary>
        /// The world transforms.
        /// </summary>
        private Matrix[] worldTransforms;

        /// <summary>
        /// The last animation time
        /// </summary>
        private TimeSpan lastAnimTime;

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether there game is performing slowly and has to drop updates when skinning.
        /// </summary>
        /// <value>
        /// <c>true</c> if the game is performing slowly; otherwise, <c>false</c>.
        /// </value>
        public static bool LowPerformance { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether LOD is enabled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if LOD is enabled; otherwise, <c>false</c>.
        /// </value>
        [DataMember]
        public bool LODEnabled { get; set; }

        /// <summary>
        /// Gets or sets the LOD maximum distance.
        /// </summary>
        /// <value>
        /// The LOD maximum distance.
        /// </value>
        [DataMember]
        public float LODMaxDistance
        {
            get
            {
                return this.lodMaxDistance;
            }

            set
            {
                this.lodMaxDistance = value;
                this.lodDiffDistance = this.lodMaxDistance - this.lodMinDistance;
            }
        }

        /// <summary>
        /// Gets or sets the LOD minimum distance.
        /// </summary>
        /// <value>
        /// The LOD minimum distance.
        /// </value>
        [DataMember]
        public float LODMinDistance
        {
            get
            {
                return this.lodMinDistance;
            }

            set
            {
                this.lodMinDistance = value;
                this.lodDiffDistance = this.lodMaxDistance - this.lodMinDistance;
            }
        }

        /// <summary>
        /// Gets or sets the color of the line used when rendering with debug lines activated.
        /// </summary>
        /// <value>
        /// The color of the line.
        /// </value>
        [DataMember]
        public Color LineColor
        {
            get
            {
                return this.lineColor;
            }

            set
            {
                this.lineColor = value;
            }
        }

        #endregion

        #region Initialize

        /// <summary>
        /// Initializes a new instance of the <see cref="SkinnedModelRenderer" /> class.
        /// </summary>
        public SkinnedModelRenderer()
            : this("SkinnedModelRenderer" + instances)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SkinnedModelRenderer"/> class.
        /// </summary>
        /// <param name="name">
        /// Name of this instance.
        /// </param>
        public SkinnedModelRenderer(string name)
            : base(name)
        {
        }

        /// <summary>
        /// Default values
        /// </summary>
        protected override void DefaultValues()
        {
            this.LODMinDistance = 600;
            this.LODMaxDistance = 2000;
            this.LODEnabled = true;
            this.boneNames = new Dictionary<string, int>();
            instances++;
            this.updateLod = true;
            this.lineColor = Color.Yellow;
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// Draws the animated model.
        /// </summary>
        /// <param name="gameTime">The elapsed game time.</param>
        /// <remarks>
        /// This method will only be called if all the following points are true:
        /// <list type="bullet">
        /// <item>
        /// <description>The entity passes a frustrum culling test.</description>
        /// </item>
        /// <item>
        /// <description>The parent of the owner <see cref="Entity" /> of the <see cref="Drawable" /> cascades its visibility to its children and it is visible.</description>
        /// </item>
        /// <item>
        /// <description>The <see cref="Drawable" /> is active.</description>
        /// </item>
        /// <item>
        /// <description>The owner <see cref="Entity" /> of the <see cref="Drawable" /> is active and visible.</description>
        /// </item>
        /// </list>
        /// </remarks>
        public override void Draw(TimeSpan gameTime)
        {
            this.RefreshAnimation3D();
            this.RefreshModel();

            if (this.skinnedModel == null)
            {
                return;
            }

            if (this.LODEnabled)
            {
                this.passUpdate--;
                if (this.passUpdate <= 0)
                {
                    float distanceToCamera = Vector3.Distance(this.Transform.Position, this.RenderManager.CurrentDrawingCamera3D.Position);
                    float amount = (distanceToCamera - this.lodMinDistance) / this.lodDiffDistance;
                    if (amount > 1)
                    {
                        amount = 1;
                    }

                    if (amount < 0)
                    {
                        amount = 0;
                    }

                    this.passUpdate = (int)(amount * Quality);

                    this.updateLod = true;
                }
            }

            float zOrder = Vector3.DistanceSquared(this.RenderManager.CurrentDrawingCamera3D.Position, this.Transform.Position);
            bool needsUpdate = (!LowPerformance && this.lastKeyFrame != this.Animation.Frame && this.updateLod) && this.lastAnimTime != this.Animation.TotalAnimTime;
            this.lastAnimTime = this.Animation.TotalAnimTime;

            if (needsUpdate)
            {
                this.UpdateTransforms();
            }

            for (int i = 0; i < this.skinnedModel.Meshes.Count; i++)
            {
                Material currentMaterial;
                SkinnedMesh currentMesh = this.skinnedModel.Meshes[i];
                this.updateVertexBuffer[i] |= needsUpdate;

                if (this.MaterialMap.Materials.ContainsKey(currentMesh.Name))
                {
                    currentMaterial = this.MaterialMap.Materials[currentMesh.Name];
                }
                else
                {
                    currentMaterial = this.MaterialMap.DefaultMaterial;
                }

                this.meshMaterials[i] = currentMaterial;

                if (currentMaterial != null)
                {
                    if (this.updateVertexBuffer[i])
                    {
                        currentMesh.SetBones(this.skinTransforms, true);
                        this.GraphicsDevice.BindVertexBuffer(currentMesh.VertexBuffer);
                        this.updateVertexBuffer[i] = false;
                    }

                    currentMesh.ZOrder = zOrder;

                    this.GraphicsDevice.UnsetBuffers();
                    Matrix transform = this.Transform.WorldTransform;
                    this.RenderManager.DrawMesh(currentMesh, currentMaterial, ref transform, false);
                    this.GraphicsDevice.UnsetBuffers();
                }
            }

            if (this.LODEnabled)
            {
                this.updateLod = false;
            }

            this.lastKeyFrame = this.Animation.Frame;
        }

        /// <summary>
        /// Tries to get the world transform of a given bone.
        /// </summary>
        /// <param name="boneName">
        /// Name of the bone.
        /// </param>
        /// <param name="transform">
        /// The transform of the bone.
        /// </param>
        /// <returns>
        /// <c>true</c> if it was possible to get the world transform, otherwise <c>false</c>
        /// </returns>
        public bool TryGetBoneWorldTransform(string boneName, out Matrix transform)
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException("SkinnedModelRenderer");
            }

            if (this.boneNames.ContainsKey(boneName))
            {
                int index = this.boneNames[boneName];
                transform = this.worldTransforms[index];
                return true;
            }
            else
            {
                List<string> internalBoneNames = this.Animation.InternalAnimation.BoneNames;
                if (internalBoneNames.Contains(boneName))
                {
                    int count = internalBoneNames.Count;
                    for (int i = 0; i < count; i++)
                    {
                        if (internalBoneNames[i] == boneName)
                        {
                            this.boneNames.Add(boneName, i);
                            transform = this.worldTransforms[i];
                            return true;
                        }
                    }
                }
            }

            transform = Matrix.Identity;
            return false;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Deletes the dependencies after .
        /// </summary>
        protected override void DeleteDependencies()
        {
            base.DeleteDependencies();
            this.boneNames.Clear();
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing">
        /// <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.
        /// </param>
        protected override void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    if (this.skinnedModel != null)
                    {
                        this.skinnedModel.Unload();
                        this.skinnedModel = null;
                    }

                    this.disposed = true;
                }
            }
        }

        /// <summary>
        /// The draw debug lines.
        /// </summary>
        protected override void DrawDebugLines()
        {
            base.DrawDebugLines();

            if (this.Animation == null || this.Animation.InternalAnimation == null)
            {
                return;
            }

            for (int i = 0; i < this.Animation.InternalAnimation.SkeletonHierarchy.Count; i++)
            {
                if (this.Animation.InternalAnimation.SkeletonHierarchy[i] != -1)
                {
                    Vector3 start;
                    Vector3 sourceTranslation = this.worldTransforms[i].Translation;
                    Matrix world = this.Transform.WorldTransform;

                    Vector3.Transform(ref sourceTranslation, ref world, out start);

                    Vector3 end;
                    Vector3 endTranslation =
                        this.worldTransforms[this.Animation.InternalAnimation.SkeletonHierarchy[i]].Translation;
                    Vector3.Transform(ref endTranslation, ref world, out end);

                    this.RenderManager.LineBatch3D.DrawLine(ref start, ref end, ref this.lineColor);
                }
            }
        }

        /// <summary>
        /// Performs further custom initialization for this instance.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();

            this.RefreshAnimation3D();
            this.RefreshModel();
        }

        /// <summary>
        /// Refresh the animnation asset 3D
        /// </summary>
        private void RefreshAnimation3D()
        {
            int boneCount = this.Animation.InternalAnimation != null ? this.Animation.InternalAnimation.BindPose.Count : 0;

            if (this.boneTransforms == null
                || (this.Animation.InternalAnimation != null && this.boneTransforms.Length != boneCount))
            {
                Array.Resize(ref this.boneTransforms, boneCount);
                Array.Resize(ref this.worldTransforms, boneCount);
                Array.Resize(ref this.skinTransforms, boneCount);
            }
        }

        /// <summary>
        /// Refresh the model references
        /// </summary>
        private void RefreshModel()
        {
            if (this.skinnedModel == null
                || (this.skinnedModel.AssetPath != this.Model.ModelPath))
            {
                if (this.skinnedModel != null)
                {
                    this.skinnedModel.Unload();
                    this.skinnedModel = null;
                }

                if (this.Model.InternalModel != null)
                {
                    this.skinnedModel = this.Model.InternalModel.Clone();
                    Array.Resize(ref this.meshMaterials, this.skinnedModel.Meshes.Count);
                    Array.Resize(ref this.updateVertexBuffer, this.skinnedModel.Meshes.Count);
                }

                this.boneNames.Clear();
            }
        }

        /// <summary>
        /// Updates the transforms.
        /// </summary>
        /// <exception cref="System.ObjectDisposedException">SkinnedModelRenderer has been diposed</exception>
        private void UpdateTransforms()
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException("SkinnedModelRenderer");
            }

            // Update Bone Transforms
            this.Animation.InternalAnimation.Animations[this.Animation.CurrentAnimation].Frames[this.Animation.Frame].CopyTo(this.boneTransforms, 0);

            // Update World Transforms
            this.worldTransforms[0] = this.boneTransforms[0];

            for (int bone = 1; bone < this.worldTransforms.Length; bone++)
            {
                int parentBone = this.Animation.InternalAnimation.SkeletonHierarchy[bone];
                Matrix.Multiply(
                    ref this.boneTransforms[bone], ref this.worldTransforms[parentBone], out this.worldTransforms[bone]);
            }

            // Update Skin Transforms();
            for (int bone = 0; bone < this.skinTransforms.Length; bone++)
            {
                Matrix bindPose = this.Animation.InternalAnimation.InverseBindPose[bone];
                Matrix.Multiply(ref bindPose, ref this.worldTransforms[bone], out this.skinTransforms[bone]);
            }

            // Apply skin transform to relocate bounding box to the correct position.
            // TODO: animate the bounding box through all the frames.
            if (this.Animation.BoundingBoxRefreshed)
            {
                int bbIndex = this.Model.InternalModel.BoundingBoxBoneIndex;
                if (bbIndex >= this.skinTransforms.Length)
                {
                    bbIndex = 0;
                }

                ////this.Model.BoundingBox.Min = Vector3.Transform(this.Animation.InternalAnimation.Animations[this.Animation.CurrentAnimation].BoundingBox.Min, this.skinTransforms[bbIndex]);
                ////this.Model.BoundingBox.Max = Vector3.Transform(this.Animation.InternalAnimation.Animations[this.Animation.CurrentAnimation].BoundingBox.Max, this.skinTransforms[bbIndex]);
                this.Animation.BoundingBoxRefreshed = false;
                this.Model.BoundingBoxRefreshed = true;
            }
        }

        #endregion
    }
}
