// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Using Statements
using System;
using WaveEngine.Common.Media;
using WaveEngine.Framework;
using WaveEngine.Framework.Services;
#endregion

namespace WaveEngine.Components.GameActions
{
    /// <summary>
    /// A game action that play a music
    /// </summary>
    public class PlayMusicGameAction : GameAction
    {
        /// <summary>
        /// Number of instances
        /// </summary>
        private static int instances;

        /// <summary>
        /// The music info
        /// </summary>
        private MusicInfo musicInfo;

        /// <summary>
        /// Music player service.
        /// </summary>
        [RequiredService]
        private MusicPlayer musicPlayer = null;

        #region Properties
        #endregion

        #region Initialize

        /// <summary>
        /// Initializes a new instance of the <see cref="PlayMusicGameAction" /> class.
        /// </summary>
        /// <param name="musicInfo">The music info to play</param>
        /// <param name="scene">The associated scene.</param>
        public PlayMusicGameAction(MusicInfo musicInfo, Scene scene = null)
            : base("PlayMusicGameAction" + instances++, scene)
        {
            this.musicInfo = musicInfo;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PlayMusicGameAction" /> class.
        /// </summary>
        /// <param name="parent">The parent task.</param>
        /// <param name="musicInfo">The music info to play</param>
        public PlayMusicGameAction(IGameAction parent, MusicInfo musicInfo)
            : base(parent, "PlayMusicGameAction" + instances++)
        {
            this.musicInfo = musicInfo;
        }
        #endregion

        #region Public methods

        /// <summary>
        /// Perform run action
        /// </summary>
        protected override void PerformRun()
        {
            this.musicPlayer.OnSongCompleted += this.OnSongCompleted;
            this.musicPlayer.Play(this.musicInfo);
        }

        /// <summary>
        /// The music has been completed
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event args.</param>
        private void OnSongCompleted(object sender, EventArgs e)
        {
            this.musicPlayer.OnSongCompleted -= this.OnSongCompleted;
            this.PerformCompleted();
        }

        /// <summary>
        /// Perform cancelation event
        /// </summary>
        protected override void PerformCancel()
        {
            this.musicPlayer.OnSongCompleted -= this.OnSongCompleted;
            this.musicPlayer.Stop();

            base.PerformCancel();
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

        #region Private Methods
        #endregion
    }
}
