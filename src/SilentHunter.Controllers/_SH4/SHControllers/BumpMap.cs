/* 
 * SHControllers.act - BumpMap
 *
 * © 2007-2016 skwas. All rights reserved.
 * This code is provided as is. Change at your own risk.
 * --------------------------------------------------
 * 
 * S3D template for the BumpMap controller of Silent Hunter.
 * 
 * For documentation on templates, requirements and restrictions, please refer to the documentation.
 * 
*/

using SilentHunter.Controllers;

namespace SH4.SHControllers
{
	/// <summary>
	/// BumpMap action controller. Texture0 must be a RGB Normal_map. Texture1 is the regular texture. Disables vertex color and dynamic lighting.
	/// </summary>
	public class BumpMap
		: BehaviorController
	{
		/*		[Optional]
				public Nullable<Vector2> LightDirection;
				[Optional]
				public SHColor AmbientColor;
				[Optional]
				public Nullable<float> AmbientScale;*/
	}
}