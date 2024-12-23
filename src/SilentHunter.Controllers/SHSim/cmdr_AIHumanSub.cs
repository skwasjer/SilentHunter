/*
 * SHSim.act - cmdr_AIHumanSub
 *
 * © 2007-2016 skwas. All rights reserved.
 * This code is provided as is. Change at your own risk.
 * --------------------------------------------------
 *
 * S3D template for the cmdr_AIHumanSub controller of Silent Hunter.
 *
 * For documentation on templates, requirements and restrictions, please refer to the documentation.
 *
 */

using SilentHunter.Controllers;
using SilentHunter.Controllers.Decoration;

namespace SHSim
{
    /// <summary>
    /// cmdr_AIHumanSub controller.
    /// </summary>
    public class cmdr_AIHumanSub
        : BehaviorController
    {
        /// <summary>
        /// </summary>
        [Optional]
        public cmdr_AIShip cmdr_AIShip;
        /// <summary>
        /// </summary>
        [Optional]
        public cmdr_AIMerchant cmdr_AIMerchant;
    }
}
