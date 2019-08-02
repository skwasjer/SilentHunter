using System;
using System.Collections;

namespace skwas.IO
{
	public interface IChunkFile : IDisposable
	{
		IList Chunks { get; }
	}
}