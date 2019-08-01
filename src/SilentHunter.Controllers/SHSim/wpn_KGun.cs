/* 
 * SHSim.act - wpn_KGun
 *
 * © 2007-2016 skwas. All rights reserved.
 * This code is provided as is. Change at your own risk.
 * --------------------------------------------------
 * 
 * S3D template for the wpn_KGun controller of Silent Hunter.
 * 
 * For documentation on templates, requirements and restrictions, please refer to the documentation.
 * 
*/

using SilentHunter.Controllers;
using System.Collections.Generic;

namespace SHSim
{
	/// <summary>
	/// wpn_KGun render controller.
	/// </summary>
	public class wpn_KGun
		: Controller
	{
		/// <summary>
		/// Visual barrel object.
		/// </summary>
		public string Barrel;
		/// <summary>
		/// The k-gun's ammo storage.
		/// </summary>
		public List<DCRackAmmoStorage> ammo_storage;
		/// <summary>
		/// The k-gun's ranges [m].
		/// </summary>
		public List<float> ranges;
		/// <summary>
		/// Time to reload a k-gun [s].
		/// </summary>
		public float reload_time;
		/// <summary>
		/// The k-gun's fire effect.
		/// </summary>
		public ulong fire_effect;
	}
}