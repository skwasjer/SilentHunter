/* 
 * EffectManager.act - InteriorEffectsManager
 *
 * © 2007-2016 skwas. All rights reserved.
 * This code is provided as is. Change at your own risk.
 * --------------------------------------------------
 * 
 * S3D template for the InteriorEffectsManager controller of Silent Hunter.
 * 
 * For documentation on templates, requirements and restrictions, please refer to the documentation.
 * 
*/

using SilentHunter.Dat;
using System.Collections.Generic;

namespace EffectManager
{
	/// <summary>
	/// InteriorEffectsManager action controller.
	/// </summary>
	public class InteriorEffectsManager
		: Controller
	{
		/// <summary>
		/// Objects in this category.
		/// </summary>
		public List<Category> Categories;
		/// <summary>
		/// Total number of effects to play random.
		/// </summary>
		public int TotalToPlay;
	}

	[SHType]
	public class Category
	{
		/// <summary>
		/// Objects in this category.
		/// </summary>
		public List<ulong> Objects;
	}
}