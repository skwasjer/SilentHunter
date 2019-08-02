using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using skwas.IO;

namespace SilentHunter.Off
{
	/// <summary>
	/// Represents an OFF file parser (font file).
	/// </summary>
	public sealed class OffFile : KeyedCollection<char, OffCharacter>, ISilentHunterFile
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="OffFile" /> class.
		/// </summary>
		public OffFile()
			: base(EqualityComparer<char>.Default, -1)
		{
		}

		/// <summary>
		/// Gets or sets the character spacing.
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public Point CharacterSpacing { get; set; }

		/// <inheritdoc />
		protected override char GetKeyForItem(OffCharacter item)
		{
			return item.Character;
		}

		/// <inheritdoc />
		public Task LoadAsync(Stream stream)
		{
			if (stream == null)
			{
				throw new ArgumentNullException(nameof(stream));
			}

			return GlobalExceptionHandler.HandleException(async () =>
				{
					using (var reader = new BinaryReader(stream, FileEncoding.Default, true))
					{
						int characterCount = reader.ReadInt32();
						CharacterSpacing = reader.ReadStruct<Point>();

						Clear();
						for (int i = 0; i < characterCount; i++)
						{
							var character = new OffCharacter();
							await ((IRawSerializable)character).DeserializeAsync(stream).ConfigureAwait(false);

							Add(character);
						}

						if (reader.BaseStream.Position != reader.BaseStream.Length)
						{
							throw new IOException($"The stream contains unexpected data at 0x{reader.BaseStream.Position:x8}");
						}
					}
				},
				"Failed to load file.");
		}

		/// <inheritdoc />
		public Task SaveAsync(Stream stream)
		{
			if (stream == null)
			{
				throw new ArgumentNullException(nameof(stream));
			}

			return GlobalExceptionHandler.HandleException(async () =>
				{
					using (var writer = new BinaryWriter(stream, FileEncoding.Default, true))
					{
						writer.Write(Count);
						writer.WriteStruct(CharacterSpacing);

						foreach (OffCharacter character in this)
						{
							await ((IRawSerializable)character).SerializeAsync(stream).ConfigureAwait(false);
						}

						writer.Flush();
					}
				},
				"Failed to save file.");
		}
	}
}