/* 
 * SHControllers.act - MipMapBias
 *
 * © 2007-2016 skwas. All rights reserved.
 * This code is provided as is. Change at your own risk.
 * --------------------------------------------------
 * 
 * S3D template for the MipMapBias controller of Silent Hunter.
 * 
 * For documentation on templates, requirements and restrictions, please refer to the documentation.
 * 
*/

using SilentHunter.Dat;

namespace SHControllers
{
	/// <summary>
	/// MipMapBias render controller.
	/// </summary>
	public class MipMapBias
		: Controller
	{
		/// <summary>
		/// Mipmap bias.
		/// </summary>
		public float Bias;
	}
}