using System.ComponentModel;
using Microsoft.Extensions.DependencyInjection;

namespace SilentHunter.FileFormats.DependencyInjection
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	public class SilentHunterParsersConfigurer : IServiceCollectionProvider
	{
		private readonly IServiceCollection _serviceCollection;

		internal SilentHunterParsersConfigurer(IServiceCollection serviceCollection)
		{
			_serviceCollection = serviceCollection;

			Controllers = new ControllerConfigurer(this);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		IServiceCollection IServiceCollectionProvider.ServiceCollection => _serviceCollection;

		public ControllerConfigurer Controllers { get; }
	}
}