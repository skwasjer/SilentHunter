#if NETSTANDARD2_0
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.Text;

namespace SilentHunter.Controllers.Compiler
{
	public class RoslynCompiler : ICSharpCompiler
	{
		private readonly IFileSystem _fileSystem;
		private readonly CSharpParseOptions _parseOptions;

		public RoslynCompiler()
			: this(new FileSystem())
		{
		}

		public RoslynCompiler(IFileSystem fileSystem)
		{
			_fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
			_parseOptions = new CSharpParseOptions(LanguageVersion.CSharp8, DocumentationMode.Diagnose);
		}

		public Assembly CompileCode(ICollection<string> fileNames, CompilerOptions options, bool loadAssembly = false)
		{
			if (fileNames == null)
			{
				throw new ArgumentNullException(nameof(fileNames));
			}

			if (options == null)
			{
				throw new ArgumentNullException(nameof(options));
			}

			if (fileNames.Count == 0)
			{
				throw new ArgumentOutOfRangeException(nameof(fileNames), "Expected at least one filename.");
			}

			List<PortableExecutableReference> references = options.ReferencedAssemblies?.Select(ra => MetadataReference.CreateFromFile(ra)).ToList() ?? new List<PortableExecutableReference>();

			CSharpCompilation compilation = CSharpCompilation.Create(
				_fileSystem.Path.GetFileNameWithoutExtension(options.OutputDir),
				fileNames.Select(fn => CSharpSyntaxTree.ParseText(SourceText.From(_fileSystem.File.ReadAllText(fn), System.Text.Encoding.UTF8), _parseOptions).WithFilePath(fn)),
				references,
				new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

			string pdbPath = _fileSystem.Path.Combine(_fileSystem.Path.GetDirectoryName(options.OutputDir), _fileSystem.Path.GetFileNameWithoutExtension(options.OutputDir) + ".pdb");
			using (Stream dllStream = _fileSystem.File.Open(options.OutputDir, FileMode.Create))
			using (Stream pdbStream = _fileSystem.File.Open(pdbPath, FileMode.Create))
			using (Stream docStream = string.IsNullOrEmpty(options.DocFile) ? Stream.Null : _fileSystem.File.Open(options.DocFile, FileMode.Create))
			{
				EmitResult emitResult = compilation.Emit(dllStream, pdbStream, docStream);
				LogResults(emitResult);

				// Display a successful compilation message.
				Debug.WriteLine("Code built into assembly '{0}' successfully.", options.OutputDir);
			}

			return loadAssembly ? Assembly.Load(options.OutputDir) : null;
		}

		public Assembly CompileCode(string code, CompilerOptions options)
		{
			throw new NotImplementedException();
		}

		private static void LogResults(EmitResult results)
		{
			//			results.Output.Cast<string>().ToList().ForEach(s => Debug.WriteLine(s));
			if (!results.Success)
			{
				// Display compilation errors.
				var errorMsg = new StringBuilder();
				errorMsg.AppendLine("Errors building assembly...");
				foreach (IGrouping<Location, Diagnostic> error in results.Diagnostics
					.Where(d => d.Severity >= DiagnosticSeverity.Warning)
					.GroupBy(d => d.Location))
				{
					//errorMsg.AppendLine(error.Key.ToString());
					foreach (Diagnostic diagnostic in error)
					{
						errorMsg.AppendFormat("  - {0}", diagnostic);
						errorMsg.AppendLine();
					}
				}

				Debug.Write(errorMsg.ToString());
				throw new Exception(errorMsg.ToString());
			}
		}
	}
}
#endif