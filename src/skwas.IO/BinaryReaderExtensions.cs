using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

namespace skwas.IO
{
	/// <summary>
	/// Extensions for <see cref="BinaryReader" />.
	/// </summary>
	public static class BinaryReaderExtensions
	{
		private static readonly Type ColorType = typeof(Color);

		/// <summary>
		/// Reads a structure/class of <typeparamref name="T" /> from the current stream and advances the current position of the stream by the size of the structure in bytes.
		/// </summary>
		/// <typeparam name="T">The type to read.</typeparam>
		/// <param name="reader">The binary reader.</param>
		/// <returns>An object of specified type, read from the stream.</returns>
		[SecuritySafeCritical]
		public static T ReadStruct<T>(this BinaryReader reader)
		{
			return (T)ReadStruct(reader, typeof(T));
		}

		/// <summary>
		/// Reads a structure/class of <paramref name="type" /> from the current stream and advances the current position of the stream by the size of the structure in bytes.
		/// </summary>
		/// <param name="reader">The binary reader.</param>
		/// <param name="type">The type to read.</param>
		/// <returns>An object of specified type, read from the stream.</returns>
		[SecuritySafeCritical]
		public static object ReadStruct(this BinaryReader reader, Type type)
		{
			// Take care of nullable types. Although we can't read null values from a stream, the caller expects the underlying type.
			Type targetType = Nullable.GetUnderlyingType(type) ?? type;

			// Booleans are deserialized as byte 0 = false, otherwise = true.
			if (targetType == typeof(bool))
			{
				return reader.ReadByte() > 0;
			}

			if (targetType == ColorType)
				// The color struct cannot be deserialized using interop.
			{
				return Color.FromArgb(reader.ReadByte(), reader.ReadByte(), reader.ReadByte(), reader.ReadByte());
			}

			// If the type is enum, we have to deserialize using underlying type.
			Type enumType = null;
			if (targetType.IsEnum)
			{
				enumType = targetType;
				targetType = Enum.GetUnderlyingType(targetType);
			}

			// Determine size of requested structure and read the struct.
			int structSize = Marshal.SizeOf(targetType);
			object value = ReadStruct(reader, targetType, structSize);

			// If enum, cast back to the enum type.
			if (enumType != null)
			{
				value = Enum.ToObject(enumType, value);
			}

			return value;
		}

		/// <summary>
		/// Reads a structure/class of <typeparamref name="T" /> from the current stream and advances the current position of the stream by the size of the structure in bytes.
		/// </summary>
		/// <typeparam name="T">The type to read.</typeparam>
		/// <param name="reader">The binary reader.</param>
		/// <param name="structSize">The size of the structure.</param>
		/// <returns>An object of specified type, read from the stream.</returns>
		[SecuritySafeCritical]
		public static object ReadStruct<T>(this BinaryReader reader, int structSize)
		{
			return ReadStruct(reader, typeof(T), structSize);
		}

		/// <summary>
		/// Reads a structure/class of specified <paramref name="type" /> from the current stream and advances the current position of the stream by the size of the structure in bytes.
		/// </summary>
		/// <param name="reader">The binary reader.</param>
		/// <param name="type">The type to read.</param>
		/// <param name="structSize">The size of the structure.</param>
		/// <returns>An object of specified type, read from the stream.</returns>
		/// <exception cref="EndOfStreamException">Thrown when the requested <paramref name="structSize" /> exceeds the remaining available data.</exception>
		[SecuritySafeCritical]
		public static object ReadStruct(this BinaryReader reader, Type type, int structSize)
		{
			// Read bytes from stream.
			byte[] structData = reader.ReadBytes(structSize);

			// Check if data read matches requested.
			if (structData.Length != structSize)
			{
				throw new EndOfStreamException("Unable to read beyond the end of the stream.");
			}

