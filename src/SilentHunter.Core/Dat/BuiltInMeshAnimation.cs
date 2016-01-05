using System.IO;
using System.Collections.Generic;

namespace SilentHunter.Dat
{
	public abstract class BuiltInMeshAnimation
		: IRawController
	{
		// NOTE: Only Frames is currently a field. S3D only uses fields currently in the property editor to distinguish if it's an editable value. The CompressedVertices property is hidden in S3D because it's a property and not a field.

		/// <summary>
		/// A list of key frames. Each frame describes which set of compressed vertices to use at which time. The 3D engine will have to perform vertex interpolation to smooth out the animation in between two frames.
		/// </summary>
		public List<AnimationKeyFrame> Frames;
		/// <summary>
		/// A list of compressed vertices. Each set of compressed vertices replaces the vertices of the source mesh for a given key frame and can used/referenced once or multiple times.
		/// </summary>
		public List<CompressedVertices> CompressedVertices { get; } = new List<CompressedVertices>();

		/// <summary>
		/// Extra unknown data (only found on chunk related to head morphing I believe).
		/// </summary>
		private byte[] UnsupportedData { get; set; }

		/// <summary>
		/// When implemented, deserializes the controller from specified <paramref name="stream"/>.
		/// </summary>
		/// <param name="stream">The stream.</param>
		void IRawController.Deserialize(Stream stream)
		{
			using (var reader = new BinaryReader(stream, Encoding.ParseEncoding, true))
			{
				// Read n frames.
				int frameCount;
				Frames = new List<AnimationKeyFrame>(frameCount = reader.ReadUInt16());
				for (var i = 0; i < frameCount; i++)
					Frames.Add(new AnimationKeyFrame
					{
						Time = reader.ReadSingle(),						
						FrameNumber = reader.ReadUInt16()
					});

				// Get the number of transforms and vertices.
				var meshTransformCount = reader.ReadInt32();
				var vertexCount = reader.ReadInt32();

				CompressedVertices.Clear();
				for (var i = 0; i < meshTransformCount; i++)
				{
					var meshTransform = new CompressedVertices
					{
						Scale = reader.ReadSingle(),
						Translation = reader.ReadSingle(),
						Vertices = new List<short>(vertexCount)
					};

					for (var j = 0; j < vertexCount; j++)
						meshTransform.Vertices.Add(reader.ReadInt16());

					CompressedVertices.Add(meshTransform);
				}

				if (stream.Position != stream.Length)
				{					
					UnsupportedData = reader.ReadBytes((int) (stream.Length - stream.Position));
				}
			}
		}

		/// <summary>
		/// When implemented, serializes the controller to specified <paramref name="stream"/>.
		/// </summary>
		/// <param name="stream">The stream.</param>
		void IRawController.Serialize(Stream stream)
		{
			using (var writer = new BinaryWriter(stream, Encoding.ParseEncoding, true))
			{
				// Write frames.
				writer.Write((ushort) Frames.Count);
				foreach (var frame in Frames)
				{
					writer.Write(frame.Time);
					writer.Write(frame.FrameNumber);
				}

				// Write mesh transforms.
				writer.Write(CompressedVertices.Count);
				writer.Write(CompressedVertices.Count > 0 ? CompressedVertices[0].Vertices.Count : 0);
				if (CompressedVertices.Count > 0)
				{
					foreach (var meshTransform in CompressedVertices)
					{
						writer.Write(meshTransform.Scale);
						writer.Write(meshTransform.Translation);

						foreach (var i in meshTransform.Vertices)
							writer.Write(i);
					}
				}

				if (UnsupportedData != null && UnsupportedData.Length > 0)
					writer.Write(UnsupportedData, 0, UnsupportedData.Length);
			}
		}
	}
}
