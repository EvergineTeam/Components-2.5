#region File Description
//-----------------------------------------------------------------------------
// PlaySoundGameAction
//
// Copyright © 2015 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using System.Text;
using WaveEngine.Common.Media;
using WaveEngine.Framework;
using WaveEngine.Framework.Services;
using WaveEngine.Framework.Sound;
#endregion

namespace WaveEngine.Components.GameActions
{
    /// <summary>
    /// A game action to play a sound
    /// </summary>
    public class PlaySoundGameAction : GameAction, IUpdatableGameAction
    {
        /// <summary>
        /// Number of instances
        /// </summary>
        private static int instances;

        /// <summary>
        /// Cached sound player
        /// </summary>
        private static SoundPlayer soundPlayer = WaveServices.SoundPlayer;

        /// <summary>
        /// The sound volume
        /// </summary>
        private float volume;

        /// <summary>
        /// The sound loop enabled
        /// </summary>
        private bool loop;

        #region Properties
        /// <summary>
        /// Gets the sound info of the task
        /// </summary>
        public SoundInfo SoundInfo { get; private set; }

        /// <summary>
        /// Gets the sound instance of the task
        /// </summary>
        public SoundInstance SoundInstance { get; private set; }
        #endregion

        #region Public Methods
        /// <summary>
        /// Initializes a new instance of the <see cref="PlaySoundGameAction" /> class.
        /// </summary>
        /// <param name="soundInfo">The sound info to play</param>
        /// <param name="scene">The scene.</param>
        /// <param name="volume">The sound volume</param>
        /// <param name="loop">The sound loop is enabled</param>
        public PlaySoundGameAction(SoundInfo soundInfo, Scene scene = null, float volume = 1, bool loop = false)
            : base("PlaySoundGameAction" + instances++, scene)
        {
            this.SoundInfo = soundInfo;
            this.volume = volume;
            this.loop = loop;
        }

        /// <summary>
        /// Update the game action
        /// </summary>
        /// <param name="gameTime">The gameTime.</param>
        public void Update(TimeSpan gameTime)
        {
            if (this.SoundInstance != null && this.SoundInstance.State == SoundState.Stopped)
            {
                this.PerformCompleted();
            }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Perform Run actions
        /// </summary>
        protected override void PerformRun()
        {
            // Play sound
            this.SoundInstance = soundPlayer.Play(this.SoundInfo, this.volume, this.loop);
        }

        /// <summary>
        /// Perform cancel
        /// </summary>
        protected override void PerformCancel()
        {
            base.PerformCancel();

            // Stop sound instance
            this.SoundInstance.Stop();
            this.SoundInstance = null;
        }

        /// <summary>
        /// Skip the action
        /// </summary>
        /// <returns>A value indicating it the game action is susscessfully skipped</returns>
        protected override bool PerformSkip()
        {
            if (this.IsSkippable)
            {
                this.Cancel();
                return base.PerformSkip();
            }
            else
            {
                return false;
            }
        }
        #endregion
    }
}
