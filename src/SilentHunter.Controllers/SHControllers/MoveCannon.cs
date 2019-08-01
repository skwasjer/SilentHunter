/* 
 * SHControllers.act - MoveCannon
 *
 * © 2007-2016 skwas. All rights reserved.
 * This code is provided as is. Change at your own risk.
 * --------------------------------------------------
 * 
 * S3D template for the MoveCannon controller of Silent Hunter.
 * 
 * For documentation on templates, requirements and restrictions, please refer to the documentation.
 * 
*/

using SilentHunter.Controllers;

namespace SHControllers
{
	/// <summary>
	/// MoveCannon controller.
	/// </summary>
	public class MoveCannon
		: Controller
	{
		/// <summary>
		/// Look at horizon when zoomed.
		/// </summary>
		public bool LookAtHorizon;
		/// <summary>
		/// Minimum range (in meters).
		/// </summary>
		public float MinRange;
		/// <summary>
		/// Maximum range (in meters).
		/// </summary>
		public float MaxRange;
		/// <summary>
		/// Range step (in meters).
		/// </summary>
		public float RangeStep;
		/// <summary>
		/// Travel inertia.
		/// </summary>
		public float TravInert;
	}
}