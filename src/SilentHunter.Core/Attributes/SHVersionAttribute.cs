using System;

namespace SilentHunter.Dat
{
	public enum SHVersions
	{
		/// <summary>
		/// This class or field is specific to SH3.
		/// </summary>
		SH3,
		/// <summary>
		/// This class or field is specific to SH4.
		/// </summary>
		SH4,
		/// <summary>
		/// This class or field is specific to SH5.
		/// </summary>
		SH5
	}

	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Class, AllowMultiple = true)]
	public sealed class SHVersionAttribute : Attribute
	{
		/// <summary>
		/// Declare the class or field specificly to a SH game version.
		/// </summary>
		/// <param name="version">The Silent Hunter version.</param>
		public SHVersionAttribute(SHVersions version)
		{
			if (!Enum.IsDefined(typeof(SHVersions), version))
				throw new ArgumentException("Specified value is not a member of the SHVersions enum.", "version");
			Version = version;
		}

		/// <summary>
		/// The Silent Hunter version the attribute applies to.
		/// </summary>
		public SHVersions Version { get; }
	}
}
