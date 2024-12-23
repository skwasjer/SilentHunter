/*
 * SHControllers.act - CityLightsR
 *
 * © 2007-2016 skwas. All rights reserved.
 * This code is provided as is. Change at your own risk.
 * --------------------------------------------------
 *
 * S3D template for the CityLightsR controller of Silent Hunter.
 *
 * For documentation on templates, requirements and restrictions, please refer to the documentation.
 *
 */

using SilentHunter.Controllers;

namespace SHControllers
{
    /// <summary>
    /// CityLightsR render controller.
    /// </summary>
    public class CityLightsR
        : BehaviorController
    {
        /// <summary>
        /// Light intensity for maximum city lights (0..1).
        /// </summary>
        public float NightIntensity;
        /// <summary>
        /// Light intensity for no city lights (0..1).
        /// </summary>
        public float DayIntensity;
    }
}
