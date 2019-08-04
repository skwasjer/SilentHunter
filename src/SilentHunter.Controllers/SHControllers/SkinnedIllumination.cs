/* 
 * SHControllers.act - SkinnedIllumination
 *
 * © 2007-2016 skwas. All rights reserved.
 * This code is provided as is. Change at your own risk.
 * --------------------------------------------------
 * 
 * S3D template for the SkinnedIllumination controller of Silent Hunter.
 * 
 * For documentation on templates, requirements and restrictions, please refer to the documentation.
 * 
*/

using SilentHunter.Controllers;

namespace SHControllers
{
	/// <summary>
	/// SkinnedIllumination controller.
	/// </summary>
	public class SkinnedIllumination
		: BehaviorController
	{
		/// <summary>
		/// Enable or disable bump map rendering.
		/// </summary>
		public bool BumpEnable;
	}
}