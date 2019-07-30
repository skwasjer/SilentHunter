/* 
 * SHCollisions.act - StaticObject
 *
 * © 2007-2016 skwas. All rights reserved.
 * This code is provided as is. Change at your own risk.
 * --------------------------------------------------
 * 
 * S3D template for the StaticObject controller of Silent Hunter.
 * 
 * For documentation on templates, requirements and restrictions, please refer to the documentation.
 * 
*/

using SilentHunter.Dat;

namespace SHCollisions
{
	/// <summary>
	/// StaticObject controller. Mark this object to be static inside scene.
	/// </summary>
	public class StaticObject
		: Controller
	{
		/// <summary>
		/// The length of the square cell [m].
		/// </summary>
		[Optional]
		public float? SquareLength;
	}
}