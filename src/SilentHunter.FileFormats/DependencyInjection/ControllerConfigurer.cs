using System;
using System.ComponentModel;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using SilentHunter.FileFormats.Dat.Controllers;

namespace SilentHunter.FileFormats.DependencyInjection
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	public class ControllerConfigurer : IServiceCollectionProvider
	{
		private readonly SilentHunterParsersConfigurer _configurer;
		private readonly IServiceCollection _serviceCollection;

		internal ControllerConfigurer(SilentHunterParsersConfigurer configurer)
		{
			_configurer = configurer;
			_serviceCollection = ((IServiceCollectionProvider)_configurer).ServiceCollection;
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		IServiceCollection IServiceCollectionProvider.ServiceCollection => _serviceCollection;

		public SilentHunterParsersConfigurer FromAssembly(Assembly assembly)
		{
			return FromAssembly(new ControllerAssembly(assembly));
		}

		public SilentHunterParsersConfigurer FromAssembly(ControllerAssembly controllerAssembly)
		{
			return FromAssembly(() => controllerAssembly);
		}

		public SilentHunterParsersConfigurer FromAssembly(Func<ControllerAssembly> controllerAssembly)
		{
			return FromAssembly(_ => controllerAssembly());
		}

		public SilentHunterParsersConfigurer FromAssembly(Func<IServiceProvider, ControllerAssembly> controllerAssembly)
		{
			_serviceCollection.AddSingleton(controllerAssembly);

			return _configurer;
		}
	}
}