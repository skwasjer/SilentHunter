/* 
 * LipsSync.act - LipsSyncMng
 *
 * © 2007-2016 skwas. All rights reserved.
 * This code is provided as is. Change at your own risk.
 * --------------------------------------------------
 * 
 * S3D template for the LipsSyncMng controller of Silent Hunter.

 * WARNING: DO NOT CHANGE THE TYPE/CLASS NAMES. I HAD TO HARDCODE A REFERENCE TO THEM.
 * Fields and descriptions may be changed though.
 * 
 * For documentation on templates, requirements and restrictions, please refer to the documentation.
 * 
*/

using SilentHunter.Controllers;
using System.Collections.Generic;

namespace LipsSync
{
	/// <summary>
	/// LipsSyncMng controller. Controls the lip synchronization of the NPC's.
	/// </summary>
	public class LipsSyncMng
		: RawController
	{
		/// <summary>
		/// The speech sound associated with this lipsync.
		/// </summary>
		public string Name;
		/// <summary>
		/// List of lip/mouth states.
		/// </summary>
		public List<LipsSyncFrame> Frames;	
	}

	public struct LipsSyncFrame
	{
		/// <summary>
		/// The time of the frame.
		/// </summary>
		public float Time;
		/// <summary>
		/// The state of the mouth.
		/// </summary>
		public LipState State;
	}

	public enum LipState
	{
		StateUnknown = -1,
		State1 = 1,
		State2 = 2,
		State3 = 3,
		State4 = 4,
		State5 = 5,
		State6 = 6,
		State7 = 7,
		State8 = 8,
	}
}