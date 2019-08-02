/* 
 * SHControllers.act - UnifiedRenderControler
 *
 * © 2007-2016 skwas. All rights reserved.
 * This code is provided as is. Change at your own risk.
 * --------------------------------------------------
 * 
 * S3D template for the UnifiedRenderControler controller of Silent Hunter.
 * 
 * For documentation on templates, requirements and restrictions, please refer to the documentation.
 * 
*/

using SilentHunter.Controllers;
using SilentHunter.Controllers.Decoration;

namespace SHControllers
{
	/// <summary>
	/// UnifiedRenderControler. Render controller for illumination, caustics, decals etc.
	/// </summary>
	public class UnifiedRenderControler
		: Controller
	{
		/// <summary>
		/// Enable bump map and caustics if distance less than this value (SH metrics 1 = 10 meters).
		/// </summary>
		public float MediumLODDistance;
		/// <summary>
		/// If distance greater than this value, only the sun is considered(SH metrics 1 = 10 meters).
		/// </summary>
		public float LowLODDistance;
		/// <summary>
		/// Enable or disable bump map.
		/// </summary>
		public bool BumpEnable;
		/// <summary>
		/// Enable or disable specular mask.
		/// </summary>
		public bool SpecularMaskEnable;
		/// <summary>
		/// Enable or disable self occlusion map.
		/// </summary>
		public bool SelfOccEnable;
		/// <summary>
		/// Use specular from diffuse texture.
		/// </summary>
		[Optional]
		public bool? UseSpecularFromDiffuse;
		/// <summary>
		/// Water caustics on objects settings.
		/// </summary>
		public WaterCaustics Caustics;
	}

	/// <summary>
	/// Water caustics on objects settings.
	/// </summary>
	[SHType]
	public class WaterCaustics
	{
		/// <summary>
		/// Enable or disable caustics.
		/// </summary>
		public bool Enabled;
		/// <summary>
		/// Caustics will fade from 0 (sea level) to max altitude (above water).
		/// </summary>
		public float MaxAltitude;
		/// <summary>
		/// Caustics will fade from 0 (sea level) to min altitude (under water).
		/// </summary>
		public float MinAltitude;
		/// <summary>
		/// Caustics strength (0.0 = none, 1.0 = full)
		/// </summary>
		public float Strength;
		/// <summary>
		/// Caustics texture scale.
		/// </summary>
		public float Scale;
		/// <summary>
		/// Caustics texture shift speed.
		/// </summary>
		public float Speed;
	}
}