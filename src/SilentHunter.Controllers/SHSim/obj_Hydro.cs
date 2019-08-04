/* 
 * SHSim.act - obj_Hydro
 *
 * © 2007-2016 skwas. All rights reserved.
 * This code is provided as is. Change at your own risk.
 * --------------------------------------------------
 * 
 * S3D template for the obj_Hydro controller of Silent Hunter.
 * 
 * For documentation on templates, requirements and restrictions, please refer to the documentation.
 * 
*/

using SilentHunter.Controllers;
using SilentHunter.Controllers.Decoration;

namespace SHSim
{
	/// <summary>
	/// obj_Hydro controller. Controls an object that can float.
	/// </summary>
	public class obj_Hydro
		: BehaviorController
	{
		/// <summary>
		/// Debug display settings.
		/// </summary>
		public obj_HydroDebugInfo Debug;
		/// <summary>
		/// The object's mass [Tons]. If 0, then the object's surfaced displacement is taken.
		/// </summary>
		public float mass;
		/// <summary>
		/// Gravity center height from the object's bottom [m]. If 0, then the half of geometric bounding box is taken.
		/// </summary>
		public float gc_height;
		/// <summary>
		/// Gravity center horizontal position [>0] (1=front, 0.5=middle, 0=rear).
		/// </summary>
		public float fr_ratio;
		/// <summary>
		/// Put the hydro object on the water at the surfaced draught.
		/// </summary>
		public bool put_on_water;
		/// <summary>
		/// The object's surface parameters.
		/// </summary>
		public obj_HydroParams Surfaced;
		/// <summary>
		/// The object's submerged parameters.
		/// </summary>
		public obj_HydroParams Submerged;
	}

	[SHType]
	public class obj_HydroParams
	{
		/// <summary>
		/// The object's displacement [Tons]. If 0 then the object's mass is taken.
		/// </summary>
		public float displacement;
		/// <summary>
		/// The object's draught[m]. If 0, then it is taken from the object's global position.
		/// </summary>
		public float draught;
		/// <summary>
		/// The object's drag (water resistance) coefs [F=C*v^2].
		/// </summary>
		public Drag drag;
	}

	[SHType]
	public class Drag
	{
		/// <summary>
		/// The object's left-right drag coef.
		/// </summary>
		public float LR;
		/// <summary>
		/// The object's up-down drag coef.
		/// </summary>
		public float UD;
		/// <summary>
		/// The object's front-rear drag coef.
		/// </summary>
		[Optional]
		public float? FR;
	}

	[SHType]
	public class obj_HydroDebugInfo
	{
		/// <summary>
		/// Display the object's parameters.
		/// </summary>
		[ParseName("params")]
		public bool parameters;
		/// <summary>
		/// Display the object's gravity center.
		/// </summary>
		public bool GC;
		/// <summary>
		/// Display the object's submerged draught.
		/// </summary>
		public bool SD;
	}
}