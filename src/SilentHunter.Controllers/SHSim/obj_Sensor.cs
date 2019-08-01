/* 
 * SHSim.act - obj_Sensor
 *
 * © 2007-2016 skwas. All rights reserved.
 * This code is provided as is. Change at your own risk.
 * --------------------------------------------------
 * 
 * S3D template for the obj_Sensor controller of Silent Hunter.
 * 
 * For documentation on templates, requirements and restrictions, please refer to the documentation.
 * 
*/

using SilentHunter.Controllers;

namespace SHSim
{
	/// <summary>
	/// obj_Sensor controller.
	/// </summary>
	public class obj_Sensor
		: Controller
	{
		/// <summary>
		/// The type of the sensor.
		/// </summary>
		public SHControllers.SensorType Type;
		/// <summary>
		/// The minimum range of the sensor [m].
		/// </summary>
		public float MinRange;
		/// <summary>
		/// The maximum range of the sensor [m].
		/// </summary>
		public float MaxRange;
		/// <summary>
		/// The minimum height of the sensor [m].
		/// </summary>
		public float MinHeight;
		/// <summary>
		/// The maximum height of the sensor [m].
		/// </summary>
		public float MaxHeight;
		/// <summary>
		/// The minimum bearing of the sensor [deg].
		/// </summary>
		public float MinBearing;
		/// <summary>
		/// The maximum bearing of the sensor [deg].
		/// </summary>
		public float MaxBearing;
		/// <summary>
		/// The minimum elevation of the sensor [deg].
		/// </summary>
		public float MinElevation;
		/// <summary>
		/// The maximum elevation of the sensor [deg].
		/// </summary>
		public float MaxElevation;
		/// <summary>
		/// The minimum surface of the detected object [m2].
		/// </summary>
		public float MinSurface;
		/// <summary>
		/// The sensity of the sensor. At (Sensitivity * MaxRange) distance we have a double detection time. If 0, then the value from sim.cfg file is taken.
		/// </summary>
		public float Sensitivity;
	}
}