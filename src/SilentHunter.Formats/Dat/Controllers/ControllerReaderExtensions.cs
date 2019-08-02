using System;
using System.IO;
using System.Reflection;
using SilentHunter.Controllers.Decoration;
using SilentHunter.Extensions;
using skwas.IO;

namespace SilentHunter.Dat.Controllers
{
	internal static class ControllerReaderExtensions
	{
		private const string DateFormat = "yyyyMMdd";

		/// <summary>
		/// Asserts that the stream is at the expected position. If not, throws an error that parsing failed for <paramref name="fieldName" />.
		/// </summary>
		/// <param name="stream">The stream.</param>
		/// <param name="expectedPosition">The position the stream is expected to be at.</param>
		/// <param name="fieldName">The field that was being parsed.</param>
		/// <exception cref="IOException">Thrown when the stream is not at the expected position.</exception>
		public static void EnsureStreamPosition(this Stream stream, long expectedPosition, string fieldName)
		{
			if (stream.Position == expectedPosition)
			{
				return;
			}

			if (stream.Position > expectedPosition)
			{
				throw new IOException($"Too much data is read from stream for property '{fieldName}'.");
			}

			// Forward stream to end.
			stream.Position = expectedPosition;
			throw new IOException($"Not all data is read from stream for property '{fieldName}'.");
		}

		/// <summary>
		/// Checks if the provided name matches the next name on the stream. Returns true if the entire member should be skipped.
		/// </summary>
		/// <param name="reader">The reader to read the name from.</param>
		/// <param name="member">The member info.</param>
		/// <param name="name">The expected name to find on the stream.</param>
		/// <returns>Returns true if the property must be ignored, because it was found to be an optional field. Returns false if the name read from stream is read succesfully and matches the provided name..</returns>
		public static bool SkipMember(this BinaryReader reader, ICustomAttributeProvider member, string name)
		{
			if (member == null)
			{
				throw new ArgumentNullException(nameof(member));
			}

			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentNullException(nameof(name));
			}

			// If a name was passed, a name is expected on the stream. Read the name and validate it.
			string nameOnStream = reader.ReadNullTerminatedString();
			if (string.Compare(name, nameOnStream, StringComparison.OrdinalIgnoreCase) == 0)
			{
				return false; // Matches, so don't skip.
			}

			// If memberInfo is type, then a controller name was expected.
			if (member is Type)
			{
				throw new ArgumentException($"Expected controller name '{name}', but read '{nameOnStream}' from the stream.", nameof(name));
			}

			if (!member.HasAttribute<OptionalAttribute>())
			{
				throw new ArgumentException($"The reflected name '{name}' does not match the name '{nameOnStream}' read from the stream.", nameof(name));
			}

			// Property must be skipped.
			return true;
		}
	}
}