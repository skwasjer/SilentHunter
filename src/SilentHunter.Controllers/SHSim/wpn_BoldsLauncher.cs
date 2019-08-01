/* 
 * SHSim.act - wpn_BoldsLauncher
 *
 * © 2007-2016 skwas. All rights reserved.
 * This code is provided as is. Change at your own risk.
 * --------------------------------------------------
 * 
 * S3D template for the wpn_BoldsLauncher controller of Silent Hunter.
 * 
 * For documentation on templates, requirements and restrictions, please refer to the documentation.
 * 
*/

using SilentHunter.Controllers;

namespace SHSim
{
	/// <summary>
	/// wpn_BoldsLauncher render controller.
	/// </summary>
	public class wpn_BoldsLauncher
		: Controller
	{
		/// <summary>
		/// The number of available bolds.
		/// </summary>
		public int bolds_count;
		/// <summary>
		/// The bold's eject speed [m/s].
		/// </summary>
		public float eject_speed;
		/// <summary>
		/// The bold's eject effect.
		/// </summary>
		public ulong eject_effect;
	}
}