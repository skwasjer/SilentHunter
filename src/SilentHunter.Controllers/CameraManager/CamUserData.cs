/* 
 * CameraManager.act - CamUserData
 *
 * © 2007-2016 skwas. All rights reserved.
 * This code is provided as is. Change at your own risk.
 * --------------------------------------------------
 * 
 * S3D template for the CamUserData controller of Silent Hunter.
 * 
 * For documentation on templates, requirements and restrictions, please refer to the documentation.
 * 
*/

using SilentHunter.Controllers;
using SilentHunter.Controllers.Decoration;

namespace CameraManager
{
	/// <summary>
	/// CamUserData controller.
	/// </summary>
	public class CamUserData
		: Controller
	{
		/// <summary>
		/// Camera will be linked by this object.
		/// </summary>
		public Cameras Camera;
		/// <summary>
		/// Camera to activate on right click.
		/// </summary>
		[Optional]
		public Cameras? PreviousCamera;
	}
}