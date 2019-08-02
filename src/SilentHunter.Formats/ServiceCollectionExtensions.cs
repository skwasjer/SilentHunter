using System;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SilentHunter.Dat;
using SilentHunter.Dat.Controllers;
using SilentHunter.Off;
using SilentHunter.Sdl;
using skwas.IO;

namespace SilentHunter
{
	/// <summary>
	/// In order to keep the fluent configuration API clean, this interface is used to provide the <see cref="IServiceCollection"/> without exposing it to Intellisense.
	/// </summary>
	public interface IServiceCollectionProvider
	{
		IServiceCollection ServiceCollection { get; }
	}

	public class SilentHunterParsersConfigurer : IServiceCollectionProvider
	{
		private readonly IServiceCollection _serviceCollection;

		internal SilentHunterParsersConfigurer(IServiceCollection serviceCollection)
		{
			_serviceCollection = serviceCollection;

			Controllers = new ControllerConfigurer(this);
		}

		IServiceCollection IServiceCollectionProvider.ServiceCollection => _serviceCollection;

		public ControllerConfigurer Controllers { get; }
	}

	public class ControllerConfigurer : IServiceCollectionProvider
	{
		private readonly SilentHunterParsersConfigurer _configurer;
		private readonly IServiceCollection _serviceCollection;

		internal ControllerConfigurer(SilentHunterParsersConfigurer configurer)
		{
			_configurer = configurer;
			_serviceCollection = ((IServiceCollectionProvider)_configurer).ServiceCollection;
		}

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

	public static class ServiceCollectionExtensions
	{
		public static IServiceCollection AddSilentHunterParsers(this IServiceCollection services, Action<SilentHunterParsersConfigurer> configurer)
		{
			var c = new SilentHunterParsersConfigurer(services);
			configurer(c);

			services.TryAddSingleton<IItemFactory, ItemFactory>();
			services.TryAddSingleton<IControllerFactory, ControllerFactory>();

			services.TryAddTransient<IControllerReader, ControllerReader>();
			services.TryAddTransient<IControllerWriter, ControllerWriter>();

			services.TryAddTransient<IChunkResolver<DatFile.Magics>, DatChunkResolver>();
			services.TryAddTransient<IChunkActivator, DependencyInjectionChunkActivator>();

			services.TryAddTransient<DatFile>();
			services.TryAddTransient<OffFile>();
			services.TryAddTransient<SdlFile>();

			return services;
		}
	}
}
