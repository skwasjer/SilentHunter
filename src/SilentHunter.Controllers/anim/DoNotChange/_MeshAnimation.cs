/*
 * © 2007-2016 skwas. All rights reserved.
 * This code is provided as is. Change at your own risk.
 * --------------------------------------------------
 *
 * S3D template for SH-controllers that contain animation data.
 *
 * WARNING: DO NOT CHANGE THE TYPE/CLASS NAMES. I HAD TO HARDCODE A REFERENCE TO THEM.
 * Fields and descriptions may be changed though.
 *
 */

using SilentHunter.Controllers;
using SilentHunter.Controllers.Decoration;

namespace anim
{
    /// <summary>
    /// Mesh animation controller data (subtype 0x5). Animation of vertices.
    /// </summary>
    [Controller(0x5)]
    public class MeshAnimationData : MeshAnimationController
    {
    }

    /// <summary>
    /// Mesh animation controller data (subtype 0x8005). Animation of vertices.
    /// </summary>
    [Controller(0x8005)]
    public class MeshAnimationData2 : MeshAnimationController
    {
    }

    /// <summary>
    /// Texture animation controller data (subtype 0x6). Animation of texture coordinates.
    /// </summary>
    [Controller(0x6)]
    public class TextureAnimationData : MeshAnimationController
    {
    }

    /// <summary>
    /// Texture animation controller data (subtype 0x8006). Animation of texture coordinates.
    /// </summary>
    [Controller(0x8006)]
    public class TextureAnimationData2 : MeshAnimationController
    {
    }
}
