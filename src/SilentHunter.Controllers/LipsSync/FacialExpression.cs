/* 
 * LipsSync.act - FacialExpression
 *
 * © 2007-2016 skwas. All rights reserved.
 * This code is provided as is. Change at your own risk.
 * --------------------------------------------------
 * 
 * S3D template for the FacialExpression controller of Silent Hunter.
 * 
 * For documentation on templates, requirements and restrictions, please refer to the documentation.
 * 
*/

using System.Collections.Generic;
using SilentHunter.Controllers;

namespace LipsSync
{
	/// <summary>
	/// FacialExpression controller.
	/// </summary>
	public class FacialExpression
		: BehaviorController
	{
		/// <summary>
		/// Interpolation time between expressions.
		/// </summary>
		public float InterpTime;
		/// <summary>
		/// Blink lowest time interval.
		/// </summary>
		public float LowTime;
		/// <summary>
		/// Blink highest time interval.
		/// </summary>
		public float HighTime;
		/// <summary>
		/// Time for eyelid to close.
		/// </summary>
		public float SpeedDown;
		/// <summary>
		/// Time for eyelid to open.
		/// </summary>
		public float SpeedUp;
		/// <summary>
		/// Time the eyelid is closed.
		/// </summary>
		public float StayDown;
		/// <summary>
		/// List of expression names.
		/// </summary>
		public List<string> ExpressionNames;
	}
}