/* 
 * SHCollisions.act - AmmoDamageInfo
 *
 * © 2007-2016 skwas. All rights reserved.
 * This code is provided as is. Change at your own risk.
 * --------------------------------------------------
 * 
 * S3D template for the AmmoDamageInfo controller of Silent Hunter.
 * 
 * For documentation on templates, requirements and restrictions, please refer to the documentation.
 * 
*/

using SilentHunter.Controllers;

namespace SHCollisions
{
	/// <summary>
	/// AmmoDamageInfo user data.
	/// </summary>
	public class AmmoDamageInfo
		: BehaviorController
	{
		/// <summary>
		/// The minimum hit points this ammunition is worth.
		/// </summary>
		public float MinEF;
		/// <summary>
		/// The maximum hit points this ammunition is worth.
		/// </summary>
		public float MaxEF;
		/// <summary>
		/// The armor level it penetrates.
		/// </summary>
		public float AP;
		/// <summary>
		/// The minimum radius of splash damage. The ammunition takes all its hitpoints until this distance.
		/// </summary>
		public float MinRadius;
		/// <summary>
		/// The maximum radius of splash damage. The ammunition takes no hitpoints beyond this distance. If 0, no splash damage!
		/// </summary>
		public float MaxRadius;
	}
}