/* 
 * CameraBehavior.act - SphericalMove
 *
 * © 2007-2016 skwas. All rights reserved.
 * This code is provided as is. Change at your own risk.
 * --------------------------------------------------
 * 
 * S3D template for the SphericalMove controller of Silent Hunter.
 * 
 * For documentation on templates, requirements and restrictions, please refer to the documentation.
 * 
*/

using SilentHunter.Controllers;

namespace CameraBehavior
{
	/// <summary>
	/// SphericalMove controller. Free move inside two spheres.
	/// </summary>
	public class SphericalMove
		: Controller
	{
		/// <summary>
		/// The height above water [0.1m].
		/// </summary>
		public FreeMove FreeMove;
	}
}