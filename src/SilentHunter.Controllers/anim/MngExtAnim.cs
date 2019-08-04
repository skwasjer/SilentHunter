/* 
 * anim.act - MngExtAnim
 *
 * © 2007-2016 skwas. All rights reserved.
 * This code is provided as is. Change at your own risk.
 * --------------------------------------------------
 * 
 * S3D template for the MngExtAnim controller of Silent Hunter.
 * 
 * For documentation on templates, requirements and restrictions, please refer to the documentation.
 * 
*/

using System.Collections.Generic;
using SilentHunter.Controllers;
using SilentHunter.Controllers.Decoration;

namespace anim
{
	/// <summary>
	/// MngExtAnim controller.
	/// </summary>
	public class MngExtAnim
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
		[Optional]
		[SHVersion(SHVersions.SH3)]
		public string AnimName;
		/// <summary>
		/// Name of the animation group.
		/// </summary>
		[Optional]
		[SHVersion(SHVersions.SH4)]
		public string AnimGroup;
		/// <summary>
		/// List of animation names to link to.
		/// </summary>
		[Optional]
		[SHVersion(SHVersions.SH4)]
		public List<string> AnimList;
	}
}