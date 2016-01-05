using System;
using System.Diagnostics;
using System.Reflection;
using Microsoft.CSharp;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// http://spellcoder.com/blogs/bashmohandes/archive/2006/05/17/72.aspx
// http://damon.agilefactor.com/2006/02/appdomain-isolation-for-plugin.html

namespace SilentHunter.Dat
{
	public sealed class ControllerCompiler
		: IDisposable
	{
		private const string RequiredCompilerOptions = "/target:library";
		private CSharpCodeProvider _codeProvider;
		private readonly CompilerParameters _compilerParams;
		private bool _disposed;

		public ControllerCompiler(string outputPath, IEnumerable<ControllerFileReference> dependencies)
		{
			_codeProvider = (CSharpCodeProvider)CodeDomProvider.CreateProvider("CSharp");
			_compilerParams = new CompilerParameters
			{
				GenerateExecutable = false,
				GenerateInMemory = false,
				// When using GenerateInMemory=true, do not name the output assembly, just provide the path.
				// Otherwise, an empty stub is created and placed in the program folder.
				OutputAssembly = outputPath,
#if DEBUG
				IncludeDebugInformation = true,
#endif
				TreatWarningsAsErrors = false
			};

			_compilerParams.ReferencedAssemblies.AddRange(dependencies.Select(dep => dep.Name).ToArray());
			
			// Add DirectX reference.
			//var refAssembly = typeof(Vector3).Assembly.Location;
			//_compilerParams.ReferencedAssemblies.Add(refAssembly);
		}

		~ControllerCompiler()
		{
			Dispose(false);
		}

		private void Dispose(bool disposing)
		{
			if (_disposed) return;

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
		/// Compiles the specified controller <paramref name="filenames"/> and <paramref name="docFile"/> into an <see cref="Assembly"/>.
		/// </summary>
		/// <param name="filenames"></param>
		/// <param name="docFile"></param>
		/// <param name="loadAssembly"></param>
		/// <returns></returns>
		public Assembly CompileCode(ICollection<string> filenames, string docFile, bool loadAssembly = false)
		{
			if (_disposed)
				throw new ObjectDisposedException(GetType().Name);

			if (filenames == null)
				throw new ArgumentNullException(nameof(filenames));
			if (filenames.Count == 0)
				throw new ArgumentOutOfRangeException(nameof(filenames), "Expected at least one filename.");

			_compilerParams.CompilerOptions = RequiredCompilerOptions;
			if (!string.IsNullOrEmpty(docFile))
				_compilerParams.CompilerOptions += string.Format(" /doc:\"{0}\"", docFile);

			var results = _codeProvider.CompileAssemblyFromFile(_compilerParams, filenames.ToArray());

			LogResults(results);

			// Display a successful compilation message.
			Debug.WriteLine("Code built into assembly '{0}' successfully.", new object[] { _compilerParams.OutputAssembly });
			return loadAssembly ? results.CompiledAssembly : null;
		}		

		/// <summary>
		/// Compiles controller code into an <see cref="Assembly"/>.
		/// </summary>
		/// <param name="code"></param>
		/// <returns></returns>
		public Assembly CompileCode(string code)
		{
			if (_disposed)
				throw new ObjectDisposedException(GetType().Name);

			if (code == null)
				throw new ArgumentNullException(nameof(code));
			if (code.Length == 0)
				throw new ArgumentOutOfRangeException(nameof(code), "Insufficient data.");

			_compilerParams.CompilerOptions = RequiredCompilerOptions;

			var results = _codeProvider.CompileAssemblyFromSource(_compilerParams, code);

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
				foreach (var error in results.Errors.Cast<CompilerError>().Where(ce => !ce.IsWarning))
					errorMsg.AppendFormat("  - {0}\r\n", error);

				Debug.Write(errorMsg.ToString());
				throw new Exception(errorMsg.ToString());
			}
		}
	}
}
