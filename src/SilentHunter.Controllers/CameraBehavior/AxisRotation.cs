/* 
 * CameraBehavior.act - AxisRotation
 *
 * © 2007-2016 skwas. All rights reserved.
 * This code is provided as is. Change at your own risk.
 * --------------------------------------------------
 * 
 * S3D template for the AxisRotation controller of Silent Hunter.
 * 
 * For documentation on templates, requirements and restrictions, please refer to the documentation.
 * 
*/

using SilentHunter;
using SilentHunter.Dat;

namespace CameraBehavior
{
	/// <summary>
	/// AxisRotation action controller.
	/// </summary>
	public class AxisRotation
		: Controller
	{
		/// <summary>
		/// The angular speed [deg/s].
		/// </summary>
		public float RotationSpeed;
		/// <summary>
		/// The inertia of rotation.
		/// </summary>
		public float RotationInertia;
		/// <summary>
		/// The radius of circle to move around [m].
		/// </summary>
		public float Radius;
		/// <summary>
		/// The minimum and maximum angles (in degrees). Specify [0, 0] for unrestricted rotation.
		/// </summary>
		public MinMax Angles;
		/// <summary>
		/// The local axis to rotate around.
		/// </summary>
		public Axis Axe;
	}

}