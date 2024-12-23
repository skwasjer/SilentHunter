/*
 * SHControllers.act - ShellUserData
 *
 * © 2007-2016 skwas. All rights reserved.
 * This code is provided as is. Change at your own risk.
 * --------------------------------------------------
 *
 * S3D template for the ShellUserData controller of Silent Hunter.
 *
 * For documentation on templates, requirements and restrictions, please refer to the documentation.
 *
 */

using SilentHunter.Controllers;

namespace SHControllers
{
    /// <summary>
    /// ShellUserData controller.
    /// </summary>
    public class ShellUserData
        : BehaviorController
    {
        /// <summary>
        /// ShellManager will be linked by this object.
        /// </summary>
        public GunPos GunPos;
    }

    public enum GunPos
    {
        None = -1,
        DeckGun,
        PortGun,
        StarbGun,
        MainFlak,
        MainFlak1,
        MainFlak2
    }
}
