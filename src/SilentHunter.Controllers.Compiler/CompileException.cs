using System;

namespace SilentHunter.Controllers.Compiler;

/// <summary>
/// Thrown when a compilation error occurs.
/// </summary>
public class CompileException : Exception
{
    internal CompileException(string message) : base(message)
    {
    }
}