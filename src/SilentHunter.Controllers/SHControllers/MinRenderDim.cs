/* 
 * SHControllers.act - MinRenderDim
 *
 * © 2007-2016 skwas. All rights reserved.
 * This code is provided as is. Change at your own risk.
 * --------------------------------------------------
 * 
 * S3D template for the MinRenderDim controller of Silent Hunter.
 * 
 * For documentation on templates, requirements and restrictions, please refer to the documentation.
 * 
*/

using SilentHunter.Controllers.Decoration;
using SilentHunter.Controllers;

namespace SHControllers
{
	/// <summary>
	/// MinRenderDim action controller.
	/// </summary>
	public class MinRenderDim
		: Controller
	{
		/// <summary>
		/// Minimum render dimension(bsphere diameter), in pixels.
		/// </summary>
		[ParseName("MinRenderDim")]
		public float MinRenderDimension;
	}
}