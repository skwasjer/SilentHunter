/* 
 * SHControllers.act - EffectChoices
 *
 * © 2007-2016 skwas. All rights reserved.
 * This code is provided as is. Change at your own risk.
 * --------------------------------------------------
 * 
 * S3D template for the EffectChoices controller of Silent Hunter.
 * 
 * For documentation on templates, requirements and restrictions, please refer to the documentation.
 * 
*/

using SilentHunter.Controllers;

namespace SHControllers
{
	/// <summary>
	/// EffectChoices controller. Replace the object with another one. Note: EffectChoices controller must be first on an object!
	/// </summary>
	public class EffectChoices
		: BehaviorController
	{
		public WaterCondition Water;
	}

	/// <summary>
	/// Water Condition
	/// </summary>
	public class WaterCondition
	{
		/// <summary>
		/// The condition is true when position is lower than height.
		/// </summary>
		public bool Under;
		/// <summary>
		/// Height interaction.
		/// </summary>
		public float Height;
		/// <summary>
		/// 3D object reference.
		/// </summary>
		public ulong Alternative;
	}
}