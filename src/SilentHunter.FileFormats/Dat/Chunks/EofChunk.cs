using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace SilentHunter.FileFormats.Dat.Chunks
{
	public sealed class EofChunk : DatChunk
	{
		public EofChunk()
			: base(DatFile.Magics.Eof)
		{
		}

		protected override Task DeserializeAsync(Stream stream)
		{
			// Verify that there are no bytes in this chunk.
			Debug.Assert(stream.Length == 0, "Eof: Expected an empty stream.");
			return Task.CompletedTask;
		}

		protected override Task SerializeAsync(Stream stream)
		{
			// Override and ignore base implementation.
			return Task.CompletedTask;
		}
	}
}