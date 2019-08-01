/* 
 * Sound.act - SoundSource
 *
 * © 2007-2016 skwas. All rights reserved.
 * This code is provided as is. Change at your own risk.
 * --------------------------------------------------
 * 
 * S3D template for the SoundSource controller of Silent Hunter.
 * 
 * For documentation on templates, requirements and restrictions, please refer to the documentation.
 * 
*/

using SilentHunter.Controllers;

namespace Sound
{
	/// <summary>
	/// SoundSource controller.
	/// </summary>
	public class SoundSource
		: Controller
	{
		/// <summary>
		/// Playlist name (also used to find properties in the library).
		/// </summary>
		public string Name;
		/// <summary>
		/// Playlist identifier.
		/// </summary>
		public string Identifier;		
	}
}