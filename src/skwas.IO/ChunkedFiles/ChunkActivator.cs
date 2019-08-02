using System;
using System.Reflection;

namespace skwas.IO
{
	public class ChunkActivator : IChunkActivator
	{
		public ChunkActivator()
		{
		}

		public IChunk Create(Type chunkType, object magic)
		{
			if (chunkType == null)
			{
				throw new ArgumentNullException(nameof(chunkType));
			}

			if (magic == null)
			{
				throw new ArgumentNullException(nameof(magic));
			}

			ConstructorInfo constructor = chunkType.GetConstructor(
				BindingFlags.Public | BindingFlags.Instance, null,
				new[] { magic.GetType() },
				null
			);
			if (constructor != null)
			{
				return (IChunk)Activator.CreateInstance(chunkType, magic);
			}
			else
			{
				// Try a parameterless constructor.
				return (IChunk)Activator.CreateInstance(chunkType);
			}
		}
	}
}