using System;

namespace SilentHunter.Controllers.Decoration
{
	/// <summary>
	/// Indicates the name a field is to be (de)serialized with. Should be used to give a more meaningful name to a field or to fix typos.
	/// </summary>
	[AttributeUsage(AttributeTargets.Field)]
	public sealed class ParseNameAttribute : Attribute
	{
		/// <summary>
		/// Initializes a new instance of <see cref="ParseNameAttribute" /> using specified <paramref name="name" />.
		/// </summary>
		/// <param name="name">The name a field is to be (de)serialized with.</param>
		public ParseNameAttribute(string name)
		{
			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentNullException(nameof(name));
			}

			Name = name;
		}

		/// <summary>
		/// Gets the name a field is to be (de)serialized with.
		/// </summary>
		public string Name { get; }
	}
}