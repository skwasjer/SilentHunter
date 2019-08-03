using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using SilentHunter.FileFormats.Extensions;

namespace SilentHunter.FileFormats.Dat.Chunks
{
	/// <summary>
	/// 
	/// </summary>
	/// <remarks>
	/// See https://www.subsim.com/radioroom/showthread.php?p=1177807#post1177807
	///
	/// While I believe poster is on the right track, I believe its bitwise flags instead. However, the names may not be accurate.
	/// </remarks>
	[Flags]
	public enum ModelType : byte
	{
		None = 0,
		Animation = 0x2,
		Model = 0x4
	}

	public sealed class ModelChunk : DatChunk
	{
		public ModelChunk()
			: base(DatFile.Magics.Model)
		{
			Type = ModelType.Animation | ModelType.Model;
			Vertices = new Vector3[0];
			TextureCoordinates = new Vector2[0];
			Normals = new Vector3[0];
			VertexIndices = new ushort[0];
			MaterialIndices = new byte[0];
		}

		public ModelType Type { get; set; }

		public Vector3[] Vertices { get; set; }

		public Vector2[] TextureCoordinates { get; set; }

		public Vector3[] Normals { get; set; }

		public ushort[] VertexIndices { get; set; }

		public UvMap[] TextureIndices { get; set; }

		public byte[] MaterialIndices { get; set; }

		/// <summary>
		/// Gets whether the chunk supports an id field.
		/// </summary>
		public override bool SupportsId => true;

		/// <summary>
		/// Deserializes the chunk.
		/// </summary>
		/// <param name="stream">The stream to read from.</param>
		protected override Task DeserializeAsync(Stream stream)
		{
			using (var reader = new BinaryReader(stream, FileEncoding.Default, true))
			{
				Id = reader.ReadUInt64();

				Type = reader.ReadStruct<ModelType>();

				LoadMesh(reader, out Vector3[] vertices, out ushort[] vertexIndices, out ushort[] textureIndices, out byte[] materialIndices);
				Vertices = vertices;
				VertexIndices = vertexIndices;
				TextureIndices = new[]
				{
					new UvMap
					{
						Channel = 1,
						TextureIndices = textureIndices
					}
				};
				MaterialIndices = materialIndices;

				TextureCoordinates = ReadTextureCoordinates(reader).ToArray();

				// Loop any remaining data. First 4 bytes per segment are a descriptor.
				while (stream.Position < stream.Length)
				{
					Enum.TryParse(reader.ReadString(4), true, out MeshDataDescriptor descriptor);
					switch (descriptor)
					{
						case MeshDataDescriptor.TMAP:
							TextureIndices = TextureIndices.Concat(ReadUvMaps(reader, VertexIndices.Length)).ToArray();
							break;

						case MeshDataDescriptor.NORM:
							Normals = ReadNormals(reader, Vertices.Length).ToArray();
							break;

						default:
							throw new SilentHunterParserException($"Unexpected descriptor '{descriptor}'.");
					}
				}
			}

			return Task.CompletedTask;
		}

		private static void LoadMesh(
			BinaryReader reader,
			out Vector3[] vertices,
			out ushort[] vertexIndices,
			out ushort[] textureIndices,
			out byte[] materialIndices)
		{
			// Read vertices.
			int vertexCount = reader.ReadInt32();
			vertices = new Vector3[vertexCount];
			for (int i = 0; i < vertices.Length; i++)
			{
				vertices[i] = reader.ReadStruct<Vector3>();
			}

			// Read faces, texture indices and material index.
			int faceCount = reader.ReadInt32();
			int vertexIndexCount = faceCount * 3;
			vertexIndices = new ushort[vertexIndexCount];
			textureIndices = new ushort[vertexIndexCount];
			materialIndices = new byte[faceCount];
			for (int i = 0; i < vertexIndices.Length; i += 3)
			{
				vertexIndices[i] = reader.ReadUInt16();
				vertexIndices[i + 1] = reader.ReadUInt16();
				vertexIndices[i + 2] = reader.ReadUInt16();
				textureIndices[i] = reader.ReadUInt16();
				textureIndices[i + 1] = reader.ReadUInt16();
				textureIndices[i + 2] = reader.ReadUInt16();
				materialIndices[i / 3] = reader.ReadByte();
			}
		}

