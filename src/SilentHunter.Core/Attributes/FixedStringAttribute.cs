using System;

// ReSharper disable once CheckNamespace
namespace SilentHunter
{
	/// <summary>
	/// Indicates that the field the attribute is applied to is a fixed length <see cref="string"/>.
	/// </summary>
	[AttributeUsage(AttributeTargets.Field)]
	public sealed class FixedStringAttribute
		: Attribute
	{
		/// <summary>
		/// Initializes a new instance of <see cref="AutoCompleteListAttribute"/> using specified <paramref name="length"/>.
		/// </summary>
		/// <param name="length">The fixed length of the string.</param>
		public FixedStringAttribute(int length)
		{
			if (length <= 0)
				throw new ArgumentOutOfRangeException(nameof(length));
			Length = length;
		}

		/// <summary>
		/// Gets the fixed length of the string.
		/// </summary>
		public int Length { get; }
	}
}
