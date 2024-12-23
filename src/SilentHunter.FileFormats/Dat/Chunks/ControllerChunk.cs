using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using SilentHunter.FileFormats.Extensions;

namespace SilentHunter.FileFormats.Dat.Chunks;

/// <summary>
/// Represents a controller identifier class.
/// </summary>
[DebuggerDisplay("{ToString(),nq}: {Name}")]
public sealed class ControllerChunk : DatChunk
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ControllerChunk" /> class.
    /// </summary>
    public ControllerChunk()
        : base(DatFile.Magics.Controller)
    {
    }

    /// <summary>
    /// Gets or sets the controller name.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets whether the chunk supports an id field.
    /// </summary>
    public override bool SupportsId { get => true; }

    /// <summary>
    /// Gets whether the chunk supports a parent id field.
    /// </summary>
    public override bool SupportsParentId { get => true; }

    /// <inheritdoc />
    protected override Task DeserializeAsync(Stream stream)
    {
        using (var reader = new BinaryReader(stream, FileEncoding.Default, true))
        {
            // Read id and parent id.
            Id = reader.ReadUInt64();
            ParentId = reader.ReadUInt64();

            // Skip byte.
            byte alwaysZero = reader.ReadByte();
            Debug.Assert(alwaysZero == byte.MinValue, "Controllers: expecting byte=0.");

            // The rest of the stream holds the name + terminating zero.
            if (stream.Length > stream.Position)
            {
                Name = reader.ReadNullTerminatedString();

                // Some files contain more data, problem seen mainly in some mods due to hex editing likely. Chunk will be correctly serialized upon next save.
                if (stream.Length > stream.Position)
                {
                    stream.Position = stream.Length;
                }
            }
        }

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    protected override Task SerializeAsync(Stream stream)
    {
        using (var writer = new BinaryWriter(stream, FileEncoding.Default, true))
        {
            // Write id and parent id.
            writer.Write(Id);
            writer.Write(ParentId);

            // Zero byte.
            writer.Write(byte.MinValue);

            // Write name + terminating zero.
            if (Name != null)
            {
                writer.Write(Name, '\0');
            }
        }

        return Task.CompletedTask;
    }
}
