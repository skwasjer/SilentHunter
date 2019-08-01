/* 
 * EnvSim.act - Flash
 *
 * © 2007-2016 skwas. All rights reserved.
 * This code is provided as is. Change at your own risk.
 * --------------------------------------------------
 * 
 * S3D template for the Flash controller of Silent Hunter.
 * 
 * For documentation on templates, requirements and restrictions, please refer to the documentation.
 * 
*/

using SilentHunter.Controllers;
using System.Collections.Generic;

namespace EnvSim
{
	/// <summary>
	/// Flash action controller.
	/// </summary>
	public class Flash
		: Controller
	{
		/// <summary>
		/// Debug mode.
		/// </summary>
		public bool Debug;
		/// <summary>
		/// The names of the effects.
		/// </summary>
		public List<string> EffectNames;
		/// <summary>
		/// The height where the lightning is placed.
		/// </summary>
		public float Height;
		/// <summary>
		/// The minimum distance from main camera where lightning appears.
		/// </summary>
		public float MinDistance;
		/// <summary>
		/// The maximum distance from main camera where lightning appears.
		/// </summary>
		public float MaxDistance;
		/// <summary>
		/// When Debug=false, minimum time in seconds between 2 lightnings.
		/// </summary>
		public float MinTime;
		/// <summary>
		/// When Debug=false, maximum time in seconds between 2 lightnings.
		/// </summary>
		public float MaxTime;
	}
}