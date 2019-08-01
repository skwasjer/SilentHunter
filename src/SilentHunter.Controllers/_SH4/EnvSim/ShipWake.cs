/* 
 * EnvSim.act - ShipWake
 *
 * © 2007-2016 skwas. All rights reserved.
 * This code is provided as is. Change at your own risk.
 * --------------------------------------------------
 * 
 * S3D template for the ShipWake controller of Silent Hunter.
 * 
 * For documentation on templates, requirements and restrictions, please refer to the documentation.
 * 
*/

using SilentHunter.Controllers.Decoration;
using SilentHunter.Controllers;

namespace SH4.EnvSim
{
	/// <summary>
	/// ShipWake render controller.
	/// </summary>
	public class ShipWake
		: Controller
	{
		/// <summary>
		/// Start offset from the dummy position.
		/// </summary>
		public float StartOffset;
		/// <summary>
		/// Length of the wake.
		/// </summary>
		public float Length;
		/// <summary>
		/// Fixed length.
		/// </summary>
		public float FixedLength;		
		/// <summary>
		/// Front width of the wake.
		/// </summary>
		public float FrontWidth;
		/// <summary>
		/// Aperture angle of the wake (degrees).
		/// </summary>
		public float ApertureAngle;
		/// <summary>
		/// Altitude when the wake fades out if the source is higher.
		/// </summary>
		[Optional]
		public float? MaxAltitude;
		/// <summary>
		/// Altitude when the wake fades out if the source is lower.
		/// </summary>
		[Optional]
		public float? MinAltitude;
		/// <summary>
		/// 
		/// </summary>
		[Optional]
		public bool? UseRootAltitude;
		/// <summary>
		/// 
		/// </summary>
		[Optional]
		public float? MaxOpacitySpeed;
		/// <summary>
		/// The texture used for boat wake shape.
		/// </summary>
		public string WakeTextureName;
	}
}