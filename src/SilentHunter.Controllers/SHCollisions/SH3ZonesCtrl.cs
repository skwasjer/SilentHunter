/* 
 * SHCollisions.act - SH3ZonesCtrl
 *
 * © 2007-2016 skwas. All rights reserved.
 * This code is provided as is. Change at your own risk.
 * --------------------------------------------------
 * 
 * S3D template for the SH3ZonesCtrl controller of Silent Hunter.
 * 
 * For documentation on templates, requirements and restrictions, please refer to the documentation.
 * 
*/

using SilentHunter.Dat;
using System.Collections.Generic;
using SilentHunter;

namespace SHCollisions
{
	/// <summary>
	/// SH3ZonesCtrl controller.
	/// </summary>
	public class SH3ZonesCtrl
		: Controller
	{
		/// <summary>
		/// Spherical zones list.
		/// </summary>
		public List<Sphere> Spheres;
		/// <summary>
		/// Boxes zones list.
		/// </summary>
		[Optional]
		public List<Box> Boxes;
	}

	[SHType]
	public class Sphere
	{
		/// <summary>
		/// Sphere's center relative to the parent.
		/// </summary>
		public Vector3 Center;
		/// <summary>
		/// The radius of the sphere in meters.
		/// </summary>
		public float Radius;
		/// <summary>
		/// The type of zone.
		/// </summary>
		public int Type;
		/// <summary>
		/// The zone is used for collision model.
		/// </summary>
		public bool Collision;
		/// <summary>
		/// The armor level of zone in cm [0..100]. -1 means it has armor of general category.
		/// </summary>
		public float ArmorLevel;
		/// <summary>
		/// Other object linked to zone.
		/// </summary>
		public ulong LinkTo;
	}

	[SHType]
	public class Box
	{
		/// <summary>
		/// 
		/// </summary>
		public Vector3 Min;
		/// <summary>
		/// 
		/// </summary>
		public Vector3 Max;		
		/// <summary>
		/// The type of zone.
		/// </summary>
		public int Type;
		/// <summary>
		/// The armor level of zone in cm [0..100]. -1 means it has armor of general category.
		/// </summary>
		public SHUnion<float, byte> ArmorLevel;
		/// <summary>
		/// Other object linked to zone.
		/// </summary>
		public ulong LinkTo;
	}
}