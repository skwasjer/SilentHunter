using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using SilentHunter.FileFormats.IO;

namespace SilentHunter.FileFormats.Sdl;

/// <summary>
/// Represents an SDL file parser.
/// </summary>
public sealed class SdlFile : KeyedCollection<string, SoundInfo>, ISilentHunterFile
{
    private const string S3DAssemblyPath = "Sdl.dll";

    /// <summary>
    /// Initializes a new instance of the <see cref="SdlFile" /> class.
    /// </summary>
    public SdlFile()
        : base(EqualityComparer<string>.Default, -1)
    {
    }

    /// <inheritdoc />
    protected override string GetKeyForItem(SoundInfo item)
    {
        return item.Name;
    }

    /// <summary>
    /// Loads from specified <paramref name="stream" />.
    /// </summary>
    /// <param name="stream">The stream to load from.</param>
    /// <exception cref="SdlFileException">Thrown when a parsing error occurs.</exception>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="stream" /> is null.</exception>
    public async Task LoadAsync(Stream stream)
    {
        if (stream == null)
        {
            throw new ArgumentNullException(nameof(stream));
        }

        if (!stream.CanRead)
        {
            throw new ArgumentException("The stream does not support reading.", nameof(stream));
        }

        Clear();

        long itemStartPosition = 0;
        try
        {
            while (stream.Position < stream.Length)
            {
                itemStartPosition = stream.Position;
                var sndInfo = new SoundInfo();
                await ((IRawSerializable)sndInfo).DeserializeAsync(stream).ConfigureAwait(false);

                // S3D adds this, so ignore.
                if (string.Compare(sndInfo.Name, S3DAssemblyPath, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    continue;
                }

                Add(sndInfo);
            }
        }
        catch (Exception ex)
        {
            throw new SdlFileException(Count, itemStartPosition, "The file could not be read due to a parser error.", ex);
        }
    }

    /// <summary>
    /// Saves to specified <paramref name="stream" />.
    /// </summary>
    /// <param name="stream">The stream to write to.</param>
    /// <exception cref="SdlFileException">Thrown when a parsing error occurs.</exception>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="stream" /> is null.</exception>
    public async Task SaveAsync(Stream stream)
    {
        if (stream == null)
        {
            throw new ArgumentNullException(nameof(stream));
        }

        if (!stream.CanWrite)
        {
            throw new ArgumentException("The stream does not support writing.", nameof(stream));
        }

        long itemStartPosition = 0;
        int itemIndex = -1;
        try
        {
            foreach (IRawSerializable sndInfo in this)
            {
                itemStartPosition = stream.Position;
                itemIndex++;
                await sndInfo.SerializeAsync(stream).ConfigureAwait(false);
            }
        }
        catch (Exception ex)
        {
            throw new SdlFileException(itemIndex, itemStartPosition, "The file could not be read due to a parser error.", ex);
        }
    }
}
