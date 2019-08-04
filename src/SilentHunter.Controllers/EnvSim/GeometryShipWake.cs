/* 
 * EnvSim.act - GeometryShipWake
 *
 * © 2007-2016 skwas. All rights reserved.
 * This code is provided as is. Change at your own risk.
 * --------------------------------------------------
 * 
 * S3D template for the GeometryShipWake controller of Silent Hunter.
 * 
 * For documentation on templates, requirements and restrictions, please refer to the documentation.
 * 
*/

using SilentHunter.Controllers;

namespace EnvSim
{
	/// <summary>
	/// GeometryShipWake render controller.
	/// </summary>
	public class GeometryShipWake
		: BehaviorController
	{
		/// <summary>
		/// Length of the wake.
		/// </summary>
		public float Length;
		/// <summary>
		/// Aperture angle of the wake (degrees).
		/// </summary>
		public float ApertureAngle;
		/// <summary>
		/// Front width of the wake.
		/// </summary>
		public float FrontWidth;
		/// <summary>
		/// Length of the trajectory which must be straight (as the ship).
		/// </summary>
		public float FixedTrajectoryLength;
		/// <summary>
		/// The transparency of foam depending on ship speed.
		/// </summary>
		public GlobalTransparency GlobalTransparency;
		/// <summary>
		/// The wake type.
		/// </summary>
		public WakeType WakeType;
	}

	public class GlobalTransparency
	{
		/// <summary>
		/// Under this speed the foam is not visible (opacity=0).
		/// </summary>
		public float MinOpacitySpeed;
		/// <summary>
		/// Over this speed the foam has full opacity.
		/// </summary>
		public float FullOpacitySpeed;
	}

	public enum WakeType
	{
		Type1,
		Type2,
		Type3
	}
}