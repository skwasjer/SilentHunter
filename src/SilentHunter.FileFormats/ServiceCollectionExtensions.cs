using System;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SilentHunter.Controllers;
using SilentHunter.FileFormats.Dat;
using SilentHunter.FileFormats.Dat.Controllers;
using SilentHunter.FileFormats.Dat.Controllers.Serialization;
using SilentHunter.FileFormats.Off;
using SilentHunter.FileFormats.Sdl;
using skwas.IO;

namespace SilentHunter.FileFormats
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

			services.AddDatFile();
			services.AddOffFile();
			services.AddSdlFile();

			return services;
		}

		private static IServiceCollection AddSdlFile(this IServiceCollection services)
		{
			services.TryAddTransient<SdlFile>();
			return services;
		}

		private static IServiceCollection AddOffFile(this IServiceCollection services)
		{
			services.TryAddTransient<OffFile>();
			return services;
		}

		private static IServiceCollection AddDatFile(this IServiceCollection services)
		{
			services.TryAddSingleton<IItemFactory, ItemFactory>();
			services.TryAddSingleton<IControllerFactory, ControllerFactory>();

			services.AddControllerSerializers();
			services.TryAddTransient<IControllerReader, ControllerReader>();
			services.TryAddTransient<IControllerWriter, ControllerWriter>();

			services.TryAddSingleton<IChunkResolver<DatFile.Magics>, DatChunkResolver>();
			services.TryAddScoped<IChunkActivator, DependencyInjectionChunkActivator>();

			services.TryAddTransient<DatFile>();
			return services;
		}

		private static void AddControllerSerializers(this IServiceCollection services)
		{
			void AddMapping<TController, TSerializer>()
				where TController : RawController
				where TSerializer : class, IControllerSerializer
			{
				services.AddTransient<TSerializer>();
				services.AddTransient<IControllerSerializer, TSerializer>();
				services.AddTransient(s => new ControllerSerializerResolver.Mapping
				{
					ControllerType = typeof(TController),
					ImplementationFactory = s.GetRequiredService<TSerializer>
				});
			}

			services.AddScoped<ControllerSerializerResolver>();

			// Note: order is important. First controller type to match, that serializer will be used. Thus, start with the most specific serializers.
			AddMapping<StateMachineController, StateMachineControllerSerializer>();
			AddMapping<MeshAnimationController, MeshAnimationControllerSerializer>();
			AddMapping<Controller, ControllerSerializer>();
			AddMapping<RawController, RawControllerSerializer>();
		}
	}
}
