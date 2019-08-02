/* 
 * SHControllers.act - UrbanismData
 *
 * © 2007-2016 skwas. All rights reserved.
 * This code is provided as is. Change at your own risk.
 * --------------------------------------------------
 * 
 * S3D template for the UrbanismData controller of Silent Hunter.
 * 
 * For documentation on templates, requirements and restrictions, please refer to the documentation.
 * 
*/

using SilentHunter.Controllers;

namespace SHControllers
{
	/// <summary>
	/// UrbanismData controller.
	/// </summary>
	public class UrbanismData
		: Controller
	{
		/// <summary>
		/// Minimum value of the urbanism prcentage at which the object is included [0-100].
		/// </summary>
		public float UrbanismPercentageIntervalMin;
		/// <summary>
		/// Maximum value of the urbanism prcentage at which the object is included [0-100].
		/// </summary>
		public float UrbanismPercentageIntervalMax;
		/// <summary>
		/// Percentage of the bounding box used to sink the object into the ground at minimum value [0-100].
		/// </summary>
		public float SinkValueMin;
		/// <summary>
		/// Percentage of the bounding box used to sink the object into the ground at maximum value [0-100].
		/// </summary>
		public float SinkValueMax;
	}
}