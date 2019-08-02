using System;
using System.Diagnostics;
using System.IO;
using skwas.IO;

namespace SilentHunter.Sdl
{
	[DebuggerDisplay("Name = {Name}, WaveName = {WaveName}")]
	public class SoundInfo : IRawSerializable, ICloneable
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private string _name, _waveName;

		public SoundInfo()
		{
			Volume = 90;
			Pitch = 1;
		}

		protected virtual void OnDeserialize(Stream stream)
		{
			using (var reader = new BinaryReader(stream, FileEncoding.Default, true))
			{
				ushort size = reader.ReadUInt16();
				uint subSize = reader.ReadUInt32();
				if (size != subSize + 4)
				{
					throw new IOException("The file appears invalid. Unexpected size specifier encountered.");
				}

				long currentPos = reader.BaseStream.Position;

				string controllerName = reader.ReadNullTerminatedString();
				if (controllerName != "SoundInfo")
				{
					throw new NotImplementedException();
				}

				Name = (string)DeserializeProperty(reader, "Name", typeof(string));
				WaveName = (string)DeserializeProperty(reader, "WaveName", typeof(string));
				IsFolder = (bool)DeserializeProperty(reader, "IsFolder", typeof(bool));
				Loop = (bool)DeserializeProperty(reader, "Loop", typeof(bool));
				Play = (bool)DeserializeProperty(reader, "Play", typeof(bool));
				Volume = (float)DeserializeProperty(reader, "Volume", typeof(float));
				VolumeVar = (float)DeserializeProperty(reader, "VolumeVar", typeof(float));
				Pitch = (float)DeserializeProperty(reader, "Pitch", typeof(float));
				PitchVar = (float)DeserializeProperty(reader, "PitchVar", typeof(float));
				Is3D = (bool)DeserializeProperty(reader, "Is3D", typeof(bool));
				DopplerFactor = (float)DeserializeProperty(reader, "DopplerFactor", typeof(float));
				MinRadius = (float)DeserializeProperty(reader, "MinRadius", typeof(float));
				MaxRadius = (float)DeserializeProperty(reader, "MaxRadius", typeof(float));
				Category = (SoundCategory)DeserializeProperty(reader, "Category", typeof(SoundCategory));
				Priority = (int)DeserializeProperty(reader, "Priority", typeof(int));
				Delay = (float)DeserializeProperty(reader, "Delay", typeof(float));

				if (reader.BaseStream.Position != currentPos + subSize)
				{
					throw new IOException("The file appears invalid. Unexpected size specifier encountered.");
				}
			}
		}

		protected virtual void OnSerialize(Stream stream)
		{
			using (var writer = new BinaryWriter(stream, FileEncoding.Default, true))
			{
				long currentPos = writer.BaseStream.Position;
				writer.BaseStream.Position += 6;

				writer.Write("SoundInfo", '\0');

				SerializeProperty(writer, "Name", Name);
				SerializeProperty(writer, "WaveName", WaveName);
				SerializeProperty(writer, "IsFolder", IsFolder);
				SerializeProperty(writer, "Loop", Loop);
				SerializeProperty(writer, "Play", Play);
				SerializeProperty(writer, "Volume", Volume);
				SerializeProperty(writer, "VolumeVar", VolumeVar);
				SerializeProperty(writer, "Pitch", Pitch);
				SerializeProperty(writer, "PitchVar", PitchVar);
				SerializeProperty(writer, "Is3D", Is3D);
				SerializeProperty(writer, "DopplerFactor", DopplerFactor);
				SerializeProperty(writer, "MinRadius", MinRadius);
				SerializeProperty(writer, "MaxRadius", MaxRadius);
				SerializeProperty(writer, "Category", Category);
				SerializeProperty(writer, "Priority", Priority);
				SerializeProperty(writer, "Delay", Delay);

				long endPos = writer.BaseStream.Position;
				writer.BaseStream.Position = currentPos;
				writer.Write((ushort)(endPos - currentPos - 2));
				writer.Write((uint)(endPos - currentPos - 6));
				writer.BaseStream.Position = endPos;
			}
		}

