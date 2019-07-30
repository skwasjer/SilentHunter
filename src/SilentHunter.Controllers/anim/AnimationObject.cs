/* 
 * anim.act - AnimationObject
 *
 * � 2007-2016 skwas. All rights reserved.
 * This code is provided as is. Change at your own risk.
 * --------------------------------------------------
 * 
 * S3D template for the AnimationObject controller of Silent Hunter.
 * 
 * For documentation on templates, requirements and restrictions, please refer to the documentation.
 * 
*/

using System.Collections.Generic;
using System.Runtime.InteropServices;
using SilentHunter.Dat;

namespace anim
{
	/// <summary>
	/// AnimationObject controller.
	/// </summary>
	[Controller(true, 0x0, false)]
	public class AnimationObject
		: RawController
	{
		public string AnimName;
		public List<AnimationSubObject> Animations;
	}

	[SHType, StructLayout(LayoutKind.Sequential)]
	public class AnimationSubObject
	{
		public ulong Id;
		public string AnimName;
	}
}