using System;
using Microsoft.Extensions.DependencyInjection;

namespace SilentHunter.FileFormats.ChunkedFiles
{
	public class DependencyInjectionChunkActivator : IChunkActivator
	{
		private readonly IServiceProvider _serviceProvider;

		public DependencyInjectionChunkActivator(IServiceProvider serviceProvider)
		{
			_serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
		}

		public IChunk Create(Type chunkType, object magic)
		{
			if (chunkType == null)
			{
				throw new ArgumentNullException(nameof(chunkType));
			}

			try
			{
				return (IChunk)ActivatorUtilities.CreateInstance(_serviceProvider, chunkType);
			}
			catch (InvalidOperationException)
			{
				if (magic != null)
				{
					return (IChunk)ActivatorUtilities.CreateInstance(_serviceProvider, chunkType, magic);
				}

				throw;
			}
		}
	}
}