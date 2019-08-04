#if NETCOREAPP
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using System.Reflection;
using System.Text;
using FluentAssertions;
using SilentHunter.Testing.FluentAssertions;
using Xunit;

namespace SilentHunter.Controllers.Compiler.Tests
{
	public class RoslynCompilerTests
	{
		private readonly string[] _typesToCompile = { "Foo", "Bar" };

		private const string TargetDllFile = @"C:\bin\output.dll";
		private readonly RoslynCompiler _sut;
		private readonly MockFileSystem _fileSystem;
		private readonly Dictionary<string, MockFileData> _fakeSourceFiles;

		public RoslynCompilerTests()
		{
			_fakeSourceFiles = new Dictionary<string, MockFileData>();
			foreach (string typeName in _typesToCompile)
			{
				_fakeSourceFiles.Add($"C:\\src\\{typeName}.cs", new MockFileData($"public class {typeName} {{ }}"));
			}

			_fileSystem = new MockFileSystem(_fakeSourceFiles);

			_sut = new RoslynCompiler(_fileSystem);
		}

		[Fact]
		public void Given_null_file_names_when_compiling_should_throw()
		{
			// Act
			Action act = () => _sut.CompileCode((string[])null, new CompilerOptions());

			// Assert
			act.Should().Throw<ArgumentNullException>()
				.WithParamName("fileNames");
		}

		[Fact]
		public void Given_null_options_when_compiling_should_throw()
		{
			// Act
			Action act = () => _sut.CompileCode(new List<string>(), null);

			// Assert
			act.Should().Throw<ArgumentNullException>()
				.WithParamName("options");
		}

		[Fact]
		public void Given_no_file_names_when_compiling_should_throw()
		{
			// Act
			Action act = () => _sut.CompileCode(new List<string>(), new CompilerOptions());

			// Assert
			act.Should().Throw<ArgumentOutOfRangeException>()
				.WithParamName("fileNames");
		}

		[Fact]
		public void Given_outputDir_does_not_exist_when_compiling_should_create_dir()
		{
			Action act = () =>
				_sut.CompileCode(_fakeSourceFiles.Keys, new CompilerOptions
				{
					OutputFile = TargetDllFile
				});

			act.Should().NotThrow<DirectoryNotFoundException>();
		}

		[Theory]
		[InlineData(true)]
		[InlineData(false)]
		public void Given_valid_source_files_and_options_when_compiling_should_build_assembly(bool loadAssemblyImmediately)
		{
			var compilerOptions = new CompilerOptions
			{
				ReferencedAssemblies = new List<string>
				{
					typeof(object).Assembly.Location,
				},
				OutputFile = TargetDllFile
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
				result = tryAssembly.Should().NotThrow("the assembly can successfully be loaded").Which;
			}

			result.GetName().Name.Should().Be("output");
			result.ExportedTypes.Should().Contain(t => _typesToCompile.Contains(t.Name));
		}

		[Fact]
		public void Given_source_file_has_syntax_error_when_compiling_should_throw()
		{
			var compilerOptions = new CompilerOptions
			{
				ReferencedAssemblies = new List<string>
				{
					typeof(object).Assembly.Location,
				},
				OutputFile = TargetDllFile
			};

			// Replace contents of first file with invalid source code.
			_fakeSourceFiles.First().Value.Contents = Encoding.UTF8.GetBytes("Invalid C# code");

			_fileSystem.GetFile(TargetDllFile).Should().BeNull("before compiling the target DLL assembly should not exist");

			// Act
			Action act = () => _sut.CompileCode(_fakeSourceFiles.Keys, compilerOptions);

			// Assert
			act.Should().Throw<CompileException>();

			MockFileData targetFileData = _fileSystem.GetFile(TargetDllFile);
			targetFileData.Should().NotBeNull("after compiling the target DLL assembly file is created");
			targetFileData.Contents.Should().BeEmpty("the contents should however be empty");
		}
	}
}
#endif