/* 
 * SHCollisions.act - TerrainCollider
 *
 * © 2007-2016 skwas. All rights reserved.
 * This code is provided as is. Change at your own risk.
 * --------------------------------------------------
 * 
 * S3D template for the TerrainCollider controller of Silent Hunter.
 * 
 * For documentation on templates, requirements and restrictions, please refer to the documentation.
 * 
*/

using SilentHunter.Controllers;

namespace SHCollisions
{
	/// <summary>
	/// TerrainCollider controller. Collide the object with terrain using a sphere model.
	/// </summary>
	public class TerrainCollider
		: BehaviorController
	{
		/// <summary>
		/// The radius of sphere modelling the object [0.1m].
		/// </summary>
		public float Radius;
	}
}