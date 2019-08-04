/* 
 * SHControllers.act - RandomGenericAxisRotation
 *
 * © 2007-2016 skwas. All rights reserved.
 * This code is provided as is. Change at your own risk.
 * --------------------------------------------------
 * 
 * S3D template for the RandomGenericAxisRotation controller of Silent Hunter.
 * 
 * For documentation on templates, requirements and restrictions, please refer to the documentation.
 * 
*/

using SilentHunter;
using SilentHunter.Controllers;

namespace SHControllers
{
	/// <summary>
	/// RandomGenericAxisRotation action controller.
	/// </summary>
	public class RandomGenericAxisRotation
		: BehaviorController
	{
		/// <summary>
		/// The minimum angular speed [deg/s].
		/// </summary>
		public float MinimumRotationSpeed;
		/// <summary>
		/// The maximum angular speed [deg/s].
		/// </summary>
		public float MaximumRotationSpeed;
		/// <summary>
		/// The minimum delay between two rotations [s].
		/// </summary>
		public float MinimumRotationDelay;
		/// <summary>
		/// The maximum delay between two rotations [s].
		/// </summary>
		public float MaximumRotationDelay;
		/// <summary>
		/// The inertia of rotation.
		/// </summary>
		public float RotationInertia;
		/// <summary>
		/// The minimum and maximum movement angles (in degrees).
		/// </summary>
		public MinMax Angles;
		/// <summary>
		/// The local axis to rotate around.
		/// </summary>
		public Axis Axe;
	}
}