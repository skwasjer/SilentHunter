using System;

namespace SilentHunter.Controllers.Decoration
{
	[AttributeUsage(AttributeTargets.Field)]
	public sealed class OptionalAttribute : Attribute
	{
	}
}