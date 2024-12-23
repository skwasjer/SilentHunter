using System;
using System.Runtime.Serialization;
using System.Text;

namespace SilentHunter.FileFormats.Dat;

/// <summary>
/// The exception that is thrown when a parsing error occurs.
/// </summary>
public class DatFileException : SilentHunterParserException
{
    /// <summary>
    /// Initializes a new instance of <see cref="DatFileException" />.
    /// </summary>
    /// <param name="chunkIndex">The index of the chunk at which the exception occurred.</param>
    /// <param name="chunkOffset">The index of the chunk at which the exception occurred.</param>
    /// <param name="fileOffset">The file offset where the exception occurred.</param>
    /// <param name="message">The error message.</param>
    /// <param name="innerException">The inner exception if any.</param>
    public DatFileException(int chunkIndex, long chunkOffset, long fileOffset, string message, Exception innerException)
        : base(message, innerException)
    {
        ChunkIndex = chunkIndex;
        ChunkOffset = chunkOffset;
        FileOffset = fileOffset;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DatFileException" /> class with serialized data.
    /// </summary>
    /// <param name="info">The <see cref="SerializationInfo" /> that holds the serialized object data about the exception being thrown.</param>
    /// <param name="context">The <see cref="StreamingContext" /> that contains contextual information about the source or destination.</param>
    /// <exception cref="ArgumentNullException">The <paramref name="info">info</paramref> parameter is null.</exception>
    /// <exception cref="SerializationException">The class name is null or <see cref="Exception.HResult"></see> is zero (0).</exception>
    protected DatFileException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
        ChunkIndex = info.GetInt32(nameof(ChunkIndex));
        ChunkOffset = info.GetInt64(nameof(ChunkOffset));
        FileOffset = info.GetInt64(nameof(FileOffset));
    }

    /// <summary>
    /// Gets the index of the chunk at which the exception occurred.
    /// </summary>
    public int ChunkIndex { get; }

    /// <summary>
    /// Gets the offset inside the chunk where the exception occurred.
    /// </summary>
    public long ChunkOffset { get; }

    /// <summary>
    /// Gets the file offset where the exception occurred.
    /// </summary>
    public long FileOffset { get; }

    /// <inheritdoc />
    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        base.GetObjectData(info, context);

        info.AddValue(nameof(ChunkIndex), ChunkIndex);
        info.AddValue(nameof(ChunkOffset), ChunkOffset);
        info.AddValue(nameof(FileOffset), FileOffset);
    }

    /// <summary>
    /// Gets a message that describes the current exception.
    /// </summary>
    /// <returns>
    /// The error message that explains the reason for the exception, or an empty string ("").
    /// </returns>
    public override string Message
    {
        get
        {
            var sb = new StringBuilder(base.Message);
            sb.AppendLine();
            sb.AppendFormat("  Chunk index: {0}", ChunkIndex);
            sb.AppendLine();
            sb.AppendFormat("  Chunk offset: 0x{0:x8}", ChunkOffset);
            sb.AppendLine();
            sb.AppendFormat("  Parsing stopped at: 0x{0:x8}", FileOffset);
            sb.AppendLine();
            sb.AppendLine();
            return sb.ToString();
        }
    }
}
