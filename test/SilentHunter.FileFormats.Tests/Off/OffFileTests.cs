using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;

namespace SilentHunter.FileFormats.Off;

public class OffFileTests
{
    private static readonly OffCharacter OffFileSingleChar = new() { Character = 'A', BoundingBox = new Rectangle(0, 0, 16, 14) };
    private static readonly byte[] OffFileSingleCharData =
    [
        // Header
        .. BitConverter.GetBytes(1), // character count
        .. BitConverter.GetBytes(2), // x spacing
        .. BitConverter.GetBytes(4), // y spacing
            // Char starts here
        (byte)'A', // character
        .. BitConverter.GetBytes(0), // x offset
        .. BitConverter.GetBytes(0), // y offset
        .. BitConverter.GetBytes(16), // width
        .. BitConverter.GetBytes(14), // height
    ];

    private readonly OffFile _sut = [];

    [Fact]
    public void Items_yields_same_collection()
    {
        (_sut as ISilentHunterFile<OffCharacter>).Items.Should().BeSameAs(_sut);
    }

    [Fact]
    public async Task LoadAsync_should_clear_items_before_loading()
    {
        var ch = new OffCharacter();
        _sut.Add(ch);

        // Act
        using var ms = new MemoryStream(OffFileSingleCharData);
        await _sut.LoadAsync(ms);

        // Assert
        _sut.Should()
            .ContainSingle()
            .Which.Should()
            .NotBeSameAs(ch)
            .And.BeEquivalentTo(OffFileSingleChar);
    }
}
