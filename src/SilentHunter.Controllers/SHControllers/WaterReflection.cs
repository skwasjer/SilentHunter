/* 
 * SHControllers.act - WaterReflection
 *
 * © 2007-2016 skwas. All rights reserved.
 * This code is provided as is. Change at your own risk.
 * --------------------------------------------------
 * 
 * S3D template for the WaterReflection controller of Silent Hunter.
 * 
 * For documentation on templates, requirements and restrictions, please refer to the documentation.
 * 
*/

using SilentHunter.Controllers.Decoration;
using SilentHunter.Controllers;

namespace SHControllers
{
	/// <summary>
	/// WaterReflection render controller.
	/// </summary>
	public class WaterReflection
		: Controller
	{
		/// <summary>
		/// Water clipping height (under -20 for no clipping).
		/// </summary>
		public float ClipHeight;
		/// <summary>
		/// Above water reflection.
		/// </summary>
		public byte Reflection;
		/// <summary>
		/// Under water refraction.
		/// </summary>
		public byte Refraction;
		/// <summary>
		/// Use frustum test for object to eliminate reflection.
		/// </summary>
		public bool FrustumTest;
		/// <summary>
		/// 
		/// </summary>
		[Optional]
		public float? MaxVisDistance;
		/// <summary>
		/// Minimum screen dimension for reflection visibility (1 = screen dimension)
		/// </summary>
		[Optional]
		public float? MinVisDim;
		/// <summary>
		/// Maximum screen dimension for reflection visibility (1 = screen dimension)
		/// </summary>
		[Optional]
		public float? MaxVisDim;
	}	
}