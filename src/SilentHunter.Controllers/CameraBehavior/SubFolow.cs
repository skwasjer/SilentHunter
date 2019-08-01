/* 
 * CameraBehavior.act - SubFolow
 *
 * © 2007-2016 skwas. All rights reserved.
 * This code is provided as is. Change at your own risk.
 * --------------------------------------------------
 * 
 * S3D template for the SubFolow controller of Silent Hunter.
 * 
 * For documentation on templates, requirements and restrictions, please refer to the documentation.
 * 
*/

using SilentHunter.Controllers;

namespace CameraBehavior
{
	/// <summary>
	/// SubFolow controller. Follow the submarin accordingly with parameters.
	/// </summary>
	public class SubFolow
		: Controller
	{
		/// <summary>
		/// The height above water [0.1m].
		/// </summary>
		public float Y;
	}
}