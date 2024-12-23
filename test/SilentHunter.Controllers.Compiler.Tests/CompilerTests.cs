using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using SilentHunter.Testing.FluentAssertions;

namespace SilentHunter.Controllers.Compiler.Tests
{
	public abstract class CompilerTests<TCompiler> : IAsyncLifetime
		where TCompiler : ICSharpCompiler
	{
		protected readonly string[] _typesToCompile = { "Foo", "Bar" };

		private static readonly string TargetDllFile = $"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}output.dll";
		protected ICSharpCompiler _sut;
		protected readonly Dictionary<string, MockFileData> _fakeSourceFiles;
		private readonly MockFileSystem _fileSystem;

		protected CompilerTests()
		{
			_fakeSourceFiles = new Dictionary<string, MockFileData>();
			foreach (string typeName in _typesToCompile)
			{
				_fakeSourceFiles.Add($"src{Path.DirectorySeparatorChar}Fake{Path.DirectorySeparatorChar}{typeName}.cs", new MockFileData($"public class {typeName} {{ }}"));
			}

			_fileSystem = new MockFileSystem(_fakeSourceFiles);
		}

		public virtual Task InitializeAsync()
		{
			_sut = CreateSubject(_fileSystem);
			return Task.CompletedTask;
		}

		public virtual Task DisposeAsync()
		{
			// ReSharper disable once SuspiciousTypeConversion.Global - justification: our CSharpCompiler implements it
			(_sut as IDisposable)?.Dispose();
			return Task.CompletedTask;
		}

		private TCompiler CreateSubject(IFileSystem fileSystem)
		{
			return (TCompiler)Activator.CreateInstance(
				typeof(TCompiler),
				BindingFlags.Instance | BindingFlags.NonPublic,
				null,
				new object[] { fileSystem },
				null);
		}

		[Fact]
		public void Given_null_file_names_when_compiling_should_throw()
		{
			// Act
			Action act = () => _sut.CompileCode((string[])null, new CompilerOptions());

			// Assert
			act.Should()
				.Throw<ArgumentNullException>()
				.WithParamName("fileNames");
		}

		[Fact]
		public void Given_null_options_when_compiling_should_throw()
		{
			// Act
			Action act = () => _sut.CompileCode(new List<string>(), null);

			// Assert
			act.Should()
				.Throw<ArgumentNullException>()
				.WithParamName("options");
		}

		[Fact]
		public void Given_no_file_names_when_compiling_should_throw()
		{
			// Act
			Action act = () => _sut.CompileCode(new List<string>(), new CompilerOptions());

			// Assert
			act.Should()
				.Throw<ArgumentOutOfRangeException>()
				.WithParamName("fileNames");
		}

		[Fact]
		public void Given_outputDir_does_not_exist_when_compiling_should_create_dir()
		{
			Action act = () =>
				_sut.CompileCode(_fakeSourceFiles.Keys, new CompilerOptions { OutputFile = TargetDllFile });

			act.Should().NotThrow<DirectoryNotFoundException>();
		}

#if NETCOREAPP
		[Theory]
		[InlineData(true)]
		[InlineData(false)]
		public void Given_valid_source_files_and_options_when_compiling_should_build_assembly(bool loadAssemblyImmediately)
		{
			var compilerOptions = new CompilerOptions
			{
				ReferencedAssemblies = GetReferencedAssemblies(),
				OutputFile = TargetDllFile,
				IgnoreCompilerErrors = new[] { "CS0042" }
			};

			// Act
			// ReSharper disable once RedundantArgumentDefaultValue
			Assembly result = _sut.CompileCode(_fakeSourceFiles.Keys, compilerOptions, loadAssemblyImmediately);

			// Assert
			MockFileData targetFileData = _fileSystem.GetFile(TargetDllFile);
			targetFileData.Should().NotBeNull("an assembly DLL should be generated");
			targetFileData.Contents.Should().NotBeEmpty("the assembly DLL contains IL");

			string pdbFile = _fileSystem.Path.Combine(_fileSystem.Path.GetDirectoryName(TargetDllFile), _fileSystem.Path.GetFileNameWithoutExtension(TargetDllFile) + ".pdb");
			MockFileData pdbFileData = _fileSystem.GetFile(pdbFile);
			pdbFileData.Should().NotBeNull("a PDB should be generated");
			pdbFileData.Contents.Should().NotBeEmpty();

			if (loadAssemblyImmediately)
			{
				result.Should().NotBeNull("'loadAssembly' was set to true");
			}
			else
			{
				result.Should().BeNull("'loadAssembly' was set to false");

				Func<Assembly> tryAssembly = () => Assembly.Load(targetFileData.Contents);
				result = tryAssembly.Should().NotThrow("the assembly should successfully be loaded").Which;
			}

			result.GetName().Name.Should().Be("output");
			result.ExportedTypes.Should().Contain(t => _typesToCompile.Contains(t.Name));
		}
#endif

		[Fact]
		public void Given_source_file_has_syntax_error_when_compiling_should_throw()
		{
			var compilerOptions = new CompilerOptions { ReferencedAssemblies = GetReferencedAssemblies(), OutputFile = TargetDllFile };

			// Replace contents of first file with invalid source code.
			_fakeSourceFiles.First().Value.Contents = Encoding.UTF8.GetBytes("Invalid C# code");

			_fileSystem.GetFile(TargetDllFile).Should().BeNull("before compiling the target DLL assembly should not exist");

			// Act
			Action act = () => _sut.CompileCode(_fakeSourceFiles.Keys, compilerOptions);

			// Assert
			act.Should().Throw<CompileException>();

			MockFileData targetFileData = _fileSystem.GetFile(TargetDllFile);
			if (targetFileData != null)
			{
				targetFileData.Should().NotBeNull("after compiling the target DLL assembly file is created");
				targetFileData.Contents.Should().BeEmpty("the contents should however be empty");
			}
		}

		protected abstract List<string> GetReferencedAssemblies();
	}
}