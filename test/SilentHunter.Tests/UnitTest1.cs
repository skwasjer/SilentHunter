using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using SilentHunter.FileFormats.Dat;
using SilentHunter.FileFormats.Dat.Chunks;
using SilentHunter.FileFormats.Dat.Controllers;
using SilentHunter.Fixtures;
using Xunit;

namespace SilentHunter
{
	public class UnitTest1 : IClassFixture<CompiledControllersFixture>
	{
		private readonly CompiledControllersFixture _compiledControllers;
		private const string GamePath = @"N:\Games\Silent Hunter 4 Wolves of the Pacific\Data\Terrain\Data";

		public UnitTest1(CompiledControllersFixture compiledControllers)
		{
			_compiledControllers = compiledControllers;
		}

		//[Fact(Skip = "No BFI")]
		//public void TestMethod1()
		//{
		//	using (var reader = new BinaryReader(File.OpenRead(Path.Combine(GamePath, "TerrainData.BFI")), FileEncoding.Default))
		//	{
		//		while (reader.BaseStream.Position < reader.BaseStream.Length)
		//		{
		//			var count = reader.ReadInt32();
		//			for (var i = 0; i < count; i++)
		//			{
		//				var str = reader.ReadNullTerminatedString();
		//				var sz = reader.ReadInt32();
		//				Debug.WriteLine(str + ":" + sz);
		//			}
		//		}
		//	}
		//}

		[Fact]
		public async Task ErrorInReader()
		{
			var datFile = _compiledControllers.ServiceProvider.GetRequiredService<DatFile>();

			using (var fs = File.OpenRead(@"N:\Games\SH Mods\Animated Fans (v1.2)\Data\Interior\NSS_Gato\error.dat"))
			{
				await datFile.LoadAsync(fs);

				datFile.Chunks.Should().NotBeEmpty();
				var c = datFile.Chunks.OfType<ControllerDataChunk>().First();
				var dt = (object)c.ControllerData;
				dt.Should().NotBeNull();

				string saveAs = @"N:\Games\SH Mods\Animated Fans (v1.2)\Data\Interior\NSS_Gato\error.dat.copy-of-unit-test.dat";
				await datFile.SaveAsync(saveAs);

				using (var fs2 = File.OpenRead(saveAs))
				{
					await datFile.LoadAsync(fs2);
				}
			}
		}

		[Fact]
		public async Task TestDat()
		{
			var ctrlAsm = _compiledControllers.ServiceProvider.GetRequiredService<ControllerAssembly>();
			var datFile = _compiledControllers.ServiceProvider.GetRequiredService<DatFile>();

			//using (var fs = File.OpenRead(@"N:\Games\SH Mods\TriggerMaru_Overhaul_17\Data\Library\Torpedoes_US.sim"))
			//using (var fs = File.OpenRead(@"N:\Games\SH Mods\Animated Fans (v1.2)\Data\Interior\NSS_Gato\NSS_Gato_CR.dat")) // List
			//using (var fs = File.OpenRead(@"N:\Games\SH Mods\TriggerMaru_Overhaul_17\Data\Sea\AuxGunboat\AuxGunboat.zon")) // SHUnion
			//using (var fs = File.OpenRead(@"N:\Games\SH Mods\TriggerMaru_Overhaul_17\Data\Scene.dat"))
			//using (var fs = File.OpenRead(@"N:\Games\SH Mods\TriggerMaru_Overhaul_17\Data\Library\Particles.dat"))
			//using (var fs = File.OpenRead(@"N:\Games\SH Mods\TriggerMaru_Overhaul_17\Data\Interior\CaptainRoom\CaptainRoom.dat"))
			//using (var fs = File.OpenRead(@"N:\Games\SH Mods\Animated Fans (v1.2)\Data\Library\GraphFanBlades.dat"))
			using (var fs = File.OpenRead(@"N:\Games\SH Mods\TriggerMaru_Overhaul_17\Data\Air\AFB_p38j\AFB_p38j.dat")) // RawList
			{
				await datFile.LoadAsync(fs);

				datFile.Chunks.Should().NotBeEmpty();
				var c = datFile.Chunks.OfType<ControllerDataChunk>().First();
				var dt = (object)c.ControllerData;
				dt.Should().NotBeNull();

				datFile.Chunks.OfType<ControllerDataChunk>()
					.Select(cd => (Type)cd.ControllerData.GetType())
					.Should()
					.NotContain(v => v == typeof(byte[]));

				string saveAs = @"N:\Games\SH Mods\Animated Fans (v1.2)\Data\Interior\NSS_Gato\NSS_Gato_CT.dat.copy-of-unit-test.dat";
				await datFile.SaveAsync(saveAs);

				using (var fs2 = File.OpenRead(saveAs))
				{
					await datFile.LoadAsync(fs2);
					datFile.Chunks.OfType<ControllerDataChunk>()
						.Select(cd => (Type)cd.ControllerData.GetType())
						.Should()
						.NotContain(v => v == typeof(byte[]));
				}
			}
		}
	}
}
