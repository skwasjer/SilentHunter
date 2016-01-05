using System;

// ReSharper disable once CheckNamespace
namespace SilentHunter
{
	/// <summary>
	/// Describes the filename that contains a string list, to be used for autocompletion. Only supported for <see cref="string"/> fields.
	/// </summary>
	/// <remarks>The filename (and path) should be relative to the application executable path.</remarks>
	[AttributeUsage(AttributeTargets.Field)]
	public class AutoCompleteListAttribute
		: Attribute
	{
		/// <summary>
		/// Initializes a new instance of <see cref="AutoCompleteListAttribute"/> using specified <paramref name="filename"/>.
		/// </summary>
		/// <param name="filename">The filename that contains a string list, to be used for autocompletion.</param>
		public AutoCompleteListAttribute(string filename)
		{
			if (string.IsNullOrWhiteSpace(filename))
				throw new ArgumentNullException(nameof(filename));

			Filename = filename;
		}

		/// <summary>
		/// Gets the filename that contains a string list, to be used for autocompletion.
		/// </summary>
		public string Filename { get; }
	}
}
