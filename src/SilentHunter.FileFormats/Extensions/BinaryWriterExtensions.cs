using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

namespace SilentHunter.FileFormats.Extensions
{
	/// <summary>
	/// Extensions for <see cref="BinaryWriter" />.
	/// </summary>
	internal static class BinaryWriterExtensions
	{
		/// <summary>
		/// Writes a string to the underlying stream. The string is terminated using a '\0' character.
		/// </summary>
		/// <param name="writer"></param>
		/// <param name="value"></param>
		[SecuritySafeCritical]
		public static void WriteNullTerminatedString(this BinaryWriter writer, string value)
		{
			Write(writer, value, '\0');
		}

		/// <summary>
		/// Writes a fixed length string to the underlying stream. If the string is shorter that <paramref name="fixedLength" />, it is padded using <paramref name="padWith" />
		/// </summary>
		/// <param name="writer">The writer.</param>
		/// <param name="value">The string to write.</param>
		/// <param name="fixedLength">The fixed length of the string.</param>
		/// <param name="padWith">The character to pad the string with.</param>
		/// <exception cref="ArgumentException"></exception>
		[SecuritySafeCritical]
		public static void Write(this BinaryWriter writer, string value, int fixedLength, char padWith = '\0')
		{
			if (value == null)
			{
				throw new ArgumentNullException(nameof(value));
			}

			if (value.Length > fixedLength)
			{
				throw new ArgumentOutOfRangeException(nameof(fixedLength), "The length of the value exceeds the fixed length.");
			}

			Write(writer, value.PadRight(fixedLength, padWith), false);
		}

		/// <summary>
		/// Writes a string to the underlying stream. If <paramref name="lengthPrefixed" /> is true, the behavior is identical to the Write method of BinaryWriter. When false, the length prefix is skipped and the string is directly written to stream using the <see cref="Encoding" /> specified for the writer.
		/// </summary>
		/// <param name="writer">The writer.</param>
		/// <param name="value">The string to write.</param>
		/// <param name="lengthPrefixed">True to write as length prefixed string, false to write the string without length prefix.</param>
		[SecuritySafeCritical]
		public static void Write(this BinaryWriter writer, string value, bool lengthPrefixed)
		{
			if (value == null)
			{
				throw new ArgumentNullException(nameof(value));
			}

			if (lengthPrefixed)
			{
				writer.Write(value);
			}
			else
			{
				foreach (char c in value)
				{
					writer.Write(c);
				}
			}
		}

		/// <summary>
		/// Writes a string to the underlying stream. The string is terminated using the specified <paramref name="terminatingCharacter" />
		/// </summary>
		/// <param name="writer">The writer.</param>
		/// <param name="value">The string to write.</param>
		/// <param name="terminatingCharacter">The terminating character.</param>
		[SecuritySafeCritical]
		public static void Write(this BinaryWriter writer, string value, char terminatingCharacter)
		{
			if (value == null)
			{
				throw new ArgumentNullException(nameof(value));
			}

			Write(writer, value, false);
			writer.Write(terminatingCharacter);
		}

		/// <summary>
		/// Writes a structure/class of specified type to the current stream and advances the current position of the stream by the size of the structure in bytes.
		/// </summary>
		/// <param name="writer"></param>
		/// <param name="value">The structure the write to the stream.</param>
		[SecuritySafeCritical]
		public static void WriteStruct(this BinaryWriter writer, object value)
		{
			if (value == null)
			{
				throw new ArgumentNullException(nameof(value));
			}

			Type type = value.GetType();
			if (type == typeof(bool))
			{
				writer.Write((bool)value ? (byte)1 : (byte)0);
				return;
			}

			if (type == typeof(byte[]))
			{
				writer.Write((byte[])value);
				return;
			}

			if (type.IsArray)
			{
				foreach (object item in (Array)value)
				{
					WriteStruct(writer, item);
				}

				return;
			}

			if (type == typeof(System.Drawing.Color))
			{
				var clr = (System.Drawing.Color)value;
				writer.Write(clr.A);
				writer.Write(clr.R);
				writer.Write(clr.G);
				writer.Write(clr.B);
				return;
			}

			if (type.IsEnum)
			{
				type = Enum.GetUnderlyingType(type);
				value = Convert.ChangeType(value, type);
			}

			// Get the size of the structure.
			int structSize = Marshal.SizeOf(type);

			byte[] structData;

			// Copy structure to un unmanaged memory block.
			IntPtr hStruct = IntPtr.Zero;
			try
			{
				hStruct = Marshal.AllocHGlobal(structSize);
				Marshal.StructureToPtr(value, hStruct, true);
				// Copy the data from the unmanaged memory back to a managed byte array.
				structData = new byte[structSize];
				Marshal.Copy(hStruct, structData, 0, structData.Length);
			}
			finally
			{
				// Free memory.
				if (hStruct != IntPtr.Zero)
				{
					Marshal.FreeHGlobal(hStruct);
				}
			}

			// Write byte array.
			writer.Write(structData, 0, structData.Length);
		}
	}
}