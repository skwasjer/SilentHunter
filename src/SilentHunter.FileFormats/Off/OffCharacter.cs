using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using SilentHunter.FileFormats.Extensions;
using SilentHunter.FileFormats.IO;

namespace SilentHunter.FileFormats.Off
{
	/// <summary>
	/// Represents a character definition for <see cref="OffFile"/>s.
	/// </summary>
	[DebuggerDisplay("{Character}, {BoundingBox}")]
	public sealed class OffCharacter : IRawSerializable, ICloneable, IEquatable<OffCharacter>
	{
		/// <summary>
		/// Gets or sets the character.
		/// </summary>
		public char Character { get; set; }

		/// <summary>
		/// Gets or sets the bounding box in a 2D font image file.
		/// </summary>
		public Rectangle BoundingBox { get; set; }

		Task IRawSerializable.DeserializeAsync(Stream stream)
		{
			using (var reader = new BinaryReader(stream, FileEncoding.Default, true))
			{
				Character = reader.ReadChar();
				BoundingBox = reader.ReadStruct<Rectangle>();
			}

			return Task.CompletedTask;
		}

		Task IRawSerializable.SerializeAsync(Stream stream)
		{
			using (var writer = new BinaryWriter(stream, FileEncoding.Default, true))
			{
				writer.Write(Character);
				writer.WriteStruct(BoundingBox);
			}

			return Task.CompletedTask;
		}

		/// <inheritdoc />
		public object Clone()
		{
			return new OffCharacter
			{
				Character = Character,
				BoundingBox = BoundingBox
			};
		}

		/// <summary>
		/// </summary>
		public bool Equals(OffCharacter other)
		{
			if (ReferenceEquals(null, other))
			{
				return false;
			}

			if (ReferenceEquals(this, other))
			{
				return true;
			}

			return Character == other.Character && BoundingBox.Equals(other.BoundingBox);
		}

		/// <inheritdoc />
		public override bool Equals(object obj)
		{
			return ReferenceEquals(this, obj) || obj is OffCharacter other && Equals(other);
		}

		/// <inheritdoc />
		public override int GetHashCode()
		{
			unchecked
			{
				return (Character.GetHashCode() * 397) ^ BoundingBox.GetHashCode();
			}
		}

		/// <summary>
		/// </summary>
		public static bool operator ==(OffCharacter left, OffCharacter right)
		{
			return Equals(left, right);
		}

		/// <summary>
		/// </summary>
		public static bool operator !=(OffCharacter left, OffCharacter right)
		{
			return !Equals(left, right);
		}
	}
}