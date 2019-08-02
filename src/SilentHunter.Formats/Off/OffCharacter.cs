using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using skwas.IO;

namespace SilentHunter.Off
{
	[DebuggerDisplay("{Character}, {Rectangle}")]
	public class OffCharacter : IRawSerializable
	{
		public char Character { get; set; }
		public Rectangle Rectangle { get; set; }

		protected virtual Task DeserializeAsync(Stream stream)
		{
			using (var reader = new BinaryReader(stream, FileEncoding.Default, true))
			{
				Character = reader.ReadChar();
				Rectangle = reader.ReadStruct<Rectangle>();
			}

			return Task.CompletedTask;
		}

		protected virtual Task SerializeAsync(Stream stream)
		{
			using (var writer = new BinaryWriter(stream, FileEncoding.Default, true))
			{
				writer.Write(Character);
				writer.WriteStruct(Rectangle);
			}

			return Task.CompletedTask;
		}

		protected bool Equals(OffCharacter other)
		{
			return Character == other.Character && Rectangle.Equals(other.Rectangle);
		}

		/// <summary>
		/// Determines whether the specified object is equal to the current object.
		/// </summary>
		/// <returns>
		/// true if the specified object  is equal to the current object; otherwise, false.
		/// </returns>
		/// <param name="obj">The object to compare with the current object. </param>
		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj))
			{
				return false;
			}

			if (ReferenceEquals(this, obj))
			{
				return true;
			}

			if (obj.GetType() != GetType())
			{
				return false;
			}

			return Equals((OffCharacter)obj);
		}

		/// <summary>
		/// Serves as the default hash function.
		/// </summary>
		/// <returns>
		/// A hash code for the current object.
		/// </returns>
		public override int GetHashCode()
		{
			return 0;
		}

		public static bool operator ==(OffCharacter left, OffCharacter right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(OffCharacter left, OffCharacter right)
		{
			return !Equals(left, right);
		}

		/// <summary>
		/// When implemented, deserializes the implemented class from specified <paramref name="stream" />.
		/// </summary>
		/// <param name="stream">The stream.</param>
		Task IRawSerializable.DeserializeAsync(Stream stream)
		{
			return DeserializeAsync(stream);
		}

		/// <summary>
		/// When implemented, serializes the implemented class to specified <paramref name="stream" />.
		/// </summary>
		/// <param name="stream">The stream.</param>
		Task IRawSerializable.SerializeAsync(Stream stream)
		{
			return SerializeAsync(stream);
		}
	}
}