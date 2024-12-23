using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using SilentHunter.FileFormats.Dat.Chunks;
using SilentHunter.FileFormats.Extensions;

namespace SilentHunter.FileFormats;

/// <summary>
/// Represents unknown chunk data and the exact location in the file.
/// </summary>
[DebuggerDisplay("Offset = {Offset}, RelOffset = {RelativeOffset}, Data = {Data}, MyGuess = {MyGuess}")]
public class UnknownChunkData
{
    /// <summary>
    /// Initializes a new instance of <see cref="UnknownChunkData" />.
    /// </summary>
    /// <param name="offset">The absolute file offset.</param>
    /// <param name="relativeOffset">The relative offset in the chunk.</param>
    /// <param name="data">The unknown data.</param>
    /// <param name="myGuess">A guess of what the data may be.</param>
    public UnknownChunkData(long offset, long relativeOffset, object data, string myGuess)
    {
        Data = data ?? throw new ArgumentNullException(nameof(data));
        Offset = offset;
        RelativeOffset = relativeOffset + DatChunk.ChunkHeaderSize; // Add header size.
        MyGuess = myGuess;
    }

    /// <summary>
    /// Gets the type of the data.
    /// </summary>
    public Type Type { get => Data.GetType(); }

    /// <summary>
    /// Gets the estimated descriptive guess that this data represents.
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public string MyGuess { get; }

    /// <summary>
    /// Gets the data.
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public object Data { get; }

    /// <summary>
    /// Gets the data as a byte array.
    /// </summary>
    public byte[] GetDataAsByteArray()
    {
        if (Data is byte[] data)
        {
            return data;
        }

        // Convert data to byte array.
        using var ms = new MemoryStream();
        using var writer = new BinaryWriter(ms, FileEncoding.Default);
        writer.WriteStruct(Data);
        return ms.ToArray();
    }

    /// <summary>
    /// Gets the absolute file offset.
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public long Offset { get; }

    /// <summary>
    /// Gets the relative offset in the chunk.
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public long RelativeOffset { get; }

    /// <summary>
    /// Returns the size of the data.
    /// </summary>
    /// <returns></returns>
    private long GetDataSize()
    {
        try
        {
            return Marshal.SizeOf(Data);
        }
        catch
        {
            try
            {
                return GetDataAsByteArray().Length;
            }
            catch
            {
                return -1;
            }
        }
    }

    /// <summary>
    /// Returns a string that represents the current object.
    /// </summary>
    /// <param name="offsetFormat">The format to write the offset in.</param>
    /// <returns>
    /// A string that represents the current object.
    /// </returns>
    public string ToString(string offsetFormat)
    {
        string leadHex = null;
        if (offsetFormat.ToUpper() == "X")
        {
            leadHex = "0x";
        }

        return string.Format("RelOffset = " + leadHex + "{0:" + offsetFormat + "}, Size = {1}", RelativeOffset, GetDataSize());
    }

    /// <summary>
    /// Returns a string that represents the current object.
    /// </summary>
    /// <returns>
    /// A string that represents the current object.
    /// </returns>
    public override string ToString()
    {
        return ToString("d");
    }
}
