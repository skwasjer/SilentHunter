using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using SilentHunter.FileFormats.Extensions;

namespace SilentHunter.FileFormats.Dat.Chunks;

/// <summary>
/// </summary>
/// <remarks>
/// Probably poorly named.
/// </remarks>
public sealed class BodyPartsChunk : DatChunk
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BodyPartsChunk" /> class.
    /// </summary>
    public BodyPartsChunk()
        : base(DatFile.Magics.BodyParts)
    {
        Parts = new List<string>();
    }

    /// <inheritdoc />
    public override ulong Id
    {
        get => base.Id;
        set
        {
            if (value > uint.MaxValue)
            {
                throw new ArgumentOutOfRangeException(nameof(value), "The id for this chunk is only 4 bytes in length (UInt32).");
            }

            base.Id = value;
        }
    }

    /// <summary>
    /// Gets the part names.
    /// </summary>
    public List<string> Parts { get; }

    /// <summary>
    /// Gets whether the chunk supports an id field.
    /// </summary>
    public override bool SupportsId { get => true; }

    /// <summary>
    /// Deserializes the chunk.
    /// </summary>
    /// <param name="stream">The stream to read from.</param>
    protected override Task DeserializeAsync(Stream stream)
    {
        using (var reader = new BinaryReader(stream, FileEncoding.Default, true))
        {
            Id = reader.ReadUInt32(); // Read an id as an uint.

            Parts.Clear();

            int partCount = reader.ReadInt32();
            for (int i = 0; i < partCount; i++)
            {
                Parts.Add(reader.ReadNullTerminatedString());
            }
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// Serializes the chunk.
    /// </summary>
    /// <param name="stream">The stream to write to.</param>
    protected override Task SerializeAsync(Stream stream)
    {
        using (var writer = new BinaryWriter(stream, FileEncoding.Default, true))
        {
            writer.Write((uint)Id); // Write an id as an uint.

            writer.Write(Parts.Count);
            foreach (string partName in Parts)
            {
                writer.Write(partName, '\0');
            }
        }

        return Task.CompletedTask;
    }
}
