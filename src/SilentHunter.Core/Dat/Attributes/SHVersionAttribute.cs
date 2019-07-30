using System;

namespace SilentHunter.Dat
{
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Class, AllowMultiple = true)]
	// ReSharper disable once InconsistentNaming
	public sealed class SHVersionAttribute : Attribute
	{
		/// <summary>
		/// Declare the class or field specifically to a SH game version.
		/// </summary>
		/// <param name="version">The Silent Hunter version.</param>
		public SHVersionAttribute(SHVersions version)
		{
			if (!Enum.IsDefined(typeof(SHVersions), version))
			{
				throw new ArgumentException("Specified value is not a member of the SHVersions enum.", nameof(version));
			}

			Version = version;
		}

		/// <summary>
		/// The Silent Hunter version the attribute applies to.
		/// </summary>
		public SHVersions Version { get; }
	}
}