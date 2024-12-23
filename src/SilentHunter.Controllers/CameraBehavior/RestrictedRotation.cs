/*
 * CameraBehavior.act - RestrictedRotation
 *
 * © 2007-2016 skwas. All rights reserved.
 * This code is provided as is. Change at your own risk.
 * --------------------------------------------------
 *
 * S3D template for the RestrictedRotation controller of Silent Hunter.
 *
 * For documentation on templates, requirements and restrictions, please refer to the documentation.
 *
 */

using SilentHunter;
using SilentHunter.Controllers;

namespace CameraBehavior
{
    /// <summary>
    /// RestrictedRotation action controller.
    /// </summary>
    public class RestrictedRotation
        : BehaviorController
    {
        /// <summary>
        /// The azimuth parameters.
        /// </summary>
        public MinMax Azimunth;
        /// <summary>
        /// The elevation parameters.
        /// </summary>
        public MinMax Elevation;
        /// <summary>
        /// The object tight with parent. False means it keeps world Y axe.
        /// </summary>
        public bool Tight;
        /// <summary>
        /// The object can look at a target.
        /// </summary>
        public bool Targetable;
    }
}
