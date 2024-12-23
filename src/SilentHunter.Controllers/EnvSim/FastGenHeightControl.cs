/*
 * EnvSim.act - FastGenHeightControl
 *
 * © 2007-2016 skwas. All rights reserved.
 * This code is provided as is. Change at your own risk.
 * --------------------------------------------------
 *
 * S3D template for the FastGenHeightControl controller of Silent Hunter.
 *
 * For documentation on templates, requirements and restrictions, please refer to the documentation.
 *
 */

using SilentHunter.Controllers;

namespace EnvSim
{
    /// <summary>
    /// FastGenHeightControl render controller. Starts/Stops the ParticleGenerators depending on the height.
    /// </summary>
    public class FastGenHeightControl
        : BehaviorController
    {
        /// <summary>
        /// Under this limit, the particles generator is stopped.
        /// </summary>
        public float Min;
        /// <summary>
        /// Over this limit, the particles generator is stopped.
        /// </summary>
        public float Max;
    }
}
