using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SilentHunter.Controllers;
using SilentHunter.FileFormats.ChunkedFiles;
using SilentHunter.FileFormats.Dat;
using SilentHunter.FileFormats.Dat.Controllers;
using SilentHunter.FileFormats.Dat.Controllers.Serialization;
using SilentHunter.FileFormats.Off;
using SilentHunter.FileFormats.Sdl;

namespace SilentHunter.FileFormats.DependencyInjection
{
	/// <summary>
	/// Extensions for <see cref="IServiceCollection"/>.
	/// </summary>
	public static class ServiceCollectionExtensions
	{
		/// <summary>
		/// Adds Silent Hunter parser services.
		/// </summary>
		/// <param name="services">The service collection.</param>
		/// <param name="configurer">The configurer.</param>
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
				where TController : Controller
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
			AddMapping<BehaviorController, BehaviorControllerSerializer>();
			AddMapping<Controller, ControllerSerializer>();
		}
	}
}
