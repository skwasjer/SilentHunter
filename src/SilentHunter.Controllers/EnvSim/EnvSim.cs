/*
 * EnvSim.act - EnvSim
 *
 * © 2007-2016 skwas. All rights reserved.
 * This code is provided as is. Change at your own risk.
 * --------------------------------------------------
 *
 * S3D template for the EnvSim controller of Silent Hunter.
 *
 * For documentation on templates, requirements and restrictions, please refer to the documentation.
 *
 */

using System.Collections.Generic;
using SilentHunter.Controllers;
using SilentHunter.Controllers.Decoration;

namespace EnvSim
{
    /// <summary>
    /// EnvSim render controller.
    /// </summary>
    public class EnvSim
        : BehaviorController
    {
        /// <summary>
        /// Debug settings.
        /// </summary>
        public DebugSettings Debug;
        /// <summary>
        /// If true get weather info from controllers, if false use mission info.
        /// </summary>
        public bool UseCotrollersWeatherData;
        /// <summary>
        /// Update sky from time to time (in seconds).
        /// </summary>
        public float UpdateTime;
        /// <summary>
        /// Horizon settings.
        /// </summary>
        public Horizon Horizon;
        /// <summary>
        /// Celestial objects radius (distance) relative to camera position.
        /// </summary>
        public SkyObjRadius SkyObjRadius;
        /// <summary>
        /// Wind parameters.
        /// </summary>
        public Wind Wind;
        /// <summary>
        /// Fog parameters.
        /// </summary>
        public Fog Fog;
        /// <summary>
        /// The fog distances.
        /// </summary>
        public List<ESFogDistance> FogDistances;
        /// <summary>
        /// Sea foam settings.
        /// </summary>
        public SeaFoam SeaFoam;
        /// <summary>
        /// Sky parameters for different sun azimuth angles. The angles start from 90 degrees (zenith) and vary with a 15 degrees step to -90 degrees.
        /// </summary>
        public List<SkyParameter> Sky;
    }

    public class SkyParameter
    {
        /// <summary>
        /// Sun azimuth angle. Angles start from 90 degrees (zenith) and vary with a 15 degrees step to -90 degrees.
        /// </summary>
        public float SunAltitudeAngle;
        /// <summary>
        /// 1.0f is normal size, the sun is larger at sunrise/sunset.
        /// </summary>
        public float SunRelativeSize;
        /// <summary>
        /// Textures are arranged in column major order. -1 means no texture at this position.
        /// </summary>
        public int SkyTextureIndex;
    }

    public class SeaFoam
    {
        /// <summary>
        /// If true take into consideration edge angles when computing wave peaks, otherwise consider only relative height.
        /// </summary>
        public bool UseSlope;
        /// <summary>
        /// The new foam is generated with this speed.
        /// </summary>
        public float IncreaseSpeed;
        /// <summary>
        /// Existing foam is destroyed with this speed.
        /// </summary>
        public float DecreaseSpeed;
        /// <summary>
        /// The wave peak angle where we start to increase the foam quantity.
        /// </summary>
        public float IncreaseAngleMin;
        /// <summary>
        /// The wave peak angle where the increase speed is maximum.
        /// </summary>
        public float IncreaseAngleMax;
        /// <summary>
        /// Foam texture scale coef.
        /// </summary>
        public float TextureScale;
        /// <summary>
        /// Foam mask texture scale coef.
        /// </summary>
        public float MaskTextureScale;
        /// <summary>
        /// </summary>
        public float WindSpeed;
        /// <summary>
        /// </summary>
        public float AnimationSpeed;
    }

    public class ESFogDistance
    {
        /// <summary>
        /// Minimum relative distance to object.
        /// </summary>
        public float ObjectsRelativeZMin;
        /// <summary>
        /// Maximum relative distance to object.
        /// </summary>
        public float ObjectsRelativeZMax;
        /// <summary>
        /// Minimum relative distance to object under water.
        /// </summary>
        public float UnderwaterObjectsRelativeZMin;
        /// <summary>
        /// Maximum relative distance to object under water.
        /// </summary>
        public float UnderwaterObjectsRelativeZMax;
        /// <summary>
        /// </summary>
        public float CloudsRelativeZMin;
        /// <summary>
        /// </summary>
        public float CloudsRelativeZMax;
        /// <summary>
        /// </summary>
        [Optional]
        [SHVersion(SHVersions.SH3)]
        public float? SkyRelativeZMin;
        /// <summary>
        /// </summary>
        [Optional]
        [SHVersion(SHVersions.SH3)]
        public float? SkyRelativeZMax;
        /// <summary>
        /// </summary>
        [Optional]
        [SHVersion(SHVersions.SH4)]
        public float? SkyYMin;
        /// <summary>
        /// </summary>
        [Optional]
        [SHVersion(SHVersions.SH4)]
        public float? SkyYMax;
        /// <summary>
        /// </summary>
        public float SeaRelativeZMin;
        /// <summary>
        /// </summary>
        public float SeaRelativeZMax;
        /// <summary>
        /// The Y (height) component of the vertex position is multiplied with this value before fog distance computation on clouds.
        /// </summary>
        public float YScaleForFog;
    }

    public class Fog
    {
        /// <summary>
        /// Start fog type.
        /// </summary>
        public FogType StartFog;
        /// <summary>
        /// End fog type.
        /// </summary>
        public FogType EndFog;
        /// <summary>
        /// Change speed for fog type.
        /// </summary>
        public float FogChangeSpeed;
    }

