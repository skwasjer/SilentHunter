/* 
 * © 2007-2016 skwas. All rights reserved.
 * This code is provided as is. Change at your own risk.
 * --------------------------------------------------
 * 
 * S3D template for SH-controllers that contain animation data.
 * 
 * WARNING: DO NOT CHANGE THE TYPE/CLASS NAMES. I HAD TO HARDCODE A REFERENCE TO THEM.
 * Fields and descriptions may be changed though.
 * 
*/

using System.Collections.Generic;
using SilentHunter.Dat;

namespace anim
{
	/// <summary>
	/// Light animation controller data (subtype 0x200)
	/// </summary>
	[Controller(0x200)]
	public class LightAnimation : AnimationController
	{
		/// <summary>
		/// A list of key frames.
		/// </summary>
		[CountType(typeof(ushort))]
		public List<LightKeyFrame> Frames;
	}

	public struct LightKeyFrame
	{
		/// <summary>
		/// The time of the key frame.
		/// </summary>
		public float Time;

		/// <summary>
		/// The new attenuation of the emitted light.
		/// </summary>
		public float Attenuation;
	}
}