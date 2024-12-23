/*
 * SHSound.act - SonarTarget
 *
 * © 2007-2016 skwas. All rights reserved.
 * This code is provided as is. Change at your own risk.
 * --------------------------------------------------
 *
 * S3D template for the SonarTarget controller of Silent Hunter.
 *
 * For documentation on templates, requirements and restrictions, please refer to the documentation.
 *
 */

using SilentHunter.Controllers;

namespace SHSound
{
    /// <summary>
    /// SonarTarget controller.
    /// </summary>
    public class SonarTarget
        : BehaviorController
    {
        /// <summary>
        /// Identifier of the sound to use in sonar mode.
        /// </summary>
        public string SoundIdentifier;
    }
}
