using System;
using System.IO;
using System.Numerics;
using System.Threading.Tasks;
using FluentAssertions;
using SilentHunter.FileFormats.FluentAssertions;
using SilentHunter.Testing.Extensions;
using SilentHunter.Testing.FluentAssertions;
using Xunit;

namespace SilentHunter.FileFormats.Dat.Chunks
{
	public class ModelChunkTests
	{
		[Fact]
		public void Should_support_id()
		{
			ulong id = unchecked((ulong)DateTime.Now.Ticks);

			// Act
			Action act = () => new ModelChunk().Id = id;

			// Assert
			act.Should().NotThrow();
		}

		[Fact]
		public void Should_not_support_parent_id()
		{
			ulong id = unchecked((ulong)DateTime.Now.Ticks);

			// Act
			Action act = () => new ModelChunk().ParentId = id;

			// Assert
			act.Should().Throw<Exception>();
		}

		[Fact]
		public void When_creating_new_instance_should_set_defaults()
		{
			var newInstance = new ModelChunk();
			var compareToInstance = new ModelChunk
			{
				ParentFile = null,
				FileOffset = 0,
				Magic = DatFile.Magics.Model,
				Id = 0,
				SubType = 0,
				MaterialIndices = new byte[0],
				Normals = new Vector3[0],
				TextureCoordinates = new Vector2[0],
				TextureIndices = new UvMap[0],
				VertexIndices = new ushort[0],
				Vertices = new Vector3[0]
			};

			// Assert
			newInstance.Should().BeEquivalentTo(compareToInstance);
		}

		[Theory]
		[InlineData(true, "ModelWithNormals.chunkdata")]
		[InlineData(false, "Model.chunkdata")]
		public async Task Given_raw_chunk_data_when_deserializing_should_load_model(bool includeNormals, string resourceName)
		{
			byte[] rawChunkData = GetType().Assembly.GetManifestResourceStream(GetType(), resourceName).ToArray();

			ModelChunk expectedChunk = GetPopulatedModelChunk(includeNormals);

			using (var ms = new MemoryStream(rawChunkData))
			{
				// Act
				var deserializedChunk = new ModelChunk();
				await deserializedChunk.DeserializeAsync(ms, false);

				// Assert
				// For purpose of test, we use a very lax epsilon, since our vectors above are not remotely as precise as the binary representation.
				deserializedChunk.Should().BeEquivalentTo(expectedChunk, opts =>
					opts.Using(new VectorEquivalencyStep<Vector3>(0.0000005f))
						.Using(new VectorEquivalencyStep<Vector2>(0.0000005f)));
				ms.Should().BeEof();
			}
		}

		[Theory]
		[InlineData(true)]
		[InlineData(false)]
		public async Task Given_model_when_serializing_and_deserializing_should_produce_same_model(bool includeNormals)
		{
			ModelChunk chunk = GetPopulatedModelChunk(includeNormals);

			using (var ms = new MemoryStream())
			{
				await chunk.SerializeAsync(ms, false);
				ms.Position = 0;

				// Act
				var deserializedChunk = new ModelChunk();
				await deserializedChunk.DeserializeAsync(ms, false);

				// Assert
				deserializedChunk.Should().BeEquivalentTo(chunk, opts =>
					opts.Using(new VectorEquivalencyStep<Vector3>(float.Epsilon))
						.Using(new VectorEquivalencyStep<Vector2>(float.Epsilon)));
				ms.Should().BeEof();
			}
		}

		[Theory]
		[InlineData("ModelWithNormals.chunkdata")]
		[InlineData("Model.chunkdata")]
		public async Task Given_raw_chunk_data_when_deserializing_and_serializing_should_produce_same_binary_representation(string resourceName)
		{
			byte[] rawChunkData = GetType().Assembly.GetManifestResourceStream(GetType(), resourceName).ToArray();
			var chunk = new ModelChunk();

			using (var ms = new MemoryStream(rawChunkData))
			{
				await chunk.DeserializeAsync(ms, false);
				// Clear out stream.
				ms.SetLength(0);

				// Act
				await chunk.SerializeAsync(ms, false);

				// Assert
				ms.ToArray().Should().BeEquivalentTo(rawChunkData);
			}
		}

