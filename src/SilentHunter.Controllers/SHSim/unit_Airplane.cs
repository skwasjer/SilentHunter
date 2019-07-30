/* 
 * SHSim.act - unit_Airplane
 *
 * � 2007-2016 skwas. All rights reserved.
 * This code is provided as is. Change at your own risk.
 * --------------------------------------------------
 * 
 * S3D template for the unit_Airplane controller of Silent Hunter.
 * 
 * For documentation on templates, requirements and restrictions, please refer to the documentation.
 * 
*/

using SilentHunter.Dat;
using System.Collections.Generic;

namespace SHSim
{
	/// <summary>
	/// unit_Airplane controller.
	/// </summary>
	public class unit_Airplane
		: Controller
	{
		/// <summary>
		/// unit_Ship parameters.
		/// </summary>
		public unit_Ship unit_Ship;
		/// <summary>
		/// Wings settings.
		/// </summary>
		public Wings Wings;
	}

	[SHType]
	public class Wings
	{
		/// <summary>
		/// Wings' efficiency coeficient.
		/// </summary>
		public float drag;
		/// <summary>
		/// Visual front left wing objects.
		/// </summary>
		public List<string> Left;
		/// <summary>
		/// Visual front right wing objects.
		/// </summary>
		public List<string> Right;
		/// <summary>
		/// Visual back wing objects.
		/// </summary>
		public List<string> Back;

	}
}