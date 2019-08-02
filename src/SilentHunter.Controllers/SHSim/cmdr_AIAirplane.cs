/* 
 * SHSim.act - cmdr_AIAirplane
 *
 * © 2007-2016 skwas. All rights reserved.
 * This code is provided as is. Change at your own risk.
 * --------------------------------------------------
 * 
 * S3D template for the cmdr_AIAirplane controller of Silent Hunter.
 * 
 * For documentation on templates, requirements and restrictions, please refer to the documentation.
 * 
*/

using SilentHunter.Controllers;

namespace SHSim
{
	/// <summary>
	/// cmdr_AIAirplane controller. Airplane unit AI commander.
	/// </summary>
	public class cmdr_AIAirplane
		: Controller
	{
		/// <summary>
		/// </summary>
		public cmdr_AIShip cmdr_AIShip;
		/// <summary>
		/// Airplane's minimum flight altitude.
		/// </summary>
		public float min_height;
		/// <summary>
		/// True if the plane is a level bomber.
		/// </summary>
		public bool level_bomber;
	}
}