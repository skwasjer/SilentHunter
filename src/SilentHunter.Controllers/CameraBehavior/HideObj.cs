/* 
 * CameraBehavior.act - HideObj
 *
 * © 2007-2016 skwas. All rights reserved.
 * This code is provided as is. Change at your own risk.
 * --------------------------------------------------
 * 
 * S3D template for the HideObj controller of Silent Hunter.
 * 
 * For documentation on templates, requirements and restrictions, please refer to the documentation.
 * 
*/

using System.Collections.Generic;
using CameraManager;
using SilentHunter.Controllers;

namespace CameraBehavior
{
	/// <summary>
	/// HideObj controller. Hide this object (with its subhierarchy) if the active camera is in the cameras list.
	/// </summary>
	public class HideObj
		: BehaviorController
	{
		/// <summary>
		/// Camera will be linked by this object.
		/// </summary>
		public List<Cameras> CamerasList;
	}
}