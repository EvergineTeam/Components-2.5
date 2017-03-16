#region File Description
//-----------------------------------------------------------------------------
// PlayVideoGameAction
//
// Copyright © 2017 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using WaveEngine.Common.Media;
using WaveEngine.Framework;
using WaveEngine.Framework.Services;
#endregion

namespace WaveEngine.Components.GameActions
{
    /// <summary>
    /// A game action that play a video
    /// </summary>
    public class PlayVideoGameAction : GameAction
    {
        /// <summary>
        /// Number of instances
        /// </summary>
        private static int instances;

        /// <summary>
        /// The music info
        /// </summary>
        private VideoInfo videoInfo;

        /// <summary>
        /// Video Player service.
        /// </summary>
        [RequiredService]
        private VideoPlayer videoPlayer = null;

        #region Properties
        #endregion

        #region Initialize
        /// <summary>
        /// Initializes a new instance of the <see cref="PlayVideoGameAction" /> class.
        /// </summary>
        /// <param name="videoInfo">The video info to play</param>
        /// <param name="scene">The associated scene.</param>
        public PlayVideoGameAction(VideoInfo videoInfo, Scene scene = null)
            : base("PlayVideoGameAction" + instances++, scene)
        {
            this.videoInfo = videoInfo;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PlayVideoGameAction" /> class.
        /// </summary>
        /// <param name="parent">The parent task.</param>
        /// <param name="videoInfo">The video info to play</param>
        public PlayVideoGameAction(IGameAction parent, VideoInfo videoInfo)
            : base(parent, "PlayVideoGameAction" + instances++)
        {
            this.videoInfo = videoInfo;
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Perform run action
        /// </summary>
        protected override void PerformRun()
        {
            this.videoPlayer.OnComplete += this.OnVideoCompleted;
            this.videoPlayer.Play(this.videoInfo);
        }

        /// <summary>
        /// The video has been completed
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event args.</param>
        private void OnVideoCompleted(object sender, EventArgs e)
        {
            this.videoPlayer.OnComplete -= this.OnVideoCompleted;
            this.PerformCompleted();
        }

        /// <summary>
        /// Perform cancelation event
        /// </summary>
        protected override void PerformCancel()
        {
            this.videoPlayer.OnComplete -= this.OnVideoCompleted;
            this.videoPlayer.Stop();

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
