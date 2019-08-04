/* 
 * SHControllers.act - WatchMan
 *
 * © 2007-2016 skwas. All rights reserved.
 * This code is provided as is. Change at your own risk.
 * --------------------------------------------------
 * 
 * S3D template for the WatchMan controller of Silent Hunter.
 * 
 * For documentation on templates, requirements and restrictions, please refer to the documentation.
 * 
*/

using SilentHunter.Controllers;

namespace SHControllers
{
	/// <summary>
	/// WatchMan controller.
	/// </summary>
	public class WatchMan
		: BehaviorController
	{
		/// <summary>
		/// Left limit.
		/// </summary>
		public float LeftLimit;
		/// <summary>
		/// Right limit.
		/// </summary>
		public float RightLimit;
		/// <summary>
		/// Initial angle inside the sector.
		/// </summary>
		public float IniAngle;
		/// <summary>
		/// Rotation speed.
		/// </summary>
		public float RotSpeed;
		/// <summary>
		/// Initially active or not.
		/// </summary>
		public bool Active;
	}
}