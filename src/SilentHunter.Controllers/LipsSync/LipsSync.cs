/* 
 * LipsSync.act - LipsSync
 *
 * © 2007-2016 skwas. All rights reserved.
 * This code is provided as is. Change at your own risk.
 * --------------------------------------------------
 * 
 * S3D template for the LipsSync controller of Silent Hunter.
 * 
 * For documentation on templates, requirements and restrictions, please refer to the documentation.
 * 
*/

using SilentHunter.Controllers;
using System.Collections.Generic;

namespace LipsSync
{
	/// <summary>
	/// LipsSync controller.
	/// </summary>
	public class LipsSync
		: Controller
	{
		/// <summary>
		/// 
		/// </summary>
		public List<int> Phrases;	
		/// <summary>
		/// List of morph set names.
		/// </summary>
		public List<string> MorphSetNames;	
	}
}