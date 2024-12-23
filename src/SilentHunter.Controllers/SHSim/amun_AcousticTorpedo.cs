/*
 * SHSim.act - amun_AcousticTorpedo
 *
 * © 2007-2016 skwas. All rights reserved.
 * This code is provided as is. Change at your own risk.
 * --------------------------------------------------
 *
 * S3D template for the amun_AcousticTorpedo controller of Silent Hunter.
 *
 * For documentation on templates, requirements and restrictions, please refer to the documentation.
 *
 */

using SilentHunter.Controllers;

namespace SHSim
{
    /// <summary>
    /// amun_AcousticTorpedo controller.
    /// </summary>
    public class amun_AcousticTorpedo
        : BehaviorController
    {
        /// <summary>
        /// General torpedo settings.
        /// </summary>
        public amun_Torpedo amun_Torpedo;
        /// <summary>
        /// Torpedo's straight run distance [m].
        /// </summary>
        public float straight_run;
    }
}
