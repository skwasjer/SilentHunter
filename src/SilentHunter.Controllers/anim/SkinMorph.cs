/*
 * anim.act - SkinMorph
 *
 * © 2007-2016 skwas. All rights reserved.
 * This code is provided as is. Change at your own risk.
 * --------------------------------------------------
 *
 * S3D template for the SkinMorph controller of Silent Hunter.
 *
 * For documentation on templates, requirements and restrictions, please refer to the documentation.
 *
 */

using System.Numerics;
using SilentHunter.Controllers;

namespace anim
{
    /// <summary>
    /// SkinMorph controller.
    /// </summary>
    public class SkinMorph
        : Controller
    {
        /// <summary>
        /// The translation part of the bone matrix.
        /// </summary>
        public Vector3 Translation;
        /// <summary>
        /// The rotation part of the bone matrix.
        /// </summary>
        public Quaternion Rotation;
        /// <summary>
        /// Id of root node of skeleton.
        /// </summary>
        public uint SkeletalRootId;
        /// <summary>
        /// Unknown (unverified), probably an array of frames vs weights/indices.
        /// </summary>
        public uint Unknown0;
    }
}
