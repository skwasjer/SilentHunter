﻿/*
 * SHControllers.act - FollowParent
 *
 * © 2007-2016 skwas. All rights reserved.
 * This code is provided as is. Change at your own risk.
 * --------------------------------------------------
 *
 * S3D template for the FollowParent controller of Silent Hunter.
 *
 * For documentation on templates, requirements and restrictions, please refer to the documentation.
 *
 */

using System.Numerics;
using SilentHunter;
using SilentHunter.Controllers;

namespace SHControllers
{
    /// <summary>
    /// FollowParent action controller.
    /// </summary>
    public class FollowParent
        : BehaviorController
    {
        /// <summary>
        /// Follow parent's global translation.
        /// </summary>
        public BoolVector3 Translation;
        /// <summary>
        /// Follow parent's global rotation.
        /// </summary>
        public BoolVector3 Rotation;
        /// <summary>
        /// Parent's position and rotation for not following components.
        /// </summary>
        public PositionRotationVectors Default;
    }

    public struct PositionRotationVectors
    {
        public Vector3 Position;
        public Vector3 Rotation;
    }
}
