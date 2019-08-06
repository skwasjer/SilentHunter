using System.Collections.Generic;
using System.Reflection;

namespace SilentHunter.Controllers.Compiler
{
	/// <summary>
	/// Describes a wrapper CSharp code compiler for use by the controller compiler.
	/// </summary>
	public interface ICSharpCompiler
	{
		/// <summary>
		/// Compiles the specified <paramref name="fileNames" /> and <see name="DocFile" /> into an <see cref="Assembly" />.
		/// </summary>
		/// <param name="fileNames">The (controller) source file names.</param>
		/// <param name="options">The compiler options.</param>
		/// <param name="loadAssembly"><see langword="true"/> to immediately load the assembly or <see langword="false"/> otherwise</param>
		/// <returns></returns>
		Assembly CompileCode(ICollection<string> fileNames, CompilerOptions options, bool loadAssembly = false);
	}
}