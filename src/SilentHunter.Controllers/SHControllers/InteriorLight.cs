﻿/*
 * SHControllers.act - InteriorLight
 *
 * © 2007-2016 skwas. All rights reserved.
 * This code is provided as is. Change at your own risk.
 * --------------------------------------------------
 *
 * S3D template for the InteriorLight controller of Silent Hunter.
 *
 * For documentation on templates, requirements and restrictions, please refer to the documentation.
 *
 */

using SilentHunter;
using SilentHunter.Controllers;

namespace SHControllers
{
    /// <summary>
    /// InteriorLight action controller.
    /// </summary>
    public class InteriorLight : BehaviorController
    {
        /// <summary>
        /// Day light color.
        /// </summary>
        public Color DayLightColor;

        /// <summary>
        /// Night light color.
        /// </summary>
        public Color NightLightColor;
    }
}
