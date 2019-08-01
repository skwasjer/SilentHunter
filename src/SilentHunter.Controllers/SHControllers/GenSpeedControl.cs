/* 
 * SHControllers.act - GenSpeedControl
 *
 * © 2007-2016 skwas. All rights reserved.
 * This code is provided as is. Change at your own risk.
 * --------------------------------------------------
 * 
 * S3D template for the GenSpeedControl controller of Silent Hunter.
 * 
 * For documentation on templates, requirements and restrictions, please refer to the documentation.
 * 
*/

using SilentHunter.Controllers;

namespace SHControllers
{
	/// <summary>
	/// GenSpeedControl render controller. Interpolates the WindSpeed, Velocity, Weight, Size and Opacity of the ParticleGenerators between the min and max speed limits.
	/// </summary>
	public class GenSpeedControl
		: Controller
	{
		/// <summary>
		/// Min speed to start interpolate.
		/// </summary>
		public float Min;
		/// <summary>
		/// Max speed to start interpolate.
		/// </summary>
		public float Max;
	}
}