/* 
 * SHControllers.act - Caustics
 *
 * © 2007-2016 skwas. All rights reserved.
 * This code is provided as is. Change at your own risk.
 * --------------------------------------------------
 * 
 * S3D template for the Caustics controller of Silent Hunter.
 * 
 * For documentation on templates, requirements and restrictions, please refer to the documentation.
 * 
*/

using SilentHunter.Controllers;

namespace SHControllers
{
	/// <summary>
	/// Caustics render controller.
	/// </summary>
	public class Caustics
		: Controller
	{
		/// <summary>
		/// Scale on local X-axis.
		/// </summary>
		public float ScaleX;
		/// <summary>
		/// Scale on local Y-axis.
		/// </summary>
		public float ScaleY;
		/// <summary>
		/// Offset on local X-axis.
		/// </summary>
		public float OffsetY;
		/// <summary>
		/// Scale on local Z-axis.
		/// </summary>
		public float ScaleZ;
		/// <summary>
		/// Animation frames / second.
		/// </summary>
		public float FPS;
	}
}