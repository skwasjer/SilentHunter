/* 
 * SH3Controllers.act - ObjectRemains
 *
 * © 2007-2016 skwas. All rights reserved.
 * This code is provided as is. Change at your own risk.
 * --------------------------------------------------
 * 
 * S3D template for the ObjectRemains controller of Silent Hunter.
 * 
 * For documentation on templates, requirements and restrictions, please refer to the documentation.
 * 
*/

using SilentHunter.Controllers;

namespace SH3.SH3Collisions
{
	/// <summary>
	/// ObjectRemains controller. The visual effects when destroying the object.
	/// </summary>
	public class ObjectRemains
		: BehaviorController
	{
		/// <summary>
		/// The name of the object to remain instead.
		/// </summary>
		public string Object;
	}
}