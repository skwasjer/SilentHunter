/* 
 * SHControllers.act - PeriscopeCtrl
 *
 * © 2007-2016 skwas. All rights reserved.
 * This code is provided as is. Change at your own risk.
 * --------------------------------------------------
 * 
 * S3D template for the PeriscopeCtrl controller of Silent Hunter.
 * 
 * For documentation on templates, requirements and restrictions, please refer to the documentation.
 * 
*/

using SilentHunter.Controllers;

namespace SHControllers
{
	/// <summary>
	/// PeriscopeCtrl action controller.
	/// </summary>
	public class PeriscopeCtrl
		: Controller
	{
		/// <summary>
		/// The periscope.
		/// </summary>
		public SHSim.ExtensibleType Periscope;		
	}
}