/*
 * EnvSim.act - Float
 *
 * © 2007-2016 skwas. All rights reserved.
 * This code is provided as is. Change at your own risk.
 * --------------------------------------------------
 *
 * S3D template for the Float controller of Silent Hunter.
 *
 * For documentation on templates, requirements and restrictions, please refer to the documentation.
 *
 */

using SilentHunter.Controllers;

namespace EnvSim
{
    /// <summary>
    /// Float controller. Keeps the Y-axis up and Y on the water surface.
    /// </summary>
    public class Float
        : BehaviorController
    {
        /// <summary>
        /// The object floats at this height above water.
        /// </summary>
        public float FloatingHeight;
        /// <summary>
        /// The angle between the ship front and the horizontal plane (in degrees).
        /// </summary>
        public float ShipFrontAngle;
    }
}
