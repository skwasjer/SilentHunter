namespace SilentHunter.Dat
{
	public enum AnimationType : ushort
	{
		AnimationObject = 0,
		PositionKeyFrames = 1,
		PositionKeyFrames2 = 0x8001,
		RotationKeyFrames = 2,
		RotationKeyFrames2 = 0x8002,
		KeyFrameAnimStartParams = 4,
		KeyFrameAnimStartParams2 = 0x8004,
		MeshAnimationData = 5,
		MeshAnimationData2 = 0x8005,
		TextureAnimationData = 6,
		TextureAnimationData2 = 0x8006,
		LightAnimation = 0x200
	}
}