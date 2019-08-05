using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using SilentHunter.FileFormats.ChunkedFiles;
using SilentHunter.Testing.FluentAssertions;
using Xunit;

namespace SilentHunter.FileFormats.Dat.Chunks
{
	public class IndexChunkTests
	{
		[Fact]
		public void Should_not_support_id()
		{
			ulong id = unchecked((ulong)DateTime.Now.Ticks);

			// Act
			Action act = () => new IndexChunk().Id = id;

			// Assert
			act.Should().Throw<Exception>();
		}

		[Fact]
		public void Should_not_support_parent_id()
		{
			ulong id = unchecked((ulong)DateTime.Now.Ticks);

			// Act
			Action act = () => new IndexChunk().ParentId = id;

			// Assert
			act.Should().Throw<Exception>();
		}

		[Fact]
		public async Task When_serializing_should_produce_correct_binary_data()
		{
			byte[] expectedRawData = { 0x06, 0x12, 0x0f, 0x00, 0x00, 0x00, 0x00, 0x00, 0x40, 0xe2, 0x01, 0x00, 0xb1, 0x68, 0xde, 0x3a, 0x00, 0x00, 0x00, 0x00, 0x15, 0xcd, 0x5b, 0x07 };

			var parentFileMock = new Mock<IChunkFile>();
			parentFileMock
				.Setup(m => m.Chunks)
				.Returns(new Collection<DatChunk>
				{
					new TextureMapChunk
					{
						Id = 987654,
						FileOffset = 123456
					},
					new TextureMapChunk
					{
						Id = 987654321,
						FileOffset = 123456789
					}
				});
			var chunk = new IndexChunk
			{
				ParentFile = parentFileMock.Object
			};

			using (var ms = new MemoryStream())
			{
				// Act
				await chunk.SerializeAsync(ms, false);

				// Assert
				ms.ToArray().Should().BeEquivalentTo(expectedRawData);
			}
		}

		[Fact]
		public async Task When_serializing_and_then_deserializing_should_produce_equivalent()
		{
			var referenceChunks = new Collection<DatChunk>
			{
				new TextureMapChunk
				{
					Id = 987654,
					FileOffset = 123456
				},
				new TextureMapChunk
				{
					Id = 987654321,
					FileOffset = 123456789
				}
			};
			var parentFileMock = new Mock<IChunkFile>();
			parentFileMock
				.Setup(m => m.Chunks)
				.Returns(referenceChunks);

			var chunk = new IndexChunk
			{
				ParentFile = parentFileMock.Object
			};

			using (var ms = new MemoryStream())
			{
				await chunk.SerializeAsync(ms, false);
				ms.Position = 0;

				// Act
				var deserializedChunk = new IndexChunk();
				await deserializedChunk.DeserializeAsync(ms, false);

				// Assert
				deserializedChunk.Should().BeEquivalentTo(chunk, options => options.Excluding(c => c.ParentFile));
				ms.Should().BeEof();
			}
		}

		[Fact]
		public async Task Given_stream_contains_more_data_than_chunk_needs_should_advance_to_end()
		{
			var chunk = new IndexChunk();
			using (var ms = new MemoryStream())
			{
				await chunk.SerializeAsync(ms, false);
				// Add garbage to end.
				ms.Write(new byte[] { 0x1, 0x2 }, 0, 2);
				ms.Position = 0;

				// Act
				await chunk.DeserializeAsync(ms, false);

				// Assert
				ms.Should().BeEof();
			}
		}
	}
}
