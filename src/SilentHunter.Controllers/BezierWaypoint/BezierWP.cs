/* 
 * BezierWaypoint.act - BezierWP
 *
 * © 2007-2016 skwas. All rights reserved.
 * This code is provided as is. Change at your own risk.
 * --------------------------------------------------
 * 
 * S3D template for the BezierWP controller of Silent Hunter.
 * 
 * For documentation on templates, requirements and restrictions, please refer to the documentation.
 * 
*/

using SilentHunter.Controllers;
using System.Collections.Generic;
using SilentHunter;

namespace BezierWaypoint
{
	/// <summary>
	/// BezierWP controller.
	/// </summary>
	public class BezierWP
		: Controller
	{
		/// <summary>
		/// object->BezierWP. The next waypoint.
		/// </summary>
		public ulong Link;
		/// <summary>
		/// 
		/// </summary>
		public Vector3 Tangent;
		/// <summary>
		/// 
		/// </summary>
		public float Speed;
		/// <summary>
		/// Waiting time.
		/// </summary>
		public float WaitTime;
		/// <summary>
		/// Misc flags.
		/// </summary>
		public BezierWPFlags Flags;
		/// <summary>
		/// Generate events.
		/// </summary>
		public List<string> Events;
	}

	public enum BezierWPFlags
	{
		LinInterpSpeed = 1
	}

}