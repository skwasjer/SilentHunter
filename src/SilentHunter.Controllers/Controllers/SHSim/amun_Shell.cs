/* 
 * SHSim.act - amun_Shell
 *
 * © 2007-2016 skwas. All rights reserved.
 * This code is provided as is. Change at your own risk.
 * --------------------------------------------------
 * 
 * S3D template for the amun_Shell controller of Silent Hunter.
 * 
 * For documentation on templates, requirements and restrictions, please refer to the documentation.
 * 
*/

using SilentHunter.Dat;

namespace SHSim
{
	/// <summary>
	/// amun_Shell controller.
	/// </summary>
	public class amun_Shell
		: Controller
	{
		/// <summary>
		/// Shell's trace effect.
		/// </summary>
		public ulong trace;
		/// <summary>
		/// Water splash effect.
		/// </summary>
		public ulong splash;
		/// <summary>
		/// Ship hit explosion effect.
		/// </summary>
		public ulong explosion;
		/// <summary>
		/// Shell's detonation settings.
		/// </summary>
		public ShellDetonation Detonation;
	}

	[SHType]
	public class ShellDetonation
	{
		/// <summary>
		/// Shell's detonation range.
		/// </summary>
		public float range;
		/// <summary>
		/// Shell's detonation effect.
		/// </summary>
		public ulong effect;
	}
}