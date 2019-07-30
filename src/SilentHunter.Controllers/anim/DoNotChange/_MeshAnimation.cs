/* 
 * © 2007-2016 skwas. All rights reserved.
 * This code is provided as is. Change at your own risk.
 * --------------------------------------------------
 * 
 * S3D template for SH-controllers that contain animation data.
 * 
 * WARNING: DO NOT CHANGE THE TYPE/CLASS NAMES. I HAD TO HARDCODE A REFERENCE TO THEM.
 * Fields and descriptions may be changed though.
 * 
*/

using SilentHunter.Dat;


namespace anim
{
	/// <summary>
	/// Mesh animation controller data (subtype 0x5). Animation of vertices.
	/// </summary>
	[Controller(true, 0x5, true)]
	public class MeshAnimationData
		: BuiltInMeshAnimation
	{		
	}	

	/// <summary>
	/// Mesh animation controller data (subtype 0x8005). Animation of vertices.
	/// </summary>
	[Controller(true, 0x8005, true)]
	public class MeshAnimationData2
		: BuiltInMeshAnimation
	{
	}

	/// <summary>
	/// Texture animation controller data (subtype 0x6). Animation of texture coordinates.
	/// </summary>
	[Controller(true, 0x6, true)]
	public class TextureAnimationData
		: BuiltInMeshAnimation
	{
	}

	/// <summary>
	/// Texture animation controller data (subtype 0x8006). Animation of texture coordinates.
	/// </summary>
	[Controller(true, 0x8006, true)]
	public class TextureAnimationData2
		: BuiltInMeshAnimation
	{
	}
}