/* 
 * EffectManager.act - EffectLauncher
 *
 * © 2007-2016 skwas. All rights reserved.
 * This code is provided as is. Change at your own risk.
 * --------------------------------------------------
 * 
 * S3D template for the EffectLauncher controller of Silent Hunter.
 * 
 * For documentation on templates, requirements and restrictions, please refer to the documentation.
 * 
*/

using SilentHunter;
using SilentHunter.Controllers;
using SilentHunter.Controllers.Decoration;

namespace EffectManager
{
	/// <summary>
	/// LightLauncher action controller.
	/// </summary>
	public class EffectLauncher
		: Controller
	{
		/// <summary>
		/// The time to wait until launching the effect.
		/// </summary>
		public float Delay;
		/// <summary>
		/// Initial velocity direction.
		/// </summary>
		public EffectLauncherDirection Direction;
		/// <summary>
		/// The 3D object to launch.
		/// </summary>
		public ulong Object;
	}

	[SHType]
	public class EffectLauncherDirection
	{
		/// <summary>
		/// Initial position.
		/// </summary>
		public Vector3 Position;
	}
}