/* 
 * .act Unknown - DynamicShadow
 *
 * © 2007-2016 skwas. All rights reserved.
 * This code is provided as is. Change at your own risk.
 * --------------------------------------------------
 * 
 * S3D template for the DynamicShadow controller of Silent Hunter.
 * 
 * For documentation on templates, requirements and restrictions, please refer to the documentation.
 * 
*/

using SilentHunter.Controllers;

namespace Unknown
{
	/// <summary>
	/// DynamicShadow controller. WARNING: I haven't found the definition of this controller in any .act file. I therefor don't know for sure if it is implemented right.
	/// </summary>
	public class DynamicShadow
		: BehaviorController
	{
		/// <summary>
		/// Unknown property. Not sure if data type is correct either.
		/// </summary>
		public ulong Light;
	}
}