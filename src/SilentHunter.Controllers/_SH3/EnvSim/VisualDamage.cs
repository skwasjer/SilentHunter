/* 
 * EnvSim.act - VisualDamage
 *
 * © 2007-2016 skwas. All rights reserved.
 * This code is provided as is. Change at your own risk.
 * --------------------------------------------------
 * 
 * S3D template for the VisualDamage controller of Silent Hunter.
 * 
 * For documentation on templates, requirements and restrictions, please refer to the documentation.
 * 
*/

using SilentHunter.Dat;


namespace SH3.EnvSim
{
	/// <summary>
	/// VisualDamage render controller.
	/// </summary>
	public class VisualDamage
		: Controller
	{
		/// <summary>
		/// The minimum number of hit points that generate a damage decal.
		/// </summary>
		public float MinDamage;
		/// <summary>
		/// The number of hit points that cause the maximum damage.
		/// </summary>
		public float MaxDamage;
		/// <summary>
		/// The radius of the damage decal for minimum damage.
		/// </summary>
		public float MinDamageRange;
		/// <summary>
		/// The radius of the damage decal for maximum damage.
		/// </summary>
		public float MaxDamageRange;
		/// <summary>
		/// The transparency for minimum damage.
		/// </summary>
		public float MinDamageAlpha;
		/// <summary>
		/// The texture used for decal shape.
		/// </summary>
		public string MaskTextureName;
		/// <summary>
		/// The tilable texture with explosion traces.
		/// </summary>
		public string DirtTextureName;
		/// <summary>
		/// The number of tiles per 10 meters of ship surface.
		/// </summary>
		public float DirtTileFactor;
		/// <summary>
		/// The angle of rotation of the dirt texture.
		/// </summary>
		public float DirtRotationAngle;
	}
}