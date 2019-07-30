﻿/* 
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

using System.Runtime.InteropServices;
using SilentHunter.Dat;


namespace anim
{
	/// <summary>
	/// Light animation controller data (subtype 0x200)
	/// </summary>
	[Controller(true, 0x200, true)]
	public class LightAnimation
		: RawController
	{
		/// <summary>
		/// A list of key frames.
		/// </summary>
		public RawList<LightKeyFrame, ushort> Frames;
	}

	[StructLayout(LayoutKind.Sequential)]
	public class LightKeyFrame
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