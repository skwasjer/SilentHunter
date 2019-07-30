using System.Collections.Generic;
using System.Reflection;

namespace SilentHunter.Dat.Controllers.Compiler
{
	public interface ICSharpCompiler
	{
		/// <summary>
		/// Gets or sets the path of the output assembly.
		/// </summary>
		string OutputPath { get; set; }

		/// <summary>
		/// Gets or sets the path of the XML documentation file.
		/// </summary>
		string DocFile { get; set; }

		/// <summary>
		/// Gets or sets the assembly dependencies.
		/// </summary>
		ICollection<string> ReferencedAssemblies { get; set; }

		/// <summary>
		/// Compiles the specified <paramref name="fileNames" /> and <see name="DocFile" /> into an <see cref="Assembly" />.
		/// </summary>
		/// <param name="fileNames"></param>
		/// <param name="loadAssembly"></param>
		/// <returns></returns>
		Assembly CompileCode(ICollection<string> fileNames, bool loadAssembly = false);

		/// <summary>
		/// Compiles code into an <see cref="Assembly" />.
		/// </summary>
		/// <param name="code"></param>
		/// <returns></returns>
		Assembly CompileCode(string code);
	}
}