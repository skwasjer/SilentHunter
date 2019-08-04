/* 
 * SHControllers.act - SearchLightEffect
 *
 * © 2007-2016 skwas. All rights reserved.
 * This code is provided as is. Change at your own risk.
 * --------------------------------------------------
 * 
 * S3D template for the SearchLightEffect controller of Silent Hunter.
 * 
 * For documentation on templates, requirements and restrictions, please refer to the documentation.
 * 
*/

using SilentHunter;
using SilentHunter.Controllers;
using SilentHunter.Controllers.Decoration;

namespace SHControllers
{
	/// <summary>
	/// SearchLightEffect render controller. Works only on water surface.
	/// </summary>
	public class SearchLightEffect : BehaviorController
	{
		/// <summary>
		/// Cone angle, in degrees.
		/// </summary>
		public float Angle;
		/// <summary>
		/// Color of light.
		/// </summary>
		public Color Color;
		/// <summary>
		/// Light range (meters).
		/// </summary>
		public float Range;
		/// <summary>
		/// Light intensity = 1 / (A + B*&lt;distance&gt;)
		/// </summary>
		public Atenuation Atenuation;
		/// <summary>
		/// Halo size (fraction from cameraX size).
		/// </summary>
		public float HaloSize;
	}

	[SHType]
	public class Atenuation
	{
		/// <summary>
		/// A factor.
		/// </summary>
		public float A;
		/// <summary>
		/// B factor.
		/// </summary>
		public float B;
	}
}