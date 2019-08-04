/* 
 * EnvSim.act - FoamWave
 *
 * © 2007-2016 skwas. All rights reserved.
 * This code is provided as is. Change at your own risk.
 * --------------------------------------------------
 * 
 * S3D template for the FoamWave controller of Silent Hunter.
 * 
 * For documentation on templates, requirements and restrictions, please refer to the documentation.
 * 
*/

using SilentHunter.Controllers;

namespace EnvSim
{
	/// <summary>
	/// FoamWave render controller.
	/// </summary>
	public class FoamWave
		: BehaviorController
	{
		/// <summary>
		/// </summary>
		public float LifeTime;
		/// <summary>
		/// </summary>
		public int BowWave;
	}

	public enum BowWaveModel
	{
		Small,
		Medium,
		Large
	}
}