    public enum FogType
    {
        None,
        Light,
        Medium,
        Heavy
    }

    public class DebugSettings
    {
        /// <summary>
        /// Show debug info.
        /// </summary>
        public bool ShowInfo;
    }

    public class Horizon
    {
        /// <summary>
        /// Angle (in deg) over the horizon when a celestial object will start to set or end to rise.
        /// </summary>
        public float OverHorizon;
        /// <summary>
        /// Angle (in deg) under the horizon when a celestial object will start to rise or end to set.
        /// </summary>
        public float UnderHorizon;
        /// <summary>
        /// Percent of the sun light decreasing over the horizon.
        /// </summary>
        public float LightOverHorizon;
        /// <summary>
        /// The angle between the sun and the horizon is multiplied with this value (0..1).
        /// </summary>
        public float SunAltitudeAngleScale;
        /// <summary>
        /// The angle between the sun reflect light and horizontal plane (0..90).
        /// </summary>
        public float SunReflectAngle;
        /// <summary>
        /// The maximum angle between the sun and horizontal plane during night (0..90).
        /// </summary>
        public float MaxUnderwaterSunLightAngle;
        /// <summary>
        /// </summary>
        public float SunHaloStartFade;
        /// <summary>
        /// </summary>
        public float SunHaloEndFade;
        /// <summary>
        /// The angle between the sun and the horizon where the halo starts to fade (-90..90).
        /// </summary>
        public float SunHaloReflectionStartFade;
        /// <summary>
        /// The angle between the sun and the horizon where the halo becomes invisible (-90..90).
        /// </summary>
        public float SunHaloReflectionFade;
    }

    public class SkyObjRadius
    {
        /// <summary>
        /// Radius of sun (in meters).
        /// </summary>
        public float SunRadius;
        /// <summary>
        /// Radius of moon (in meters).
        /// </summary>
        public float MoonRadius;
        /// <summary>
        /// Radius of stars (in meters).
        /// </summary>
        public float StarsRadius;
    }

    public class Wind
    {
        /// <summary>
        /// Start wind speed (in meters/sec).
        /// </summary>
        public float StartWindSpeed;
        /// <summary>
        /// Start wind heading (0..360).
        /// </summary>
        public float StartWindHeading;
        /// <summary>
        /// End wind speed (in meters/sec).
        /// </summary>
        public float EndWindSpeed;
        /// <summary>
        /// End wind heading (0..360).
        /// </summary>
        public float EndWindHeading;
        /// <summary>
        /// The current wind heading changes to the target heading with this speed.
        /// </summary>
        public float WindHeadingChangeSpeed;
        /// <summary>
        /// The current wind speed changes to the target wind speed with this speed.
        /// </summary>
        public float WindSpeedChangeSpeed;
        /// <summary>
        /// The waves properties for different wind speeds.
        /// </summary>
        public List<WavesProperties> WavesProperties;
    }

    public class WavesProperties
    {
        /// <summary>
        /// The wind speed.
        /// </summary>
        public float WindSpeed;
        /// <summary>
        /// Water scale parameters.
        /// </summary>
        public WindScale Scale;
        /// <summary>
        /// Bump scale parameters.
        /// </summary>
        public BumpScale BumpScale;
        /// <summary>
        /// Water specularity parameters.
        /// </summary>
        [Optional]
        [SHVersion(SHVersions.SH4)]
        public WaterSpecularity WaterSpecularity;
        /// <summary>
        /// Fresnel reflection coefficient.
        /// </summary>
        public float FresnelCoef;
        /// <summary>
        /// Supress large waves first armonics.
        /// </summary>
        public int LargeWavesArmonics;
        /// <summary>
        /// Large waves supression coefficient.
        /// </summary>
        public float LargeWavesCoef;
        /// <summary>
        /// The speed of sea waves movement.
        /// </summary>
        public float SeaSpeed;
        /// <summary>
        /// Reflection intensity.
        /// </summary>
        public float ReflIntensity;
        /// <summary>
        /// Reflection bias.
        /// </summary>
        public float ReflBias;
        /// <summary>
        /// The wind coef used for surface bump movement speed.
        /// </summary>
        public float WindCoef;
    }

    public class WindScale
    {
        /// <summary>
        /// Water scale on X.
        /// </summary>
        public float X;
        /// <summary>
        /// Water scale on Y.
        /// </summary>
        public float Y;
        /// <summary>
        /// Water scale on Z.
        /// </summary>
        public float Z;
        /// <summary>
        /// Water scale on normal.
        /// </summary>
        public float Normal;
    }

    public class BumpScale
    {
        /// <summary>
        /// The horizontal scale.
        /// </summary>
        public float U;
        /// <summary>
        /// The height scale.
        /// </summary>
        public float V;
        /// <summary>
        /// The amplitude scale.
        /// </summary>
        public float Amplitude;
    }

    public class WaterSpecularity
    {
        /// <summary>
        /// Above water glossiness.
        /// </summary>
        public float Glossiness;
        /// <summary>
        /// Above water specular intensity.
        /// </summary>
        public float Intensity;
        /// <summary>
        /// Under water glossiness.
        /// </summary>
        public float UnderGlossiness;
        /// <summary>
        /// Under water specular intensity.
        /// </summary>
        public float UnderIntensity;
    }
}
