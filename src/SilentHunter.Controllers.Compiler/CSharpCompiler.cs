#if NETFRAMEWORK
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Abstractions;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.CSharp;

namespace SilentHunter.Controllers.Compiler
{
	public sealed class CSharpCompiler : ICSharpCompiler, IDisposable
	{
		private const string RequiredCompilerOptions = "/target:library /optimize";
		private readonly IFileSystem _fileSystem;
		private readonly CompilerParameters _compilerParams;
		private CodeDomProvider _codeProvider;
		private bool _disposed;

		public CSharpCompiler()
			: this(new FileSystem())
		{
		}

		// ReSharper disable once MemberCanBePrivate.Global - justification: used by unit test
		internal CSharpCompiler(IFileSystem fileSystem)
			: this(fileSystem, (CSharpCodeProvider)CodeDomProvider.CreateProvider("CSharp"))
		{
		}

		private CSharpCompiler(IFileSystem fileSystem, CodeDomProvider codeDomProvider)
		{
			_fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
			_codeProvider = codeDomProvider ?? throw new ArgumentNullException(nameof(codeDomProvider));
			_compilerParams = new CompilerParameters
			{
				GenerateExecutable = false,
				GenerateInMemory = false,
#if DEBUG
				IncludeDebugInformation = true,
#endif
				TreatWarningsAsErrors = false
			};
		}

		~CSharpCompiler()
		{
			Dispose(false);
		}

		private void Dispose(bool disposing)
		{
			if (_disposed)
			{
				return;
			}

			if (disposing)
			{
				_codeProvider?.Dispose();
				_codeProvider = null;
			}

			_disposed = true;
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Compiles the specified <paramref name="fileNames" /> and <see name="DocFile" /> into an <see cref="Assembly" />.
		/// </summary>
		/// <param name="fileNames"></param>
		/// <param name="options"></param>
		/// <param name="loadAssembly"></param>
		/// <returns></returns>
		public Assembly CompileCode(ICollection<string> fileNames, CompilerOptions options, bool loadAssembly = false)
		{
			if (_disposed)
			{
				throw new ObjectDisposedException(GetType().Name);
			}

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

			string outputDir = _fileSystem.Path.GetDirectoryName(options.OutputFile);
			if (!_fileSystem.Directory.Exists(outputDir))
			{
				_fileSystem.Directory.CreateDirectory(outputDir);
			}

			_compilerParams.OutputAssembly = options.OutputFile;

			_compilerParams.ReferencedAssemblies.Clear();
			if (options.ReferencedAssemblies != null)
			{
				_compilerParams.ReferencedAssemblies.AddRange(new HashSet<string>(options.ReferencedAssemblies).ToArray());
			}

			_compilerParams.CompilerOptions = RequiredCompilerOptions;
			if (!string.IsNullOrEmpty(options.DocFile))
			{
				_compilerParams.CompilerOptions += $" /doc:\"{options.DocFile}\"";
			}

			CompilerResults results = _codeProvider.CompileAssemblyFromSource(_compilerParams, fileNames.Select(fn => _fileSystem.File.ReadAllText(fn)).ToArray());

			LogResults(options, results);

			// Display a successful compilation message.
			Debug.WriteLine("Code built into assembly '{0}' successfully.", options.OutputFile);
			return loadAssembly ? Assembly.Load(_fileSystem.File.ReadAllBytes(options.OutputFile)) : null;
		}

		private static void LogResults(CompilerOptions options, CompilerResults results)
		{
			results.Output.Cast<string>().ToList().ForEach(s => Debug.WriteLine(s));

			if (results.Errors.Count > 0)
			{
				// Display compilation errors.
				var errorMsg = new StringBuilder();
				errorMsg.AppendLine("Errors building assembly...");

				List<CompilerError> filteredErrors = results.Errors
					.Cast<CompilerError>()
					.Where(ce => !ce.IsWarning)
					.Where(ce => options.IgnoreCompilerErrors == null || !options.IgnoreCompilerErrors.Contains(ce.ErrorNumber))
					.ToList();

				if (filteredErrors.Count == 0)
				{
					return;
				}

				foreach (CompilerError error in filteredErrors)
				{
					errorMsg.AppendFormat("  - {0}", error);
					errorMsg.AppendLine();
				}

				Debug.Write(errorMsg.ToString());
				throw new CompileException(errorMsg.ToString());
			}
		}
	}
}
#endif