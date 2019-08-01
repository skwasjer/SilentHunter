/* 
 * SHControllers.act - LightHallo
 *
 * © 2007-2016 skwas. All rights reserved.
 * This code is provided as is. Change at your own risk.
 * --------------------------------------------------
 * 
 * S3D template for the LightHallo controller of Silent Hunter.
 * 
 * For documentation on templates, requirements and restrictions, please refer to the documentation.
 * 
*/

using SilentHunter.Controllers;

namespace SHControllers
{
	/// <summary>
	/// LightHallo render controller.
	/// </summary>
	public class LightHallo
		: Controller
	{
		/// <summary>
		/// Size.
		/// </summary>
		public float Size;
		/// <summary>
		/// Is day light.
		/// </summary>
		public bool IsDayLight;
	}
}