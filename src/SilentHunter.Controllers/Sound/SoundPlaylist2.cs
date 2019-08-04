/* 
 * Sound.act - SoundPlaylist2
 *
 * © 2007-2016 skwas. All rights reserved.
 * This code is provided as is. Change at your own risk.
 * --------------------------------------------------
 * 
 * S3D template for the SoundPlaylist2 controller of Silent Hunter.
 * 
 * For documentation on templates, requirements and restrictions, please refer to the documentation.
 * 
*/

using System.Collections.Generic;
using SilentHunter.Controllers;

namespace Sound
{
	/// <summary>
	/// Playlist controller.
	/// </summary>
	public class SoundPlaylist2
		: BehaviorController
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
		/// Will load the sound files in the specified directory (leave it blank to be ignored) (insert a path without the trailing '\').
		/// </summary>
		public string Directory;
		/// <summary>
		/// Play mode.
		/// </summary>
		public PlayMode PlayMode;
		/// <summary>
		/// Preload files when playlist created.
		/// </summary>
		public Preload Preload;
		/// <summary>
		/// How the files will be preloaded.
		/// </summary>
		public PreloadMode PreloadMode;
		/// <summary>
		/// What to do when song is needed, but it is not avaiable (not loaded yet).
		/// </summary>
		public WaitMode WaitMode;
		/// <summary>
		/// Delay between playing items - min time.
		/// </summary>
		public float DelayMin;
		/// <summary>
		/// Delay between playing items - max time.
		/// </summary>
		public float DelayMax;
		/// <summary>
		/// Set this to 0 if the sounds will never use the equalizer.
		/// </summary>
		public int CanUseEqualizer;
		/// <summary>
		/// Set this to 1 if the sounds will use, by default, the equalizer.
		/// </summary>
		public int UseEqualizer;
		/// <summary>
		/// Sound files for playlist.
		/// </summary>
		public List<string> Sounds;
	}

	public enum PlayMode
	{
		Single,
		Sequence,
		Shuffle
	}

	public enum Preload
	{
		NoPreload,
		FullPreload
	}

	public enum PreloadMode
	{
		Immediately,
		Thread,
		Update
	}

	public enum WaitMode
	{
		ForceLoad,
		WaitForSong
	}
}