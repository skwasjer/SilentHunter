/* 
 * EnvSim.act - EnvColors
 *
 * © 2007-2016 skwas. All rights reserved.
 * This code is provided as is. Change at your own risk.
 * --------------------------------------------------
 * 
 * S3D template for the EnvColors controller of Silent Hunter.
 * 
 * For documentation on templates, requirements and restrictions, please refer to the documentation.
 * 
*/

using System.Collections.Generic;
using SilentHunter;
using SilentHunter.Dat;

namespace EnvSim
{
	/// <summary>
	/// EnvColors user data.
	/// </summary>
	public class EnvColors : Controller
	{
		/// <summary>
		/// List of different weather types (0=ClearSky, 1=Overcast, 2=Rain).
		/// </summary>
		public List<EnvColorWeatherType> WeatherTypes;
	}

	[SHType]
	public class EnvColorWeatherType
	{
		/// <summary>
		/// Colors for diferent sun positions.
		/// </summary>
		public List<EnvColor> Colors;
	}

	[SHType]
	public class EnvColor
	{
		/// <summary>
		/// The sun altitude angle corresponding to following values.
		/// </summary>
		public float SunAltitudeAngle;

		/// <summary>
		/// The clouds should be white during the day, red at sunset and dark during the night.
		/// </summary>
		public Color CloudsColor;

		/// <summary>
		/// The fog color shuld match the sky texture color at horizon.
		/// </summary>
		public Color FogColor;

		/// <summary>
		/// The underwater fog color.
		/// </summary>
		public Color UnderwaterFogColor;

		/// <summary>
		/// Color of the sky near the sun.
		/// </summary>
		[Optional]
		public Color? SkyColor0;

		/// <summary>
		/// Color of the sky away from the sun.
		/// </summary>
		[Optional]
		public Color? SkyColor1;

		/// <summary>
		/// Cloud sharpness.
		/// </summary>
		[Optional]
		public float? CloudSharpness;

		/// <summary>
		/// Haze Z Min.
		/// </summary>
		[Optional]
		public float? HazeRelativeZMin;

		/// <summary>
		/// Haze Z Max.
		/// </summary>
		[Optional]
		public float? HazeRelativeZMaz;

		/// <summary>
		/// Haze Y Factor.
		/// </summary>
		[Optional]
		public float? HazeYFactor;

		/// <summary>
		/// Sun turns to red at sunrise/sunset.
		/// </summary>
		public Color SunColor;

		/// <summary>
		/// Sun halo color.
		/// </summary>
		public Color SunHaloColor;

		/// <summary>
		/// Color of the sun directional light.
		/// </summary>
		public Color SunLightColor;

		/// <summary>
		/// Color of the light reflected by sea water.
		/// </summary>
		public Color SunReflectColor;

		/// <summary>
		/// Ambient light color.
		/// </summary>
		public Color AmbientLightColor;

		/// <summary>
		/// Light water color.
		/// </summary>
		public Color WaterLightColor;

		/// <summary>
		/// Dark water color.
		/// </summary>
		public Color WaterDarkColor;

		/// <summary>
		/// Light near water surface.
		/// </summary>
		public UpDownColor UnderwaterUpColor;

		/// <summary>
		/// Light at water bottom.
		/// </summary>
		public UpDownColor UnderwaterDownColor;

		/// <summary>
		/// </summary>
		public Color ShipWakeAboveWaterColor;

		/// <summary>
		/// </summary>
		public Color ShipWakeUnderWaterColor;

		/// <summary>
		/// </summary>
		public float SunLightMultiplication;

		/// <summary>
		/// </summary>
		[Optional]
		public float? SunSkyInfluence;
	}

	public struct UpDownColor
	{
		public Color Color;
		public float Intensity;
	}
}