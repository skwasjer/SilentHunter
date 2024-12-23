/*
 * SHControllers.act - ShellUserDataCfg
 *
 * © 2007-2016 skwas. All rights reserved.
 * This code is provided as is. Change at your own risk.
 * --------------------------------------------------
 *
 * S3D template for the ShellUserDataCfg controller of Silent Hunter.
 *
 * For documentation on templates, requirements and restrictions, please refer to the documentation.
 *
 */

using SilentHunter.Controllers;
using SilentHunter.Controllers.Decoration;

namespace SHControllers
{
    /// <summary>
    /// ShellUserDataCfg controller.
    /// </summary>
    public class ShellUserDataCfg
        : BehaviorController
    {
        /// <summary>
        /// ShellUserData parameters.
        /// </summary>
        public ShellUserData ShellUserData;
        /// <summary>
        /// The instance occurrence to be configured; 0 = all ocurrences.
        /// </summary>
        [Optional]
        public int? InstaceOccurence;
    }
}
