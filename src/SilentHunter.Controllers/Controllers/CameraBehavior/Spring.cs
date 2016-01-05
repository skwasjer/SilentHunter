/* 
 * CameraBehavior.act - Spring
 *
 * © 2007-2016 skwas. All rights reserved.
 * This code is provided as is. Change at your own risk.
 * --------------------------------------------------
 * 
 * S3D template for the Spring controller of Silent Hunter.
 * 
 * For documentation on templates, requirements and restrictions, please refer to the documentation.
 * 
*/

using SilentHunter.Dat;

namespace CameraBehavior
{
	/// <summary>
	/// Spring controller. 
	/// </summary>
	public class Spring
		: Controller
	{
		/// <summary>
		/// The spring's elasticity factor (>= 0).
		/// </summary>
		public float Elasticity;
		/// <summary>
		/// The spring's friction factor (>= 0).
		/// </summary>
		public float Friction;
	}
}