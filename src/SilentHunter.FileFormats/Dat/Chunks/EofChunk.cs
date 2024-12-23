using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace SilentHunter.FileFormats.Dat.Chunks;

/// <summary>
/// Represents the end-of-file chunk, typically found at, well... the end.
/// </summary>
public sealed class EofChunk : DatChunk
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EofChunk" /> class.
    /// </summary>
    public EofChunk()
        : base(DatFile.Magics.Eof)
    {
    }

    /// <inheritdoc />
    protected override Task DeserializeAsync(Stream stream)
    {
        // Verify that there are no bytes in this chunk.
        Debug.Assert(stream.Length == 0, "Eof: Expected an empty stream.");
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    protected override Task SerializeAsync(Stream stream)
    {
        // Override and ignore base implementation. This is on purpose!
        return Task.CompletedTask;
    }
}
