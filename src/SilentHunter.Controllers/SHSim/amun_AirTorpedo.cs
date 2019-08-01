/* 
 * SHSim.act - amun_AirTorpedo
 *
 * © 2007-2016 skwas. All rights reserved.
 * This code is provided as is. Change at your own risk.
 * --------------------------------------------------
 * 
 * S3D template for the amun_AirTorpedo controller of Silent Hunter.
 * 
 * For documentation on templates, requirements and restrictions, please refer to the documentation.
 * 
*/

using SilentHunter.Controllers;

namespace SHSim
{
	/// <summary>
	/// amun_AirTorpedo controller.
	/// </summary>
	public class amun_AirTorpedo
		: Controller
	{
		/// <summary>
		/// General torpedo settings.
		/// </summary>
		public amun_Torpedo amun_Torpedo;
	}
}