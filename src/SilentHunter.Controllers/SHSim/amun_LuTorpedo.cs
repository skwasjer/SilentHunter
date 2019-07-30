/* 
 * SHSim.act - amun_LuTorpedo
 *
 * © 2007-2016 skwas. All rights reserved.
 * This code is provided as is. Change at your own risk.
 * --------------------------------------------------
 * 
 * S3D template for the amun_LuTorpedo controller of Silent Hunter.
 * 
 * For documentation on templates, requirements and restrictions, please refer to the documentation.
 * 
*/

using SilentHunter.Dat;

namespace SHSim
{
	/// <summary>
	/// amun_LuTorpedo controller.
	/// </summary>
	public class amun_LuTorpedo
		: Controller
	{
		/// <summary>
		/// General torpedo settings.
		/// </summary>
		public amun_Torpedo amun_Torpedo;
		/// <summary>
		/// Torpedo's straight run distance.
		/// </summary>
		public float straight_run;
	}
}