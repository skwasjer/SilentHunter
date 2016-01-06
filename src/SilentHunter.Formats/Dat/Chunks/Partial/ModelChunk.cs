using System;
using System.Collections.Generic;
using System.IO;
using skwas.IO;

namespace SilentHunter.Dat.Chunks.Partial
{
	public sealed class ModelChunk : DatChunk
	{
		public ModelChunk()
			: base(DatFile.Magics.Model)
		{
			Vertices = new Vector3[0];
			TextureCoordinates = new Vector2[0];
			Normals = new Vector3[0];
			VertexIndices = new ushort[0];
			FaceMaterialIndices = new byte[0];

			UnknownData.Add(new UnknownChunkData(0, 0, byte.MinValue, "Some sort of flags, with hints for how to render the model. Thinks like culling? Which channels are supported?"));
		}

		public byte Unknown { get; set; }

		private readonly List<UvMap> _textureIndices = new List<UvMap>();

		public Vector3[] Vertices { get; set; }

		public Vector2[] TextureCoordinates { get; set; }

		public Vector3[] Normals { get; set; }

		public ushort[] VertexIndices { get; set; }

		public UvMap[] TextureIndices
		{
			get { return _textureIndices.ToArray(); }
			set
			{
				_textureIndices.Clear();
				_textureIndices.AddRange(value);
			}
		}

		public byte[] FaceMaterialIndices { get; set; }

		/// <summary>
		/// Gets whether the chunk supports an id field.
		/// </summary>
		public override bool SupportsId => true;

		/// <summary>
		/// Deserializes the chunk.
		/// </summary>
		/// <param name="stream">The stream to read from.</param>
		protected override void Deserialize(Stream stream)
		{
			var regionStream = stream as RegionStream;
			UnknownData.Clear();

			using (var reader = new BinaryReader(stream, Encoding.ParseEncoding, true))
			{
				Id = reader.ReadUInt64();

				UnknownData.Add(new UnknownChunkData(
					regionStream?.BaseStream.Position ?? stream.Position, stream.Position,
					Unknown = reader.ReadByte(),
					"Some sort of flags, with hints for how to render the model. Thinks like culling? Which channels are supported?")
					);

#if DEBUG
				var debugMsg = "Model:\t" + Path.GetFileName(GetBaseStreamName(stream));
				debugMsg += "\t" + UnknownData[0].Data.ToString();
				//Debug.WriteLine(debugMsg);
#endif

				LoadMesh(reader);

				// Loop any remaining data. First 4 bytes per segment are a descriptor.
				while (stream.Position < stream.Length)
				{
					MeshDataDescriptor descr;
					Enum.TryParse(reader.ReadString(4), true, out descr);
					switch (descr)
					{
						case MeshDataDescriptor.TMAP:
							LoadUvMap(reader);
							break;

						case MeshDataDescriptor.NORM:
							LoadNormals(reader);
							break;

						default:
							throw new IOException("Unexpected stream.");
					}
				}
			}
		}

		private void LoadNormals(BinaryReader reader)
		{
			// Read a Vector3 per vertex.
			Normals = new Vector3[Vertices.Length];
			for (var i = 0; i < Normals.Length; i++)
				Normals[i] = reader.ReadStruct<Vector3>();
		}

		private void LoadUvMap(BinaryReader reader)
		{
			// Get map channel count.
			var mapCount = reader.ReadByte();
			for (var i = 0; i < mapCount; i++)
			{
				// Get channel index.
				var uvChannel = reader.ReadByte();

				var textureIndices = new ushort[FaceMaterialIndices.Length*3];
				for (var j = 0; j < textureIndices.Length; j++)
					textureIndices[j] = reader.ReadUInt16();

				_textureIndices.Add(new UvMap
				{
					Channel = uvChannel,
					TextureIndices = textureIndices
				});
			}
		}

		private void LoadMesh(BinaryReader reader)
		{
			// Read vertices.
			Vertices = new Vector3[reader.ReadInt32()];
			for (var i = 0; i < Vertices.Length; i++)
				Vertices[i] = reader.ReadStruct<Vector3>();

			// Read faces, texture indices and material index.
			var trisCount = reader.ReadInt32();
			VertexIndices = new ushort[trisCount*3];
			var textureIndices = new ushort[trisCount*3];
			FaceMaterialIndices = new byte[trisCount];
			for (var i = 0; i < VertexIndices.Length; i += 3)
			{
				VertexIndices[i] = reader.ReadUInt16();
				VertexIndices[i + 1] = reader.ReadUInt16();
				VertexIndices[i + 2] = reader.ReadUInt16();
				textureIndices[i] = reader.ReadUInt16();
				textureIndices[i + 1] = reader.ReadUInt16();
				textureIndices[i + 2] = reader.ReadUInt16();
				FaceMaterialIndices[i/3] = reader.ReadByte();
			}

			_textureIndices.Clear();
			_textureIndices.Add(new UvMap
			{
				Channel = 1,
				TextureIndices = textureIndices
			});

			// Read texture coordinates.
			TextureCoordinates = new Vector2[reader.ReadInt32()];
			for (var i = 0; i < TextureCoordinates.Length; i++)
				TextureCoordinates[i] = reader.ReadStruct<Vector2>();
		}

		/// <summary>
		/// Serializes the chunk.
		/// </summary>
		/// <param name="stream">The stream to write to.</param>
		protected override void Serialize(Stream stream)
		{
			using (var writer = new BinaryWriter(stream, Encoding.ParseEncoding, true))
			{
				writer.Write(Id);

				writer.Write(Unknown);

				writer.Write(Vertices.Length);
				foreach (var vert in Vertices)
					writer.WriteStruct(vert);

				writer.Write(VertexIndices.Length/3);
				for (var i = 0; i < VertexIndices.Length; i += 3)
				{
					writer.Write(VertexIndices[i]);
					writer.Write(VertexIndices[i + 1]);
					writer.Write(VertexIndices[i + 2]);
					writer.Write(_textureIndices[0].TextureIndices[i]);
					writer.Write(_textureIndices[0].TextureIndices[i + 1]);
					writer.Write(_textureIndices[0].TextureIndices[i + 2]);
					writer.Write(FaceMaterialIndices[i/3]);
				}

				writer.Write(TextureCoordinates.Length);
				foreach (var texV in TextureCoordinates)
					writer.WriteStruct(texV);

				// Write TMAP.
				if (_textureIndices.Count > 1)
				{
					writer.Write(MeshDataDescriptor.TMAP.ToString(), false);
					// Write number of channels.
					writer.Write((byte) (_textureIndices.Count - 1));
					// Write indices for all channels except the first (since this channel is saved with SaveMesh).
					for (var i = 1; i < _textureIndices.Count; i++)
					{
						// Write map channel index.
						writer.Write(_textureIndices[i].Channel);
						foreach (var index in _textureIndices[i].TextureIndices)
							writer.Write(index);
					}
				}

				// Write NORM.
				if ((Normals != null) && (Normals.Length > 0))
				{
					writer.Write(MeshDataDescriptor.NORM.ToString(), false);
					foreach (var normal in Normals)
						writer.WriteStruct(normal);
				}
			}
		}
	}
}