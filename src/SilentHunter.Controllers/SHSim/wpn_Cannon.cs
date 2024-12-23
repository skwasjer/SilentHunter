/*
 * SHSim.act - wpn_Cannon
 *
 * © 2007-2016 skwas. All rights reserved.
 * This code is provided as is. Change at your own risk.
 * --------------------------------------------------
 *
 * S3D template for the wpn_Cannon controller of Silent Hunter.
 *
 * For documentation on templates, requirements and restrictions, please refer to the documentation.
 *
 */

using System.Collections.Generic;
using System.Numerics;
using SilentHunter.Controllers;

namespace SHSim
{
    /// <summary>
    /// wpn_Cannon render controller.
    /// </summary>
    public class wpn_Cannon
        : BehaviorController
    {
        /// <summary>
        /// Turret settings.
        /// </summary>
        public obj_Turret obj_Turret;
        /// <summary>
        /// Cannon type.
        /// </summary>
        public CannonType type;
        /// <summary>
        /// Shell's speed [m/s].
        /// </summary>
        public float shell_speed;
        /// <summary>
        /// Recoil time [s].
        /// </summary>
        public float recoil_time;
        /// <summary>
        /// Recoil distance [m].
        /// </summary>
        public float recoil_dist;
        /// <summary>
        /// The clip size.
        /// </summary>
        public int clip_size;
        /// <summary>
        /// Time to reload a clip [s].
        /// </summary>
        public float reload_time;
        /// <summary>
        /// Reload animation.
        /// </summary>
        public string anm_reload;
        /// <summary>
        /// Cannon's range at an elevation angle.
        /// </summary>
        public CannonRange Range;
        /// <summary>
        /// The cannon's ammo storage.
        /// </summary>
        public CannonAmmoStorage ammo_storage;
        /// <summary>
        /// Cannon fire settings.
        /// </summary>
        public CannonFire Fire;
    }

    public class CannonFire
    {
        /// <summary>
        /// The cannon's fire effect.
        /// </summary>
        public ulong effect;
        /// <summary>
        /// Traverse angle tolerance on fire [deg].
        /// </summary>
        public float trav_tolerance;
        /// <summary>
        /// Elevation angle tolerance on fire [deg].
        /// </summary>
        public float elev_tolerance;
        /// <summary>
        /// Visual barrel object.
        /// </summary>
        public string barrel;
        /// <summary>
        /// The cannon's muzzles.
        /// </summary>
        public List<CannonMuzzle> Muzzles;
    }

    public class CannonMuzzle
    {
        /// <summary>
        /// The muzzle's position.
        /// </summary>
        public Vector3 position;
    }

    public class CannonRange
    {
        /// <summary>
        /// Range in meters.
        /// </summary>
        public float range;
        /// <summary>
        /// Angle in degrees.
        /// </summary>
        public float angle;
    }

    public class CannonAmmoStorage
    {
        /// <summary>
        /// Armor piercing shells.
        /// </summary>
        public ShellStorage AP;
        /// <summary>
        /// High explosive shells.
        /// </summary>
        public ShellStorage HE;
        /// <summary>
        /// Anti aircraft shells.
        /// </summary>
        public ShellStorage AA;
        /// <summary>
        /// Star shells.
        /// </summary>
        public ShellStorage SS;
    }

    public class ShellStorage
    {
        /// <summary>
        /// object->amun_Shell. The shell's type.
        /// </summary>
        public ulong shell;
        /// <summary>
        /// The shells amount of this type.
        /// </summary>
        public int amount;
    }

    public enum CannonType
    {
        Cannon,
        AAGun
    }
}
