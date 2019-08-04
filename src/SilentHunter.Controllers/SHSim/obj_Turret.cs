/* 
 * SHSim.act - obj_Turret
 *
 * © 2007-2016 skwas. All rights reserved.
 * This code is provided as is. Change at your own risk.
 * --------------------------------------------------
 * 
 * S3D template for the obj_Turret controller of Silent Hunter.
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
	/// obj_Turret controller.
	/// </summary>
	public class obj_Turret
		: BehaviorController
	{
		/// <summary>
		/// Debug display settings.
		/// </summary>
		public obj_TurretDebugInfo Debug;
		/// <summary>
		/// Visual train object.
		/// </summary>
		public string Train;
		/// <summary>
		/// Visual barrel object.
		/// </summary>
		public string Barrel;
		/// <summary>
		/// Traverse restrictions.
		/// </summary>
		public TraverseRestrictions Traverse;
		/// <summary>
		/// Elevation restrictions.
		/// </summary>
		public ElevationRestrictions Elevation;
		/// <summary>
		/// Fire restrictions.
		/// </summary>
		public List<FireRestrictions> Fire;
	}

	[SHType]
	public class obj_TurretDebugInfo
	{
		/// <summary>
		/// Display the turret's restrictions.
		/// </summary>
		public bool show_restr;
		/// <summary>
		/// Restrictions cone size [m].
		/// </summary>
		public float restr_dist;
	}

	[SHType]
	public class TraverseRestrictions
	{
		/// <summary>
		/// Minimum traverse (in degrees).
		/// </summary>
		public float min;
		/// <summary>
		/// Maximum traverse (in degrees).
		/// </summary>
		public float max;
		/// <summary>
		/// Traverse speed in degrees/sec.
		/// </summary>
		public float speed;
		/// <summary>
		/// </summary>
		public string anm_train;
		/// <summary>
		/// </summary>
		public string anm_human;
	}

	[SHType]
	public class ElevationRestrictions
	{
		/// <summary>
		/// Minimum elevation (in degrees).
		/// </summary>
		public float min;
		/// <summary>
		/// Maximum elevation (in degrees).
		/// </summary>
		public float max;
		/// <summary>
		/// Elevation speed in degrees/sec.
		/// </summary>
		public float speed;
		/// <summary>
		/// </summary>
		public string anm_barel;
		/// <summary>
		/// </summary>
		public string anm_human;
	}

	[SHType]
	public class FireRestrictions
	{
		/// <summary>
		/// </summary>
		public float trav_min;
		/// <summary>
		/// </summary>
		public float trav_max;
		/// <summary>
		/// </summary>
		public float elev_min;
		/// <summary>
		/// </summary>
		public float elev_max;
	}
}