		/// <summary>
		/// Returns a bogus model chunk that is similar to the one stored in "Model.chunkdata" and "ModelWithNormals.chunkdata" stream. There is some precision loss in this model however.
		/// </summary>
		private static ModelChunk GetPopulatedModelChunk(bool withNormals = false)
		{
			return new ModelChunk
			{
				Id = 0x2e1c19768305903d,
				MaterialIndices = new byte[16],
				Normals = withNormals ? new[] { new Vector3(0f, -0.5589384f, -0.1626943f), new Vector3(0f, -0.5595705f, 0.1622064f), new Vector3(-0.1624507f, -0.5592543f, -0.0002439159f), new Vector3(0f, 0.0001958847f, 0.1622064f), new Vector3(-0.1624507f, 0.0005121231f, -0.0002439159f), new Vector3(0f, 0.0008279801f, -0.1626943f), new Vector3(0f, -1.261157f, 0.1622065f), new Vector3(-0.1624507f, -1.260841f, -0.0002439159f), new Vector3(0f, -1.260525f, -0.1626943f), new Vector3(0.1624507f, 0.0005121231f, -0.0002439159f), new Vector3(0.1624507f, -0.5592543f, -0.0002439159f), new Vector3(0.1624507f, -1.260841f, -0.0002439159f) } : new Vector3[0],
				TextureCoordinates = new[] { new Vector2(0.005930096f, 0.8524942f), new Vector2(0.03132344f, 0.7459748f), new Vector2(0.005928968f, 0.7459748f), new Vector2(0.03132456f, 0.8524942f), new Vector2(0.0821135f, 0.8524941f), new Vector2(0.1075068f, 0.7459747f), new Vector2(0.08211233f, 0.7459747f), new Vector2(0.1075079f, 0.8524941f), new Vector2(0.004882118f, 0.9978725f), new Vector2(0.03022239f, 0.8587411f), new Vector2(0.004880993f, 0.8587412f), new Vector2(0.03022351f, 0.9978725f), new Vector2(0.0809064f, 0.9978724f), new Vector2(0.1062466f, 0.8587409f), new Vector2(0.08090524f, 0.858741f), new Vector2(0.1062478f, 0.9978724f), new Vector2(0.08420019f, 0.7459748f), new Vector2(0.1095935f, 0.8524942f), new Vector2(0.1095947f, 0.7459748f), new Vector2(0.08419906f, 0.8524942f), new Vector2(0.008016853f, 0.7459747f), new Vector2(0.03341012f, 0.8524941f), new Vector2(0.03341129f, 0.7459747f), new Vector2(0.00801569f, 0.8524941f), new Vector2(0.6750224f, 0.7376471f), new Vector2(0.6560723f, 0.806747f), new Vector2(0.6750224f, 0.8067329f), new Vector2(0.6560723f, 0.7376611f), new Vector2(0.4480654f, 0.009885192f), new Vector2(0.4346656f, 0.07898527f), new Vector2(0.4480654f, 0.07897115f), new Vector2(0.4346656f, 0.009899437f), new Vector2(0.5069904f, 0.4297114f), new Vector2(0.4880401f, 0.5204267f), new Vector2(0.5069904f, 0.5204126f), new Vector2(0.4880401f, 0.4297255f), new Vector2(0.5922109f, 0.4352057f), new Vector2(0.578811f, 0.525921f), new Vector2(0.5922109f, 0.5259069f), new Vector2(0.578811f, 0.4352198f), new Vector2(-5.540406E-08f, -0.2674973f), new Vector2(-0.896256f, 0.1037441f), new Vector2(-0.8962559f, 1.896256f), new Vector2(1.669251E-07f, 2.267497f) },
				TextureIndices = new[]
				{
					new UvMap(1, new ushort[] { 0, 1, 2, 0, 3, 1, 4, 5, 6, 4, 7, 5, 8, 9, 10, 8, 11, 9, 12, 13, 14, 12, 15, 13, 16, 17, 18, 19, 17, 16, 20, 21, 22, 23, 21, 20, 9, 8, 10, 11, 8, 9, 13, 12, 14, 15, 12, 13 }),
					new UvMap(2, new ushort[] { 24, 25, 26, 24, 27, 25, 28, 29, 30, 28, 31, 29, 32, 33, 34, 32, 35, 33, 36, 37, 38, 36, 39, 37, 25, 24, 26, 27, 24, 25, 29, 28, 30, 31, 28, 29, 33, 32, 34, 35, 32, 33, 37, 36, 38, 39, 36, 37 }),
					new UvMap(3, new ushort[] { 40, 41, 40, 40, 41, 41, 42, 43, 42, 42, 43, 43, 40, 41, 40, 40, 41, 41, 42, 43, 42, 42, 43, 43, 41, 40, 40, 41, 40, 41, 43, 42, 42, 43, 42, 43, 41, 40, 40, 41, 40, 41, 43, 42, 42, 43, 42, 43 })
				},
				VertexIndices = new ushort[] { 1, 4, 3, 1, 2, 4, 2, 5, 4, 2, 0, 5, 6, 2, 1, 6, 7, 2, 7, 0, 2, 7, 8, 0, 9, 1, 3, 10, 1, 9, 5, 10, 9, 0, 10, 5, 10, 6, 1, 11, 6, 10, 0, 11, 10, 8, 11, 0 },
				Vertices = new[] { new Vector3(0f, -0.5589384f, -0.1626943f), new Vector3(0f, -0.5595705f, 0.1622064f), new Vector3(-0.1624507f, -0.5592543f, -0.0002439159f), new Vector3(0f, 0.0001958847f, 0.1622064f), new Vector3(-0.1624507f, 0.0005121231f, -0.0002439159f), new Vector3(0f, 0.0008279801f, -0.1626943f), new Vector3(0f, -1.261157f, 0.1622065f), new Vector3(-0.1624507f, -1.260841f, -0.0002439159f), new Vector3(0f, -1.260525f, -0.1626943f), new Vector3(0.1624507f, 0.0005121231f, -0.0002439159f), new Vector3(0.1624507f, -0.5592543f, -0.0002439159f), new Vector3(0.1624507f, -1.260841f, -0.0002439159f) }
			};
		}
	}
}