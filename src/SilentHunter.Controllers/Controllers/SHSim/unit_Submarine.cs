/* 
 * SHSim.act - unit_Submarine
 *
 * © 2007-2016 skwas. All rights reserved.
 * This code is provided as is. Change at your own risk.
 * --------------------------------------------------
 * 
 * S3D template for the unit_Submarine controller of Silent Hunter.
 * 
 * For documentation on templates, requirements and restrictions, please refer to the documentation.
 * 
*/

using SilentHunter.Dat;
using System.Collections.Generic;

namespace SHSim
{
	/// <summary>
	/// unit_Submarine controller.
	/// </summary>
	public class unit_Submarine
		: Controller
	{
		/// <summary>
		/// 
		/// </summary>
		public unit_Ship unit_Ship;
		/// <summary>
		/// Front diveplane settings.
		/// </summary>
		public DivePlane Front_diveplane;
		/// <summary>
		/// Rear diveplane settings.
		/// </summary>
		public DivePlane Rear_diveplane;
		/// <summary>
		/// Electric propulsion settings.
		/// </summary>
		public SubPropulsion E_propulsion;
		/// <summary>
		/// Hydrogen peroxide propulsion settings. (patch 1.5)
		/// </summary>
		[Optional]
		public SubPropulsion HydroPeroxid_propulsion;
		/// <summary>
		/// Range settings.
		/// </summary>
		public SubRanges Ranges;
		/// <summary>
		/// Ballast tanks settings.
		/// </summary>
		public Ballast Ballast;
	}

	[SHType]
	public class DivePlane
	{
		/// <summary>
		/// Visual diveplane objects.
		/// </summary>
		public List<string> Objects;
		/// <summary>
		/// Diveplane efficiency coeficient.
		/// </summary>
		public float drag;
		/// <summary>
		/// Propulsion (propeller) influence factor.
		/// </summary>
		public float prop_fact;
	}

	[SHType]
	public class SubPropulsion
		: Controller
	{
		/// <summary>
		/// Max submerged speed [kt].
		/// </summary>
		public float max_speed;
		/// <summary>
		/// Engine's max power [shp].
		/// </summary>
		public float eng_power;
		/// <summary>
		/// Engine's max rotation/min [rpm].
		/// </summary>
		public float eng_rpm;
	}

	[SHType]
	public class Ballast
	{
		/// <summary>
		/// Max main balast tanks flood speed [l/s].
		/// </summary>
		public float ManBT_flood_speed;
		/// <summary>
		/// Max dive ballast tanks flood speed [l/s].
		/// </summary>
		[Optional, SHVersion(SHVersions.SH4)]
		public float? DiveBT_flood_speed;
		/// <summary>
		/// Specifies whether the sub is diesel-electric or normal.
		/// </summary>
		[Optional, SHVersion(SHVersions.SH4)]
		public bool? Is_DieselElectric;
		/// <summary>
		/// Specifies whether the sub has hydrogen peroxide propulsion. (patch 1.5)
		/// </summary>
		[Optional, SHVersion(SHVersions.SH4)]
		public bool? HasHydrogenPeroxidProp;
	}

	[SHType]
	public class SubRanges
	{
		/// <summary>
		/// Surfaced range parameters.
		/// </summary>
		public SubRange Surfaced;
		/// <summary>
		/// Submerged range parameters.
		/// </summary>
		public SubRange Submerged;
		/// <summary>
		/// Hydrogen peroxide range parameters. (patch 1.4)
		/// </summary>
		[Optional]
		public SubRange HydroPeroxidPropulsion;
	}

	[SHType]
	public class SubRange
		: Controller
	{
		/// <summary>
		/// Maximum range in miles.
		/// </summary>
		public float miles;
		/// <summary>
		/// Speed for the given range [kt].
		/// </summary>
		public float knots;
	}
}