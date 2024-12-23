﻿using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using SilentHunter.FileFormats.Extensions;

namespace SilentHunter.FileFormats.Dat.Chunks;

/// <summary>
/// Represents a chunk to store settings or other data in the dat file, that can later be used to restore state when used by the same parser. The chunk is ignored by the game.
/// </summary>
internal sealed class S3DSettingsChunk : DatChunk
{
    public S3DSettingsChunk()
        : base(DatFile.Magics.S3DSettings, 0xFFFF)
    {
    }

    /// <inheritdoc />
    protected override Task DeserializeAsync(Stream stream)
    {
#if DEBUG
        Debug.WriteLine("Contains S3DSettings : " + stream.GetBaseStreamName());
#endif
        return base.DeserializeAsync(stream);
    }
}
