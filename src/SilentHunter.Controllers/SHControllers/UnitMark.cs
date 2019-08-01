/* 
 * SHControllers.act - UnitMark
 *
 * © 2007-2016 skwas. All rights reserved.
 * This code is provided as is. Change at your own risk.
 * --------------------------------------------------
 * 
 * S3D template for the UnitMark controller of Silent Hunter.
 * 
 * For documentation on templates, requirements and restrictions, please refer to the documentation.
 * 
*/

using SilentHunter.Controllers;
using SilentHunter.Controllers.Decoration;

namespace SHControllers
{
	/// <summary>
	/// UnitMark controller.
	/// </summary>
	public class UnitMark
		: Controller
	{
		/// <summary>
		/// Tga file.
		/// </summary>
		[FixedString(260)]
		public string Tga_file;
		/// <summary>
		/// Mark X dimension (in meters).
		/// </summary>
		public float XDim;
		/// <summary>
		/// Mark Y dimension (in meters).
		/// </summary>
		public float YDim;
		/// <summary>
		/// Mark will be displayed when object is below this depth (in meters).
		/// </summary>
		public float Depth;
	}
}