/*
 * CameraBehavior.act - FreeMove
 *
 * © 2007-2016 skwas. All rights reserved.
 * This code is provided as is. Change at your own risk.
 * --------------------------------------------------
 *
 * S3D template for the FreeMove controller of Silent Hunter.
 *
 * For documentation on templates, requirements and restrictions, please refer to the documentation.
 *
 */

using SilentHunter.Controllers;

namespace CameraBehavior
{
    /// <summary>
    /// FreeMove controller.
    /// </summary>
    public class FreeMove
        : BehaviorController
    {
        /// <summary>
        /// The speed of movement [m/s].
        /// </summary>
        public float MoveSpeed;
        /// <summary>
        /// The inertia of movement.
        /// </summary>
        public float MoveInertia;
        /// <summary>
        /// The coeficient to apply on movement in run mode [m/s].
        /// </summary>
        public float MoveRun;
        /// <summary>
        /// The speed of rotation.
        /// </summary>
        public float RotationSpeed;
        /// <summary>
        /// The inertia of inertia.
        /// </summary>
        public float RotationInertia;
        /// <summary>
        /// The coeficient to apply on rotation in run mode.
        /// </summary>
        public float RotationRun;
        /// <summary>
        /// The maximum angle with the vertical [deg].
        /// </summary>
        public float InclAngle;
        /// <summary>
        /// The inertia of inclination from vertical.
        /// </summary>
        public float InclInertia;
        /// <summary>
        /// Maximum rotation angle around X axis [deg]
        /// </summary>
        public float MaxAngle;
        /// <summary>
        /// Maximum height for the camera [m].
        /// </summary>
        public float MaxHeight;
    }
}
