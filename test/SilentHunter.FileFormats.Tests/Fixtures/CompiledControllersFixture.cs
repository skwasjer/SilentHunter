﻿using System;
using Microsoft.Extensions.DependencyInjection;
using SilentHunter.Controllers.Compiler.DependencyInjection;
using SilentHunter.FileFormats.DependencyInjection;
using Xunit;

namespace SilentHunter.FileFormats.Fixtures
{
	[CollectionDefinition(nameof(CompiledControllers))]
	public class CompiledControllers : ICollectionFixture<CompiledControllersFixture>
	{
	}

	public class CompiledControllersFixture : IDisposable
	{
		public CompiledControllersFixture()
		{
			const string controllerPath = @"..\..\..\..\..\src\SilentHunter.Controllers";
			var services = new ServiceCollection();
			services.AddSilentHunterParsers(configurer => configurer
				.Controllers.CompileFrom(controllerPath, "Controllers", ignorePaths: f => f.StartsWith(@"obj\"))
			);

			ServiceProvider = services.BuildServiceProvider();
		}

		public IServiceProvider ServiceProvider { get; }

		public void Dispose()
		{
			((IDisposable)ServiceProvider)?.Dispose();
		}
	}
}
