/* 
 * anim.act - ExtAnimCtl
 *
 * © 2007-2016 skwas. All rights reserved.
 * This code is provided as is. Change at your own risk.
 * --------------------------------------------------
 * 
 * S3D template for the ExtAnimCtl controller of Silent Hunter.
 * 
 * For documentation on templates, requirements and restrictions, please refer to the documentation.
 * 
*/

using SilentHunter.Controllers;

namespace anim
{
	/// <summary>
	/// ExtAnimCtl controller.
	/// </summary>
	public class ExtAnimCtl
		: BehaviorController
	{
		/// <summary>
		/// Minimum value to map to zero time of ctl.
		/// </summary>
		public float MinValue;
		/// <summary>
		/// Maximum value to map to end time of ctl.
		/// </summary>
		public float MaxValue;
		/// <summary>
		/// Name of the animation to control.
		/// </summary>
		public string AnimName;
	}
}