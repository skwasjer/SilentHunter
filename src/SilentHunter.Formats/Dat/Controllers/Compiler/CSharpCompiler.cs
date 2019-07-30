using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.CSharp;

namespace SilentHunter.Dat.Controllers.Compiler
{
	public sealed class CSharpCompiler : ICSharpCompiler, IDisposable
	{
		private const string RequiredCompilerOptions = "/target:library";
		private CSharpCodeProvider _codeProvider;
		private readonly CompilerParameters _compilerParams;
		private bool _disposed;

		#region .ctor/cleanup

		public CSharpCompiler()
		{
			_codeProvider = (CSharpCodeProvider)CodeDomProvider.CreateProvider("CSharp");
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

		#endregion

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

			_compilerParams.OutputAssembly = options.OutputPath;

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

			CompilerResults results = _codeProvider.CompileAssemblyFromFile(_compilerParams, fileNames.ToArray());

			LogResults(results);

			// Display a successful compilation message.
			Debug.WriteLine("Code built into assembly '{0}' successfully.", options.OutputPath);
			return loadAssembly ? results.CompiledAssembly : null;
		}

		/// <summary>
		/// Compiles code into an <see cref="Assembly" />.
		/// </summary>
		/// <param name="code"></param>
		/// <param name="options"></param>
		/// <returns></returns>
		public Assembly CompileCode(string code, CompilerOptions options)
		{
			if (_disposed)
			{
				throw new ObjectDisposedException(GetType().Name);
			}

			if (code == null)
			{
				throw new ArgumentNullException(nameof(code));
			}

			if (options == null)
			{
				throw new ArgumentNullException(nameof(options));
			}

			if (code.Length == 0)
			{
				throw new ArgumentOutOfRangeException(nameof(code), "Insufficient data.");
			}

			_compilerParams.OutputAssembly = options.OutputPath;
			_compilerParams.CompilerOptions = RequiredCompilerOptions;

			CompilerResults results = _codeProvider.CompileAssemblyFromSource(_compilerParams, code);

			LogResults(results);

			// Display a successful compilation message.
			Debug.WriteLine("Code built into memory assembly '{0}' successfully.", results.CompiledAssembly.ToString());
			return results.CompiledAssembly;
		}

		private static void LogResults(CompilerResults results)
		{
			results.Output.Cast<string>().ToList().ForEach(s => Debug.WriteLine(s));

			if (results.Errors.Count > 0)
			{
				// Display compilation errors.
				var errorMsg = new StringBuilder();
				errorMsg.AppendLine("Errors building assembly...");
				foreach (CompilerError error in results.Errors.Cast<CompilerError>().Where(ce => !ce.IsWarning))
				{
					errorMsg.AppendFormat("  - {0}", error);
					errorMsg.AppendLine();
				}

				Debug.Write(errorMsg.ToString());
				throw new Exception(errorMsg.ToString());
			}
		}
	}
}