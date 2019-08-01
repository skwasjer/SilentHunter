/* 
 * SHControllers.act - AlertLight
 *
 * © 2007-2016 skwas. All rights reserved.
 * This code is provided as is. Change at your own risk.
 * --------------------------------------------------
 * 
 * S3D template for the AlertLight controller of Silent Hunter.
 * 
 * For documentation on templates, requirements and restrictions, please refer to the documentation.
 * 
*/

using SilentHunter;
using SilentHunter.Controllers;

namespace SHControllers
{
	/// <summary>
	/// AlertLight render controller.
	/// </summary>
	public class AlertLight : Controller
	{
		/// <summary>
		/// Light color.
		/// </summary>
		public Color LightColor;
		/// <summary>
		/// Intermittent period (0 = always ON).
		/// </summary>
		public float IntermittentPeriod;
		/// <summary>
		/// Flickering probability (0 = non flicking, 1 = intense flickering).
		/// </summary>
		public float RandomFlickering;
		/// <summary>
		/// Initial state.
		/// </summary>
		public bool InitialState;
		/// <summary>
		/// When true, alert light is visible when sub is surfaced.
		/// </summary>
		public bool VisibleAboveWater;
		/// <summary>
		/// When true, alert light is visible when sub is submerged.
		/// </summary>
		public bool VisibleUnderwater;
	}
}