using System.Collections.Generic;
using System.IO;
using skwas.IO;

namespace SilentHunter.Dat
{
	public abstract class MeshAnimationController : AnimationController, IRawSerializable
	{
		// NOTE: Only Frames is currently a field. S3D only uses fields currently in the property editor to distinguish if it's an editable value. The CompressedFrames property is hidden in S3D because it's a property and not a field.

		/// <summary>
		/// A list of key frames. Each frame describes which set of compressed vertices to use at which time. The 3D engine will have to perform vertex interpolation to smooth out the animation in between two frames.
		/// </summary>
		public List<AnimationKeyFrame> Frames;

		/// <summary>
		/// A list of compressed vertices. Each set of compressed vertices replaces the vertices of the source mesh for a given key frame and can used/referenced once or multiple times.
		/// </summary>
		public List<CompressedVertices> CompressedFrames { get; } = new List<CompressedVertices>();

		/// <summary>
		/// Extra unknown data (only found on chunk related to head morphing I believe).
		/// </summary>
		private byte[] UnsupportedData { get; set; }

		/// <summary>
		/// When implemented, deserializes the controller from specified <paramref name="stream" />.
		/// </summary>
		/// <param name="stream">The stream.</param>
		void IRawSerializable.Deserialize(Stream stream)
		{
			using (var reader = new BinaryReader(stream, Encoding.ParseEncoding, true))
			{
				// Read n frames.
				int frameCount;
				Frames = new List<AnimationKeyFrame>(frameCount = reader.ReadUInt16());
				for (var i = 0; i < frameCount; i++)
				{
					Frames.Add(new AnimationKeyFrame
					{
						Time = reader.ReadSingle(),
						FrameNumber = reader.ReadUInt16()
					});
				}

				// Get the number of compressed frames and vertices.
				int compressedFrameCount = reader.ReadInt32();
				int vertexCount = reader.ReadInt32();

				CompressedFrames.Clear();
				for (var i = 0; i < compressedFrameCount; i++)
				{
					var cv = new CompressedVertices
					{
						Scale = reader.ReadSingle(),
						Translation = reader.ReadSingle(),
						Vertices = new List<short>(vertexCount)
					};

					for (var j = 0; j < vertexCount; j++)
					{
						cv.Vertices.Add(reader.ReadInt16());
					}

					CompressedFrames.Add(cv);
				}

				if (stream.Position != stream.Length)
				{
					UnsupportedData = reader.ReadBytes((int)(stream.Length - stream.Position));
				}
			}
		}

		/// <summary>
		/// When implemented, serializes the controller to specified <paramref name="stream" />.
		/// </summary>
		/// <param name="stream">The stream.</param>
		void IRawSerializable.Serialize(Stream stream)
		{
			using (var writer = new BinaryWriter(stream, Encoding.ParseEncoding, true))
			{
				// Write frames.
				writer.Write((ushort)Frames.Count);
				foreach (AnimationKeyFrame frame in Frames)
				{
					writer.Write(frame.Time);
					writer.Write(frame.FrameNumber);
				}

				// Write mesh transforms.
				writer.Write(CompressedFrames.Count);

				// Each frame should be same size, so if we have a frame, use vertex count of first.
				writer.Write(CompressedFrames.Count > 0 ? CompressedFrames[0].Vertices.Count : 0);
				foreach (CompressedVertices cv in CompressedFrames)
				{
					writer.Write(cv.Scale);
					writer.Write(cv.Translation);

					foreach (short i in cv.Vertices)
					{
						writer.Write(i);
					}
				}

				if (UnsupportedData != null && UnsupportedData.Length > 0)
				{
					writer.Write(UnsupportedData, 0, UnsupportedData.Length);
				}
			}
		}
	}
}