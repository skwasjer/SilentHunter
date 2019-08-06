using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Numerics;
using System.Threading.Tasks;
using SilentHunter.FileFormats.Extensions;
using SilentHunter.FileFormats.IO;

namespace SilentHunter.FileFormats.Dat.Chunks.Partial
{
	/// <summary>
	/// The node chunk.
	/// </summary>
	public sealed class NodeChunk : DatChunk
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="NodeChunk"/>.
		/// </summary>
		public NodeChunk()
			: base(DatFile.Magics.Node)
		{
			Visible = true;
			PushUnknownData(0, 0, byte.MinValue, "The byte just before the 'Visibility' byte. No idea what it means. Values found: 1, 2, 128, 64, possibly others.");
			Transform = Matrix4x4.Identity;
		}

		private Vector3 _translation, _rotation;

		// Sub type 2: Light
		private Light _light;

		// Sub type 3: ??
		private Interior _interior;

		/// <summary>
		/// Gets light(ing) info.
		/// </summary>
		public Light Light
		{
			get
			{
				if (SubType != 2)
				{
					return _light = null;
				}

				return _light ?? (_light = new Light());
			}
			private set
			{
				if (SubType != 2)
				{
					throw new ArgumentException("This object is not valid for sub type (2).", nameof(value));
				}

				_light = value ?? new Light();
			}
		}

		/// <summary>
		/// Gets interior (root node) info.
		/// </summary>
		public Interior Interior
		{
			get
			{
				if (SubType != 3)
				{
					return _interior = null;
				}

				return _interior ?? (_interior = new Interior());
			}
			private set
			{
				if (SubType != 3)
				{
					throw new ArgumentException("This object is not valid for sub type (3).", nameof(value));
				}

				_interior = value ?? new Interior();
			}
		}

		/// <summary>
		/// Gets the model id.
		/// </summary>
		public ulong ModelId { get; set; }

		/// <summary>
		/// Gets a list of material ids.
		/// </summary>
		public List<ulong> Materials { get; } = new List<ulong>();

		/// <summary>
		/// Gets or sets whether the node (and/or attached behaviors) is visible in game.
		/// </summary>
		public bool Visible { get; set; }

		/// <summary>
		/// Gets or sets the translation of the node.
		/// </summary>
		public Vector3 Translation
		{
			get => _translation;
			set
			{
				if (_translation.Equals(value))
				{
					return;
				}

				_translation = value;
				UpdateTransform();
			}
		}

		/// <summary>
		/// Gets or sets the rotation of the node.
		/// </summary>
		public Vector3 Rotation
		{
			get => _rotation;
			set
			{
				if (_rotation.Equals(value))
				{
					return;
				}

				_rotation = value;
				UpdateTransform();
			}
		}

		private void UpdateTransform()
		{
			Matrix4x4 m = Matrix4x4.CreateRotationX(-_rotation.X);
			m *= Matrix4x4.CreateRotationY(-_rotation.Y);
			m *= Matrix4x4.CreateRotationZ(-_rotation.Z);
			m *= Matrix4x4.CreateTranslation(_translation);

			Transform = m;
		}

		/// <summary>
		/// Gets the transform based on <see cref="Rotation"/> and <see cref="Translation"/>.
		/// </summary>
		public Matrix4x4 Transform { get; private set; }

		/// <summary>
		/// Gets whether the chunk supports an id field.
		/// </summary>
		public override bool SupportsId => true;

		/// <summary>
		/// Gets whether the chunk supports a parent id field.
		/// </summary>
		public override bool SupportsParentId => true;

