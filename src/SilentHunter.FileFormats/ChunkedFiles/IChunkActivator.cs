using System;

namespace SilentHunter.FileFormats.ChunkedFiles
{
	public interface IChunkActivator
	{
		IChunk Create(Type chunkType, object magic);
	}
}