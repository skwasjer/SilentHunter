using System.IO;
using System.Numerics;
using System.Threading.Tasks;
using SilentHunter.FileFormats.Extensions;

namespace SilentHunter.FileFormats.Dat.Chunks.Partial
{
#if DEBUG
	/// <summary>
	/// 
	/// </summary>
	/// <remarks>
	/// Probably not the best name for this chunk. Also, not even sure if its actually what it is.
	/// </remarks>
	public sealed class WorldTransformChunk : DatChunk
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="WorldTransformChunk"/>.
		/// </summary>
		public WorldTransformChunk()
			: base(DatFile.Magics.WorldTransform)
		{
		}

		private Vector3 _worldTranslation, _worldRotation;

		/// <summary>
		/// Gets the transform based on <see cref="Rotation"/> and <see cref="Translation"/>.
		/// </summary>
		public Matrix4x4 Transform { get; private set; }

		/// <summary>
		/// Gets or sets the world rotation?
		/// </summary>
		public Vector3 Rotation
		{
			get => _worldRotation;
			set
			{
				_worldRotation = value;
				UpdateMatrix();
			}
		}

		/// <summary>
		/// Gets or sets the world translation?
		/// </summary>
		public Vector3 Translation
		{
			get => _worldTranslation;
			set
			{
				_worldTranslation = value;
				UpdateMatrix();
			}
		}

		private void UpdateMatrix()
		{
			Matrix4x4 m = Matrix4x4.CreateRotationX(-_worldRotation.X);
			m *= Matrix4x4.CreateRotationY(-_worldRotation.Y);
			m *= Matrix4x4.CreateRotationZ(-_worldRotation.Z);
			m *= Matrix4x4.CreateTranslation(_worldTranslation);

			Transform = m;
		}

		/// <inheritdoc />
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

		/// <inheritdoc />
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