namespace SilentHunter.FileFormats.Dat;

/// <summary>
/// The animation types for which the are specific controllers.
/// </summary>
public enum AnimationType : ushort
{
#pragma warning disable 1591
    AnimationObject = 0, // No count field
    PositionKeyFrames = 1,
    PositionKeyFrames2 = 0x8001,
    RotationKeyFrames = 2,
    RotationKeyFrames2 = 0x8002,
    KeyFrameAnimStartParams = 4, // No count field
    KeyFrameAnimStartParams2 = 0x8004, // No count field
    MeshAnimationData = 5,
    MeshAnimationData2 = 0x8005,
    TextureAnimationData = 6,
    TextureAnimationData2 = 0x8006,
    LightAnimation = 0x0200
#pragma warning restore 1591
}