		protected virtual object DeserializeProperty(BinaryReader reader, string propertyName, Type propertyType)
		{
			uint size = reader.ReadUInt32();
			long currentPos = reader.BaseStream.Position;
			string name = reader.ReadString(propertyName.Length);
			if (propertyName != name)
			{
				throw new IOException("The file appears invalid. Unexpected property name encountered.");
			}

			// Skip past the terminating null.
			reader.BaseStream.Position++;

			// Read value.
			object value = propertyType == typeof(string)
				? reader.ReadNullTerminatedString()
				: reader.ReadStruct(propertyType);

			if (reader.BaseStream.Position != currentPos + size)
			{
				throw new IOException("The file appears invalid. The data type does not match.");
			}

			return value;
		}

		protected virtual void SerializeProperty(BinaryWriter writer, string propertyName, object value)
		{
			long currentPos = writer.BaseStream.Position;

			// Write size descriptor. We don't know final size yet.
			writer.Write(0);

			// Write name.
			writer.Write(propertyName, '\0');

			// Write value.
			if (value is string str)
			{
				writer.Write(str, '\0');
			}
			else
			{
				writer.WriteStruct(value);
			}

			// Overwrite size descriptor with total size.
			long endPos = writer.BaseStream.Position;
			writer.BaseStream.Position = currentPos;
			writer.Write((int)(endPos - currentPos - 4));
			writer.BaseStream.Position = endPos;
		}

		public string WaveName
		{
			get => _waveName ?? string.Empty;
			set => _waveName = value;
		}

		public string Name
		{
			get => _name ?? string.Empty;
			set => _name = value;
		}

		public bool Is3D { get; set; }
		public bool Play { get; set; }
		public bool Loop { get; set; }
		public bool IsFolder { get; set; }
		public float Delay { get; set; }
		public float MaxRadius { get; set; }
		public float MinRadius { get; set; }
		public float DopplerFactor { get; set; }
		public float PitchVar { get; set; }
		public float Pitch { get; set; }
		public float VolumeVar { get; set; }
		public float Volume { get; set; }
		public int Priority { get; set; }
		public SoundCategory Category { get; set; }

		public object Clone()
		{
			var clone = new SoundInfo();
			using (var ms = new MemoryStream())
			{
				OnSerialize(ms);

				ms.Position = 0;

				((IRawSerializable)clone).Deserialize(ms);
			}

			return clone;
		}

		protected bool Equals(SoundInfo other)
		{
			return string.Equals(_name, other._name) && string.Equals(_waveName, other._waveName) && IsFolder == other.IsFolder && Loop == other.Loop && Play == other.Play && Is3D == other.Is3D && Volume.Equals(other.Volume) && VolumeVar.Equals(other.VolumeVar) && Pitch.Equals(other.Pitch) && PitchVar.Equals(other.PitchVar) && DopplerFactor.Equals(other.DopplerFactor) && MinRadius.Equals(other.MinRadius) && MaxRadius.Equals(other.MaxRadius) && Delay.Equals(other.Delay) && Category == other.Category && Priority == other.Priority;
		}

		/// <summary>
		/// Determines whether the specified object is equal to the current object.
		/// </summary>
		/// <returns>
		/// true if the specified object  is equal to the current object; otherwise, false.
		/// </returns>
		/// <param name="obj">The object to compare with the current object. </param>
		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj))
			{
				return false;
			}

			if (ReferenceEquals(this, obj))
			{
				return true;
			}

			if (obj.GetType() != GetType())
			{
				return false;
			}

			return Equals((SoundInfo)obj);
		}

		/// <summary>
		/// Serves as the default hash function.
		/// </summary>
		/// <returns>
		/// A hash code for the current object.
		/// </returns>
		public override int GetHashCode()
		{
			return 0;
		}

		public static bool operator ==(SoundInfo left, SoundInfo right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(SoundInfo left, SoundInfo right)
		{
			return !Equals(left, right);
		}

		/// <summary>
		/// When implemented, deserializes the implemented class from specified <paramref name="stream" />.
		/// </summary>
		/// <param name="stream">The stream.</param>
		void IRawSerializable.Deserialize(Stream stream)
		{
			OnDeserialize(stream);
		}

		/// <summary>
		/// When implemented, serializes the implemented class to specified <paramref name="stream" />.
		/// </summary>
		/// <param name="stream">The stream.</param>
		void IRawSerializable.Serialize(Stream stream)
		{
			OnSerialize(stream);
		}
	}
}