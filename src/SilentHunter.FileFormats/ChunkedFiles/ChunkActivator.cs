using System;
using System.Reflection;

namespace SilentHunter.FileFormats.ChunkedFiles
{
	public class ChunkActivator : IChunkActivator
	{
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
				BindingFlags.Public | BindingFlags.Instance,
				null,
				new[] { magic.GetType() },
				null
			);
			if (constructor != null)
			{
				return (IChunk)Activator.CreateInstance(chunkType, magic);
			}

			// Try a parameterless constructor.
			return (IChunk)Activator.CreateInstance(chunkType);
		}
	}
}