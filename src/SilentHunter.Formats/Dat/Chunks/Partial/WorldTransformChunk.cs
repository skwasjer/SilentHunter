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

			Matrix = new Matrix
			{
				M11 = m.M11,
				M12 = m.M12,
				M13 = m.M13,
				M14 = m.M14,
				M21 = m.M21,
				M22 = m.M22,
				M23 = m.M23,
				M24 = m.M24,
				M31 = m.M31,
				M32 = m.M32,
				M33 = m.M33,
				M34 = m.M34,
				M41 = m.M41,
				M42 = m.M42,
				M43 = m.M43,
				M44 = m.M44
			};
		}

		/// <summary>
		/// Deserializes the chunk.
		/// </summary>
		/// <param name="stream">The stream to read from.</param>
		protected override void Deserialize(Stream stream)
		{
			using (var reader = new BinaryReader(stream, Encoding.ParseEncoding, true))
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
			using (var writer = new BinaryWriter(stream, Encoding.ParseEncoding, true))
			{
				writer.WriteStruct(_worldTranslation);
				writer.WriteStruct(_worldRotation);
			}
		}
	}
#endif
}