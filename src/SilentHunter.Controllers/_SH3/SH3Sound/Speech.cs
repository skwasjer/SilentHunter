/* 
 * SH3Sound.act - Speech
 *
 * © 2007-2016 skwas. All rights reserved.
 * This code is provided as is. Change at your own risk.
 * --------------------------------------------------
 * 
 * S3D template for the Speech controller of Silent Hunter.
 * 
 * For documentation on templates, requirements and restrictions, please refer to the documentation.
 * 
*/

using SilentHunter.Controllers;

namespace SH3.SH3Sound
{
	/// <summary>
	/// Speech action controller.
	/// </summary>
	public class Speech
		: Controller
	{
		/// <summary>
		/// Folder used as a base for voices.
		/// </summary>
		public string BaseFolder;
	}
}