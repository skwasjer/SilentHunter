/* 
 * SHSim.act - obj_Extensible
 *
 * © 2007-2016 skwas. All rights reserved.
 * This code is provided as is. Change at your own risk.
 * --------------------------------------------------
 * 
 * S3D template for the obj_Extensible controller of Silent Hunter.
 * 
 * For documentation on templates, requirements and restrictions, please refer to the documentation.
 * 
*/

using SilentHunter.Controllers;

namespace SHSim
{
	/// <summary>
	/// obj_Extensible controller.
	/// </summary>
	public class obj_Extensible
		: BehaviorController
	{
		/// <summary>
		/// The extensible's type.
		/// </summary>
		public ExtensibleType type;
		/// <summary>
		/// Max extended height [m].
		/// </summary>
		public float max_height;
		/// <summary>
		/// Extension time [s].
		/// </summary>
		public float ext_time;
	}

	public enum ExtensibleType
	{
		AttackPeriscope,
		ObservationPeriscope,
		SkyPeriscope,
		RadioAntenna,
		RadarAntenna,
		Snorkel
	}
}