/*
 * EffectManager.act - TextureChanger
 *
 * © 2007-2016 skwas. All rights reserved.
 * This code is provided as is. Change at your own risk.
 * --------------------------------------------------
 *
 * S3D template for the TextureChanger controller of Silent Hunter.
 *
 * For documentation on templates, requirements and restrictions, please refer to the documentation.
 *
 */

using SilentHunter.Controllers;

namespace SH3.EffectManager
{
    /// <summary>
    /// TextureChanger controller. Changes texture properties on object materials.
    /// </summary>
    public class TextureChanger
        : BehaviorController
    {
        /// <summary>
        /// The texture channel to apply on.
        /// </summary>
        public int Channel;
        /// <summary>
        /// The offset to apply on X.
        /// </summary>
        public float OffsetX;
        /// <summary>
        /// The offset to apply on Y.
        /// </summary>
        public float OffsetY;
    }
}
