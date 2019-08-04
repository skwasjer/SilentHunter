/* 
 * SHSim.act - unit_Ship
 *
 * © 2007-2016 skwas. All rights reserved.
 * This code is provided as is. Change at your own risk.
 * --------------------------------------------------
 * 
 * S3D template for the unit_Ship controller of Silent Hunter.
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
	/// unit_Ship controller.
	/// </summary>
	public class unit_Ship
		: BehaviorController
	{
		/// <summary>
		/// </summary>
		public obj_Hydro obj_Hydro;
		/// <summary>
		/// Propulsion settings.
		/// </summary>
		public Propulsion Propulsion;
		/// <summary>
		/// Rudders settings.
		/// </summary>
		[Optional]
		public Rudders Rudders;
	}

	[SHType]
	public class Propulsion
	{
		/// <summary>
		/// Visual propeller objects.
		/// </summary>
		public List<string> Propellers;
		/// <summary>
		/// Ship's max speed [kt].
		/// </summary>
		public float max_speed;
		/// <summary>
		/// Engine's max force [Tons].
		/// </summary>
		public float max_force;
		/// <summary>
		/// Engine's max power [bhp].
		/// </summary>
		public float eng_power;
		/// <summary>
		/// Engine's max rotation/min [rpm].
		/// </summary>
		public float eng_rpm;
	}

	[SHType]
	public class Rudders
	{
		/// <summary>
		/// Visual rudder objects.
		/// </summary>
		public List<string> Objects;
		/// <summary>
		/// Rudders' efficiency coeficient.
		/// </summary>
		public float drag;
		/// <summary>
		/// Propulsion (propeller) influence factor.
		/// </summary>
		public float prop_fact;
	}
}