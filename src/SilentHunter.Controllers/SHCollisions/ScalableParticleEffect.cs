/*
 * SHCollisions.act - ScalableParticleEffect
 *
 * © 2007-2016 skwas. All rights reserved.
 * This code is provided as is. Change at your own risk.
 * --------------------------------------------------
 *
 * S3D template for the ScalableParticleEffect controller of Silent Hunter.
 *
 * For documentation on templates, requirements and restrictions, please refer to the documentation.
 *
 */

using Particles;
using SilentHunter.Controllers;

namespace SHCollisions
{
    /// <summary>
    /// ScalableParticleEffect user data.
    /// </summary>
    public class ScalableParticleEffect
        : BehaviorController
    {
        /// <summary>
        /// Emission area (circular) radius and solid angle. Direction is along generator's Y axis (up).
        /// </summary>
        public EmissionArea EmissionArea;
        /// <summary>
        /// Global scale coefficients for all subparticles.
        /// </summary>
        public GlobalScale GlobalScale;
        /// <summary>
        /// The new target to be moved when scale=1. It MUST be brother of generator object!
        /// </summary>
        public ulong MoveTo;
        /// <summary>
        /// The coef to be applied on the parameters. [0..1]
        /// </summary>
        public float Scale;
    }
}
