/*
 * © 2007-2016 skwas. All rights reserved.
 * This code is provided as is. Change at your own risk.
 * --------------------------------------------------
 *
 * S3D template for SH-controllers that contain animation data.
 * A special thanks to privateer for sharing info to enable
 * animation support in S3D.
 *
 * WARNING: DO NOT CHANGE THE TYPE/CLASS NAMES. I HAD TO HARDCODE A REFERENCE TO THEM.
 * Fields and descriptions may be changed though.
 *
 */

using System.Collections.Generic;
using System.Numerics;
using SilentHunter.Controllers;
using SilentHunter.Controllers.Decoration;

namespace anim
{
    /// <summary>
    /// Key frame start parameters controller data (subtype 0x4)
    /// </summary>
    [Controller(0x4)]
    public class KeyFrameAnimStartParams : Controller
    {
        /// <summary>
        /// The origin of position and rotation point. The origin does not move with the parent object during animation. Can be used to move an object to/away from a point, and/or spin an object around a point.
        /// </summary>
        public Origin Local;

        /// <summary>
        /// The origin of position and rotation point. The origin moves with the parent object during animation. Can be used to move an object while keeping the rotation around a local point instead of a world point.
        /// </summary>
        public Origin FollowPath;

        /// <summary>
        /// The start (initial) position and rotation of the parent object. Usually equals the x,y,z/xy,xz,yz values of the parent object.
        /// </summary>
        public Origin Start;
    }

    /// <summary>
    /// Key frame start parameters controller data (subtype 0x8004)
    /// </summary>
    [Controller(0x8004)]
    public class KeyFrameAnimStartParams2 : KeyFrameAnimStartParams
    {
    }

    /// <summary>
    /// Rotation key frames controller data (subtype 0x2)
    /// </summary>
    [Controller(0x2)]
    public class RotationKeyFrames : AnimationController
    {
        /// <summary>
        /// A list of key frames.
        /// </summary>
        public List<RotationKeyFrame> Frames;
    }

    /// <summary>
    /// Rotation key frames controller data (subtype 0x8002)
    /// </summary>
    [Controller(0x8002)]
    public class RotationKeyFrames2 : RotationKeyFrames
    {
    }

    /// <summary>
    /// Position key frames controller data (subtype 0x1)
    /// </summary>
    [Controller(0x1)]
    public class PositionKeyFrames : AnimationController
    {
        /// <summary>
        /// A list of key frames.
        /// </summary>
        public List<PositionKeyFrame> Frames;
    }

    /// <summary>
    /// Position key frames controller data (subtype 0x8001)
    /// </summary>
    [Controller(0x8001)]
    public class PositionKeyFrames2 : PositionKeyFrames
    {
    }

    public struct Origin
    {
        public Vector3 Position;
        public Vector3 Rotation;
    }

    public struct RotationKeyFrame
    {
        /// <summary>
        /// The time of the key frame.
        /// </summary>
        public float Time;

        /// <summary>
        /// Quaternion representing the arbitrary axis (xyz) and rotation (w). More info: http://www.gamedev.net/reference/articles/article1095.asp
        /// </summary>
        public Quaternion Rotation;
    }

    /// <summary>
    /// Position key frames controller data.
    /// </summary>
    public struct PositionKeyFrame
    {
        /// <summary>
        /// The time of the key frame.
        /// </summary>
        public float Time;

        /// <summary>
        /// The new position.
        /// </summary>
        public Vector3 Position;
    }
}
