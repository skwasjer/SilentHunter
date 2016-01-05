using System;
using System.IO;
using System.Reflection;
using skwas.IO;
using SilentHunter.Dat;

namespace SilentHunter.Formats
{
	static class ControllerReader
	{
		private const string DateFormat = "yyyyMMdd";

		/// <summary>
		/// Asserts that the stream is at the expected position. If not, throws an error that parsing failed for <paramref name="fieldName"/>.
		/// </summary>
		/// <param name="stream">The stream.</param>
		/// <param name="expectedPosition">The position the stream is expected to be at.</param>
		/// <param name="fieldName">The field that was being parsed.</param>
		/// <exception cref="IOException">Thrown when the stream is not at the expected position.</exception>
		public static void EnsureStreamPosition(this Stream stream, long expectedPosition, string fieldName)
		{
			if (stream.Position == expectedPosition) return;

			if (stream.Position > expectedPosition)
				throw new IOException($"Too much data is read from stream for property '{fieldName}'.");
			
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
				throw new ArgumentNullException(nameof(member));
			if (string.IsNullOrEmpty(name))
				throw new ArgumentNullException(nameof(name));

			// If a name was passed, a name is expected on the stream. Read the name and validate it.
			var nameOnStream = reader.ReadNullTerminatedString();
			if (string.Compare(name, nameOnStream, StringComparison.OrdinalIgnoreCase) == 0) return false;	// Matches, so don't skip.

			// If memberInfo is type, then a controller name was expected.
			if (member is Type)
				throw new ArgumentException($"Expected controller name '{name}', but read '{nameOnStream}' from the stream.", nameof(name));

			if (!member.HasAttribute<OptionalAttribute>())
				throw new ArgumentException($"The reflected name '{name}' does not match the name '{nameOnStream}' read from the stream.", nameof(name));

			// Property must be skipped.
			return true;
		}

		public static bool ReadString(this BinaryReader reader, ICustomAttributeProvider member, long expectedEndPosition, out string result)
		{
			var fixedLength = member.GetAttribute<FixedStringAttribute>()?.Length ?? -1;
			if (reader.BaseStream.Position == expectedEndPosition || reader.BaseStream.Position == reader.BaseStream.Length)
			{
				result = null;
				if (fixedLength == -1) return true;
			}

			if (fixedLength > 0)
			{
				var s = reader.ReadString(fixedLength);
				// Take care of possible nullchar in middle of string.
				var arr = s.Split(new[] {'\0'}, 2, StringSplitOptions.None);

				result = arr.Length == 0 ? null : arr[0];
			}
			else
			{
				result = reader.ReadNullTerminatedString();

				// If there are bytes left for this field, move to last byte...
				if (reader.BaseStream.Position < expectedEndPosition)
					reader.BaseStream.Position = expectedEndPosition;
			}
			return result != null && (expectedEndPosition == -1 || reader.BaseStream.Position == expectedEndPosition);
		}

		public static bool ReadBoolean(this BinaryReader reader, ICustomAttributeProvider member, long expectedEndPosition, out bool result)
		{
			var boolLen = (expectedEndPosition == -1 ? reader.BaseStream.Length : expectedEndPosition) - reader.BaseStream.Position;
			switch (boolLen)
			{
				case 1:
					result = reader.ReadByte() > 0;
					return true;
				case 4:
					// Some files have int32 stored as boolean. This seems like a bug and we should resave the file properly.
					result = reader.ReadInt32() > 0;
					return true;
				default:
					result = false;
					return false;
			}
		}

		public static bool ReadDateTime(this BinaryReader reader, ICustomAttributeProvider member, long expectedEndPosition, out DateTime date)
		{
			date = DateTime.MinValue;

			var sDate = reader.ReadInt32().ToString();
			var success = expectedEndPosition == -1 || reader.BaseStream.Position == expectedEndPosition;
			if (!success) return false;

			if (DateTime.TryParseExact(sDate, DateFormat, null, System.Globalization.DateTimeStyles.None, out date)) return true;

			// If a parse error occurred, this most like is because the days of month are 31, for instance for april, or 30-31 for feb, or even 29 if not a leap year. Correct this by attempting to parse by lowering the days.
			var days = int.Parse(sDate.Substring(6, 2));
			sDate = sDate.Remove(6);

			// 3 extra attempts max.
			for (var i = 0; i < 3; i++)
			{
				days--;
				if (DateTime.TryParseExact(sDate + days, DateFormat, null, System.Globalization.DateTimeStyles.None, out date))
				{
					// We are successful.
					return true;
				}
			}
			return false;
		}

		public static object ReadValue(this BinaryReader reader, Type type)
		{
			
			return null;
		}
	}
}
