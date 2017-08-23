

#region Using Statements
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using WaveEngine.Framework;
using WaveEngine.Framework.Physics3D;
#endregion

namespace WaveEngine.Components.Helpers
{
    /// <summary>
    /// A trigger volum who raise event whe a specific entity enters in.
    /// </summary>
    [DataContract(Namespace = "WaveEngine.Framework.Physics3D")]
    public class TriggerVolume : Behavior
    {
        /// <summary>
        /// The 3d collider using to check 
        /// </summary>
        [RequiredComponent]
        public Collider3D Collider;

        ///// <summary>
        ///// Occurs when an entity enter.
        ///// </summary>
        ////public event EventHandler<Entity> TriggerEnter;

        ///// <summary>
        ///// Occurs when an entity exit.
        ///// </summary>
        ////public event EventHandler<Entity> TriggerExit;

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
        }

        /// <summary>
        /// Update the current state.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        protected override void Update(TimeSpan gameTime)
        {
        }
    }
}
