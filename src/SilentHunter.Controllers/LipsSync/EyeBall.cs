/* 
 * LipsSync.act - EyeBall
 *
 * © 2007-2016 skwas. All rights reserved.
 * This code is provided as is. Change at your own risk.
 * --------------------------------------------------
 * 
 * S3D template for the EyeBall controller of Silent Hunter.
 * 
 * For documentation on templates, requirements and restrictions, please refer to the documentation.
 * 
*/

using SilentHunter.Dat;

namespace LipsSync
{
	/// <summary>
	/// EyeBall controller.
	/// </summary>
	public class EyeBall
		: Controller
	{
		/// <summary>
		/// Up limit eyeball angle.
		/// </summary>
		public float UpLimit;
		/// <summary>
		/// Down limit eyeball angle.
		/// </summary>
		public float DownLimit;
		/// <summary>
		/// Right/left limit eyeball angle.
		/// </summary>
		public float RightLeftLimit;
		/// <summary>
		/// Time to move the eyeball to next target.
		/// </summary>
		public float TargetToTarget;
	}
}