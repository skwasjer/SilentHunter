/* 
 * SHSim.act - obj_Funnel
 *
 * © 2007-2016 skwas. All rights reserved.
 * This code is provided as is. Change at your own risk.
 * --------------------------------------------------
 * 
 * S3D template for the obj_Funnel controller of Silent Hunter.
 * 
 * For documentation on templates, requirements and restrictions, please refer to the documentation.
 * 
*/

using SilentHunter.Controllers;

namespace SHSim
{
	/// <summary>
	/// obj_Funnel controller.
	/// </summary>
	public class obj_Funnel
		: BehaviorController
	{
		/// <summary>
		/// object->ScalableParticleEffect. The funnel's smoke effect.
		/// </summary>
		public ulong smoke;
		/// <summary>
		/// Index of the attached shaft.
		/// </summary>
		public int shaft;
	}
}