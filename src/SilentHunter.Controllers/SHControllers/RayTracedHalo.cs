/* 
 * SHControllers.act - RayTracedHalo
 *
 * © 2007-2016 skwas. All rights reserved.
 * This code is provided as is. Change at your own risk.
 * --------------------------------------------------
 * 
 * S3D template for the RayTracedHalo controller of Silent Hunter.
 * 
 * For documentation on templates, requirements and restrictions, please refer to the documentation.
 * 
*/

using SilentHunter.Controllers;

namespace SHControllers
{
	/// <summary>
	/// RayTracedHalo action controller.
	/// </summary>
	public class RayTracedHalo
		: BehaviorController
	{
		/// <summary>
		/// Is Halo(true) or Obstacle (false).
		/// </summary>
		public bool Halo;
	}
}