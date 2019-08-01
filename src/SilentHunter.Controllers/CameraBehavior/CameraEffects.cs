/* 
 * CameraBehavior.act - CameraEffects
 *
 * © 2007-2016 skwas. All rights reserved.
 * This code is provided as is. Change at your own risk.
 * --------------------------------------------------
 * 
 * S3D template for the CameraEffects controller of Silent Hunter.
 * 
 * For documentation on templates, requirements and restrictions, please refer to the documentation.
 * 
*/

using SilentHunter.Controllers;

namespace CameraBehavior
{
	/// <summary>
	/// CameraEffects controller. Water effects on the camera.
	/// </summary>
	public class CameraEffects
		: Controller
	{
		/// <summary>
		/// The duration of water drops effect [sec].
		/// </summary>
		public float DropsDuration;
		/// <summary>
		/// The duration of blur effect [sec].
		/// </summary>
		public float BlurDuration;
		/// <summary>
		/// The radius of camera size [0.1m].
		/// </summary>
		public float Radius;
	}
}