﻿/*
 * CameraBehavior.act - HideObjCfg
 *
 * © 2007-2016 skwas. All rights reserved.
 * This code is provided as is. Change at your own risk.
 * --------------------------------------------------
 *
 * S3D template for the HideObjCfg controller of Silent Hunter.
 *
 * For documentation on templates, requirements and restrictions, please refer to the documentation.
 *
 */

using SilentHunter.Controllers;
using SilentHunter.Controllers.Decoration;

namespace CameraBehavior
{
    /// <summary>
    /// HideObjCfg controller.
    /// </summary>
    public class HideObjCfg
        : BehaviorController
    {
        /// <summary>
        /// The HideObj parameters.
        /// </summary>
        public HideObj HideObj;
        /// <summary>
        /// The instance occurrence to be configured; 0 = all ocurrences.
        /// </summary>
        [Optional]
        public int? InstaceOccurence;
    }
}
