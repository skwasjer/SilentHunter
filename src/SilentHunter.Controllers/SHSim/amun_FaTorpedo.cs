/* 
 * SHSim.act - amun_FaTorpedo
 *
 * © 2007-2016 skwas. All rights reserved.
 * This code is provided as is. Change at your own risk.
 * --------------------------------------------------
 * 
 * S3D template for the amun_FaTorpedo controller of Silent Hunter.
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
	/// amun_FaTorpedo controller.
	/// </summary>
	public class amun_FaTorpedo
		: Controller
	{
		/// <summary>
		/// General torpedo settings.
		/// </summary>
		public amun_Torpedo amun_Torpedo;
		/// <summary>
		/// The torpedo's ladder settings.
		/// </summary>
		public List<Ladder> Ladder;
	}

	[SHType]
	public class Ladder
	{
		/// <summary>
		/// The ladder's leg length [m].
		/// </summary>
		public float leg_length;
	}
}