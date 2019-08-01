/* 
 * SHSim.act - amun_DepthCharge
 *
 * © 2007-2016 skwas. All rights reserved.
 * This code is provided as is. Change at your own risk.
 * --------------------------------------------------
 * 
 * S3D template for the amun_DepthCharge controller of Silent Hunter.
 * 
 * For documentation on templates, requirements and restrictions, please refer to the documentation.
 * 
*/

using SilentHunter.Controllers;

namespace SHSim
{
	/// <summary>
	/// amun_DepthCharge controller.
	/// </summary>
	public class amun_DepthCharge
		: Controller
	{
		/// <summary>
		/// The under water falling speed [m/s].
		/// </summary>
		public float fall_speed;
		/// <summary>
		/// The detonation depth [m].
		/// </summary>
		public float detonate_depth;
		/// <summary>
		/// The depth sensor precision [m].
		/// </summary>
		public float depth_precision;
		/// <summary>
		/// The explosion range [m].
		/// </summary>
		public float explosion_range;
		/// <summary>
		/// The explosion impulse [t*m/s].
		/// </summary>
		public float explosion_impulse;
		/// <summary>
		/// Water hit splash effect.
		/// </summary>
		public ulong splash;
		/// <summary>
		/// Water bubbles effect.
		/// </summary>
		public ulong bubbles;
		/// <summary>
		/// Water column explosion effect.
		/// </summary>
		public ulong water_explosion;
		/// <summary>
		/// Under water explosion effect.
		/// </summary>
		public ulong under_explosion;
		/// <summary>
		/// Above water explosion effect.
		/// </summary>
		public ulong above_explosion;
	}	
}