			// Copy byte array to unmanaged memory block.
			object value;
			IntPtr hBuffer = IntPtr.Zero;
			try
			{
				hBuffer = Marshal.AllocHGlobal(structSize);
				Marshal.Copy(structData, 0, hBuffer, structSize);
				// Marshal the unmanaged memory pointer back to a typed object.
				value = Marshal.PtrToStructure(hBuffer, type);
			}
			finally
			{
				// Free memory.
				if (hBuffer != IntPtr.Zero)
				{
					Marshal.FreeHGlobal(hBuffer);
				}
			}

			return value;
		}

		/// <summary>
		/// Reads a string from the current stream, using specified number of characters.
		/// </summary>
		/// <param name="reader">The binary reader.</param>
		/// <param name="characters">The number of characters to read.</param>
		/// <returns>The string.</returns>
		[SecuritySafeCritical]
		public static string ReadString(this BinaryReader reader, int characters)
		{
			var buffer = new StringBuilder();
			for (int i = 0; i < characters; i++)
			{
				char c = reader.ReadChar();
				buffer.Append(c);
			}

			return buffer.ToString();
		}

		/// <summary>
		/// Reads a string from the current stream. The stream is read until one of the characters is found. The terminating character is not included in the returned string.
		/// </summary>
		/// <param name="reader">The binary reader.</param>
		/// <param name="terminatingCharacters">The terminating characters.</param>
		/// <returns>The string.</returns>
		/// <remarks>Note that the behavior is different from ReadString which takes a string for a terminator.</remarks>
		[SecuritySafeCritical]
		public static string ReadString(this BinaryReader reader, char[] terminatingCharacters)
		{
			var tc = (IList<char>)terminatingCharacters;
			var buffer = new StringBuilder();
			while (true)
			{
				char c = reader.ReadChar();
				if (tc.Contains(c))
				{
					break;
				}

				buffer.Append(c);
			}

			return buffer.ToString();
		}

		/// <summary>
		/// Reads a null terminated string from the current stream. The stream is read until the null character is found. The terminating character is not included in the returned string.
		/// </summary>
		[SecuritySafeCritical]
		public static string ReadNullTerminatedString(this BinaryReader reader)
		{
			return ReadString(reader, new[] { '\0' });
		}

		/// <summary>
		/// Reads a string from the current stream. The stream is read until the specified character is found. The terminating character is not included in the returned string.
		/// </summary>
		/// <param name="reader">The binary reader.</param>
		/// <param name="terminatingCharacter">The terminating character.</param>
		/// <returns>The string.</returns>
		[SecuritySafeCritical]
		public static string ReadString(this BinaryReader reader, char terminatingCharacter)
		{
			return ReadString(reader, new[] { terminatingCharacter });
		}

		/// <summary>
		/// Reads a string from the current stream. The stream is read until the terminating string is found. The terminating string is not included in the returned string.
		/// </summary>
		/// <param name="reader">The binary reader.</param>
		/// <param name="terminatingString">The terminating string.</param>
		/// <returns>The string.</returns>
		/// <remarks>Note that the behavior is different from ReadString which takes a character array for a terminator.</remarks>
		[SecuritySafeCritical]
		public static string ReadString(this BinaryReader reader, string terminatingString)
		{
			if (terminatingString == null)
			{
				throw new ArgumentNullException(nameof(terminatingString));
			}

			if (terminatingString == string.Empty)
			{
				throw new ArgumentException("Specify the terminating string.", nameof(terminatingString));
			}

			char[] tc = terminatingString.ToCharArray();
			int pos = 0;
			var buffer = new StringBuilder();
			char c = reader.ReadChar();
			while (true)
			{
				while (tc[pos] == c)
				{
					if (++pos == tc.Length)
					{
						return buffer.ToString();
					}

					// Read next.
					c = reader.ReadChar();
				}

				if (pos > 0)
				{
					buffer.Append(terminatingString.Substring(0, pos));
					pos = 0;
				}
				else
				{
					buffer.Append(c);
					// Read next.
					c = reader.ReadChar();
				}
			}
		}
	}
}