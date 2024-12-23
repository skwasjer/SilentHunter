using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using SilentHunter.FileFormats.Extensions;
using SilentHunter.FileFormats.IO;

namespace SilentHunter.FileFormats.Dat.Chunks;

/// <summary>
/// Represents the AuthorInfo chunk of a Silent Hunter game file.
/// </summary>
[DebuggerDisplay("{ToString(),nq}: {Author}, Description = {Description}")]
public sealed class AuthorInfoChunk : DatChunk
{
    private string _author;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthorInfoChunk" /> class.
    /// </summary>
    public AuthorInfoChunk()
        : base(DatFile.Magics.AuthorInfo)
    {
    }

    /// <summary>
    /// Gets or sets an unknown <see cref="long" /> value.
    /// </summary>
    public long Unknown { get; set; }

    /// <summary>
    /// Gets or sets the author that created or last modified the file.
    /// </summary>
    public string Author
    {
        get => _author ?? string.Empty;
        set => _author = value;
    }

    /// <summary>
    /// Gets or sets the description of the file. In case this is an original, unmodified file, this property will most likely hold the value "Created/modified with Kashmir".
    /// </summary>
    public string Description { get; set; }

    /// <inheritdoc />
    protected override Task DeserializeAsync(Stream stream)
    {
        var regionStream = stream as RegionStream;

        using var reader = new BinaryReader(stream, FileEncoding.Default, true);
        Unknown = ReadUnknownData(reader, r => r.ReadInt64(), "No idea");

        Author = reader.ReadNullTerminatedString();
        Description = stream.Position < stream.Length ? reader.ReadNullTerminatedString() : null;

        if (stream.Position == stream.Length)
        {
            return Task.CompletedTask;
        }

        // S3D adds a signature. Ignore.
        string s3dSignature = reader.ReadString((int)(stream.Length - stream.Position - 1))?.TrimEnd(" \0".ToCharArray());
        Debug.WriteLine(s3dSignature);
        if (stream.Length > stream.Position)
        {
            stream.Position = stream.Length;
        }

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    protected override Task SerializeAsync(Stream stream)
    {
        using (var writer = new BinaryWriter(stream, FileEncoding.Default, true))
        {
            writer.WriteStruct(Unknown);

            writer.Write(Author, '\0');
            if (Description != null)
            {
                writer.Write(Description, '\0');
            }
        }

        return Task.CompletedTask;
    }
}
