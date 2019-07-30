/* 
 * SHSound.act - ShipSound
 *
 * © 2007-2016 skwas. All rights reserved.
 * This code is provided as is. Change at your own risk.
 * --------------------------------------------------
 * 
 * S3D template for the ShipSound controller of Silent Hunter.
 * 
 * For documentation on templates, requirements and restrictions, please refer to the documentation.
 * 
*/

using SilentHunter.Dat;

namespace SHSound
{
	/// <summary>
	/// ShipSound controller. Manages all sounds on a ship.
	/// </summary>
	public class ShipSound
		: Controller
	{		
		/// <summary>
		/// At standard RPM the sounds parameters are default.
		/// </summary>
		public float StdByMaxRpm;
		/// <summary>
		/// Distance where low sound detail is activated (engine units).
		/// </summary>
		public float LodDistance;
	}
}