/* 
 * EnvSim.act - FogDistance
 *
 * © 2007-2016 skwas. All rights reserved.
 * This code is provided as is. Change at your own risk.
 * --------------------------------------------------
 * 
 * S3D template for the FogDistance controller of Silent Hunter.
 * 
 * For documentation on templates, requirements and restrictions, please refer to the documentation.
 * 
*/

using SilentHunter.Dat;

namespace EnvSim
{
	/// <summary>
	/// FogDistance controller.
	/// </summary>
	public class FogDistance
		: Controller
	{
		/// <summary>
		/// Minimum distance to start fog.
		/// </summary>
		public float ZMin;
		/// <summary>
		/// Maximum distance to start fog.
		/// </summary>
		public float ZMax;
	}
}