using System.Diagnostics;
using System.IO;

namespace SilentHunter.Dat.Chunks
{
	public sealed class EofChunk : DatChunk
	{
		public EofChunk()
			: base(DatFile.Magics.Eof)
		{
		}

		protected override void Deserialize(Stream stream)
		{
			// Verify that there are no bytes in this chunk.
			Debug.Assert(stream.Length == 0, "Eof: Expected an empty stream.");
		}

		protected override void Serialize(Stream stream)
		{
			// Override and ignore base implementation.
		}
	}
}