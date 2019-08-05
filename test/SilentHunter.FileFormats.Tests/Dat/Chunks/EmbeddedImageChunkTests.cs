using System;
using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using SilentHunter.FileFormats.Graphics;
using SilentHunter.Testing.FluentAssertions;
using Xunit;

namespace SilentHunter.FileFormats.Dat.Chunks
{
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
			byte[] fakeImageData = { 0x1, 0x2, 0x3, 0x4 };
			var imageFormatDetectorMock = new Mock<IImageFormatDetector>();
			imageFormatDetectorMock
				.Setup(m => m.GetImageFormat(It.IsAny<Stream>()))
				.Returns(imageIdentifier);
			var chunk = new EmbeddedImageChunk(new ImageFormatDetection(new[] { imageFormatDetectorMock.Object }));

			// Act
			await chunk.WriteAsync(fakeImageData);

			// Assert
			imageFormatDetectorMock.Verify(m => m.GetImageFormat(It.IsAny<Stream>()), Times.Once);
			chunk.ImageFormat.Should().Be(expectedImageFormat);
		}

		[Fact]
		public async Task Given_imageData_When_serializing_should_save_exact_same_imageData()
		{
			byte[] fakeImageData = { 0x1, 0x2, 0x3, 0x4 };
			var chunk = new EmbeddedImageChunk();

			using (var ms = new MemoryStream())
			{
				// Act
				await chunk.WriteAsync(fakeImageData);
				await chunk.SerializeAsync(ms, false);

				// Assert
				ms.ToArray().Should().BeEquivalentTo(fakeImageData);
			}
		}

		[Theory]
		[InlineData("tga", ImageFormat.Tga)]
		[InlineData("dds", ImageFormat.Dds)]
		[InlineData(null, ImageFormat.Unknown)]
		public async Task When_serializing_and_then_deserializing_should_produce_equivalent(string imageIdentifier, ImageFormat expectedImageFormat)
		{
			byte[] fakeImageData = { 0x1, 0x2, 0x3, 0x4 };
			var imageFormatDetectorMock = new Mock<IImageFormatDetector>();
			imageFormatDetectorMock
				.Setup(m => m.GetImageFormat(It.IsAny<Stream>()))
				.Returns(imageIdentifier);
			var chunk = new EmbeddedImageChunk();

			using (var ms = new MemoryStream())
			{
				await chunk.WriteAsync(fakeImageData);
				await chunk.SerializeAsync(ms, false);
				ms.Position = 0;

				// Act
				var deserializedChunk = new EmbeddedImageChunk(new ImageFormatDetection(new[] { imageFormatDetectorMock.Object }));
				await deserializedChunk.DeserializeAsync(ms, false);

				// Assert
				(await deserializedChunk.ReadAsByteArrayAsync()).Should().BeEquivalentTo(fakeImageData);
				deserializedChunk.ImageFormat.Should().Be(expectedImageFormat);
				ms.Should().BeEof();
			}
		}

		[Fact]
		public async Task When_writing_imageData_should_set_length()
		{
			byte[] fakeImageData = { 0x1, 0x2, 0x3, 0x4 };
			var chunk = new EmbeddedImageChunk();

			// Act
			await chunk.WriteAsync(fakeImageData);

			// Assert
			chunk.Length.Should().Be(fakeImageData.Length);
		}

		[Fact]
		public void Given_null_byteArray_when_writing_should_throw()
		{
			byte[] imageData = null;
			var chunk = new EmbeddedImageChunk();

			// Act
			// ReSharper disable once ExpressionIsAlwaysNull
			Func<Task> act = () => chunk.WriteAsync(imageData);

			// Assert
			act.Should()
				.Throw<ArgumentNullException>()
				.WithParamName(nameof(imageData));
		}

		[Fact]
		public void Given_null_stream_when_writing_should_throw()
		{
			Stream imageData = null;
			var chunk = new EmbeddedImageChunk();

			// Act
			// ReSharper disable once ExpressionIsAlwaysNull
			Func<Task> act = () => chunk.WriteAsync(imageData);

			// Assert
			act.Should()
				.Throw<ArgumentNullException>()
				.WithParamName(nameof(imageData));
		}

		[Fact]
		public void Given_non_readable_stream_when_writing_should_throw()
		{
			var streamMock = new Mock<Stream>();
			streamMock.Setup(m => m.CanRead).Returns(false);
			Stream imageData = streamMock.Object;
			var chunk = new EmbeddedImageChunk();

			// Act
			Func<Task> act = () => chunk.WriteAsync(imageData);

			// Assert
			act.Should()
				.Throw<ArgumentException>()
				.WithMessage("The stream does not support reading.*")
				.WithParamName(nameof(imageData));
		}

		[Fact]
		public void Given_no_data_when_writing_should_throw()
		{
			var imageData = new byte[0];
			var chunk = new EmbeddedImageChunk();

			// Act
			Func<Task> act = () => chunk.WriteAsync(imageData);

			// Assert
			act.Should()
				.Throw<ArgumentException>()
				.WithMessage("No data to read from stream.*")
				.WithParamName(nameof(imageData));
		}

		[Fact]
		public async Task Given_stream_does_not_support_seeking_when_writing_should_not_throw()
		{
			byte[] fakeImageData = { 0x1, 0x2, 0x3, 0x4 };

			// Create stream stub that does not support seeking.
			var streamMock = new Mock<MemoryStream> { CallBase = true };
			streamMock.Setup(m => m.CanSeek).Returns(false);
			Stream imageData = streamMock.Object;
			imageData.Write(fakeImageData, 0, fakeImageData.Length);
			imageData.Position = 0;

			var imageFormatDetectorMock = new Mock<IImageFormatDetector>();
			imageFormatDetectorMock
				.Setup(m => m.GetImageFormat(It.IsAny<Stream>()))
				.Returns("tga")
				.Verifiable();

			var chunk = new EmbeddedImageChunk(new ImageFormatDetection(new []
			{
				imageFormatDetectorMock.Object
			}));

			// Act
			Func<Task> act = () => chunk.WriteAsync(imageData);

			// Assert
			act.Should().NotThrow<Exception>();
			chunk.ImageFormat.Should().Be(ImageFormat.Tga);
			(await chunk.ReadAsByteArrayAsync()).Should().BeEquivalentTo(fakeImageData);
			imageFormatDetectorMock.Verify();
		}
	}
}
