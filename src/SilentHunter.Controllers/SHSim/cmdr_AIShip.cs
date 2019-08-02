/* 
 * SHSim.act - cmdr_AIShip
 *
 * © 2007-2016 skwas. All rights reserved.
 * This code is provided as is. Change at your own risk.
 * --------------------------------------------------
 * 
 * S3D template for the cmdr_AIShip controller of Silent Hunter.
 * 
 * For documentation on templates, requirements and restrictions, please refer to the documentation.
 * 
*/

using SilentHunter.Controllers;

namespace SHSim
{
	/// <summary>
	/// cmdr_AIShip controller. Ship unit AI commander.
	/// </summary>
	public class cmdr_AIShip
		: Controller
	{
		/// <summary>
		/// </summary>
		public cmdr_AIFight cmdr_AIFight;
	}
}