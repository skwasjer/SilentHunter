/*
 * SHControllers.act - ConfigureBumpMap
 *
 * © 2007-2016 skwas. All rights reserved.
 * This code is provided as is. Change at your own risk.
 * --------------------------------------------------
 *
 * S3D template for the ConfigureBumpMap controller of Silent Hunter.
 *
 * For documentation on templates, requirements and restrictions, please refer to the documentation.
 *
 */

using SilentHunter.Controllers;

namespace SHControllers
{
    /// <summary>
    /// ConfigureBumpMap user data.
    /// </summary>
    public class ConfigureBumpMap
        : BehaviorController
    {
        /// <summary>
        /// Bump map parameters for body.
        /// </summary>
        public Blend Body;
        /// <summary>
        /// Bump map parameters for head.
        /// </summary>
        public Blend Head;
        /// <summary>
        /// Bump map parameters for eyes.
        /// </summary>
        public Blend Eyes;
        /// <summary>
        /// Bump map parameters for helmet.
        /// </summary>
        public Blend Helm;
    }
}
