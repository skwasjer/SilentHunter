using System.IO;
using System.Diagnostics;

namespace SilentHunter.Dat
{
	public sealed class Eof : DatChunk
	{
		public Eof()
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
