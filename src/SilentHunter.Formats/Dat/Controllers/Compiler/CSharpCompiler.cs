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
		/// Gets or sets the path of the output assembly.
		/// </summary>
		public string OutputPath
		{
			get => _compilerParams.OutputAssembly;
			set => _compilerParams.OutputAssembly = value;
		}

		/// <summary>
		/// Gets or sets the path of the XML documentation file.
		/// </summary>
		public string DocFile { get; set; }

		/// <summary>
		/// Gets or sets the assembly dependencies.
		/// </summary>
		public ICollection<string> ReferencedAssemblies { get; set; }

		/// <summary>
		/// Compiles the specified <paramref name="fileNames" /> and <see name="DocFile" /> into an <see cref="Assembly" />.
		/// </summary>
		/// <param name="fileNames"></param>
		/// <param name="loadAssembly"></param>
		/// <returns></returns>
		public Assembly CompileCode(ICollection<string> fileNames, bool loadAssembly = false)
		{
			if (_disposed)
			{
				throw new ObjectDisposedException(GetType().Name);
			}

			if (fileNames == null)
			{
				throw new ArgumentNullException(nameof(fileNames));
			}

			if (fileNames.Count == 0)
			{
				throw new ArgumentOutOfRangeException(nameof(fileNames), "Expected at least one filename.");
			}

			_compilerParams.ReferencedAssemblies.Clear();
			if (ReferencedAssemblies != null)
			{
				_compilerParams.ReferencedAssemblies.AddRange(new HashSet<string>(ReferencedAssemblies).ToArray());
			}

			_compilerParams.CompilerOptions = RequiredCompilerOptions;
			if (!string.IsNullOrEmpty(DocFile))
			{
				_compilerParams.CompilerOptions += $" /doc:\"{DocFile}\"";
			}

			CompilerResults results = _codeProvider.CompileAssemblyFromFile(_compilerParams, fileNames.ToArray());

			LogResults(results);

			// Display a successful compilation message.
			Debug.WriteLine("Code built into assembly '{0}' successfully.", OutputPath);
			return loadAssembly ? results.CompiledAssembly : null;
		}

		/// <summary>
		/// Compiles code into an <see cref="Assembly" />.
		/// </summary>
		/// <param name="code"></param>
		/// <returns></returns>
		public Assembly CompileCode(string code)
		{
			if (_disposed)
			{
				throw new ObjectDisposedException(GetType().Name);
			}

			if (code == null)
			{
				throw new ArgumentNullException(nameof(code));
			}

			if (code.Length == 0)
			{
				throw new ArgumentOutOfRangeException(nameof(code), "Insufficient data.");
			}

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