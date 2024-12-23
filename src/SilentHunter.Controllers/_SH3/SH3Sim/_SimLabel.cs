/*
 * SHSim.act - _SimLabel
 *
 * © 2007-2016 skwas. All rights reserved.
 * This code is provided as is. Change at your own risk.
 * --------------------------------------------------
 *
 * S3D template for the _SimLabel controller of Silent Hunter.
 *
 * For documentation on templates, requirements and restrictions, please refer to the documentation.
 *
 */

using SilentHunter.Controllers;

namespace SH3.SH3Sim
{
    /// <summary>
    /// _SimLabel user data.
    /// </summary>
    public class _SimLabel
        : BehaviorController
    {
        /// <summary>
        /// Label
        /// </summary>
        public LabelFlags label;
    }

    public enum LabelFlags
    {
        Crue
    }
}
