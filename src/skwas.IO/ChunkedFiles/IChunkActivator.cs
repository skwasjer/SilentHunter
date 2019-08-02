using System;

namespace skwas.IO
{
	public interface IChunkActivator
	{
		IChunk Create(Type chunkType, object magic);
	}
}