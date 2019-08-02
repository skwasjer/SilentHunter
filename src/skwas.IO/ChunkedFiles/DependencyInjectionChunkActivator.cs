using System;
using Microsoft.Extensions.DependencyInjection;

namespace skwas.IO
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

			if (magic == null)
			{
				throw new ArgumentNullException(nameof(magic));
			}

			try
			{
				return (IChunk)ActivatorUtilities.CreateInstance(_serviceProvider, chunkType);
			}
			catch (InvalidOperationException)
			{
				return (IChunk)ActivatorUtilities.CreateInstance(_serviceProvider, chunkType, magic);
			}
		}
	}
}