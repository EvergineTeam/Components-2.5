// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Using Statements
using System;
using System.Collections.Generic;
using WaveEngine.Common.Math;
using WaveEngine.Components.Graphics3D;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics3D;
#endregion

namespace WaveEngine.Components.Animation
{
    /// <summary>
    /// Define the skeleton pose
    /// </summary>
    public class NodeHierarchyMapping
    {
        /// <summary>
        /// The internal model
        /// </summary>
        private InternalModel internalModel;

        /// <summary>
        /// The root entity
        /// </summary>
        private Entity rootEntity;

        private Entity[] entities;

        #region Properties

        /// <summary>
        /// Gets the mapped entities
        /// </summary>
        public Entity[] Entities => this.entities;
        #endregion

        #region Initialization

        /// <summary>
        /// Initializes a new instance of the <see cref="NodeHierarchyMapping"/> class.
        /// </summary>
        /// <param name="internalModel">The inetrnal model</param>
        /// <param name="rootEntity">The root entity</param>
        public NodeHierarchyMapping(InternalModel internalModel, Entity rootEntity)
        {
            if (internalModel == null)
            {
                throw new ArgumentNullException("internalModel");
            }

            if (rootEntity == null)
            {
                throw new ArgumentNullException("rootEntity");
            }

            this.internalModel = internalModel;
            this.rootEntity = rootEntity;

            this.ResolveHierarchy();
        }

        /// <summary>
        /// Resolve the hierarchy
        /// </summary>
        private void ResolveHierarchy()
        {
            Array.Resize(ref this.entities, this.internalModel.Nodes.Length);
            for (int i = 0; i < this.entities.Length; i++)
            {
                var node = this.internalModel.Nodes[i];
                Entity channelEntity = this.rootEntity;
                var nodePath = node.NodePath.Split('.');
                if (nodePath.Length == 1)
                {
                    this.entities[i] = this.rootEntity;
                }
                else
                {
                    for (int j = 1; j < nodePath.Length; j++)
                    {
                        var childNodeName = nodePath[j];
                        channelEntity = channelEntity.FindChild(childNodeName);
                        if (channelEntity == null)
                        {
                            break;
                        }
                    }

                    this.entities[i] = channelEntity;
                }
            }
        }
        #endregion
    }
}
