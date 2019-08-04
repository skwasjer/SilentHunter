/* 
 * SHControllers.act - GenHeightControl
 *
 * © 2007-2016 skwas. All rights reserved.
 * This code is provided as is. Change at your own risk.
 * --------------------------------------------------
 * 
 * S3D template for the GenHeightControl controller of Silent Hunter.
 * 
 * For documentation on templates, requirements and restrictions, please refer to the documentation.
 * 
*/

using SilentHunter.Controllers;

namespace SHControllers
{
	/// <summary>
	/// GenHeightControl render controller. Starts/Stops the ParticleGenerators depending on the height.
	/// </summary>
	public class GenHeightControl
		: BehaviorController
	{
		/// <summary>
		/// Under this limit, the particles generator is stopped.
		/// </summary>
		public float Min;
		/// <summary>
		/// Over this limit, the particles generator is stopped.
		/// </summary>
		public float Max;
	}
}