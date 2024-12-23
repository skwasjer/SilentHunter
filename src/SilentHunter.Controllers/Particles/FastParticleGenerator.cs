/*
 * Particles.act - FastParticleGenerator
 *
 * © 2007-2016 skwas. All rights reserved.
 * This code is provided as is. Change at your own risk.
 * --------------------------------------------------
 *
 * S3D template for the FastParticleGenerator controller of Silent Hunter.
 *
 * For documentation on templates, requirements and restrictions, please refer to the documentation.
 *
 */

using System.Collections.Generic;
using SilentHunter;
using SilentHunter.Controllers;
using SilentHunter.Controllers.Decoration;

namespace Particles
{
    /// <summary>
    /// FastParticleGenerator render controller.
    /// </summary>
    public class FastParticleGenerator
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
        /// The maximum oncreen particle size in pixels. If 0 the default max size is used.
        /// </summary>
        public float MaxParticleSize;
        /// <summary>
        /// The scene ambient color is the ship wakecolor instead of the one computed from lights.
        /// </summary>
        public bool UseShipWakeColor;
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
        public List<FastBitmapParticles> BitmapParticles;
    }

    public class FastBitmapParticles
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
        /// Enable depth buffer test.
        /// </summary>
        public bool DepthTest;
        /// <summary>
        /// Particle movement: true = generator's space, false = in global space.
        /// </summary>
        public bool LocalMovement;
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
        public FastParticleSize Size;
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
        public FastParticleOpacity Opacity;
        /// <summary>
        /// Color evolution overlife.
        /// </summary>
        public List<FastColorOverLife> Color;
        /// <summary>
        /// Wind coefficient (0=notaffected by wind).
        /// </summary>
        public float WindCoef;
        /// <summary>
        /// Coefficient of particle's speed inherited from generator.
        /// </summary>
        public float InertiaCoef;
        /// <summary>
        /// Scene light scale factor for particle color
        /// </summary>
        public float GlobalColorScale;
    }

    public class FastParticleSize
    {
        /// <summary>
        /// Initial x size,in engine units.
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

    public class FastParticleOpacity
    {
        /// <summary>
        /// Particle initial opacity (0..1).
        /// </summary>
        public float Opacity;
        /// <summary>
        /// Initial opacity variation (0..1).
        /// </summary>
        public float Variation;
    }

    public struct FastColorOverLife
    {
        /// <summary>
        /// Life time (0..1).
        /// </summary>
        public float LifeTime;
        /// <summary>
        /// Particle color.
        /// </summary>
        public Color Color;
        /// <summary>
        /// Opacity.
        /// </summary>
        public float Opacity;
    }
}
