using System;

namespace SilentHunter.Controllers.Compiler
{
	/// <summary>
	/// Thrown when a compilation error occurs.
	/// </summary>
	public class CompileException : Exception
	{
		public CompileException(string message) : base(message)
		{
		}
	}
}