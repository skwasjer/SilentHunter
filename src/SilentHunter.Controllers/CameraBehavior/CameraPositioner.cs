/*
 * CameraBehavior.act - CameraPositioner
 *
 * © 2007-2016 skwas. All rights reserved.
 * This code is provided as is. Change at your own risk.
 * --------------------------------------------------
 *
 * S3D template for the CameraPositioner controller of Silent Hunter.
 *
 * For documentation on templates, requirements and restrictions, please refer to the documentation.
 *
 */

using System.Collections.Generic;
using SilentHunter;
using SilentHunter.Controllers;

namespace CameraBehavior
{
    /// <summary>
    /// CameraPositioner controller. CameraPositioner allows one to specify the position of a virtual camera when activated.
    /// </summary>
    public class CameraPositioner
        : BehaviorController
    {
        /// <summary>
        /// List of objects belonging to virtual cameras to get pos from.
        /// </summary>
        public List<VirtualCamera> VirtualCameras;
    }

    public class VirtualCamera
    {
        /// <summary>
        /// Object to get coordinates from.
        /// </summary>
        public ulong Object;
        /// <summary>
        /// Follow object's global translation.
        /// </summary>
        public BoolVector3 Translation;
        /// <summary>
        /// Follow object's global rotation.
        /// </summary>
        public BoolVector3 Rotation;
        /// <summary>
        /// Always apply this position.
        /// </summary>
        public bool Always;
    }
}
