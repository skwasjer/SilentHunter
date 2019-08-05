using System;
using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using SilentHunter.Testing.FluentAssertions;
using Xunit;

namespace SilentHunter.FileFormats.Dat.Chunks
{
	public class TextureMapChunkTests
	{
		[Fact]
		public void Should_support_id()
		{
			ulong id = unchecked((ulong)DateTime.Now.Ticks);

			// Act
			Action act = () => new TextureMapChunk().Id = id;

			// Assert
			act.Should().NotThrow();
		}

		[Fact]
		public void Should_support_parent_id()
		{
			ulong id = unchecked((ulong)DateTime.Now.Ticks);

			// Act
			Action act = () => new TextureMapChunk().ParentId = id;

			// Assert
			act.Should().NotThrow();
		}

		[Fact]
		public void When_creating_new_instance_should_set_defaults()
		{
			var newInstance = new TextureMapChunk();
			var compareToInstance = new TextureMapChunk
			{
				ParentFile = null,
				FileOffset = 0,
				Magic = DatFile.Magics.TextureMap,
				ParentId = 0,
				Id = 0,
				SubType = 0,
				MapChannel = 2,
				MapType = TextureMapType.AmbientOcclusionMap,
				Attributes = MaterialAttributes.MagFilterLinear | MaterialAttributes.MinFilterLinear,
				CreationTime = DateTime.UtcNow,
				Texture = null,
				TgaTextureSize = 0
			};

			// Assert
			newInstance.Should().BeEquivalentTo(compareToInstance);
		}

		[Fact]
		public void When_setting_mapType_to_invalid_value_should_throw()
		{
			// Act
			Action act = () => new TextureMapChunk().MapType = (TextureMapType)int.MaxValue;

			// Assert
			act.Should().Throw<ArgumentException>();
		}

		[Fact]
		public async Task When_serializing_should_produce_correct_binary_data()
		{
			byte[] expectedRawData = { 0xc8, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x7b, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x73, 0x70, 0x65, 0x63, 0x75, 0x6c, 0x61, 0x72, 0x02, 0x00, 0x00, 0x00, 0x02, 0x80, 0x00, 0x00, 0x15, 0x03, 0x00, 0x00, 0xc5, 0x10, 0xa3, 0x48, 0x00, 0x00, 0x00, 0x00, 0x6d, 0x79, 0x20, 0x74, 0x65, 0x78, 0x74, 0x75, 0x72, 0x65, 0x2e, 0x74, 0x67, 0x61, 0x00 };
			var chunk = new TextureMapChunk
			{
				ParentId = 123,
				Id = 456,
				MapChannel = 2,
				MapType = TextureMapType.SpecularMap,
				Attributes = MaterialAttributes.CullNone | MaterialAttributes.NoDxtCompression,
				CreationTime = new DateTime(2008, 8, 13, 16, 50, 13),
				Texture = "my texture.tga",
				TgaTextureSize = 789
			};

			using (var ms = new MemoryStream())
			{
				// Act
				await chunk.SerializeAsync(ms, false);

				// Assert
				ms.ToArray().Should().BeEquivalentTo(expectedRawData);
			}
		}

		[Theory]
		[InlineData("my texture.tga", TextureMapType.SpecularMap, MaterialAttributes.CullNone | MaterialAttributes.NoDxtCompression)]
		[InlineData("", TextureMapType.AmbientOcclusionMap, MaterialAttributes.DisableZBufferWrite)]
		[InlineData(null, TextureMapType.NormalMap, MaterialAttributes.None)]
		public async Task When_serializing_and_then_deserializing_should_produce_equivalent(string texture, TextureMapType textureMapType, MaterialAttributes materialAttributes)
		{
			var chunk = new TextureMapChunk
			{
				ParentId = 123,
				Id = 456,
				MapChannel = 2,
				MapType = textureMapType,
				Attributes = materialAttributes,
				CreationTime = new DateTime(2008, 8, 13, 16, 50, 13),
				Texture = texture,
				TgaTextureSize = 789
			};

			using (var ms = new MemoryStream())
			{
				await chunk.SerializeAsync(ms, false);
				ms.Position = 0;

				// Act
				var deserializedChunk = new TextureMapChunk();
				await deserializedChunk.DeserializeAsync(ms, false);

				// Assert
				deserializedChunk.Should().BeEquivalentTo(chunk);
				ms.Should().BeEof();
			}
		}

		[Fact]
		public async Task Given_stream_contains_more_data_than_chunk_needs_should_advance_to_end()
		{
			var chunk = new TextureMapChunk();
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
