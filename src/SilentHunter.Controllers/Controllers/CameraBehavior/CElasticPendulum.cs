/* 
 * CameraBehavior.act - CElasticPendulum
 *
 * © 2007-2016 skwas. All rights reserved.
 * This code is provided as is. Change at your own risk.
 * --------------------------------------------------
 * 
 * S3D template for the CElasticPendulum controller of Silent Hunter.
 * 
 * For documentation on templates, requirements and restrictions, please refer to the documentation.
 * 
*/

using SilentHunter.Dat;

namespace CameraBehavior
{
	/// <summary>
	/// CElasticPendulum render controller.
	/// </summary>
	public class CElasticPendulum
		: Controller
	{
		/// <summary>
		/// The k/mass constant.
		/// </summary>
		public float Elasticity;
		/// <summary>
		/// The dumping coeficient C/mass constant [0..1].
		/// </summary>
		public float Dumper;
		/// <summary>
		/// The maximum angle with Y axis.
		/// </summary>
		public float MaxAngle;		
	}

}