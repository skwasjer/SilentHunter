/* 
 * SHControllers.act - SensorData
 *
 * © 2007-2016 skwas. All rights reserved.
 * This code is provided as is. Change at your own risk.
 * --------------------------------------------------
 * 
 * S3D template for the SensorData controller of Silent Hunter.
 * 
 * For documentation on templates, requirements and restrictions, please refer to the documentation.
 * 
*/

using SilentHunter.Controllers;
using System.Collections.Generic;
using SilentHunter.Controllers.Decoration;

namespace SHControllers
{
	/// <summary>
	/// SensorData render controller.
	/// </summary>
	public class SensorData
		: Controller
	{
		/// <summary>
		/// Sensor type.
		/// </summary>
		public SensorType SensorType;
		/// <summary>
		/// Precise range (in meters).
		/// </summary>
		public float PreciseRange;
		/// <summary>
		/// Maximum range (in meters).
		/// </summary>
		public float MaxRange;
		/// <summary>
		/// Minimum detection height (in meters).
		/// </summary>
		public float MinHeight;
		/// <summary>
		/// Maximum detection height (in meters).
		/// </summary>
		public float MaxHeight;
		/// <summary>
		/// Minimum sensor working height (in meters).
		/// </summary>
		public float MinSensorHeight;
		/// <summary>
		/// Maximum sensor working height (in meters).
		/// </summary>
		public float MaxSensorHeight;
		/// <summary>
		/// Standard detectable surface at maximum range (in meters).
		/// </summary>
		public float Surface;
		/// <summary>
		/// Detection level at percentage of maximum RPM (between 0 and 1).
		/// </summary>
		public float RPMDetLevel;
		/// <summary>
		/// Full sweep period (in seconds).
		/// </summary>
		public float SweepPeriod;
		/// <summary>
		/// Sweep arc (in degrees, 0 if sensor does not need a sweep arc).
		/// </summary>
		public float SweepArc;
		/// <summary>
		/// Detection probability inside the sweep arc (0-1).
		/// </summary>
		public float ProbInsideArc;
		/// <summary>
		/// Sensor 3D Object revolving when sweeping.
		/// </summary>
		public bool Revolving;
		/// <summary>
		/// When sweeping sensor skip invalid sectors.
		/// </summary>
		public bool SkipSweep;
		/// <summary>
		/// Bearing sectors.
		/// </summary>
		public List<BearingMinMax> Bearing;
		/// <summary>
		/// Elevation sectors.
		/// </summary>
		public List<ElevationMinMax> Elevation;
	}

	[SHType]
	public class BearingMinMax
	{
		/// <summary>
		/// Minimum bearing (in degrees 0-360).
		/// </summary>
		public float BearingMin;
		/// <summary>
		/// Maximum bearing (in degrees 0-360).
		/// </summary>
		public float BearingMax;
	}

	[SHType]
	public class ElevationMinMax
	{
		/// <summary>
		/// Minimum elevation (in degrees 0-360).
		/// </summary>
		public float ElevationMin;
		/// <summary>
		/// Maximum elevation (in degrees 0-360).
		/// </summary>
		public float ElevationMax;
	}

	
	public enum SensorType
	{
		Visual,
		Radar,
		Sonar,
		Hydrophone,
		RadarWarning,
		RadioDF
	}

}