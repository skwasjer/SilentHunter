/* 
 * SHSim.act - wpn_SubTorpedoSys
 *
 * © 2007-2016 skwas. All rights reserved.
 * This code is provided as is. Change at your own risk.
 * --------------------------------------------------
 * 
 * S3D template for the wpn_SubTorpedoSys controller of Silent Hunter.
 * 
 * For documentation on templates, requirements and restrictions, please refer to the documentation.
 * 
*/

using System.Collections.Generic;
using SilentHunter.Controllers;
using SilentHunter.Controllers.Decoration;

namespace SHSim
{
	/// <summary>
	/// wpn_SubTorpedoSys render controller.
	/// </summary>
	public class wpn_SubTorpedoSys
		: BehaviorController
	{
		/// <summary>
		/// Automatically open the tube doors before torpedo launch.
		/// </summary>
		public bool auto_open;
		/// <summary>
		/// Torpedo evacuating time [s].
		/// </summary>
		public float eject_time;
		/// <summary>
		/// The torpedo's eject effect.
		/// </summary>
		public ulong eject_effect;
		/// <summary>
		/// The submarine's torpedo tubes.
		/// </summary>
		public List<Tube> Tubes;
		/// <summary>
		/// The torpedo rooms settings.
		/// </summary>
		public TorpedoRooms Rooms;
		/// <summary>
		/// The torpedo rooms settings.
		/// </summary>
		public ExternalStorages External;
	}

	[SHType]
	public class Tube
	{
		/// <summary>
		/// The tube's door.
		/// </summary>
		public string door;
		/// <summary>
		/// The tube's external door, if any.
		/// </summary>
		public string ext_door;
		/// <summary>
		/// The initial tube's torpedo type.
		/// </summary>
		public TorpedoType torpedo;
	}

	[SHType]
	public class TorpedoRooms
	{
		/// <summary>
		/// The fore torpedo room settings.
		/// </summary>
		public TorpedoRoom Fore;
		/// <summary>
		/// The aft torpedo room settings.
		/// </summary>
		public TorpedoRoom Aft;
	}

	[SHType]
	public class TorpedoRoom
	{
		/// <summary>
		/// The torpedo storage.
		/// </summary>
		public List<TorpedoType> Storage;
		/// <summary>
		/// Tube load time [min].
		/// </summary>
		public float load_time;
	}

	[SHType]
	public class ExternalStorages
	{
		/// <summary>
		/// The fore external torpedo storage settings.
		/// </summary>
		public ExternalStorage Fore;
		/// <summary>
		/// The aft external torpedo storage settings.
		/// </summary>
		public ExternalStorage Aft;
	}

	[SHType]
	public class ExternalStorage
	{
		/// <summary>
		/// The torpedo storage.
		/// </summary>
		public List<TorpedoType> Storage;
		/// <summary>
		/// Internal transfer time [min].
		/// </summary>
		public float xfer_time;
	}
}