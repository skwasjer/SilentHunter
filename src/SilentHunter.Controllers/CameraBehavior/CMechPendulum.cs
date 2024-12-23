/*
 * CameraBehavior.act - CMechPendulum
 *
 * © 2007-2016 skwas. All rights reserved.
 * This code is provided as is. Change at your own risk.
 * --------------------------------------------------
 *
 * S3D template for the CMechPendulum controller of Silent Hunter.
 *
 * For documentation on templates, requirements and restrictions, please refer to the documentation.
 *
 */

using SilentHunter.Controllers;

namespace CameraBehavior
{
    /// <summary>
    /// CMechPendulum render controller. A pendulum controller which works on a mechanical object.
    /// </summary>
    public class CMechPendulum
        : BehaviorController
    {
        /// <summary>
        /// The CElasticPendulum controller.
        /// </summary>
        public CElasticPendulum CElasticPendulum;
    }
}
