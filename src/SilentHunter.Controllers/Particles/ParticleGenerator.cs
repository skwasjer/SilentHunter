/*
 * Particles.act - ParticleGenerator
 *
 * © 2007-2016 skwas. All rights reserved.
 * This code is provided as is. Change at your own risk.
 * --------------------------------------------------
 *
 * S3D template for the ParticleGenerator controller of Silent Hunter.
 *
 * For documentation on templates, requirements and restrictions, please refer to the documentation.
 *
 */

using System.Collections.Generic;
using System.Numerics;
using SilentHunter;
using SilentHunter.Controllers;
using SilentHunter.Controllers.Decoration;

namespace Particles
{
    /// <summary>
    /// ParticleGenerator render controller.
    /// </summary>
    public class ParticleGenerator
        : BehaviorController
    {
        /// <summary>
        /// Density modifier (game menu) is active or not.
        /// </summary>
        [Optional]
        public bool? IsDensityModifierActive;
        /// <summary>
        /// Generator's bounding sphere radius (approx), for frustum test.
        /// </summary>
        [Optional]
        public float? SphereRadius;
        /// <summary>
        /// Emission area(circular)radius and solidangle. Direction is along generator's Y axis (up).
        /// </summary>
        public EmissionArea EmissionArea;
        /// <summary>
        /// Life time for the generator, in seconds. The generator is destroyed when all particles are destroyed.
        /// </summary>
        public float LifeTime;
        /// <summary>
        /// No delete children when destroy theowner object.
        /// </summary>
        public bool NoDeleteChildren;
        /// <summary>
        /// Run mode for the generator.
        /// </summary>
        public RunMode RunMode;
        /// <summary>
        /// Use transparency sorting.
        /// </summary>
        public bool Transparent;
        /// <summary>
        /// Depth bias for transparency sorting.
        /// </summary>
        public float DepthBias;
        /// <summary>
        /// Don't make action when system mature and not in frustum. System is considered mature after the first particle death.
        /// </summary>
        [Optional]
        public bool? NoActionMatureOutOfFrustum;
        /// <summary>
        /// Wind speed, local and global.
        /// </summary>
        public Wind Wind;
        /// <summary>
        /// Global scale coefficients for near subparticles.
        /// </summary>
        public GlobalScale GlobalScale;
        /// <summary>
        /// Global scale coefficients for far subparticles.
        /// </summary>
        [Optional]
        public GlobalScaleFar GlobalScaleFar;
        /// <summary>
        /// List with bitmap particles.
        /// </summary>
        public List<BitmapParticles> BitmapParticles;
        /// <summary>
        /// List with object particles.
        /// </summary>
        public List<ObjectParticles> ObjectParticles;
    }

