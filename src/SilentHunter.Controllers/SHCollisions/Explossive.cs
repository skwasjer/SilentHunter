/* 
 * SHCollisions.act - Explossive
 *
 * © 2007-2016 skwas. All rights reserved.
 * This code is provided as is. Change at your own risk.
 * --------------------------------------------------
 * 
 * S3D template for the Explossive controller of Silent Hunter.
 * 
 * For documentation on templates, requirements and restrictions, please refer to the documentation.
 * 
*/

using SilentHunter.Controllers;

namespace SHCollisions
{
	/// <summary>
	/// Explossive controller. A static object exploding when hit.
	/// </summary>
	public class Explossive
		: BehaviorController
	{
		/// <summary>
		/// The damage info of this exploding object.
		/// </summary>
		public AmmoDamageInfo AmmoDamageInfo;
		/// <summary>
		/// The explosion effect.
		/// </summary>
		public ulong Explosion_Effect;
		/// <summary>
		/// The water column explosion effect.
		/// </summary>
		public ulong Water_Effect;
	}
}