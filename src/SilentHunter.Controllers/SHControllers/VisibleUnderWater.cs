/*
 * SHControllers.act - VisibleUnderWater
 *
 * © 2007-2016 skwas. All rights reserved.
 * This code is provided as is. Change at your own risk.
 * --------------------------------------------------
 *
 * S3D template for the VisibleUnderWater controller of Silent Hunter.
 *
 * For documentation on templates, requirements and restrictions, please refer to the documentation.
 *
 */

using SilentHunter.Controllers;

namespace SHControllers
{
    /// <summary>
    /// VisibleUnderWater action controller. Place it before other render controllers to stop them above water.
    /// </summary>
    public class VisibleUnderWater
        : BehaviorController
    {
        /// <summary>
        /// True for visible only under water and false for visible only above water.
        /// </summary>
        public bool Under;
    }
}
