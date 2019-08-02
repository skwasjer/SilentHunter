/* 
 * EnvSim.act - FastGenSpeedControl
 *
 * © 2007-2016 skwas. All rights reserved.
 * This code is provided as is. Change at your own risk.
 * --------------------------------------------------
 * 
 * S3D template for the FastGenSpeedControl controller of Silent Hunter.
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
	/// FastGenSpeedControl render controller. Interpolates the WindSpeed, Velocity, Weight, Size and Opacity of the ParticleGenerators between the min and max speed limits.
	/// </summary>
	public class FastGenSpeedControl
		: Controller
	{
		/// <summary>
		/// Min speed to start interpolate.
		/// </summary>
		public float Min;
		/// <summary>
		/// Max speed to start interpolate.
		/// </summary>
		public float Max;
		/// <summary>
		/// Global scale coefficients for all subparticles.
		/// </summary>
		public MinParameters MinParameters;
	}

	[SHType]
	public class MinParameters
	{
		/// <summary>
		/// Velocity scale.
		/// </summary>
		public float VelocityScale;
		/// <summary>
		/// Weight scale.
		/// </summary>
		public float WeightScale;
		/// <summary>
		/// Size scale.
		/// </summary>
		public float SizeScale;
		/// <summary>
		/// Opacity scale.
		/// </summary>
		public float OpacityScale;
		/// <summary>
		/// Life scale.
		/// </summary>
		public float LifeScale;
		/// <summary>
		/// Density scale.
		/// </summary>
		public float DensityScale;
		/// <summary>
		/// Wind speed in generator's space.
		/// </summary>
		public Vector3 LocalWind;
		/// <summary>
		/// Wind speed in generator's space for the second generator (hack).
		/// </summary>
		public Vector3 LocalWind2;
	}
}