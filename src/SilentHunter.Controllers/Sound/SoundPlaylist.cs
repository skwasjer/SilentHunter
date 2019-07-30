/* 
 * Sound.act - SoundPlaylist
 *
 * © 2007-2016 skwas. All rights reserved.
 * This code is provided as is. Change at your own risk.
 * --------------------------------------------------
 * 
 * S3D template for the SoundPlaylist controller of Silent Hunter.
 * 
 * For documentation on templates, requirements and restrictions, please refer to the documentation.
 * 
*/

using SilentHunter.Dat;
using System.Collections.Generic;

namespace Sound
{
	/// <summary>
	/// Playlist render controller.
	/// </summary>
	public class SoundPlaylist
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
		/// <summary>
		/// Play mode.
		/// </summary>
		public PlayMode PlayMode;
		/// <summary>
		/// Preload files when playlist created.
		/// </summary>
		public bool Preload;
		/// <summary>
		/// Delay between items.
		/// </summary>
		public float Delay;
		/// <summary>
		/// A random delay is added.
		/// </summary>
		public float RandomDelay;
		/// <summary>
		/// Sound files for playlist.
		/// </summary>
		public List<string> Sounds;
	}
}