using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using SilentHunter.FileFormats.Extensions;
using SilentHunter.FileFormats.IO;

namespace SilentHunter.FileFormats.Off;

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

    /// <summary>
    /// Loads from specified <paramref name="stream" />.
    /// </summary>
    /// <param name="stream">The stream to load from.</param>
    /// <exception cref="OffFileException">Thrown when a parsing error occurs.</exception>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="stream" /> is null.</exception>
    public async Task LoadAsync(Stream stream)
    {
        if (stream == null)
        {
            throw new ArgumentNullException(nameof(stream));
        }

        using var reader = new BinaryReader(stream, FileEncoding.Default, true);
        long itemStartPosition = 0;
        try
        {
            int characterCount = reader.ReadInt32();
            CharacterSpacing = reader.ReadStruct<Point>();

            Clear();
            for (int i = 0; i < characterCount; i++)
            {
                itemStartPosition = stream.Position;
                var character = new OffCharacter();
                await ((IRawSerializable)character).DeserializeAsync(stream).ConfigureAwait(false);

                Add(character);
            }

            if (reader.BaseStream.Position != reader.BaseStream.Length)
            {
                throw new SilentHunterParserException($"The stream contains unexpected data at 0x{reader.BaseStream.Position:x8}");
            }
        }
        catch (Exception ex)
        {
            throw new OffFileException(Count, itemStartPosition, "The file could not be read due to a parser error.", ex);
        }
    }

    /// <summary>
    /// Saves to specified <paramref name="stream" />.
    /// </summary>
    /// <param name="stream">The stream to write to.</param>
    /// <exception cref="OffFileException">Thrown when a parsing error occurs.</exception>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="stream" /> is null.</exception>
    public async Task SaveAsync(Stream stream)
    {
        if (stream == null)
        {
            throw new ArgumentNullException(nameof(stream));
        }

        using var writer = new BinaryWriter(stream, FileEncoding.Default, true);
        long itemStartPosition = 0;
        int itemIndex = -1;
        try
        {
            writer.Write(Count);
            writer.WriteStruct(CharacterSpacing);

            foreach (OffCharacter character in this)
            {
                itemStartPosition = stream.Position;
                itemIndex++;
                await ((IRawSerializable)character).SerializeAsync(stream).ConfigureAwait(false);
            }

            writer.Flush();
        }
        catch (Exception ex)
        {
            throw new OffFileException(itemIndex, itemStartPosition, "The file could not be written due to a parser error.", ex);
        }
    }
}
