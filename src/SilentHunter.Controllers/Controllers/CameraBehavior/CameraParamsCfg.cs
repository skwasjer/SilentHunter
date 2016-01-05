/* 
 * CameraBehavior.act - CameraParamsCfg
 *
 * © 2007-2016 skwas. All rights reserved.
 * This code is provided as is. Change at your own risk.
 * --------------------------------------------------
 * 
 * S3D template for the CameraParamsCfg controller of Silent Hunter.
 * 
 * For documentation on templates, requirements and restrictions, please refer to the documentation.
 * 
*/

using SilentHunter.Dat;

namespace CameraBehavior
{
	/// <summary>
	/// CameraParamsCfg action controller.
	/// </summary>
	public class CameraParamsCfg
		: Controller
	{
		/// <summary>
		/// The CameraParams parameters.
		/// </summary>
		public CameraParams CameraParams;
		/// <summary>
		/// The instance occurrence to be configured; 0 = all ocurrences.
		/// </summary>
		[Optional]
		public int? InstaceOccurence;

	}
}