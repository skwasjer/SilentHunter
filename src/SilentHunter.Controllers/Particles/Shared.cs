/*
 * Particles.act - Shared generator definitions.
 *
 * © 2007-2016 skwas. All rights reserved.
 * This code is provided as is. Change at your own risk.
 * --------------------------------------------------
 *
 * S3D template for the particle controllers of Silent Hunter.
 *
 * For documentation on templates, requirements and restrictions, please refer to the documentation.
 *
 */

using System.Collections.Generic;
using System.Numerics;
using SilentHunter.Controllers;
using SilentHunter.Controllers.Decoration;

namespace Particles
{
    public struct EmissionArea
    {
        /// <summary>
        /// Radius.
        /// </summary>
        public float Radius;
        /// <summary>
        /// Angle in degrees.
        /// </summary>
        public float SolidAngle;
    }

    public enum RunMode
    {
        Stop,
        Loop,
        Infinity
    }

    public class Wind
    {
        /// <summary>
        /// Windspeed in generator's space.
        /// </summary>
        public Vector3 LocalWind;
        /// <summary>
        /// Windspeed in global space.
        /// </summary>
        public Vector3 GlobalWind;
    }

    /// <summary>
    /// Global scale coefficients for near subparticles.
    /// </summary>
    public class GlobalScale
    {
        /// <summary>
        /// Near distance value (as first LOD) in meters.
        /// </summary>
        [Optional]
        public float? Distance;
        /// <summary>
        /// Particle life scale.
        /// </summary>
        public float LifeScale;
        /// <summary>
        /// Global density (number) scale.
        /// </summary>
        public float DensityScale;
        /// <summary>
        /// Global velocity scale.
        /// </summary>
        public float VelocityScale;
        /// <summary>
        /// Global weight scale.
        /// </summary>
        public float WeightScale;
        /// <summary>
        /// Global size scale.
        /// </summary>
        public float SizeScale;
        /// <summary>
        /// Global spin scale.
        /// </summary>
        [Optional]
        public float? SpinScale;
        /// <summary>
        /// Global opacity scale.
        /// </summary>
        [Optional]
        public float? OpacityScale;
    }
    /*
        /// <summary>
        /// Global scale coefficients for near subparticles.
        /// </summary>

        public class GlobalScale2
        {
            /// <summary>
            /// Near distance value (as first LOD) in meters.
            /// </summary>
            [Optional]
            public Nullable<float> Distance;
            /// <summary>
            /// Particle life scale.
            /// </summary>
            public float LifeScale;
            /// <summary>
            /// Global density (number) scale.
            /// </summary>
            public float DensityScale;
            /// <summary>
            /// Global velocity scale.
            /// </summary>
            public float VelocityScale;
            /// <summary>
            /// Global weight scale.
            /// </summary>
            public float WeightScale;
            /// <summary>
            /// Global size scale.
            /// </summary>
            public float SizeScale;
            /// <summary>
            /// Global opacity scale.
            /// </summary>
            public float OpacityScale;
        }*/

    /// <summary>
    /// Global scale coefficients for far subparticles.
    /// </summary>
    public class GlobalScaleFar
        : BehaviorController
    {
        /// <summary>
        /// Far distance value (as first LOD) in meters.
        /// </summary>
        public float Distance;
        /// <summary>
        /// Global density (number) scale.
        /// </summary>
        public float DensityScale;
        /// <summary>
        /// Global opacity scale.
        /// </summary>
        public float OpacityScale;
    }

    public class Creation
    {
        /// <summary>
        /// Particles per second (PPS)
        /// </summary>
        public float Rate;
        /// <summary>
        /// Generation interval variation (+/- random percent from current rate) (0..1)
        /// </summary>
        public float Variation;
        /// <summary>
        /// Rate evolution over generator's life (linear interpolation).
        /// </summary>
        public List<OverLife> OverLife;
    }

