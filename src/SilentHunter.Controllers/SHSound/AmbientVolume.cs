/* 
 * SHSound.act - AmbientVolume
 *
 * © 2007-2016 skwas. All rights reserved.
 * This code is provided as is. Change at your own risk.
 * --------------------------------------------------
 * 
 * S3D template for the AmbientVolume controller of Silent Hunter.
 * 
 * For documentation on templates, requirements and restrictions, please refer to the documentation.
 * 
*/

using SilentHunter.Controllers;
using System.Collections.Generic;
using SilentHunter.Controllers.Decoration;

namespace SHSound
{
	/// <summary>
	/// AmbientVolume controller.
	/// </summary>
	public class AmbientVolume
		: Controller
	{
		/// <summary>
		/// Water level (object).
		/// </summary>
		public float ObjectWaterLevel;
		/// <summary>
		/// Water level (object).
		/// </summary>
		public List<SoundObject> Sounds;
	}

	[SHType]
	public class SoundObject
	{
		/// <summary>
		/// The identifier of the sound object.
		/// </summary>
		public string Identifier;
		/// <summary>
		/// Volume modifier for soundobject above water.
		/// </summary>
		public float ObjectAboveWater;
		/// <summary>
		/// Volume modifier for soundobject below water.
		/// </summary>
		public float ObjectBelowWater;
		/// <summary>
		/// Volume modifier for soundobject when camera is above water.
		/// </summary>
		public float CameraAboveWater;
		/// <summary>
		/// Volume modifier for soundobject when camera is below water.
		/// </summary>
		public float CameraBelowWater;
		/// <summary>
		/// Volume modifier for soundobject when camera is in interior.
		/// </summary>
		public float CameraInterior;
		/// <summary>
		/// Volume modifier for soundobject when camera is in sonar room (not sure if this is correct!).
		/// </summary>
		public float CameraSonar;
	}
}