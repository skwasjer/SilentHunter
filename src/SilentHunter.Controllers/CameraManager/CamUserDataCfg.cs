/* 
 * CameraManager.act - CamUserDataCfg
 *
 * © 2007-2016 skwas. All rights reserved.
 * This code is provided as is. Change at your own risk.
 * --------------------------------------------------
 * 
 * S3D template for the CamUserDataCfg controller of Silent Hunter.
 * 
 * For documentation on templates, requirements and restrictions, please refer to the documentation.
 * 
*/

using SilentHunter.Controllers.Decoration;
using SilentHunter.Controllers;

namespace CameraManager
{
	/// <summary>
	/// CamUserDataCfg controller.
	/// </summary>
	public class CamUserDataCfg
		: Controller
	{
		/// <summary>
		/// The CamUserData parameters.
		/// </summary>
		public CamUserData CamUserData;
		/// <summary>
		/// The instance occurrence to be configured; 0 = all ocurrences.
		/// </summary>
		[Optional]
		public int? InstaceOccurence;
	}
}