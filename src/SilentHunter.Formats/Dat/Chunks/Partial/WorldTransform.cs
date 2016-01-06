using System.IO;
using skwas.IO;

namespace SilentHunter.Dat.Chunks.Partial
{
#if DEBUG
	public sealed class WorldTransform : DatChunk
	{
		private Matrix _matrix;
		private Vector3 _worldTranslation, _worldRotation;

		//public Matrix Matrix
		//{
		//	get { return _matrix; }
		//}

		public Vector3 Rotation
		{
			get { return _worldRotation; }
			set
			{
				_worldRotation = value;
				UpdateMatrix();
			}
		}

		public Vector3 Translation
		{
			get { return _worldTranslation; }
			set
			{
				_worldTranslation = value;
				UpdateMatrix();
			}
		}

		public WorldTransform()
			: base(DatFile.Magics.WorldTransform)
		{
		}


		private void UpdateMatrix()
		{
			var m = SlimDX.Matrix.Identity;

			m *= SlimDX.Matrix.RotationX(-_worldRotation.X);
			m *= SlimDX.Matrix.RotationY(-_worldRotation.Y);
			m *= SlimDX.Matrix.RotationZ(-_worldRotation.Z);
			m *= SlimDX.Matrix.Translation(_worldTranslation);

			_matrix = SlimDX.Matrix.Invert(m);
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
				//UpdateMatrix();
//			base.OnDeserialize(stream);
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
//			base.OnSerialize(stream);
			}
		}
	}
#endif
}
