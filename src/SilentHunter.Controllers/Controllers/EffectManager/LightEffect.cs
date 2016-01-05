/* 
 * EffectManager.act - LightEffect
 *
 * © 2007-2016 skwas. All rights reserved.
 * This code is provided as is. Change at your own risk.
 * --------------------------------------------------
 * 
 * S3D template for the LightEffect controller of Silent Hunter.
 * 
 * For documentation on templates, requirements and restrictions, please refer to the documentation.
 * 
*/

using SilentHunter.Dat;
using System.Collections.Generic;

namespace EffectManager
{
	/// <summary>
	/// LightEffect controller.
	/// </summary>
	public class LightEffect
		: Controller
	{
		/// <summary>
		/// Objects in this category.
		/// </summary>
		public List<List<Effect>> Categories;
		/// <summary>
		/// The index of set to be played.
		/// </summary>
		public int SetToPlay;
	}

	[SHType]
	public class Effect
	{
		/// <summary>
		/// The multiplier to apply on light.
		/// </summary>
		public float Value;
		/// <summary>
		/// The time this multiplier is active.
		/// </summary>
		public float Time;
	}
}