using System.IO;
using System.Numerics;
using System.Threading.Tasks;
using skwas.IO;

namespace SilentHunter.FileFormats.Dat.Chunks.Partial
{
#if DEBUG
	public sealed class WorldTransformChunk : DatChunk
	{
		private Vector3 _worldTranslation, _worldRotation;

		public Matrix4x4 Transform { get; private set; }

		public Vector3 Rotation
		{
			get => _worldRotation;
			set
			{
				_worldRotation = value;
				UpdateMatrix();
			}
		}

		public Vector3 Translation
		{
			get => _worldTranslation;
			set
			{
				_worldTranslation = value;
				UpdateMatrix();
			}
		}

		public WorldTransformChunk()
			: base(DatFile.Magics.WorldTransform)
		{
		}

		private void UpdateMatrix()
		{
			Matrix4x4 m = Matrix4x4.CreateRotationX(-_worldRotation.X);
			m *= Matrix4x4.CreateRotationY(-_worldRotation.Y);
			m *= Matrix4x4.CreateRotationZ(-_worldRotation.Z);
			m *= Matrix4x4.CreateTranslation(_worldTranslation);

			Transform = m;
		}

		/// <summary>
		/// Deserializes the chunk.
		/// </summary>
		/// <param name="stream">The stream to read from.</param>
		protected override Task DeserializeAsync(Stream stream)
		{
			using (var reader = new BinaryReader(stream, FileEncoding.Default, true))
			{
				_worldTranslation = reader.ReadStruct<Vector3>();
				_worldRotation = reader.ReadStruct<Vector3>();
				UpdateMatrix();
			}

			return Task.CompletedTask;
		}

		/// <summary>
		/// Serializes the chunk.
		/// </summary>
		/// <param name="stream">The stream to write to.</param>
		protected override Task SerializeAsync(Stream stream)
		{
			using (var writer = new BinaryWriter(stream, FileEncoding.Default, true))
			{
				writer.WriteStruct(_worldTranslation);
				writer.WriteStruct(_worldRotation);
			}

			return Task.CompletedTask;
		}
	}
#endif
}