    public class BitmapParticles
    {
        /// <summary>
        /// Particle name.
        /// </summary>
        [FixedString(16)]
        public string Name;
        /// <summary>
        /// Report the maximum number of particles used.
        /// </summary>
        public bool Report;
        /// <summary>
        /// Blending mode.
        /// </summary>
        public BlendMode Blending;
        /// <summary>
        /// Enable depth buffer write.
        /// </summary>
        public bool DepthWrite;
        /// <summary>
        /// Enable depth buffer test.
        /// </summary>
        public bool DepthTest;
        /// <summary>
        /// Alignment mode.
        /// </summary>
        public BPAlignement Alignement;
        /// <summary>
        /// Particle's -Z axis direction for 'World' aligned particles. For 'Motion' aligned, -Z is speed aligned.
        /// </summary>
        public Vector3 Direction;
        /// <summary>
        /// Bitmap reference point. (0,0)=down left.
        /// </summary>
        public Vector2 RefPoint;
        /// <summary>
        /// Particle movement: true = generator's space, false = in global space.
        /// </summary>
        public bool LocalMovement;
        /// <summary>
        /// Parameters for bitmap sequences.
        /// </summary>
        public SequenceParameters SequenceParameters;
        /// <summary>
        /// Particle material.
        /// </summary>
        public ulong Material;
        /// <summary>
        /// Maximum number of particles.
        /// </summary>
        public int MaxParticles;
        /// <summary>
        /// Particle life parameters.
        /// </summary>
        public ParticleLife Life;
        /// <summary>
        /// Particle creation parameters.
        /// </summary>
        public Creation Creation;
        /// <summary>
        /// Parameters for particle size.
        /// </summary>
        public ParticleSize Size;
        /// <summary>
        /// Parameters for particle velocity.
        /// </summary>
        public ParticleVelocity Velocity;
        /// <summary>
        /// Parameters for particle weight.
        /// </summary>
        public ParticleWeight Weight;
        /// <summary>
        /// Opacity parameters.
        /// </summary>
        public ParticleOpacity Opacity;
        /// <summary>
        /// Spin parameters.
        /// </summary>
        public ParticleSpin Spin;
        /// <summary>
        /// Color evolution overlife.
        /// </summary>
        public List<ColorOverLife> Color;
        /// <summary>
        /// Parameters for speed randomization.
        /// </summary>
        public RandomMotion RandomMotion;
        /// <summary>
        /// Wind coefficient (0=notaffected by wind).
        /// </summary>
        public float WindCoef;
        /// <summary>
        /// Coefficient of particle's speed inherited from generator.
        /// </summary>
        public float InertiaCoef;
        /// <summary>
        /// Scene light scale for particle color. (( I = ((&lt;factor&gt;+1)*Light-1)*&lt;Scale&gt;+1 )
        /// </summary>
        public float GlobalColorScale;
        /// <summary>
        /// Scene light factor for particle color. (( I = ((&lt;factor&gt;+1)*Light-1)*&lt;Scale&gt;+1 )
        /// </summary>
        [Optional]
        public float? GlobalColorFactor;
    }

    public class ParticleOpacity
    {
        /// <summary>
        /// Particle initial opacity (0..1).
        /// </summary>
        public float Opacity;
        /// <summary>
        /// Initial opacity variation (0..1).
        /// </summary>
        public float Variation;
        /// <summary>
        /// Opacity evolution over generator's life (linear interpolation).
        /// </summary>
        public List<OverLife> OverLife;
    }

    public class ObjectParticles
    {
        /// <summary>
        /// Particle name.
        /// </summary>
        [FixedString(16)]
        public string Name;
        /// <summary>
        /// Report the maximum number of particles used.
        /// </summary>
        public bool Report;
        /// <summary>
        /// 3D object reference.
        /// </summary>
        public ulong Object;
        /// <summary>
        /// Alignment mode.
        /// </summary>
        public OPAlignement Alignement;
        /// <summary>
        /// Particle's Z axis direction for 'World' aligned particles. For 'Motion' aligned, Z is speed aligned.
        /// </summary>
        public Vector3 Direction;
        /// <summary>
        /// Maximum number of particles.
        /// </summary>
        public int MaxParticles;
        /// <summary>
        /// Particle life parameters.
        /// </summary>
        public ParticleLife Life;
        /// <summary>
        /// Particle creation parameters.
        /// </summary>
        public Creation Creation;
        /// <summary>
        /// Parameters for particle velocity.
        /// </summary>
        public ParticleVelocity Velocity;
        /// <summary>
        /// Parameters for particle weight.
        /// </summary>
        public ParticleWeight Weight;
        /// <summary>
        /// Spin parameters (rotation around Z-axis).
        /// </summary>
        public ParticleSpin Spin;
        /// <summary>
        /// Wind coefficient (0=notaffected by wind).
        /// </summary>
        public float WindCoef;
        /// <summary>
        /// Coefficient of particle's speed inherited from generator.
        /// </summary>
        public float InertiaCoef;
    }

    public struct ColorOverLife
    {
        /// <summary>
        /// Life time (0..1).
        /// </summary>
        public float LifeTime;
        /// <summary>
        /// Particle color.
        /// </summary>
        public Color Color;
    }
}
