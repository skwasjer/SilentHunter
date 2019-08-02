/* 
 * CameraBehavior.act - CameraParams
 *
 * © 2007-2016 skwas. All rights reserved.
 * This code is provided as is. Change at your own risk.
 * --------------------------------------------------
 * 
 * S3D template for the CameraParams controller of Silent Hunter.
 * 
 * For documentation on templates, requirements and restrictions, please refer to the documentation.
 * 
*/

using SilentHunter.Controllers;
using SilentHunter.Controllers.Decoration;

namespace CameraBehavior
{
	/// <summary>
	/// CameraParams action controller.
	/// </summary>
	public class CameraParams
		: Controller
	{
		/// <summary>
		/// The angular angle [deg].
		/// </summary>
		public float AngularAngle;
		/// <summary>
		/// The clipping distance [m].
		/// </summary>
		public float ClipDistance;
		/// <summary>
		/// The maximum distance on Z axis [m].
		/// </summary>
		public float FarDistance;
		/// <summary>
		/// Receive action if game in pause mode.
		/// </summary>
		public bool FreeMode;
		/// <summary>
		/// Use to specify the interior.
		/// </summary>
		public CameraType Environment;
		/// <summary>
		/// Normalized viewport coordinates.
		/// </summary>
		public Rectangle Viewport;
	}

	public enum CameraType
	{
		Interior = 1,
		Periscope,
		Exterior
	}

	[SHType]
	public class Rectangle
	{
		/// <summary>
		/// Left.
		/// </summary>
		public float Left;
		/// <summary>
		/// Right.
		/// </summary>
		public float Right;
		/// <summary>
		/// Top.
		/// </summary>
		public float Top;
		/// <summary>
		/// Bottom.
		/// </summary>
		public float Bottom;
	}
}