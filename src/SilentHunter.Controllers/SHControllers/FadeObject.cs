/* 
 * SHControllers.act - FadeObject
 *
 * © 2007-2016 skwas. All rights reserved.
 * This code is provided as is. Change at your own risk.
 * --------------------------------------------------
 * 
 * S3D template for the FadeObject controller of Silent Hunter.
 * 
 * For documentation on templates, requirements and restrictions, please refer to the documentation.
 * 
*/

using SilentHunter.Dat;

namespace SHControllers
{
	/// <summary>
	/// FadeObject controller. Fade out an object in specified time.
	/// </summary>
	public class FadeObject
		: Controller
	{
		/// <summary>
		/// Time to fade.
		/// </summary>
		public float Time;
		/// <summary>
		/// Delete owner object after fading.
		/// </summary>
		public bool DeleteMe;
	}
}