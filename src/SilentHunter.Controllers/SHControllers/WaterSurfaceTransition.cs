/*
 * SHControllers.act - WaterSurfaceTransition
 *
 * © 2007-2016 skwas. All rights reserved.
 * This code is provided as is. Change at your own risk.
 * --------------------------------------------------
 *
 * S3D template for the WaterSurfaceTransition controller of Silent Hunter.
 *
 * For documentation on templates, requirements and restrictions, please refer to the documentation.
 *
 */

using SilentHunter.Controllers;

namespace SHControllers
{
    /// <summary>
    /// WaterSurfaceTransition action controller.
    /// </summary>
    public class WaterSurfaceTransition
        : BehaviorController
    {
        /// <summary>
        /// True if the controller starts all particle generators when the object enter in the water (depth becomes negative) or false if they start when exiting the water.
        /// </summary>
        public bool Diving;
        /// <summary>
        /// True if the controller uses real wave height to check for above or below water instead of using the depth height.
        /// </summary>
        public bool UsePreciseWaterHeight;
        /// <summary>
        /// Depth used on transition (works only when not using precise water height);
        /// </summary>
        public float Depth;
    }
}
