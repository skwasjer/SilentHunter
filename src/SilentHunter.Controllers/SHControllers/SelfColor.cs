/* 
 * SHControllers.act - SelfColor
 *
 * © 2007-2016 skwas. All rights reserved.
 * This code is provided as is. Change at your own risk.
 * --------------------------------------------------
 * 
 * S3D template for the SelfColor controller of Silent Hunter.
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
	/// SelfColor render controller.
	/// </summary>
	public class SelfColor
		: Controller
	{
		/// <summary>
		/// Variable.
		/// </summary>
		public SelfColorVariable Variable;
		/// <summary>
		/// List of colors.
		/// </summary>
		public List<ColorsList> ColorsList;		
	}

	public enum SelfColorVariable
	{
		Tube1,
		Tube2,
		Tube3,
		Tube4,
		Tube5,
		Tube6,
		FaTOrLuT,
		RechargePort,
		RechargeStarb,
		LowBatteryFore,
		LowBatteryAft,
		AutoTgt
	}

	[SHType]
	public class ColorsList
	{
		/// <summary>
		/// Red color component.
		/// </summary>
		public int Red;
		/// <summary>
		/// Green color component.
		/// </summary>
		public int Green;
		/// <summary>
		/// Blue color component.
		/// </summary>
		public int Blue;
		/// <summary>
		/// Alpha component.
		/// </summary>
		public int Alpha;
		/// <summary>
		/// Strength.
		/// </summary>
		public float Strength;
	}
}