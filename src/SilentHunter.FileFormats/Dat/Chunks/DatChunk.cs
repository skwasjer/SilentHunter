﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using SilentHunter.FileFormats.ChunkedFiles;
using SilentHunter.FileFormats.IO;

namespace SilentHunter.FileFormats.Dat.Chunks;

/// <summary>
/// Represents a base class for extending specific chunks. Alternatively, serves as a raw chunk for unknown magics.
/// </summary>
public class DatChunk : Chunk<DatFile.Magics>, IChunk<DatFile.Magics>, ICloneable
{
    /// <summary>
    /// Gets the DAT-chunk header size (a header consists of a type (int), sub type (int) and chunk size (int)).
    /// </summary>
    public const int ChunkHeaderSize = 12;

    private ulong _id, _parentId;
    private long _size;

    /// <summary>
    /// Initializes a new instance of <see cref="DatChunk" /> using specified <paramref name="magic" />.
    /// </summary>
    /// <param name="magic">The magic for this chunk.</param>
    public DatChunk(DatFile.Magics magic)
        : base(magic)
    {
    }

    /// <summary>
    /// Initializes a new instance of <see cref="DatChunk" /> using specified <paramref name="magic" /> and chunk <paramref name="subType" />.
    /// </summary>
    /// <param name="magic">The magic for this chunk.</param>
    /// <param name="subType">The sub type of this chunk.</param>
    public DatChunk(DatFile.Magics magic, int subType)
        : this(magic)
    {
        SubType = subType;
    }

    /// <summary>
    /// Gets or sets the chunk subtype.
    /// </summary>
    public int SubType { get; set; }

    /// <summary>
    /// Gets whether the chunk supports an id field.
    /// </summary>
    public virtual bool SupportsId { get => false; }

    /// <summary>
    /// Gets whether the chunk supports a parent id field.
    /// </summary>
    public virtual bool SupportsParentId { get => false; }

    /// <summary>
    /// Gets or sets the chunk id.
    /// </summary>
    public virtual ulong Id
    {
        get => _id;
        set
        {
            if (!SupportsId)
            {
                throw new NotSupportedException("Id is not supported.");
            }

            _id = value;
        }
    }

    /// <summary>
    /// Gets or sets the chunk its parent id.
    /// </summary>
    public virtual ulong ParentId
    {
        get => _parentId;
        set
        {
            if (!SupportsParentId)
            {
                throw new NotSupportedException("ParentId is not supported.");
            }

            _parentId = value;
        }
    }

    /// <summary>
    /// Gets the size of the chunk.
    /// </summary>
    public override long Size { get => _size; }

    private List<UnknownChunkData> _unknownData;

    /// <summary>
    /// Gets the unknown data.
    /// </summary>
    public List<UnknownChunkData> UnknownData { get => _unknownData ?? (_unknownData = new List<UnknownChunkData>()); }

    /// <summary>
    /// Deserializes the chunk. Note that the first 12 bytes (type, subtype and chunk size) are already read by the base class. Inheritors can override the default behavior, which is nothing more then reading all data, and caching it for later (ie. for serialization).
    /// </summary>
    /// <param name="stream">The stream to read from.</param>
    protected override async Task DeserializeAsync(Stream stream)
    {
        var regionStream = stream as RegionStream;
        long origin = regionStream?.BaseStream.Position ?? stream.Position;
        long relativeOrigin = stream.Position;

        await base.DeserializeAsync(stream).ConfigureAwait(false);

        if (Bytes != null && Bytes.Length > 0)
        {
            UnknownData.Add(new UnknownChunkData(origin, relativeOrigin, Bytes, "Unknown"));
        }
    }

    /// <summary>
    /// Deserializes the chunk, optionally including the 8 byte header, excluding the magic. To deserialize an entire chunk including magic use a ChunkReader.
    /// </summary>
    /// <param name="stream">The stream to read from.</param>
    /// <param name="includeHeader">True to include the 8 byte header (chunk sub type and chunk size).</param>
    public Task DeserializeAsync(Stream stream, bool includeHeader)
    {
        return includeHeader ? ((IRawSerializable)this).DeserializeAsync(stream) : DeserializeAsync(stream);
    }

    /// <summary>
    /// Serializes the chunk, optionally including the 8 byte header, excluding the magic. The serialize an entire chunk including magic use a ChunkWriter.
    /// </summary>
    /// <param name="stream">The stream to write to.</param>
    /// <param name="includeHeader">True to include the 8 byte header (chunk sub type and chunk size).</param>
    public Task SerializeAsync(Stream stream, bool includeHeader)
    {
        return includeHeader ? ((IRawSerializable)this).SerializeAsync(stream) : SerializeAsync(stream);
    }

