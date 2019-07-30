/* 
 * SHCollisions.act - ColisionableObject
 *
 * © 2007-2016 skwas. All rights reserved.
 * This code is provided as is. Change at your own risk.
 * --------------------------------------------------
 * 
 * S3D template for the ColisionableObject controller of Silent Hunter.
 * 
 * For documentation on templates, requirements and restrictions, please refer to the documentation.
 * 
*/

using SilentHunter.Dat;

namespace SHCollisions
{
	/// <summary>
	/// ColisionableObject controller.
	/// </summary>
	public class ColisionableObject
		: Controller
	{
		/// <summary>
		/// The armor level in cm [0..100].
		/// </summary>
		public float ArmorLevel;
		/// <summary>
		/// The amount of hitpoints this ship can take.
		/// </summary>
		public float Hit_Points;
		/// <summary>
		/// The maximum depth this ship survives.
		/// </summary>
		public float CrashDepth;
		/// <summary>
		/// The number of hitpoints to be acumulated in 1 sec when ship under crash depth.
		/// </summary>
		public float CrashSpeed;
		/// <summary>
		/// The rebound coeficient [0..1] to be used when colliding with other objects.
		/// </summary>
		public float Rebound;
	}
}