/*
 * SHSim.act - amun_Torpedo
 *
 * © 2007-2016 skwas. All rights reserved.
 * This code is provided as is. Change at your own risk.
 * --------------------------------------------------
 *
 * S3D template for the amun_Torpedo controller of Silent Hunter.
 *
 * For documentation on templates, requirements and restrictions, please refer to the documentation.
 *
 */

using System;
using System.Collections.Generic;
using SilentHunter.Controllers;
using SilentHunter.Controllers.Decoration;

namespace SHSim
{
    /// <summary>
    /// amun_Torpedo controller.
    /// </summary>
    public class amun_Torpedo
        : BehaviorController
    {
        /// <summary>
        /// The torpedo's type.
        /// </summary>
        public TorpedoType type;
        /// <summary>
        /// Default torpedo's depth [m].
        /// </summary>
        public float depth;
        /// <summary>
        /// The torpedo's max dive angle [deg].
        /// </summary>
        public float max_dive_angle;
        /// <summary>
        /// The torpedo's speeds settings.
        /// </summary>
        public List<TorpedoSpeed> Speeds;
        /// <summary>
        /// The torpedo's max turn angle [deg].
        /// </summary>
        public float max_turn_angle;
        /// <summary>
        /// Torpedo's detonating settings.
        /// </summary>
        public TorpedoDetonation Detonating;
        /// <summary>
        /// Pistol settings.
        /// </summary>
        [Optional]
        [SHVersion(SHVersions.SH4)]
        public List<TorpedoPistol> Pistols;
        /// <summary>
        /// Guidance systems.
        /// </summary>
        [Optional]
        [SHVersion(SHVersions.SH4)]
        public List<TorpedoGuidance> Guidances;
    }

    public class TorpedoSpeed
    {
        /// <summary>
        /// The torpedo's speed [kt].
        /// </summary>
        public float speed;
        /// <summary>
        /// .The torpedo's range at this speed [m].
        /// </summary>
        public float range;
    }

    public class TorpedoDetonation
    {
        /// <summary>
        /// The impulse applied to the hitted hydro object [t*m/s].
        /// </summary>
        public float impulse;
        /// <summary>
        /// Water hit splash effect.
        /// </summary>
        public ulong splash;
        /// <summary>
        /// Dud torpedo effect.
        /// </summary>
        public ulong dud_effect;
        /// <summary>
        /// Water column explosion effect.
        /// </summary>
        public ulong water_explosion;
        /// <summary>
        /// Under water explosion effect.
        /// </summary>
        public ulong under_explosion;
        /// <summary>
        /// Above water explosion effect.
        /// </summary>
        public ulong above_explosion;
    }

    public class TorpedoPistol
    {
        /// <summary>
        /// Earliest date for this pistol.
        /// </summary>
        public DateTime min_date;
        /// <summary>
        /// Latest date for this pistol.
        /// </summary>
        public DateTime max_date;
        /// <summary>
        /// Arming distance in meters.
        /// </summary>
        public float arming_distance;
        /// <summary>
        /// True if the pistol is magnetic.
        /// </summary>
        public bool magnetic;
        /// <summary>
        /// Magnetic detonation range in meters.
        /// </summary>
        public float mag_deton_range;
        /// <summary>
        /// Dud chances by angle intervals. If some intervals overlap, only the first one will be used.
        /// </summary>
        public List<DudChance> DudChances;
        /// <summary>
        /// Below this speed [kt] all dud chances are scaled down by the percentage specified in 'dud_reduction_rate'. Set to zero for no reduction.
        /// </summary>
        public float dud_reduction_speed;
        /// <summary>
        /// At low speed, the dud chances are scaled down by this percentage.
        /// </summary>
        public float dud_reduction_rate;
        /// <summary>
        /// Premature detonation chances by wave height intervals. If some intervals overlap, only the first one will be used.
        /// </summary>
        public List<PrematureChance> PrematureChances;
    }

    public class DudChance
    {
        /// <summary>
        /// Minimum angle.
        /// </summary>
        public float min_angle;
        /// <summary>
        /// Maximum angle.
        /// </summary>
        public float max_angle;
        /// <summary>
        /// Dud chance in percent.
        /// </summary>
        public float chance;
    }

    public class PrematureChance
    {
        /// <summary>
        /// Minimum wave height in meters.
        /// </summary>
        public float min_wave;
        /// <summary>
        /// Maximum wave height in meters.
        /// </summary>
        public float max_wave;
        /// <summary>
        /// Premature detonation chance in percent.
        /// </summary>
        public float chance;
    }

    public class TorpedoGuidance
    {
        /// <summary>
        /// Earliest date for this guidance system.
        /// </summary>
        public DateTime min_date;
        /// <summary>
        /// Latest date for this guidance system.
        /// </summary>
        public DateTime max_date;
        /// <summary>
        /// Chance of circle runner malfunction (percent).
        /// </summary>
        public float circle_runner_chance;
        /// <summary>
        /// Gyro problems.
        /// </summary>
        public List<TorpedoGyroProblem> GyroProblems;
        /// <summary>
        /// Depth keeping problems.
        /// </summary>
        public List<TorpedoDepthKeepingProblem> DepthKeepingProblems;
    }

    public class TorpedoGyroProblem
    {
        /// <summary>
        /// Maximum deviation (degrees).
        /// </summary>
        public float max_deviation;
        /// <summary>
        /// Gyro problem chance in percent.
        /// </summary>
        public float chance;
    }

    public class TorpedoDepthKeepingProblem
    {
        /// <summary>
        /// Minimum deviation (meters).
        /// </summary>
        public float min_deviation;
        /// <summary>
        /// Maximum deviation (meters).
        /// </summary>
        public float max_deviation;
        /// <summary>
        /// Depth keeping problem chance in percent.
        /// </summary>
        public float chance;
    }
}