    /// <summary>
    /// When implemented, deserializes the implemented class from specified <paramref name="stream" />.
    /// </summary>
    /// <param name="stream">The stream.</param>
    async Task IRawSerializable.DeserializeAsync(Stream stream)
    {
        // Deserialize the chunk header. The magic is already read for this chunk.
        using var reader = new BinaryReader(stream, FileEncoding.Default, true);
        // Read the subtype.
        SubType = reader.ReadInt32();

        // Read chunk size.
        int chunkSize = reader.ReadInt32();
        _size = chunkSize + ChunkHeaderSize;
        if (_size < 0)
        {
            stream.Position -= 4;
            throw new SilentHunterParserException($"Invalid chunk size ({_size} bytes). Can't be negative.");
        }

        long startPos = stream.Position;
        if (startPos + chunkSize > stream.Length)
        {
            stream.Position -= 4;
            throw new SilentHunterParserException($"Invalid chunk size ({_size} bytes). The stream has {stream.Length - stream.Position} bytes left.");
        }

        // Allow inheritors to deserialize the remainder of the chunk.
        using (var regionStream = new RegionStream(stream, chunkSize))
        {
            await DeserializeAsync(regionStream).ConfigureAwait(false);
        }

        // Verify that the inheritor read the entire chunk. If not, the inheritor does not implement it correctly, so we have to halt.
        if (stream.Position == startPos + chunkSize)
        {
            return;
        }

        if (stream.Position < startPos + chunkSize)
        {
            throw new SilentHunterParserException("Invalid deserialization of " + ToString() + ". More unparsed data in chunk.");

            //				stream.Position = startPos + chunkSize;
        }

        throw new SilentHunterParserException("Invalid deserialization of " + ToString() + ". Too much data was read while deserializing. This may indicate an invalid size specifier somewhere.");
    }

    /// <summary>
    /// When implemented, serializes the implemented class to specified <paramref name="stream" />.
    /// </summary>
    /// <param name="stream">The stream.</param>
    async Task IRawSerializable.SerializeAsync(Stream stream)
    {
        using var writer = new BinaryWriter(stream, FileEncoding.Default, true);
        writer.Write(SubType);

        long origin = stream.Position;
        writer.Write(0); // Empty placeholder for size.

        await SerializeAsync(stream).ConfigureAwait(false);

        long currentPos = stream.Position;
        stream.Seek(origin, SeekOrigin.Begin);
        _size = currentPos - origin - 4 + ChunkHeaderSize;
        writer.Write((uint)(_size - ChunkHeaderSize));
        stream.Seek(currentPos, SeekOrigin.Begin);
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return Enum.IsDefined(typeof(DatFile.Magics), Magic) ? Magic.ToString() : GetType().Name;
    }

    /// <inheritdoc />
    public virtual object Clone()
    {
        using var ms = new MemoryStream();
        using (var writer = new ChunkWriter<DatFile.Magics, DatChunk>(ms, true))
        {
            writer.WriteAsync(this).GetAwaiter().GetResult();
        }

        ms.Position = 0;

        using (ChunkReader<DatFile.Magics, DatChunk> reader = ((DatFile)ParentFile).CreateReader(ms))
        {
            return reader.ReadAsync().GetAwaiter().GetResult();
        }
    }

    /// <summary>
    /// Reads data with the <paramref name="reader" /> using specified <paramref name="func" /> and registers it as 'unknown' data.
    /// </summary>
    /// <typeparam name="T">The data type.</typeparam>
    /// <param name="reader">The reader.</param>
    /// <param name="func">The func providing the data.</param>
    /// <param name="description">The description of it, if any</param>
    /// <returns>Returns the value read using the <paramref name="func" />.</returns>
    protected T ReadUnknownData<T>(BinaryReader reader, Func<BinaryReader, T> func, string description)
        where T : struct
    {
        int structSize = Marshal.SizeOf<T>();
        long relativeOffset = reader.BaseStream.Position - structSize;
        var regionStream = reader.BaseStream as RegionStream;
        long absoluteOffset = regionStream?.BaseStream.Position - structSize ?? relativeOffset;
        T value = func(reader);
        PushUnknownData(absoluteOffset, relativeOffset, value, description);
        return value;
    }

    /// <summary>
    /// Registers data as 'unknown' data, at a specific <paramref name="absoluteOffset" /> and <paramref name="relativeOffset" />.
    /// </summary>
    /// <typeparam name="T">The data type.</typeparam>
    /// <param name="absoluteOffset">The absolute offset where the data is located in the file.</param>
    /// <param name="relativeOffset">The relative offset in relation to this chunk where the data is located.</param>
    /// <param name="data">The value.</param>
    /// <param name="description">The description of it, if any</param>
    protected void PushUnknownData<T>(long absoluteOffset, long relativeOffset, T data, string description)
    {
        UnknownData.Add(new UnknownChunkData(absoluteOffset, relativeOffset, data, description));
    }
}
