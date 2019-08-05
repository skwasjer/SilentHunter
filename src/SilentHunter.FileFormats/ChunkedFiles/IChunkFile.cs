using System;
using System.Collections;

namespace SilentHunter.FileFormats.ChunkedFiles
{
	public interface IChunkFile
	{
		IList Chunks { get; }
	}
}