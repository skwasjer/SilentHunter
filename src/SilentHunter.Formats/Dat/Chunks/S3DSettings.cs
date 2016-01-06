using System.Diagnostics;
using System.IO;

namespace SilentHunter.Dat.Chunks
{
	public sealed class S3DSettings : DatChunk
	{
		public S3DSettings()
			: base(DatFile.Magics.S3DSettings, 0xFFFF)
		{
		}

		protected override void Deserialize(Stream stream)
		{
			base.Deserialize(stream);
#if DEBUG
			Debug.WriteLine("Contains S3DSettings : " + GetBaseStreamName(stream));
#endif
		}
	}
}
