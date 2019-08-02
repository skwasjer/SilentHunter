using System.IO;
using skwas.IO;

namespace SilentHunter.Dat.Chunks.Partial
{
#if DEBUG
	public sealed class WorldTransformChunk : DatChunk
	{
		private Vector3 _worldTranslation, _worldRotation;

		public Matrix Matrix { get; private set; }

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
			SharpDX.Matrix m = SharpDX.Matrix.RotationX(-_worldRotation.X);
			m *= SharpDX.Matrix.RotationY(-_worldRotation.Y);
			m *= SharpDX.Matrix.RotationZ(-_worldRotation.Z);
			m *= SharpDX.Matrix.Translation(new SharpDX.Vector3(_worldTranslation.X, _worldTranslation.Y, _worldTranslation.Z));

			Matrix = m;
		}

		/// <summary>
		/// Deserializes the chunk.
		/// </summary>
		/// <param name="stream">The stream to read from.</param>
		protected override void Deserialize(Stream stream)
		{
			using (var reader = new BinaryReader(stream, FileEncoding.Default, true))
			{
				_worldTranslation = reader.ReadStruct<Vector3>();
				_worldRotation = reader.ReadStruct<Vector3>();
				UpdateMatrix();
			}
		}

		/// <summary>
		/// Serializes the chunk.
		/// </summary>
		/// <param name="stream">The stream to write to.</param>
		protected override void Serialize(Stream stream)
		{
			using (var writer = new BinaryWriter(stream, FileEncoding.Default, true))
			{
				writer.WriteStruct(_worldTranslation);
				writer.WriteStruct(_worldRotation);
			}
		}
	}
#endif
}