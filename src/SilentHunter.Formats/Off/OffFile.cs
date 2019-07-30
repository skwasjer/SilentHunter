using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using skwas.IO;

namespace SilentHunter.Off
{
	public sealed class OffFile : KeyedCollection<char, OffCharacter>, ISilentHunterFile
	{
		public OffFile()
			: base(EqualityComparer<char>.Default, -1)
		{
		}

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public Point CharacterSpacing { get; set; }

		#region Implementation of ISilentHunterFile

		/// <summary>
		/// Loads the file from specified stream.
		/// </summary>
		/// <param name="stream">The stream to load from.</param>
		public void Load(Stream stream)
		{
			using (var reader = new BinaryReader(stream, Encoding.ParseEncoding, true))
			{
				int characterCount = reader.ReadInt32();
				CharacterSpacing = reader.ReadStruct<Point>();

				Clear();
				for (var i = 0; i < characterCount; i++)
				{
					var offChar = new OffCharacter();
					((IRawSerializable)offChar).Deserialize(stream);

					Add(offChar);
				}

				if (reader.BaseStream.Position != reader.BaseStream.Length)
				{
					throw new IOException($"The stream contains unexpected data at 0x{reader.BaseStream.Position:x8}");
				}
			}
		}

		/// <summary>
		/// Saves the file to specified stream.
		/// </summary>
		/// <param name="stream">The stream to write to.</param>
		public void Save(Stream stream)
		{
			using (var writer = new BinaryWriter(stream, Encoding.ParseEncoding, true))
			{
				writer.Write(Count);
				writer.WriteStruct(CharacterSpacing);

				foreach (OffCharacter c in this)
				{
					((IRawSerializable)c).Serialize(stream);
				}

				writer.Flush();
			}
		}

		#endregion

		#region Implementation of IRawSerializable

		/// <summary>
		/// When implemented, deserializes the implemented class from specified <paramref name="stream" />.
		/// </summary>
		/// <param name="stream">The stream.</param>
		void IRawSerializable.Deserialize(Stream stream)
		{
			Load(stream);
		}

		/// <summary>
		/// When implemented, serializes the implemented class to specified <paramref name="stream" />.
		/// </summary>
		/// <param name="stream">The stream.</param>
		void IRawSerializable.Serialize(Stream stream)
		{
			Save(stream);
		}

		#endregion

		#region Overrides of KeyedCollection<char,OffCharacter>

		/// <summary>
		/// When implemented in a derived class, extracts the key from the specified element.
		/// </summary>
		/// <returns>
		/// The key for the specified element.
		/// </returns>
		/// <param name="item">The element from which to extract the key.</param>
		protected override char GetKeyForItem(OffCharacter item)
		{
			return item.Character;
		}

		#endregion
	}
}