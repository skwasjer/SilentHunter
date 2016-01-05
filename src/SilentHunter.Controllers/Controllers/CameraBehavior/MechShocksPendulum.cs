/* 
 * CameraBehavior.act - MechShocksPendulum
 *
 * © 2007-2016 skwas. All rights reserved.
 * This code is provided as is. Change at your own risk.
 * --------------------------------------------------
 * 
 * S3D template for the MechShocksPendulum controller of Silent Hunter.
 * 
 * For documentation on templates, requirements and restrictions, please refer to the documentation.
 * 
*/

using SilentHunter.Dat;

namespace CameraBehavior
{
	/// <summary>
	/// MechShocksPendulum controller. Apply shocks on interior camera when collision &amp; hits occured.
	/// </summary>
	public class MechShocksPendulum
		: Controller
	{
		/// <summary>
		/// A pendulum controller which works on a mechanical object.
		/// </summary>
		public CMechPendulum CMechPendulum;
		/// <summary>
		/// The coeficient to apply on the shocks.
		/// </summary>
		public float Coef;
	}
}