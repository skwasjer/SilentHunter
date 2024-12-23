/*
 * SHControllers.act - FogParameters
 *
 * © 2007-2016 skwas. All rights reserved.
 * This code is provided as is. Change at your own risk.
 * --------------------------------------------------
 *
 * S3D template for the FogParameters controller of Silent Hunter.
 *
 * For documentation on templates, requirements and restrictions, please refer to the documentation.
 *
 */

using SilentHunter;
using SilentHunter.Controllers;

namespace SHControllers
{
    /// <summary>
    /// FogParameters user data.  Apply on submarine interior sectors to set the fog parameters.
    /// </summary>
    public class FogParameters : BehaviorController
    {
        /// <summary>
        /// Near distance in meters.
        /// </summary>
        public float NearDistance;

        /// <summary>
        /// Far distance in meters.
        /// </summary>
        public float FarDistance;

        /// <summary>
        /// Fog color.
        /// </summary>
        public Color Color;
    }
}
