using System;

namespace SilentHunter.Controllers.Decoration
{
	/// <summary>
	/// Indicates a class is a type that is serialized in a specific file format, similar to controllers. Each individual field is stored with a size specifier, the name of the field, and then the data.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class)]
	// ReSharper disable once InconsistentNaming
	public sealed class SHTypeAttribute : Attribute
	{
	}
}