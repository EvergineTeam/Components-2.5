#region File Description
//-----------------------------------------------------------------------------
// DefaultTransitions
//
// Copyright © 2017 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaveEngine.Common.Graphics;
using WaveEngine.Framework.Services;
#endregion

namespace WaveEngine.Components.Transitions
{
    /// <summary>
    /// Provided fast creation to all default transitions
    /// </summary>
    public static class DefaultTransitions
    {
        /// <summary>
        /// Easy access to create a new <see cref="ChequeredAppearTransition"/>.
        /// </summary>
        /// <param name="duration">The transitino duration.</param>
        /// <returns>A new instance of ChequeredAppearTransition.</returns>
        public static ScreenTransition ChequeredAppear(TimeSpan duration)
        {
            return new ChequeredAppearTransition(duration);
        }

        /// <summary>
        /// Easy access to create a new <see cref="ColorFadeTransition"/>.
        /// </summary>
        /// <param name="transitionColor">Color of the transition.</param>
        /// <param name="duration">The duration.</param>
        /// <returns>A new instance of ColorFaceTransition.</returns>
        public static ScreenTransition ColorFade(Color transitionColor, TimeSpan duration)
        {
            return new ColorFadeTransition(transitionColor, duration);
        }

        /// <summary>
        /// Easy access to create a new <see cref="CombTransition"/>.
        /// </summary>
        /// <param name="duration">The duration.</param>
        /// <param name="effect">The effect.</param>
        /// <returns>A new instance of CombTransition.</returns>
        public static ScreenTransition Comb(TimeSpan duration, CombTransition.EffectOptions effect)
        {
            return new CombTransition(duration, effect);
        }

        /// <summary>
        /// Easy access to create a new <see cref="CoverTransition"/>.
        /// </summary>
        /// <param name="duration">The duration.</param>
        /// <param name="effect">The effect.</param>
        /// <returns>A new instance of CoverTransition.</returns>
        public static ScreenTransition Cover(TimeSpan duration, CoverTransition.EffectOptions effect)
        {
            return new CoverTransition(duration, effect);
        }

        /// <summary>
        /// Easy access to create a new <see cref="CrossFadeTransition"/>.
        /// </summary>
        /// <param name="duration">The duration.</param>
        /// <returns>A new instance of CrossFaceTransition.</returns>
        public static ScreenTransition CrossFade(TimeSpan duration)
        {
            return new CrossFadeTransition(duration);
        }

        /// <summary>
        /// Easy access to create a new <see cref="CurtainsTransition"/>.
        /// </summary>
        /// <param name="duration">The duration.</param>
        /// <returns>A new instance of CurtainsTransition.</returns>
        public static ScreenTransition Curtains(TimeSpan duration)
        {
            return new CurtainsTransition(duration);
        }

        /// <summary>
        /// Easy access to create a new <see cref="DoorwayTransition"/>.
        /// </summary>
        /// <param name="duration">The duration.</param>
        /// <returns>A new instance of DoorwayTransition.</returns>
        public static ScreenTransition Doorway(TimeSpan duration)
        {
            return new DoorwayTransition(duration);
        }

        /// <summary>
        /// Easy access to create a new <see cref="FallingLinesTransition"/>.
        /// </summary>
        /// <param name="duration">The duration.</param>
        /// <returns>A new instance of FallingLinesTransition.</returns>
        public static ScreenTransition FallingLines(TimeSpan duration)
        {
            return new FallingLinesTransition(duration);
        }

        /// <summary>
        /// Easy access to create a new <see cref="FanTransition"/>.
        /// </summary>
        /// <param name="duration">The duration.</param>
        /// <returns>A new instance of FanTransition.</returns>
        public static ScreenTransition Fan(TimeSpan duration)
        {
            return new FanTransition(duration);
        }

        /// <summary>
        /// Easy access to create a new <see cref="PushTransition"/>.
        /// </summary>
        /// <param name="duration">The duration.</param>
        /// <param name="effect">The effect.</param>
        /// <returns>A new instance of PushTransition.</returns>
        public static ScreenTransition Push(TimeSpan duration, PushTransition.EffectOptions effect)
        {
            return new PushTransition(duration, effect);
        }

        /// <summary>
        /// Easy access to create a new <see cref="RotateTransition"/>.
        /// </summary>
        /// <param name="duration">The duration.</param>
        /// <returns>A new instance of RotateTransition.</returns>
        public static ScreenTransition Rotate(TimeSpan duration)
        {
            return new RotateTransition(duration);
        }

        /// <summary>
        /// Easy access to create a new <see cref="ScaleTransition"/>.
        /// </summary>
        /// <param name="duration">The duration.</param>
        /// <returns>A new instance of ScaleTransition.</returns>
        public static ScreenTransition Scale(TimeSpan duration)
        {
            return new ScaleTransition(duration);
        }

        /// <summary>
        /// Easy access to create a new <see cref="ShrinkAndSpinTransition"/>.
        /// </summary>
        /// <param name="duration">The duration.</param>
        /// <returns>A new instance of ShrinkAndSpinTransition.</returns>
        public static ScreenTransition ShrinkAndSpin(TimeSpan duration)
        {
            return new ShrinkAndSpinTransition(duration);
        }

        /// <summary>
        /// Easy access to create a new <see cref="SpinningSquaresTransition"/>.
        /// </summary>
        /// <param name="duration">The duration.</param>
        /// <returns>A new of SpinningSquaresTransition.</returns>
        public static ScreenTransition SpinningSquares(TimeSpan duration)
        {
            return new SpinningSquaresTransition(duration);
        }

        /// <summary>
        /// Easy access to create a new <see cref="UncoverTransition"/>.
        /// </summary>
        /// <param name="duration">The duration.</param>
        /// <param name="effect">The effect.</param>
        /// <returns>A new instance of UncoverTransition.</returns>
        public static ScreenTransition Uncover(TimeSpan duration, UncoverTransition.EffectOptions effect)
        {
            return new UncoverTransition(duration, effect);
        }

        /// <summary>
        /// Easy access to create a new <see cref="ZoomTransition"/>.
        /// </summary>
        /// <param name="duration">The duration.</param>
        /// <returns>A new instance of ZoomTransition.</returns>
        public static ScreenTransition Zoom(TimeSpan duration)
        {
            return new ZoomTransition(duration);
        }
    }
}
