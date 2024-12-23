/*
 * CameraBehavior.act - Optical
 *
 * © 2007-2016 skwas. All rights reserved.
 * This code is provided as is. Change at your own risk.
 * --------------------------------------------------
 *
 * S3D template for the Optical controller of Silent Hunter.
 *
 * For documentation on templates, requirements and restrictions, please refer to the documentation.
 *
 */

using System.Collections.Generic;
using SilentHunter.Controllers;

namespace CameraBehavior
{
    /// <summary>
    /// Optical controller.
    /// </summary>
    public class Optical
        : BehaviorController
    {
        /// <summary>
        /// Minimum zoom value.
        /// </summary>
        public float MinZoom;
        /// <summary>
        /// Maximum zoom value.
        /// </summary>
        public float MaxZoom;
        /// <summary>
        /// Zoom levels.
        /// </summary>
        public List<float> ZoomLevels;
    }
}
