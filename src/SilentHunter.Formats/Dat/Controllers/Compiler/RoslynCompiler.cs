#if NETSTANDARD2_0
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.Text;

namespace SilentHunter.Dat.Controllers.Compiler
{
	public class RoslynCompiler : ICSharpCompiler
	{
		private readonly CSharpParseOptions _parseOptions;

		public RoslynCompiler()
		{
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
				Path.GetFileNameWithoutExtension(options.OutputPath),
				fileNames.Select(fn => CSharpSyntaxTree.ParseText(SourceText.From(File.ReadAllText(fn), System.Text.Encoding.UTF8), _parseOptions).WithFilePath(fn)),
				references,
				new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

			string pdbPath = Path.Combine(Path.GetDirectoryName(options.OutputPath), Path.GetFileNameWithoutExtension(options.OutputPath) + ".pdb");
			using (Stream dllStream = File.Open(options.OutputPath, FileMode.Create))
			using (Stream pdbStream = File.Open(pdbPath, FileMode.Create))
			using (Stream docStream = string.IsNullOrEmpty(options.DocFile) ? Stream.Null : File.Open(options.DocFile, FileMode.Create))
			{
				EmitResult emitResult = compilation.Emit(dllStream, pdbStream, docStream);
				LogResults(emitResult);

				// Display a successful compilation message.
				Debug.WriteLine("Code built into assembly '{0}' successfully.", options.OutputPath);
			}

			return loadAssembly ? Assembly.Load(options.OutputPath) : null;
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