/*
 * SHControllers.act - WaterInteraction
 *
 * © 2007-2016 skwas. All rights reserved.
 * This code is provided as is. Change at your own risk.
 * --------------------------------------------------
 *
 * S3D template for the WaterInteraction controller of Silent Hunter.
 *
 * For documentation on templates, requirements and restrictions, please refer to the documentation.
 *
 */

using SilentHunter.Controllers;

namespace SHControllers
{
    /// <summary>
    /// WaterInteraction action controller.
    /// </summary>
    public class WaterInteraction
        : BehaviorController
    {
        /// <summary>
        /// Stop all particle generators when the object enters the water (Y becomes negative).
        /// </summary>
        public bool StopAllParticles;
        /// <summary>
        /// The object to create.
        /// </summary>
        public ulong CreateObject;
        /// <summary>
        /// Link the new created object to world (instead of generator's parent).
        /// </summary>
        public bool WorldLink;
        /// <summary>
        /// Depth interaction.
        /// </summary>
        public float Depth;
        /// <summary>
        /// Delete owner object at interaction.
        /// </summary>
        public bool DeleteMe;
    }
}
