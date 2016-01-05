/* 
 * SHSim.act - wpn_Searchlight
 *
 * © 2007-2016 skwas. All rights reserved.
 * This code is provided as is. Change at your own risk.
 * --------------------------------------------------
 * 
 * S3D template for the wpn_Searchlight controller of Silent Hunter.
 * 
 * For documentation on templates, requirements and restrictions, please refer to the documentation.
 * 
*/

using SilentHunter.Dat;

namespace SHSim
{
	/// <summary>
	/// wpn_Searchlight controller.
	/// </summary>
	public class wpn_Searchlight
		: Controller
	{
		/// <summary>
		/// Turret settings.
		/// </summary>
		public obj_Turret obj_Turret;
		/// <summary>
		/// Visual beam object.
		/// </summary>
		public string beam;
	}
}