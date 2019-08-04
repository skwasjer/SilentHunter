using System.Collections.Generic;

namespace SilentHunter.Controllers.Compiler
{
	public class CompilerOptions
	{
		/// <summary>
		/// Gets or sets the path of the output assembly.
		/// </summary>
		public string OutputFile { get; set; }

		/// <summary>
		/// Gets or sets the path of the XML documentation file.
		/// </summary>
		public string DocFile { get; set; }

		/// <summary>
		/// Gets or sets the assembly dependencies.
		/// </summary>
		public ICollection<string> ReferencedAssemblies { get; set; }
	}
}