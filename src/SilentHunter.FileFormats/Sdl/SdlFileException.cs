using System;
using System.Runtime.Serialization;

namespace SilentHunter.FileFormats.Sdl;

/// <summary>
/// Represents an exception thrown during SDL read/write operations.
/// </summary>
public class SdlFileException : SilentHunterParserException
{
    /// <summary>
    /// Initializes a new instance of <see cref="SdlFileException" />.
    /// </summary>
    /// <param name="itemIndex">The index of the item at which the exception occurred.</param>
    /// <param name="fileOffset">The file offset where the exception occurred.</param>
    /// <param name="message">The error message.</param>
    /// <param name="innerException">The inner exception if any.</param>
    public SdlFileException(int itemIndex, long fileOffset, string message, Exception innerException)
        : base(message, innerException)
    {
        ItemIndex = itemIndex;
        FileOffset = fileOffset;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SdlFileException" /> class with serialized data.
    /// </summary>
    /// <param name="info">The <see cref="SerializationInfo" /> that holds the serialized object data about the exception being thrown.</param>
    /// <param name="context">The <see cref="StreamingContext" /> that contains contextual information about the source or destination.</param>
    /// <exception cref="ArgumentNullException">The <paramref name="info">info</paramref> parameter is null.</exception>
    /// <exception cref="SerializationException">The class name is null or <see cref="Exception.HResult"></see> is zero (0).</exception>
    protected SdlFileException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
        ItemIndex = info.GetInt32(nameof(ItemIndex));
        FileOffset = info.GetInt64(nameof(FileOffset));
    }

    /// <summary>
    /// Gets the index of the item at which the exception occurred.
    /// </summary>
    public int ItemIndex { get; }

    /// <summary>
    /// Gets the file offset where the exception occurred.
    /// </summary>
    public long FileOffset { get; }

    /// <inheritdoc />
    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        base.GetObjectData(info, context);

        info.AddValue(nameof(ItemIndex), ItemIndex);
        info.AddValue(nameof(FileOffset), FileOffset);
    }
}