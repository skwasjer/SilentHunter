using System;
using System.Collections;

namespace SilentHunter.FileFormats.ChunkedFiles
{
	public interface IChunkFile : IDisposable
	{
		IList Chunks { get; }
	}
}