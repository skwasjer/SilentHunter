/* 
 * SHSound.act - SoundProxyStop
 *
 * © 2007-2016 skwas. All rights reserved.
 * This code is provided as is. Change at your own risk.
 * --------------------------------------------------
 * 
 * S3D template for the SoundProxyStop controller of Silent Hunter.
 * 
 * For documentation on templates, requirements and restrictions, please refer to the documentation.
 * 
*/

using SilentHunter.Dat;

namespace SHSound
{
	/// <summary>
	/// SoundProxyStop controller.
	/// </summary>
	public class SoundProxyStop
		: Controller
	{
		/// <summary>
		/// Identifier of the sound to use in sonar mode.
		/// </summary>
		public string SoundIdentifier;
		/// <summary>
		/// 
		/// </summary>
		public int Category;
	}
}