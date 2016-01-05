namespace SilentHunter.Dat
{
	partial class DatFile
	{
		public enum Magics
		{
			Unknown = 0,
			Model = 1,
			Material = 2,
			EmbeddedImage = 3,
			NodeLink = 4,
			Properties = 6,
			Label = 8,
			Controllers = 10,
			Placement = 11,
			TextureMap = 13,
			S3DSettings = 31,
			AuthorInfo = 1000,
			Index = 1001,
			Eof = 1002,

			BodyParts = 1516687618,
			BodyParts2 = 813372786,
			BodyParts3 = -499342062,
			BoneInfluences = -660898478,

#if DEBUG
			WorldTransform = 102,

			Animation0 = unchecked((int)0xE6C4BDA2),
			Animation1 = unchecked((int)0x9A4168A2),

			//Unknown0 = -660898478
#endif
		}
	}
}
