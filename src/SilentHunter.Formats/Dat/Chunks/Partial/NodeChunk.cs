using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using skwas.IO;

namespace SilentHunter.Dat.Chunks.Partial
{
	public sealed class NodeChunk : DatChunk
	{
		public NodeChunk()
			: base(DatFile.Magics.Node)
		{
			Visible = true;
			UnknownData.Add(new UnknownChunkData(0, 0, byte.MinValue, "The byte just before the 'Visibility' byte. No idea what it means. Values found: 1, 2, 128, 64, possibly others."));
			_transform = Matrix.Identity;
		}

		private Vector3 _translation, _rotation;
		private Matrix _transform;

		// Sub type 2: Light
		private Light _light;

		// Sub type 3: ??
		private Interior _interior;

		public Light Light
		{
			get
			{
				if (SubType != 2)
					throw new NotSupportedException("This object is not valid for sub type (2).");
				return _light ?? (_light = new Light());
			}
		}

		public Interior Interior
		{
			get
			{
				if (SubType != 3)
					throw new NotSupportedException("This object is not valid for sub type (3).");
				return _interior ?? (_interior = new Interior());
			}
		}
		
		public ulong ModelId { get; set; }

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
			get { return _translation; }
			set
			{
				if (_translation.Equals(value))
					return;
				_translation = value;
				UpdateTransform();
			}
		}

		/// <summary>
		/// Gets or sets the rotation of the node.
		/// </summary>
		public Vector3 Rotation
		{
			get { return _rotation; }
			set
			{
				if (_rotation.Equals(value))
					return;
				_rotation = value;
				UpdateTransform();
			}
		}

		private void UpdateTransform()
		{
			var m = Matrix.RotationX(-_rotation.X);
			m *= Matrix.RotationY(-_rotation.Y);
			m *= Matrix.RotationZ(-_rotation.Z);
			m *= Matrix.Translation(_translation);

			_transform = m;
		}

		public Matrix Transform => _transform;

		/// <summary>
		/// Gets whether the chunk supports an id field.
		/// </summary>
		public override bool SupportsId => true;

		/// <summary>
		/// Gets whether the chunk supports a parent id field.
		/// </summary>
		public override bool SupportsParentId => true;

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
				ParentId = reader.ReadUInt64();
				ModelId = reader.ReadUInt64();

				UnknownData.Add(new UnknownChunkData(regionStream?.BaseStream.Position ?? stream.Position, stream.Position,
					reader.ReadByte(),
					"The byte just before the 'Visibility' byte. No idea what it means. Values found: 1, 2, 128, 64, possibly others."));

#if DEBUG
				//				if ((byte)UnknownData[0].Data != byte.MinValue)
				//					Debug.WriteLine(GetBaseStreamName(stream) + "\r\n\tNodeLink-Unknown1: " + UnknownData[0].Data);

				// Data\Library\Characters\SECONDARY.dat
				// 	NodeLink-Unknown1: 1
				// Data\Library\AntiSubNet.dat
				// 	NodeLink-Unknown1: 2
				// Data\Library\NavalMine.dat
				// 	NodeLink-Unknown1: 2
				// Data\Library\EventCamera.dat
				// 	NodeLink-Unknown1: 128
				// 	NodeLink-Unknown1: 128
				// 	NodeLink-Unknown1: 128
				// 	NodeLink-Unknown1: 128
				// Data\scene.dat
				// 	NodeLink-Unknown1: 64
				// 	NodeLink-Unknown1: 64
#endif

				Visible = reader.ReadByte() > 0;
				Translation = reader.ReadStruct<Vector3>();
				Rotation = reader.ReadStruct<Vector3>();

				Materials.Clear();

				// Read material id's that apply to this node.
				var count = reader.ReadInt32();
				if (count > 0) // If 0, then we are at end of chunk...
				{
					for (var i = 0; i < count; i++)
					{
						// Read material id...
						Materials.Add(reader.ReadUInt64());
					}
				}
				else
				{
					// No materials linked. We should now have a uint64 of 0.
					var expectZero = reader.ReadUInt64();
					// However, some mods are broken in that they do list a material id here (even though count was 0).
					// To fix this problem we simply store the material, upon next save it will be serialized correctly.
					if (expectZero != 0)
						Materials.Add(expectZero);
				}

				var basePos = regionStream?.BaseStream.Position ?? stream.Position;
				var curPos = stream.Position;

				switch (SubType)
				{
					// Interiors. Light...
					case 2:
						// 20 bytes, or 16 bytes depending on light type.

						// Read a 0.
						Light.Reserved0 = reader.ReadUInt32();

						_light.Type = (LightType) reader.ReadInt32();

						// Ignore alpha component.
						_light.Color = Color.FromArgb(byte.MaxValue, reader.ReadStruct<Color>());
						_light.Attenuation = reader.ReadSingle();

						// Omni lights have an additional float value.
						_light.Radius = _light.Type == LightType.Omni ? reader.ReadSingle() : 0;

						break;

					case 3:
						// Interior node.
						_interior = reader.ReadStruct<Interior>();

						Debug.Assert(_interior.Reserved0 == 0, "Expected 0.");
						Debug.Assert(_interior.Reserved1 == byte.MinValue, "Expected 0.");
						Debug.Assert(_interior.Reserved2 == 0, "Expected 0.");

						break;

					case 100:
						// Some chunks with this sub type seem to have a size specifier, followed by an int-array of data. The 'size' seems to match the number of vertices of a linked model, and is related to animation.
						var size = reader.ReadInt32();
						if (size > 0)
						{
							UnknownData.Add(new UnknownChunkData(basePos, curPos, size,
								"Array size specifier. Some chunks with subtype 100 seem to have this size specifier, followed by an int-array of data. The 'size' seems to match the number of vertices of a linked model, and looks to be related to animation. In most cases though, this specifier is just 0 and the final data of the chunk."));

							var remainingIntData = new int[size];
							for (var i = 0; i < size; i++)
								remainingIntData[i] = reader.ReadInt32();

							UnknownData.Add(new UnknownChunkData(basePos + 4, curPos + 4, remainingIntData,
								"Array. Some chunks with subtype 100 seem to have this array of ints.  The number of items seems to match the number of vertices of a linked model, and looks to be related to animation."));
						}

						if (stream.Length > stream.Position)
							// GWX error? Still data remaining in some files. As far as I know, this is not allowed/used by the game. Ignore the data.
							stream.Position = stream.Length;

						break;
				}
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
					foreach (var matId in Materials)
						writer.Write(matId);
				}
				else
					writer.Write(ulong.MinValue);

				switch (SubType)
				{
					// Interiors. Light...
					case 2:
						writer.Write(Light.Reserved0);

						writer.Write((int) Light.Type);
						// Ignore alpha component.
						writer.WriteStruct(Color.FromArgb(byte.MinValue, Light.Color));
						writer.Write(Light.Attenuation);
						if (Light.Type == LightType.Omni)
							writer.WriteStruct(Light.Radius);
						break;

					// Interior first node... ?
					case 3:
						writer.WriteStruct(Interior);
						break;

					case 100:
						if (UnknownData.Count > 1)
						{
							var remainingIntData = (int[]) UnknownData[2].Data;
							writer.Write(remainingIntData.Length);
							foreach (var t in remainingIntData)
								writer.Write(t);
						}
						else
							writer.Write(0);
						break;
				}
			}
		}
	}
}
