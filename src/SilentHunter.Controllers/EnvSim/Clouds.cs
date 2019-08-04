/* 
 * EnvSim.act - Clouds
 *
 * © 2007-2016 skwas. All rights reserved.
 * This code is provided as is. Change at your own risk.
 * --------------------------------------------------
 * 
 * S3D template for the Clouds controller of Silent Hunter.
 * 
 * For documentation on templates, requirements and restrictions, please refer to the documentation.
 * 
*/

using System.Collections.Generic;
using SilentHunter.Controllers;
using SilentHunter.Controllers.Decoration;

namespace EnvSim
{
	/// <summary>
	/// Clouds render controller.
	/// </summary>
	public class Clouds
		: BehaviorController
	{
		/// <summary>
		/// Clouds altitude.
		/// </summary>
		[Optional]
		[SHVersion(SHVersions.SH4)]
		public float? Altitude;
		/// <summary>
		/// Wind direction and speed.
		/// </summary>
		public CloudWind Wind;
		/// <summary>
		/// Texture for each weather condition.
		/// </summary>
		public List<string> CloudTextures;
		/// <summary>
		/// Normal map for each weather condition.
		/// </summary>
		[Optional]
		[SHVersion(SHVersions.SH4)]
		public List<string> CloudNormalMaps;
		/// <summary>
		/// Scene parameters.
		/// </summary>
		public List<SceneParameters> SceneParameters;
	}

	[SHType]
	public class SceneParameters
	{
		/// <summary>
		/// Bumpiness (1..1000).
		/// </summary>
		[Optional]
		[SHVersion(SHVersions.SH4)]
		public float? Bumpiness;
		/// <summary>
		/// Clouds edge blur factor (0..1).
		/// </summary>
		[Optional]
		[SHVersion(SHVersions.SH4)]
		public float? EdgeBlur;
		/// <summary>
		/// The clouds reflection alpha (0..1).
		/// </summary>
		public float CloudsReflection;
		/// <summary>
		/// Fresnel is multiplied with this coef.
		/// </summary>
		public float GlobalFresnel;
		/// <summary>
		/// Reflection is multiplied with this coef.
		/// </summary>
		public float GlobalReflectionIntensity;
	}

	[SHType]
	public class CloudWind
	{
		/// <summary>
		/// The current weather.
		/// </summary>
		public WeatherType CurrentWeather;
		/// <summary>
		/// The upcoming weather. If different from CurrentWeather, a transition between the two weather types is made.
		/// </summary>
		public WeatherType ChangeToWeather;
		/// <summary>
		/// The texture used for blending two different cloud textures.
		/// </summary>
		public string CloudsTransitionTextureName;
		/// <summary>
		/// The clouds textures are tiled by this factor.
		/// </summary>
		public float CloudsTilingFactor;
		/// <summary>
		/// The sun light on clouds is multiplied with this value.
		/// </summary>
		public float SunLightScale;
	}

	public enum WeatherType
	{
		ClearSky,
		Overcast,
		Rain
	}
}