/* 
 * CameraBehavior.act - RotSpring
 *
 * © 2007-2016 skwas. All rights reserved.
 * This code is provided as is. Change at your own risk.
 * --------------------------------------------------
 * 
 * S3D template for the RotSpring controller of Silent Hunter.
 * 
 * For documentation on templates, requirements and restrictions, please refer to the documentation.
 * 
*/

using SilentHunter;
using SilentHunter.Dat;

namespace CameraBehavior
{
	/// <summary>
	/// RotSpring controller. Keeps the owner's Z axe pointing to the target point.
	/// </summary>
	public class RotSpring
		: Controller
	{
		/// <summary>
		/// Spring.
		/// </summary>
		public Spring Spring;		
		/// <summary>
		/// The point the owner's Z axe points to. Its (x, y, z) coordinates are local into the target object's system.
		/// </summary>
		public Vector3 Target_point;
	}
}