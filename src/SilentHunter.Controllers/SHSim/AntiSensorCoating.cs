/*
 * SHSim.act - AntiSensorCoating
 *
 * © 2007-2016 skwas. All rights reserved.
 * This code is provided as is. Change at your own risk.
 * --------------------------------------------------
 *
 * S3D template for the AntiSensorCoating controller of Silent Hunter.
 *
 * For documentation on templates, requirements and restrictions, please refer to the documentation.
 *
 */

using SilentHunter.Controllers;

namespace SHSim
{
    /// <summary>
    /// AntiSensorCoating controller.
    /// </summary>
    public class AntiSensorCoating
        : BehaviorController
    {
        /// <summary>
        /// The radar signal reduction (percent)
        /// </summary>
        public float RadarReduction;
        /// <summary>
        /// The sonar signal reduction (percent)
        /// </summary>
        public float SonarReduction;
        /// <summary>
        /// The suffix for the replacement submarine textures (e.g. "_coating")
        /// </summary>
        public string TextureSuffix;
    }
}
