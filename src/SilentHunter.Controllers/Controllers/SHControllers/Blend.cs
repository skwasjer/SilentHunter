/* 
 * SHControllers.act - Blend
 *
 * © 2007-2016 skwas. All rights reserved.
 * This code is provided as is. Change at your own risk.
 * --------------------------------------------------
 * 
 * S3D template for the Blend controller of Silent Hunter.
 * 
 * For documentation on templates, requirements and restrictions, please refer to the documentation.
 * 
*/

using System.Drawing;
using SilentHunter.Dat;
using System.Runtime.InteropServices;

namespace SHControllers
{
	/// <summary>
	/// Blend render controller.
	/// </summary>
	public class Blend
		: Controller
	{
		/// <summary>
		/// Parallel light direction in global space (spherical polar coordinates, angles in degrees).
		/// </summary>
		public LightDirection LightDirection;
		/// <summary>
		/// Ambient light color.
		/// </summary>
		public Color AmbientColor;
		/// <summary>
		/// Ambient light color scale.
		/// </summary>
		public float AmbientScale;
		/// <summary>
		/// Parallel light color.
		/// </summary>
		public Color ParallelColor;
		/// <summary>
		/// Parallel light color scale.
		/// </summary>
		public float ParallelScale;
		/// <summary>
		/// Specular intensity.
		/// </summary>
		public Color SpecularColor;
		/// <summary>
		/// Specular exponent (glosiness).
		/// </summary>
		public float SpecularExponent;
		/// <summary>
		/// Parallel backlight coefficient, multiplied with parallel frontlight color.
		/// </summary>
		public float BackLightFactor;
		/// <summary>
		/// Inverse bump height.
		/// </summary>
		public bool Inverse;
		/// <summary>
		/// Use &lt;u&gt; or &lt;v&gt; tangent vector.
		/// </summary>
		public bool UTangent;
	}

	[StructLayout(LayoutKind.Sequential)]
	public class LightDirection
	{
		public float HrzAngle;
		public float VerAngle;
	}         
}