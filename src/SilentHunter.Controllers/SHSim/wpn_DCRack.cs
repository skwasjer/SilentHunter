/* 
 * SHSim.act - wpn_DCRack
 *
 * © 2007-2016 skwas. All rights reserved.
 * This code is provided as is. Change at your own risk.
 * --------------------------------------------------
 * 
 * S3D template for the wpn_DCRack controller of Silent Hunter.
 * 
 * For documentation on templates, requirements and restrictions, please refer to the documentation.
 * 
*/

using SilentHunter.Controllers;
using System.Collections.Generic;
using SilentHunter.Controllers.Decoration;

namespace SHSim
{
	/// <summary>
	/// wpn_DCRack render controller.
	/// </summary>
	public class wpn_DCRack
		: Controller
	{
		/// <summary>
		/// Visual barrel object.
		/// </summary>
		public string Barrel;
		/// <summary>
		/// The DCRack ammo storage.
		/// </summary>
		public List<DCRackAmmoStorage> ammo_storage;
	}

	[SHType]
	public class DCRackAmmoStorage
	{
		/// <summary>
		/// object->amun_DepthCharge. The depth charge's type.
		/// </summary>
		public ulong type;
		/// <summary>
		/// The amount of this type.
		/// </summary>
		public int amount;
	}
}