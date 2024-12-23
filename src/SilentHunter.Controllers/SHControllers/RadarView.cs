/*
 * SHControllers.act - RadarView
 *
 * © 2007-2016 skwas. All rights reserved.
 * This code is provided as is. Change at your own risk.
 * --------------------------------------------------
 *
 * S3D template for the RadarView controller of Silent Hunter.
 *
 * For documentation on templates, requirements and restrictions, please refer to the documentation.
 *
 */

using System.Collections.Generic;
using SilentHunter.Controllers;
using SilentHunter.Controllers.Decoration;

namespace SHControllers
{
    /// <summary>
    /// RadarView render controller.
    /// </summary>
    public class RadarView
        : BehaviorController
    {
        /// <summary>
        /// Radar mode.
        /// </summary>
        [Optional]
        [SHVersion(SHVersions.SH4)]
        public RadarMode? Mode;
        /// <summary>
        /// Tga file.
        /// </summary>
        [FixedString(260)]
        public string Tga_file;
        /// <summary>
        /// Radar texture dimension (power of 2).
        /// </summary>
        public int Tex_dim;
        /// <summary>
        /// Radar border (will not be drawn anything in this area).
        /// </summary>
        public int Border;
        /// <summary>
        /// PPI Contact Time To Live in seconds (after this period of time a contact will completely fade out).
        /// </summary>
        [Optional]
        [SHVersion(SHVersions.SH4)]
        public float? PPIContactTTL;
        /// <summary>
        /// Range dimension (in meters).
        /// </summary>
        public RangeDim RangeDim;
        /// <summary>
        /// Logarithmic display factor.
        /// </summary>
        public float LogFactor;
    }

    public enum RadarMode
    {
        AScope,
        PPI
    }

    public class RangeDim
    {
        /// <summary>
        /// Range steps.
        /// </summary>
        public int RangeSteps;
        /// <summary>
        /// Minimum range and step.
        /// </summary>
        public List<RangeMinStep> Ranges;
    }

    public struct RangeMinStep
    {
        public int Min;
        public int Step;
    }
}
