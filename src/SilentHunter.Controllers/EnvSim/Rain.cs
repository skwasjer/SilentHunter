/* 
 * EnvSim.act - Rain
 *
 * © 2007-2016 skwas. All rights reserved.
 * This code is provided as is. Change at your own risk.
 * --------------------------------------------------
 * 
 * S3D template for the Rain controller of Silent Hunter.
 * 
 * For documentation on templates, requirements and restrictions, please refer to the documentation.
 * 
*/

using SilentHunter;
using SilentHunter.Controllers;
using SilentHunter.Controllers.Decoration;

namespace EnvSim
{
	/// <summary>
	/// Rain render controller.
	/// </summary>
	public class Rain : BehaviorController
	{
		/// <summary>
		/// Random wind parameters.
		/// </summary>
		public RandomWind RandomWind;
		/// <summary>
		/// Total number of rain drops.
		/// </summary>
		public int DropsNumber;
		/// <summary>
		/// Rain drop fall speed.
		/// </summary>
		public float FallSpeed;
		/// <summary>
		/// The speed of change for rain intensity.
		/// </summary>
		public float RainIntensityChangeSpeed;
		/// <summary>
		/// Rain drop minimum size.
		/// </summary>
		public float MinSize;
		/// <summary>
		/// Rain drop maximum size.
		/// </summary>
		public float MaxSize;
		/// <summary>
		/// The color of the start of the drop.
		/// </summary>
		public Color HeadColor;
		/// <summary>
		/// The alpha of the start of the drop.
		/// </summary>
		public float HeadAlpha;
		/// <summary>
		/// The color of the end of the drop.
		/// </summary>
		public Color TailColor;
		/// <summary>
		/// The alpha of the end of the drop.
		/// </summary>
		public float TailAlpha;
	}

	[SHType]
	public class RandomWind
	{
		/// <summary>
		/// The minimum time after which the random wind changes.
		/// </summary>
		public float ChangeTimeMin;
		/// <summary>
		/// The maximum time after which the random wind changes.
		/// </summary>
		public float ChangeTimeMax;
		/// <summary>
		/// The maximum intensity of the random wind.
		/// </summary>
		public float RandomWindSpeedMax;
		/// <summary>
		/// The speed of intensity change for random wind.
		/// </summary>
		public float RandomWindSpeedVariationSpeed;
		/// <summary>
		/// The speed of heading change for random wind.
		/// </summary>
		public float RandomWindHeadingVariationSpeed;
	}
}