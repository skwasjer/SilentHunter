using System;

namespace SilentHunter.Dat
{
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
	public sealed class OptionalAttribute
		: Attribute
	{
	}
}
