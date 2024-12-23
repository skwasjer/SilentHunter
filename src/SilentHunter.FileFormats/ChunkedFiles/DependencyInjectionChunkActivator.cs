using System;
using System.Runtime.ExceptionServices;
using Microsoft.Extensions.DependencyInjection;

namespace SilentHunter.FileFormats.ChunkedFiles
{
	/// <summary>
	/// Represents an activator for creating chunks with dependencies using <see cref="IServiceProvider" />.
	/// </summary>
	public class DependencyInjectionChunkActivator : IChunkActivator
	{
		private readonly IServiceProvider _serviceProvider;

		/// <summary>
		/// Initializes a new instance of the <see cref="DependencyInjectionChunkActivator" /> using specified <paramref name="serviceProvider" /> container.
		/// </summary>
		/// <param name="serviceProvider">The service provider to use to create new chunk instances with dependencies.</param>
		public DependencyInjectionChunkActivator(IServiceProvider serviceProvider)
		{
			_serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
		}

		/// <summary>
		/// Creates a chunk of type <paramref name="chunkType" /> with optional <paramref name="magic" />, and injects it with dependencies from <see cref="IServiceProvider" />.
		/// </summary>
		/// <param name="chunkType">The chunk type.</param>
		/// <param name="magic">The magic.</param>
		/// <returns>the newly created chunk</returns>
		/// <exception cref="InvalidOperationException">Thrown when the chunk could not be created.</exception>
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
			catch (InvalidOperationException ex)
			{
				if (magic != null)
				{
					try
					{
						return (IChunk)ActivatorUtilities.CreateInstance(_serviceProvider, chunkType, magic);
					}
					catch
					{
						ExceptionDispatchInfo.Capture(ex).Throw();
					}
				}

				throw;
			}
		}
	}
}