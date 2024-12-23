/*
 * SHControllers.act - Label
 *
 * © 2007-2016 skwas. All rights reserved.
 * This code is provided as is. Change at your own risk.
 * --------------------------------------------------
 *
 * S3D template for the Label controller of Silent Hunter.
 *
 * For documentation on templates, requirements and restrictions, please refer to the documentation.
 *
 */

using SilentHunter.Controllers;
using SilentHunter.Controllers.Decoration;

namespace SHControllers
{
    /// <summary>
    /// Label action controller.
    /// </summary>
    public class Label
        : BehaviorController
    {
        /// <summary>
        /// Object's name/label.
        /// </summary>
        [ParseName("Label")]
        public string Name;
    }
}