		private static IEnumerable<Vector2> ReadTextureCoordinates(BinaryReader reader)
		{
			// Read texture coordinates.
			var tc = new Vector2[reader.ReadInt32()];
			for (int i = 0; i < tc.Length; i++)
			{
				tc[i] = reader.ReadStruct<Vector2>();
			}

			return tc;
		}

		private static IEnumerable<UvMap> ReadUvMaps(BinaryReader reader, int vertexCount)
		{
			ushort[] GetTextureIndices()
			{
				var textureIndices = new ushort[vertexCount];
				for (int i = 0; i < vertexCount; i++)
				{
					textureIndices[i] = reader.ReadUInt16();
				}

				return textureIndices;
			}

			// Get map channel count.
			byte mapCount = reader.ReadByte();
			for (int i = 0; i < mapCount; i++)
			{
				yield return new UvMap
				{
					Channel = reader.ReadByte(),
					TextureIndices = GetTextureIndices()
				};
			}
		}

		private static IEnumerable<Vector3> ReadNormals(BinaryReader reader, int vertexCount)
		{
			// Read a Vector3 per vertex.
			var verts = new Vector3[vertexCount];
			for (int i = 0; i < vertexCount; i++)
			{
				verts[i] = reader.ReadStruct<Vector3>();
			}

			return verts;
		}

		/// <summary>
		/// Serializes the chunk.
		/// </summary>
		/// <param name="stream">The stream to write to.</param>
		protected override Task SerializeAsync(Stream stream)
		{
			using (var writer = new BinaryWriter(stream, FileEncoding.Default, true))
			{
				writer.Write(Id);

				writer.WriteStruct(Type);

				WriteMesh(writer, Vertices, VertexIndices, TextureIndices[0].TextureIndices, MaterialIndices);

				WriteTextureCoordinates(writer, TextureCoordinates);

				WriteUvMaps(writer, TextureIndices);

				WriteNormals(writer, Normals);
			}

			return Task.CompletedTask;
		}

		private static void WriteMesh(
			BinaryWriter writer,
			IReadOnlyCollection<Vector3> vertices,
			IReadOnlyList<ushort> vertexIndices,
			IReadOnlyList<ushort> textureIndices,
			IReadOnlyList<byte> materialIndices)
		{
			writer.Write(vertices.Count);
			foreach (Vector3 vector in vertices)
			{
				writer.WriteStruct(vector);
			}

			int faceCount = vertexIndices.Count / 3;
			writer.Write(faceCount);
			for (int i = 0; i < vertexIndices.Count; i += 3)
			{
				writer.Write(vertexIndices[i]);
				writer.Write(vertexIndices[i + 1]);
				writer.Write(vertexIndices[i + 2]);
				writer.Write(textureIndices[i]);
				writer.Write(textureIndices[i + 1]);
				writer.Write(textureIndices[i + 2]);
				writer.Write(materialIndices[i / 3]);
			}
		}

		private static void WriteTextureCoordinates(BinaryWriter writer, IReadOnlyCollection<Vector2> textureCoordinates)
		{
			writer.Write(textureCoordinates.Count);
			foreach (Vector2 texV in textureCoordinates)
			{
				writer.WriteStruct(texV);
			}
		}

		private static void WriteUvMaps(BinaryWriter writer, IReadOnlyList<UvMap> textureIndices)
		{
			if (textureIndices.Count <= 1)
			{
				return;
			}

			writer.Write(MeshDataDescriptor.TMAP.ToString(), false);

			// Write number of channels.
			writer.Write((byte)(textureIndices.Count - 1));

			// Write indices for all channels except the first (since this channel is saved with SaveMesh).
			for (int i = 1; i < textureIndices.Count; i++)
			{
				// Write map channel index.
				writer.Write(textureIndices[i].Channel);
				foreach (ushort index in textureIndices[i].TextureIndices)
				{
					writer.Write(index);
				}
			}
		}

		private static void WriteNormals(BinaryWriter writer, IReadOnlyList<Vector3> normals)
		{
			if (normals == null || normals.Count <= 0)
			{
				return;
			}

			writer.Write(MeshDataDescriptor.NORM.ToString(), false);
			foreach (Vector3 normal in normals)
			{
				writer.WriteStruct(normal);
			}
		}
	}
}