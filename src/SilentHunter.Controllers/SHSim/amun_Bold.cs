/* 
 * SHSim.act - amun_Bold
 *
 * © 2007-2016 skwas. All rights reserved.
 * This code is provided as is. Change at your own risk.
 * --------------------------------------------------
 * 
 * S3D template for the amun_Bold controller of Silent Hunter.
 * 
 * For documentation on templates, requirements and restrictions, please refer to the documentation.
 * 
*/

using SilentHunter.Controllers;

namespace SHSim
{
	/// <summary>
	/// amun_Bold controller.
	/// </summary>
	public class amun_Bold
		: Controller
	{
		/// <summary>
		/// Bold's bubbles effect.
		/// </summary>
		public ulong bubbles;
		/// <summary>
		/// The bold's life time [min].
		/// </summary>
		public float life_time;
		/// <summary>
		/// The apparent bold's surface [m2].
		/// </summary>
		public float surface;
		/// <summary>
		/// The bold's noise [>=0].
		/// </summary>
		public float noise;
	}
}