/* 
 * CameraBehavior.act - EditObjPos
 *
 * © 2007-2016 skwas. All rights reserved.
 * This code is provided as is. Change at your own risk.
 * --------------------------------------------------
 * 
 * S3D template for the EditObjPos controller of Silent Hunter.
 * 
 * For documentation on templates, requirements and restrictions, please refer to the documentation.
 * 
*/

using SilentHunter;
using SilentHunter.Controllers;

namespace CameraBehavior
{
	/// <summary>
	/// EditObjPos controller.
	/// </summary>
	public class EditObjPos
		: Controller
	{
		/// <summary>
		/// Set initial local position with coordinates specified in InitialLocalPos.
		/// </summary>
		public bool UseInitialLocalPos;
		/// <summary>
		/// The initial local position.
		/// </summary>
		public Vector3 InitialLocalPos;
	}

}