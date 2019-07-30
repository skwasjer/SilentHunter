using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using FluentAssertions;
using SilentHunter.Dat;
using SilentHunter.Dat.Chunks;
using SilentHunter.Dat.Controllers;
using skwas.IO;
using Xunit;

namespace SilentHunter.Tests
{
	public class UnitTest1
	{
		private const string GamePath = @"N:\Games\Silent Hunter 4 Wolves of the Pacific\Data\Terrain\Data";

		[Fact(Skip = "No BFI")]
		public void TestMethod1()
		{
			using (var reader = new BinaryReader(File.OpenRead(Path.Combine(GamePath, "TerrainData.BFI")), Encoding.ParseEncoding))
			{
				while (reader.BaseStream.Position < reader.BaseStream.Length)
				{
					var count = reader.ReadInt32();
					for (var i = 0; i < count; i++)
					{
						var str = reader.ReadNullTerminatedString();
						var sz = reader.ReadInt32();
						Debug.WriteLine(str + ":" + sz);
					}
				}
			}
		}

		[Fact]
		public void ErrorInReader()
		{
			string controllerPath = @"..\..\..\..\src\SilentHunter.Controllers";

			var controllerAssemblyCompiler = new ControllerAssemblyCompiler(controllerPath)
				.AssemblyName("Controllers");
			ControllerAssembly assembly = controllerAssemblyCompiler.Compile();

			ControllerAssembly.Current = assembly;

			var x = ControllerAssembly.Current.HelpText["asd"];

			//			ControllerAssembly.LoadFrom(assembly);

			using (var fs = File.OpenRead(@"N:\Games\SH Mods\Animated Fans (v1.2)\Data\Interior\NSS_Gato\error.dat"))
			using (var datFile = new DatFile(false))
			{
				datFile.Load(fs);

				datFile.Chunks.Should().NotBeEmpty();
				var c = datFile.Chunks.OfType<ControllerDataChunk>().First();
				var dt = (object)c.ControllerData;
				dt.Should().NotBeNull();

				string saveAs = @"N:\Games\SH Mods\Animated Fans (v1.2)\Data\Interior\NSS_Gato\error.dat.copy-of-unit-test.dat";
				datFile.Save(saveAs);

				using (var fs2 = File.OpenRead(saveAs))
				{
					datFile.Load(fs2);
				}
			}
		}

		[Fact]
		public void TestDat()
		{
			string controllerPath = @"..\..\..\..\src\SilentHunter.Controllers";

			var controllerAssemblyCompiler = new ControllerAssemblyCompiler(controllerPath)
				.AssemblyName("Controllers");
			//controllerAssemblyCompiler.CleanArtifacts();
			ControllerAssembly assembly = controllerAssemblyCompiler.Compile();

			ControllerAssembly.Current = assembly;

			var x = ControllerAssembly.Current.HelpText["asd"];

//			ControllerAssembly.LoadFrom(assembly);

			using (var fs = File.OpenRead(@"N:\Games\SH Mods\Animated Fans (v1.2)\Data\Interior\NSS_Gato\NSS_Gato_CT.dat"))
			using (var datFile = new DatFile(false))
			{
				datFile.Load(fs);

				datFile.Chunks.Should().NotBeEmpty();
				var c = datFile.Chunks.OfType<ControllerDataChunk>().First();
				var dt = (object)c.ControllerData;
				dt.Should().NotBeNull();

				string saveAs = @"N:\Games\SH Mods\Animated Fans (v1.2)\Data\Interior\NSS_Gato\NSS_Gato_CT.dat.copy-of-unit-test.dat";
				datFile.Save(saveAs);

				using (var fs2 = File.OpenRead(saveAs))
				{
					datFile.Load(fs2);
				}
			}
		}
	}
}
