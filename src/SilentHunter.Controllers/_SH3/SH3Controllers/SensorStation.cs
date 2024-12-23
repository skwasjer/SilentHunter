/*
 * SH3Controllers.act - SensorStation
 *
 * © 2007-2016 skwas. All rights reserved.
 * This code is provided as is. Change at your own risk.
 * --------------------------------------------------
 *
 * S3D template for the SensorStation controller of Silent Hunter.
 *
 * For documentation on templates, requirements and restrictions, please refer to the documentation.
 *
 */

using SilentHunter.Controllers;

namespace SH3.SH3Controllers
{
    /// <summary>
    /// SensorStation action controller.
    /// </summary>
    public class SensorStation
        : BehaviorController
    {
        /// <summary>
        /// The sensor type.
        /// </summary>
        public int SensorType;
    }
}