		/// <inheritdoc />
		protected override Task DeserializeAsync(Stream stream)
		{
			var regionStream = stream as RegionStream;

			UnknownData.Clear();

			using (var reader = new BinaryReader(stream, FileEncoding.Default, true))
			{
				Id = reader.ReadUInt64();
				ParentId = reader.ReadUInt64();
				ModelId = reader.ReadUInt64();

				ReadUnknownData(reader, r => r.ReadByte(), "The byte just before the 'Visibility' byte. No idea what it means. Values found: 1, 2, 128, 64, possibly others.");

#if DEBUG
				// Data\Library\Characters\SECONDARY.dat
				// 	Unknown1: 1
				// Data\Library\AntiSubNet.dat
				// 	Unknown1: 2
				// Data\Library\NavalMine.dat
				// 	Unknown1: 2
				// Data\Library\EventCamera.dat
				// 	Unknown1: 128
				// 	Unknown1: 128
				// 	Unknown1: 128
				// 	Unknown1: 128
				// Data\scene.dat
				// 	Unknown1: 64
				// 	Unknown1: 64
#endif

				Visible = reader.ReadByte() > 0;
				Translation = reader.ReadStruct<Vector3>();
				Rotation = reader.ReadStruct<Vector3>();

				Materials.Clear();

				// Read material id's that apply to this node.
				int count = reader.ReadInt32();
				if (count > 0) // If 0, then we are at end of chunk...
				{
					for (int i = 0; i < count; i++)
					{
						// Read material id...
						Materials.Add(reader.ReadUInt64());
					}
				}
				else
				{
					// No materials linked. We should now have a uint64 of 0.
					ulong expectZero = reader.ReadUInt64();
					// However, some mods are broken in that they do list a material id here (even though count was 0).
					// To fix this problem we simply store the material, upon next save it will be serialized correctly.
					if (expectZero != 0)
					{
						Materials.Add(expectZero);
					}
				}

				long basePos = regionStream?.BaseStream.Position ?? stream.Position;
				long curPos = stream.Position;

				switch (SubType)
				{
					// Interiors. Light...
					case 2:
						// 20 bytes, or 16 bytes depending on light type.

						var light = new Light
						{
							Reserved0 = reader.ReadUInt32(),

							Type = reader.ReadStruct<LightType>(),

							// Ignore alpha component.
							Color = Color.FromArgb(byte.MaxValue, reader.ReadStruct<Color>()),
							Attenuation = reader.ReadSingle(),

						};

						// Omni lights have an additional float value.
						light.Radius = _light.Type == LightType.Omni ? reader.ReadSingle() : 0;

						Light = light;

						break;

					case 3:
						// Interior node.
						Interior = reader.ReadStruct<Interior>();

						Debug.Assert(_interior.Reserved0 == 0, "Expected 0.");
						Debug.Assert(_interior.Reserved1 == byte.MinValue, "Expected 0.");
						Debug.Assert(_interior.Reserved2 == 0, "Expected 0.");

						break;

					case 100:
						// Some chunks with this sub type seem to have a size specifier, followed by an int-array of data. The 'size' seems to match the number of vertices of a linked model, and is related to animation.
						int size = reader.ReadInt32();
						if (size > 0)
						{
							PushUnknownData(basePos,
								curPos,
								size,
								"Array size specifier. Some chunks with subtype 100 seem to have this size specifier, followed by an int-array of data. The 'size' seems to match the number of vertices of a linked model, and looks to be related to animation. In most cases though, this specifier is just 0 and the final data of the chunk.");

							var remainingIntData = new int[size];
							for (int i = 0; i < size; i++)
							{
								remainingIntData[i] = reader.ReadInt32();
							}

							PushUnknownData(basePos + 4,
								curPos + 4,
								remainingIntData,
								"Array. Some chunks with subtype 100 seem to have this array of ints.  The number of items seems to match the number of vertices of a linked model, and looks to be related to animation.");
						}

						if (stream.Length > stream.Position)
							// GWX error? Still data remaining in some files. As far as I know, this is not allowed/used by the game. Ignore the data.
						{
							stream.Position = stream.Length;
						}

						break;
				}
			}

			return Task.CompletedTask;
		}

		/// <inheritdoc />
		protected override Task SerializeAsync(Stream stream)
		{
			using (var writer = new BinaryWriter(stream, FileEncoding.Default, true))
			{
				writer.Write(Id);
				writer.Write(ParentId);
				writer.Write(ModelId);

				writer.WriteStruct(UnknownData[0].Data);
				writer.Write(Visible ? (byte)1 : byte.MinValue);

				writer.WriteStruct(_translation);
				writer.WriteStruct(_rotation);

				writer.Write(Materials.Count);
				if (Materials.Count > 0)
				{
					foreach (ulong matId in Materials)
					{
						writer.Write(matId);
					}
				}
				else
				{
					writer.Write(ulong.MinValue);
				}

				switch (SubType)
				{
					// Interiors. Light...
					case 2:
						writer.Write(Light.Reserved0);

						writer.WriteStruct(Light.Type);
						// Ignore alpha component.
						writer.WriteStruct(Color.FromArgb(byte.MinValue, Light.Color));
						writer.Write(Light.Attenuation);
						if (Light.Type == LightType.Omni)
						{
							writer.Write(Light.Radius);
						}

						break;

					// Interior first node... ?
					case 3:
						writer.WriteStruct(Interior);
						break;

					case 100:
						if (UnknownData.Count > 1)
						{
							var remainingIntData = (int[])UnknownData[2].Data;
							writer.Write(remainingIntData.Length);
							foreach (int t in remainingIntData)
							{
								writer.Write(t);
							}
						}
						else
						{
							writer.Write(0);
						}

						break;
				}
			}

			return Task.CompletedTask;
		}
	}
}