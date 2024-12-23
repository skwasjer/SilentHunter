using System;

namespace SilentHunter.Controllers.Decoration;

/// <summary>
/// Used to mark field optional. While value types can be made nullable easily, for reference types there is no way to determine if the field is optional. Thus, this attribute should be used whenever a field is optional, regardless of whether they are value/reference types.
/// </summary>
[AttributeUsage(AttributeTargets.Field)]
public sealed class OptionalAttribute : Attribute
{
}