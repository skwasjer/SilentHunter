/* 
 * EventCamera.act - EventProps
 *
 * © 2007-2016 skwas. All rights reserved.
 * This code is provided as is. Change at your own risk.
 * --------------------------------------------------
 * 
 * S3D template for the EventProps controller of Silent Hunter.
 * 
 * For documentation on templates, requirements and restrictions, please refer to the documentation.
 * 
*/

using SilentHunter.Dat;

namespace EventCamera
{
	/// <summary>
	/// EventProps controller.
	/// </summary>
	public class EventProps
		: Controller
	{
		/// <summary>
		/// Event managed by these waypoints.
		/// </summary>
		public int Event;
		/// <summary>
		/// Priority (higher value = higher priority).
		/// </summary>
		public int Priority;
		/// <summary>
		/// Interrupt same priority (conditional=on same key).
		/// </summary>
		public IntrSamePrio IntrSamePrio;
		/// <summary>
		/// Event lifetime.
		/// </summary>
		public float LifeTime;
		/// <summary>
		/// The name of the start waypoint for camera.
		/// </summary>
		public string WpCameraStart;
		/// <summary>
		/// The name of the start waypoint for target.
		/// </summary>
		public string WpTargetStart;
	}

	public enum IntrSamePrio
	{
		No,
		Yes,
		Conditional
	}
}