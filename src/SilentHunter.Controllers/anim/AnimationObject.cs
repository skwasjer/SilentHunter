/*
 * anim.act - AnimationObject
 *
 * © 2007-2016 skwas. All rights reserved.
 * This code is provided as is. Change at your own risk.
 * --------------------------------------------------
 *
 * S3D template for the AnimationObject controller of Silent Hunter.
 *
 * For documentation on templates, requirements and restrictions, please refer to the documentation.
 *
 */

using System.Collections.Generic;
using SilentHunter.Controllers;
using SilentHunter.Controllers.Decoration;

namespace anim
{
    /// <summary>
    /// AnimationObject controller.
    /// </summary>
    [Controller(0x0)]
    public class AnimationObject
        : Controller
    {
        public string AnimName;
        public List<AnimationSubObject> Animations;
    }

    public class AnimationSubObject
    {
        public ulong Id;
        public string AnimName;
    }
}