    public class ParticleVelocity
    {
        /// <summary>
        /// Initial velocity direction.
        /// </summary>
        public Vector3 SpeedDirection;
        /// <summary>
        /// Initial velocity, in engine units/second.
        /// </summary>
        public float Velocity;
        /// <summary>
        /// Initial velocity variation.
        /// </summary>
        public float Variation;
        /// <summary>
        /// Velocity evolution over generator's life (linear interpolation).
        /// </summary>
        public List<OverLife> OverLife;
    }

    public class ParticleWeight
    {
        /// <summary>
        /// Initial medium weight, in Kg.
        /// </summary>
        public float Weight;
        /// <summary>
        /// Initial weight variation, in Kg.
        /// </summary>
        public float Variation;
        /// <summary>
        /// Weight evolution over generator's life (linear interpolation).
        /// </summary>
        public List<OverLife> OverLife;
    }

    public class ParticleSpin
    {
        /// <summary>
        /// Initial rotation in degrees (not used icw. BitmapParticles - Alignement='LookAt').
        /// </summary>
        public Vector3 InitialRot;
        /// <summary>
        /// Initial spin axis.
        /// </summary>
        public Vector3 SpinAxis;
        /// <summary>
        /// Initial spin angle (deg) around spin axis.
        /// </summary>
        public float SpinAngle;
        /// <summary>
        /// Initial spin variation (deg).
        /// </summary>
        public float Variation;
        /// <summary>
        /// Spin evolution over generator's life (linear interpolation).
        /// </summary>
        public List<OverLife> OverLife;
    }

    public struct OverLife
    {
        /// <summary>
        /// Life time (0..1).
        /// </summary>
        public float LifeTime;
        /// <summary>
        /// Size scale factor.
        /// </summary>
        public float Scale;
    }

    public enum BlendMode
    {
        Alpha,
        Add,
        Multiply
    }

    public struct ParticleLife
    {
        /// <summary>
        /// Particle's medium life, in seconds.
        /// </summary>
        public float Life;
        /// <summary>
        /// Variation (in seconds).
        /// </summary>
        public float Variation;
    }

    public class SequenceParameters
    {
        /// <summary>
        /// Run mode for sequence.
        /// </summary>
        public Run Run;
        /// <summary>
        /// Start frame.
        /// </summary>
        public int StartFrame;
        /// <summary>
        /// Initial play direction.
        /// </summary>
        public bool Forward;
        /// <summary>
        /// Total number of frames in bitmap.
        /// </summary>
        public int Frames;
        /// <summary>
        /// Number of horizontal tiles.
        /// </summary>
        public int XTiles;
        /// <summary>
        /// Number of vertical tiles.
        /// </summary>
        public int YTiles;
        /// <summary>
        /// Frames per second.
        /// </summary>
        public float FPS;
    }

    public enum Run
    {
        Loop,
        Reflect,
        Stop
    }

    public class RandomMotion
    {
        /// <summary>
        /// Random time interval between speed changes.
        /// </summary>
        public MinMax RandomInterval;
        /// <summary>
        /// Perturbation speed dispersion (direction is random).
        /// </summary>
        public MinMax Dispersion;
        /// <summary>
        /// Renormalize speed after perturbation.
        /// </summary>
        public bool Renormalize;
    }

    /// <summary>
    /// </summary>
    /// <remarks>Note: this type is not the same as the MinMax type declared by the SilentHunter.Core library.</remarks>
    public struct MinMax
    {
        public float Min;
        public float Max;
    }

    public enum BPAlignement
    {
        LookAtCamera,
        MotionAligned,
        WorldAligned,
        Random
    }

    public enum OPAlignement
    {
        MotionAligned = 1,
        WorldAligned,
        Random
    }

    public class ParticleSize
    {
        /// <summary>
        /// Aspect ratio (y/x).
        /// </summary>
        public float AspectRatio;
        /// <summary>
        /// Initial x size, in engine units.
        /// </summary>
        public float Size;
        /// <summary>
        /// Creation size variation.
        /// </summary>
        public float Variation;
        /// <summary>
        /// Size evolution over generator's life (linear interpolation).
        /// </summary>
        public List<OverLife> OverLife;
    }
}
