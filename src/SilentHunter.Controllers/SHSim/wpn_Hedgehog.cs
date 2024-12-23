/*
 * SHSim.act - wpn_Hedgehog
 *
 * © 2007-2016 skwas. All rights reserved.
 * This code is provided as is. Change at your own risk.
 * --------------------------------------------------
 *
 * S3D template for the wpn_Hedgehog controller of Silent Hunter.
 *
 * For documentation on templates, requirements and restrictions, please refer to the documentation.
 *
 */

using SilentHunter.Controllers;

namespace SHSim
{
    /// <summary>
    /// wpn_Hedgehog render controller.
    /// </summary>
    public class wpn_Hedgehog
        : BehaviorController
    {
        /// <summary>
        /// The hedgehog's ammo storage.
        /// </summary>
        public HedgehogAmmoStorage ammo_storage;
        /// <summary>
        /// The hedgehog's fire effect.
        /// </summary>
        public ulong fire_effect;
        /// <summary>
        /// The maximum range of launches.
        /// </summary>
        public float max_range;
        /// <summary>
        /// The circular pattern diameter.
        /// </summary>
        public float diameter;
        /// <summary>
        /// Time to reload a depth charge [s].
        /// </summary>
        public float reload_time;
    }

    public class HedgehogAmmoStorage
    {
        /// <summary>
        /// object->amun_Bomb or object->amun_DepthCharge? The bomb's type.
        /// </summary>
        public ulong bomb;
        /// <summary>
        /// The available number of launches.
        /// </summary>
        public int launches;
    }
}
