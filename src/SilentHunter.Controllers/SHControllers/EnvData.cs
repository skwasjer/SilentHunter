/* 
 * SHControllers.act - EnvData
 *
 * © 2007-2016 skwas. All rights reserved.
 * This code is provided as is. Change at your own risk.
 * --------------------------------------------------
 * 
 * S3D template for the EnvData controller of Silent Hunter.
 * 
 * For documentation on templates, requirements and restrictions, please refer to the documentation.
 * 
*/

using System.Numerics;
using SilentHunter;
using SilentHunter.Controllers;
using SilentHunter.Controllers.Decoration;

namespace SHControllers
{
	/// <summary>
	/// EnvData user data.
	/// </summary>
	public class EnvData : Controller
	{
		/// <summary>
		/// Water scale (4, 0.08, 4, 1, 4).
		/// </summary>
		public EnvDataScale Scale;

		/// <summary>
		/// Water Bump Scale (27, 33, 1) (X,Y,Z = U,V,Amplitude).
		/// </summary>
		public Vector3 BumpScale;

		/// <summary>
		/// Earth Radius, in meters/10 (137813).
		/// </summary>
		public float EarthRadius;

		/// <summary>
		/// Light water color (67, 128, 139).
		/// </summary>
		public Color LightColor;

		/// <summary>
		/// Dark water color (36, 76, 93).
		/// </summary>
		public Color DarkColor;

		/// <summary>
		/// Water opacity (0..255).
		/// </summary>
		public int WaterOpacity;

		/// <summary>
		/// Fresnel Reflection coefficient (8).
		/// </summary>
		public float FresnelCoef;

		/// <summary>
		/// Reflection intensity (1).
		/// </summary>
		public float ReflIntensity;

		/// <summary>
		/// Reflection bias (0).
		/// </summary>
		public float ReflBias;

		/// <summary>
		/// Reflection deformation (1).
		/// </summary>
		public float ReflDeformation;

		/// <summary>
		/// Highlight Threshold (0.75).
		/// </summary>
		[Optional]
		public float? HighlightThreshold;

		/// <summary>
		/// Maximum Wave Height (256).
		/// </summary>
		public float MaxWaveHeight;

		/// <summary>
		/// Wind Speed (10).
		/// </summary>
		public float param_WindSpeed;

		/// <summary>
		/// Supress large or small waves.
		/// </summary>
		public SupressWaves SupressWaves;

		/// <summary>
		/// Sea speed (0.08).
		/// </summary>
		public float SeaSpeed;

		/// <summary>
		/// Choppy Wave factor (0.01).
		/// </summary>
		public float ChoppyWave;

		/// <summary>
		/// Normalized wind direction (0.7, 0.7). (X,Y = X,Z).
		/// </summary>
		public Vector2 WindDirection;

		/// <summary>
		/// Above Water Fog parameters. (todo: what are the parameters then?)
		/// </summary>
		public Vector3 Fog;

		/// <summary>
		/// Camera parameters. (X,Y,Z = ZMin,ZMax,Angle)
		/// </summary>
		public Vector3 Camera;

		/// <summary>
		/// Lod factor.
		/// </summary>
		public float LodFactor;

		/// <summary>
		/// Under water parameters.
		/// </summary>
		public UnderWater Underwater;
	}

	public struct EnvDataScale
	{
		public Vector3 Scale;
		public float Normal;
		public int Patches;
	}

	[SHType]
	public class SupressWaves
	{
		/// <summary>
		/// Supress small waves coefficient (5e-5).
		/// </summary>
		public float SmallWaves;

		/// <summary>
		/// Supress large waves first armonics.
		/// </summary>
		public int LargeWavesArmonics;

		/// <summary>
		/// Large waves supression coefficient (0..1). 0 is maximum supression.
		/// </summary>
		public float LargeWavesCoef;
	}

	[SHType]
	public class UnderWater
	{
		/// <summary>
		/// Fresnel Reflection coefficient under water(3).
		/// </summary>
		public float FresnelCoef;

		/// <summary>
		/// Under Water Fog parameters.
		/// </summary>
		public UnderWaterFog Fog;

		/// <summary>
		/// Light near water surface.
		/// </summary>
		public UpDownLight UpLight;

		/// <summary>
		/// Light at water bottom.
		/// </summary>
		public UpDownLight DownLight;
	}

	public struct UnderWaterFog
	{
		public float ZMin;
		public float ZMax;
		public Color Color;
	}

	public struct UpDownLight
	{
		public Color Color;

		/// <summary>
		/// 0..1
		/// </summary>
		public float Intensity;
	}
}