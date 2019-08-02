/* 
 * CameraBehavior.act - ShocksCollision
 *
 * © 2007-2016 skwas. All rights reserved.
 * This code is provided as is. Change at your own risk.
 * --------------------------------------------------
 * 
 * S3D template for the ShocksCollision controller of Silent Hunter.
 * 
 * For documentation on templates, requirements and restrictions, please refer to the documentation.
 * 
*/

using SilentHunter.Controllers;

namespace CameraBehavior
{
	/// <summary>
	/// ShocksCollision controller. Apply shocks on object when collision occured.
	/// </summary>
	public class ShocksCollision
		: Controller
	{
		/// <summary>
		/// A pendulum controller which works on a mechanical object.
		/// </summary>
		public CElasticPendulum CElasticPendulum;
		/// <summary>
		/// The coeficient to apply on the shocks.
		/// </summary>
		public float Coef;
	}
}