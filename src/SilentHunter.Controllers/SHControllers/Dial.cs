/* 
 * SHControllers.act - Dial
 *
 * © 2007-2016 skwas. All rights reserved.
 * This code is provided as is. Change at your own risk.
 * --------------------------------------------------
 * 
 * S3D template for the Dial controller of Silent Hunter.
 * 
 * For documentation on templates, requirements and restrictions, please refer to the documentation.
 * 
*/

using SilentHunter.Dat;
using System.Runtime.InteropServices;
using SilentHunter;

namespace SHControllers
{
	/// <summary>
	/// Dial action controller.
	/// </summary>
	public class Dial
		: Controller
	{
		/// <summary>
		/// The type of the dial.
		/// </summary>
		public DialType Type;
		/// <summary>
		/// The display mode of the dial.
		/// </summary>
		public DialDisplay Display;
		/// <summary>
		/// Min and max dial display values (in degrees for circular dials or engine units for linear dials).
		/// </summary>
		public DialValues DispVal;
		/// <summary>
		/// Min and max real values (in specific units).
		/// </summary>
		public DialValues RealVal;
		/// <summary>
		/// Logarithmic factor. If zero then is a linear dial.
		/// </summary>
		public float LogFactor;
		/// <summary>
		/// Associated command.
		/// </summary>
		[FixedString(64)]
		[AutoCompleteList(@"Cfg\Commands.txt")]
		public string Command;
		/// <summary>
		/// Command on drag.
		/// </summary>
		public bool CmdOnDrag;
		/// <summary>
		/// Relative dragging.
		/// </summary>
		public bool RelativeDrag;
	}

	public struct DialValues
	{		
		public float Minimum;
		public float Maximum;
	}

	/// <summary>
	/// Enumeration of dial display modes.
	/// </summary>
	public enum DialDisplay
	{
		Circular,
		Linear
	}

	/// <summary>
	/// Enumeration of dial types.
	/// </summary>
	public enum DialType
	{
		Depth,
		Speed,
		PortEngineRpm,
		StarbEngineRpm,
		Throttle,
		Rudder,
		ForeDivepl,
		AftDivepl,
		ClockHour,
		ClockMinute,
		WaterLevel,
		Trim,
		Gyrocompass,
		ChronoSec,
		ChronoMin,
		SalvoMode,
		TorpDepth,
		TorpSpeed,
		TorpSpeedIdx,
		TorpPistol,
		TorpStrRun,
		Torp2ndGyroangle,
		TorpLeg,
		TorpPatternAngle,
		SolBearing,
		SolBearing10ths,
		SolRange,
		SolAngonbow,
		SolSpeed,
		SolGyroangle,
		SolGyroangle10ths,
		TubesSingleSel,
		TubesSalvoSel,
		SpreadAngle,
		Fuel,
		ComprAir,
		Batteries,
		ForeBatteries,
		AftBatteries,
		Co2,
		TorpEstimSec,
		TorpEstimMin,
		TgtAngOnBow,
		ThrottlePort,
		ThrottleStarb,
		DepthUnderSubKeel,
		DepthUnderSubKeel10ths,
		Hydrophone,
		OpenTube1,
		OpenTube2,
		OpenTube3,
		OpenTube4,
		OpenTube5,
		OpenTube6,
		RadarAngle,
		RadarOnoff,
		RadarSweep,
		RadarRange,
		RadarRngDigit1,
		RadarRngDigit2,
		RadarRngDigit3,
		RadarRngDigit4,
		Dummy,
		HydrophoneVol,
		ThrottleEscort,
		RealspreadAngle,
		SolTrackangle,
		GyrocompassEscort,
		TgtRange,
		TgtSpeed,
		TgtMastHeightInternational,
		SonarRange,
		ChronometerFirstHand,
		ChronometerSecondHand,
		RadioRandomIndicator,
		DepthInternational,
		SolRangeInternational,
		TorpDepthInternational,
		PeriscopeDepthInternational,
		TestDepthInternational,
		ClockSecond
	}
}