using System;
using System.IO;
using System.Threading.Tasks;
using SilentHunter.FileFormats.Graphics;
using SilentHunter.Testing.FluentAssertions;

namespace SilentHunter.FileFormats.Dat.Chunks;

public class EmbeddedImageChunkTests
{
    [Fact]
    public void Should_not_support_id()
    {
        ulong id = unchecked((ulong)DateTime.Now.Ticks);

        // Act
        Action act = () => new EmbeddedImageChunk().Id = id;

        // Assert
        act.Should().Throw<Exception>();
    }

    [Fact]
    public void Should_not_support_parent_id()
    {
        ulong id = unchecked((ulong)DateTime.Now.Ticks);

        // Act
        Action act = () => new EmbeddedImageChunk().ParentId = id;

        // Assert
        act.Should().Throw<Exception>();
    }

    [Theory]
    [InlineData("tga", ImageFormat.Tga)]
    [InlineData("dds", ImageFormat.Dds)]
    [InlineData(null, ImageFormat.Unknown)]
    public async Task When_writing_imageData_should_attempt_to_detect_image_format(string imageIdentifier, ImageFormat expectedImageFormat)
    {
        byte[] fakeImageData = [0x1, 0x2, 0x3, 0x4];
        IImageFormatDetector imageFormatDetectorMock = Substitute.For<IImageFormatDetector>();
        imageFormatDetectorMock
            .GetImageFormat(Arg.Any<Stream>())
            .Returns(imageIdentifier);
        var chunk = new EmbeddedImageChunk(new ImageFormatDetection([imageFormatDetectorMock]));

        // Act
        await chunk.WriteAsync(fakeImageData);

        // Assert
        imageFormatDetectorMock.Received(1).GetImageFormat(Arg.Any<Stream>());
        chunk.ImageFormat.Should().Be(expectedImageFormat);
    }

    [Fact]
    public async Task Given_imageData_When_serializing_should_save_exact_same_imageData()
    {
        byte[] fakeImageData = [0x1, 0x2, 0x3, 0x4];
        var chunk = new EmbeddedImageChunk();

        using var ms = new MemoryStream();
        // Act
        await chunk.WriteAsync(fakeImageData);
        await chunk.SerializeAsync(ms, false);

        // Assert
        ms.ToArray().Should().BeEquivalentTo(fakeImageData);
    }

    [Theory]
    [InlineData("tga", ImageFormat.Tga)]
    [InlineData("dds", ImageFormat.Dds)]
    [InlineData(null, ImageFormat.Unknown)]
    public async Task When_serializing_and_then_deserializing_should_produce_equivalent(string imageIdentifier, ImageFormat expectedImageFormat)
    {
        byte[] fakeImageData = [0x1, 0x2, 0x3, 0x4];
        IImageFormatDetector imageFormatDetectorMock = Substitute.For<IImageFormatDetector>();
        imageFormatDetectorMock
            .GetImageFormat(Arg.Any<Stream>())
            .Returns(imageIdentifier);
        var chunk = new EmbeddedImageChunk();

        using var ms = new MemoryStream();
        await chunk.WriteAsync(fakeImageData);
        await chunk.SerializeAsync(ms, false);
        ms.Position = 0;

        // Act
        var deserializedChunk = new EmbeddedImageChunk(new ImageFormatDetection([imageFormatDetectorMock]));
        await deserializedChunk.DeserializeAsync(ms, false);

        // Assert
        (await deserializedChunk.ReadAsByteArrayAsync()).Should().BeEquivalentTo(fakeImageData);
        deserializedChunk.ImageFormat.Should().Be(expectedImageFormat);
        ms.Should().BeEof();
    }

    [Fact]
    public async Task When_writing_imageData_should_set_length()
    {
        byte[] fakeImageData = [0x1, 0x2, 0x3, 0x4];
        var chunk = new EmbeddedImageChunk();

        // Act
        await chunk.WriteAsync(fakeImageData);

        // Assert
        chunk.Length.Should().Be(fakeImageData.Length);
    }

    [Fact]
    public async Task Given_null_byteArray_when_writing_should_throw()
    {
        byte[] imageData = null;
        var chunk = new EmbeddedImageChunk();

        // Act
        // ReSharper disable once ExpressionIsAlwaysNull
        Func<Task> act = () => chunk.WriteAsync(imageData);

        // Assert
        await act.Should()
            .ThrowAsync<ArgumentNullException>()
            .WithParameterName(nameof(imageData));
    }

    [Fact]
    public async Task Given_null_stream_when_writing_should_throw()
    {
        Stream imageData = null;
        var chunk = new EmbeddedImageChunk();

        // Act
        // ReSharper disable once ExpressionIsAlwaysNull
        Func<Task> act = () => chunk.WriteAsync(imageData);

        // Assert
        await act.Should()
            .ThrowAsync<ArgumentNullException>()
            .WithParameterName(nameof(imageData));
    }

    [Fact]
    public async Task Given_non_readable_stream_when_writing_should_throw()
    {
        Stream imageData = Substitute.For<Stream>();
        imageData.CanRead.Returns(false);
        var chunk = new EmbeddedImageChunk();

        // Act
        Func<Task> act = () => chunk.WriteAsync(imageData);

        // Assert
        await act.Should()
            .ThrowAsync<ArgumentException>()
            .WithMessage("The stream does not support reading.*")
            .WithParameterName(nameof(imageData));
    }

    [Fact]
    public async Task Given_no_data_when_writing_should_throw()
    {
        byte[] imageData = [];
        var chunk = new EmbeddedImageChunk();

        // Act
        Func<Task> act = () => chunk.WriteAsync(imageData);

        // Assert
        await act.Should()
            .ThrowAsync<ArgumentException>()
            .WithMessage("No data to read from stream.*")
            .WithParameterName(nameof(imageData));
    }

    [Fact]
    public async Task Given_stream_does_not_support_seeking_when_writing_should_not_throw()
    {
        byte[] fakeImageData = [0x1, 0x2, 0x3, 0x4];

        // Create stream stub that does not support seeking.
        MemoryStream imageDataMock = Substitute.ForPartsOf<MemoryStream>();
        imageDataMock.CanSeek.Returns(false);
        imageDataMock.Write(fakeImageData, 0, fakeImageData.Length);
        imageDataMock.Position = 0;

        IImageFormatDetector imageFormatDetectorMock = Substitute.For<IImageFormatDetector>();
        imageFormatDetectorMock
            .GetImageFormat(Arg.Any<Stream>())
            .Returns("tga");

        var chunk = new EmbeddedImageChunk(new ImageFormatDetection([imageFormatDetectorMock]));

        // Act
        Func<Task> act = () => chunk.WriteAsync(imageDataMock);

        // Assert
        await act.Should().NotThrowAsync<Exception>();
        chunk.ImageFormat.Should().Be(ImageFormat.Tga);
        (await chunk.ReadAsByteArrayAsync()).Should().BeEquivalentTo(fakeImageData);
        imageFormatDetectorMock.DidNotReceive().GetImageFormat(imageDataMock);
        imageFormatDetectorMock.Received().GetImageFormat(Arg.Any<Stream>());
    }
}
