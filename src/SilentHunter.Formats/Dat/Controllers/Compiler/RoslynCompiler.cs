#if NETSTANDARD2_0
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;

namespace SilentHunter.Dat.Controllers.Compiler
{
	public class RoslynCompiler : ICSharpCompiler
	{
		private readonly CSharpParseOptions _parseOptions;
		private static readonly MetadataReference NetStandard = MetadataReference.CreateFromFile(Assembly.Load("netstandard, Version=2.0.0.0").Location);
		private static MetadataReference Corlib = MetadataReference.CreateFromFile(typeof(object).GetTypeInfo().Assembly.Location);
		private static MetadataReference Primitives = MetadataReference.CreateFromFile(typeof(Color).GetTypeInfo().Assembly.Location);
		private static MetadataReference Runtime = MetadataReference.CreateFromFile(Assembly.Load("System.Runtime, PublicKeyToken=b03f5f7f11d50a3a").Location);
		private static MetadataReference RuntimeExtensions = MetadataReference.CreateFromFile(Assembly.Load("System.Runtime.Extensions, PublicKeyToken=b03f5f7f11d50a3a").Location);

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

			var references = new List<MetadataReference>
			{
				NetStandard,
				Corlib,
				Primitives,
				Runtime,
				RuntimeExtensions
			};
			if (options.ReferencedAssemblies != null)
			{
				references.AddRange(options.ReferencedAssemblies.Select(ra => MetadataReference.CreateFromFile(ra)));
			}

			CSharpCompilation compilation = CSharpCompilation.Create(
				"Controllers",
				fileNames.Select(fn => CSharpSyntaxTree.ParseText(SourceText.From(File.ReadAllText(fn)), _parseOptions).WithFilePath(fn)),
				references,
				new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

			FileStream docStream = string.IsNullOrEmpty(options.DocFile) ? null : File.Open(options.DocFile, FileMode.Create);
			try
			{
				using (FileStream dllStream = File.Open(options.OutputPath, FileMode.Create))
				{
					EmitResult emitResult = compilation.Emit(dllStream, xmlDocumentationStream: docStream);
					LogResults(emitResult);

					// Display a successful compilation message.
					Debug.WriteLine("Code built into assembly '{0}' successfully.", options.OutputPath);
				}
			}
			finally
			{
				docStream?.Dispose();
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
				foreach (var error in results.Diagnostics
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