#if NETFRAMEWORK
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using FluentAssertions;
using Xunit;

namespace SilentHunter.Controllers.Compiler.Tests
{
	public class CSharpCompilerTests : CompilerTests<CSharpCompiler>
	{
		[Fact]
		public void Given_valid_source_files_and_options_when_compiling_should_build_assembly()
		{
			string targetDllFile = Path.GetTempFileName();

			var compilerOptions = new CompilerOptions
			{
				ReferencedAssemblies = GetReferencedAssemblies(),
				OutputFile = targetDllFile,
				IgnoreCompilerErrors = new[] { "CS0042" }
			};

			// Act
			// ReSharper disable once RedundantArgumentDefaultValue
			string pdbFile = Path.Combine(Path.GetDirectoryName(targetDllFile), Path.GetFileNameWithoutExtension(targetDllFile) + ".pdb");
			try
			{
				Assembly result = _sut.CompileCode(_fakeSourceFiles.Keys, compilerOptions);

				// Assert
				var targetFileData = new FileInfo(targetDllFile);
				targetFileData.Exists.Should().BeTrue("an assembly DLL should be generated");
				targetFileData.Length.Should().NotBe(0, "the assembly DLL contains IL");

				var pdbFileData = new FileInfo(pdbFile);
				pdbFileData.Exists.Should().BeTrue("a PDB should be generated");
				pdbFileData.Length.Should().NotBe(0);
				if (File.Exists(pdbFile))
				{
					File.Delete(pdbFile);
				}

				result.Should().BeNull("'loadAssembly' was set to false");

				// Load into mem first, so we can delete the DLL later and not lock it.
				Func<Assembly> tryAssembly = () => Assembly.Load(File.ReadAllBytes(targetDllFile));
				result = tryAssembly.Should().NotThrow("the assembly should successfully be loaded").Which;

				result.ExportedTypes.Should().Contain(t => _typesToCompile.Contains(t.Name));
			}
			finally
			{
				if (File.Exists(targetDllFile))
				{
					File.Delete(targetDllFile);
				}
			}
		}

		protected override List<string> GetReferencedAssemblies()
		{
			return new List<string>
			{
				"System.dll"
			};
		}
	}
}
#endif