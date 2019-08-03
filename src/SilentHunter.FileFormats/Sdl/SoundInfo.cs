using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using SilentHunter.FileFormats.Extensions;
using skwas.IO;

namespace SilentHunter.FileFormats.Sdl
{
	/// <summary>
	/// Represents a sound info entry for <see cref="SdlFile"/>s.
	/// </summary>
	[DebuggerDisplay("Name = {Name}, WaveName = {WaveName}")]
	public sealed class SoundInfo : IRawSerializable, ICloneable, IEquatable<SoundInfo>
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private string _name, _waveName;

		/// <summary>
		/// Initializes a new instance of the <see cref="SoundInfo"/> class.
		/// </summary>
		public SoundInfo()
		{
			Volume = 90;
			Pitch = 1;
		}

		/// <summary>
		/// Gets or sets the name of the wave file.
		/// </summary>
		public string WaveName
		{
			get => _waveName ?? string.Empty;
			set => _waveName = value;
		}

		/// <summary>
		/// Gets or sets the entry name.
		/// </summary>
		public string Name
		{
			get => _name ?? string.Empty;
			set => _name = value;
		}

		/// <summary>
		/// Gets or sets whether the sound is played as 3D.
		/// </summary>
		public bool Is3D { get; set; }

		public bool Play { get; set; }

		/// <summary>
		/// Gets or sets whether the sound should be looped.
		/// </summary>
		public bool Loop { get; set; }
		public bool IsFolder { get; set; }

		/// <summary>
		/// Gets or sets the playback delay.
		/// </summary>
		public float Delay { get; set; }

		/// <summary>
		/// Gets or sets the max radius (3D).
		/// </summary>
		public float MaxRadius { get; set; }

		/// <summary>
		/// Gets or sets the min radius (3D).
		/// </summary>
		public float MinRadius { get; set; }

		/// <summary>
		/// Gets or sets the doppler factor.
		/// </summary>
		public float DopplerFactor { get; set; }

		/// <summary>
		/// Gets or sets the pitch variation.
		/// </summary>
		public float PitchVar { get; set; }

		/// <summary>
		/// Gets or sets the pitch.
		/// </summary>
		public float Pitch { get; set; }

		/// <summary>
		/// Gets or sets the volume variation.
		/// </summary>
		public float VolumeVar { get; set; }

		/// <summary>
		/// Gets or sets the volume.
		/// </summary>
		public float Volume { get; set; }

		/// <summary>
		/// Gets or sets the priority (in case too many sound effects are playing).
		/// </summary>
		public int Priority { get; set; }

		/// <summary>
		/// Gets or set the category the sound effect belongs to.
		/// </summary>
		public SoundCategory Category { get; set; }

		Task IRawSerializable.DeserializeAsync(Stream stream)
		{
			using (var reader = new BinaryReader(stream, FileEncoding.Default, true))
			{
				ushort size = reader.ReadUInt16();
				uint subSize = reader.ReadUInt32();
				if (size != subSize + 4)
				{
					throw new SilentHunterParserException("The file appears invalid. Unexpected size specifier encountered.");
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
					throw new SilentHunterParserException("The file appears invalid. Unexpected size specifier encountered.");
				}
			}

			return Task.CompletedTask;
		}

		Task IRawSerializable.SerializeAsync(Stream stream)
		{
			using (var writer = new BinaryWriter(stream, FileEncoding.Default, true))
			{
				long currentPos = writer.BaseStream.Position;
				writer.BaseStream.Position += 6;

				writer.Write("SoundInfo", '\0');

				SerializeProperty(writer, "Name", Name ?? string.Empty);
				SerializeProperty(writer, "WaveName", WaveName ?? string.Empty);
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

			return Task.CompletedTask;
		}

		/// <summary>
		/// Deserializes a property using specified <paramref name="reader"/>.
		/// </summary>
		/// <param name="reader">The reader.</param>
		/// <param name="propertyName">The name of the property.</param>
		/// <param name="propertyType">The type of the property.</param>
		/// <returns>The property value.</returns>
		private static object DeserializeProperty(BinaryReader reader, string propertyName, Type propertyType)
		{
			uint size = reader.ReadUInt32();
			long currentPos = reader.BaseStream.Position;
			string name = reader.ReadString(propertyName.Length);
			if (propertyName != name)
			{
				throw new InvalidOperationException("The file appears invalid. Unexpected property name encountered.");
			}

			// Skip past the terminating null.
			reader.BaseStream.Position++;

			// Read value.
			object value = propertyType == typeof(string)
				? reader.ReadNullTerminatedString()
				: reader.ReadStruct(propertyType);

			if (reader.BaseStream.Position != currentPos + size)
			{
				throw new InvalidOperationException("The file appears invalid. The data type does not match.");
			}

			return value;
		}

		/// <summary>
		/// Serializes a property using specified <paramref name="writer"/>.
		/// </summary>
		/// <param name="writer">The writer.</param>
		/// <param name="propertyName">The name of the property.</param>
		/// <param name="value">The property value.</param>
		private static void SerializeProperty(BinaryWriter writer, string propertyName, object value)
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

		/// <inheritdoc />
		public object Clone()
		{
			var clone = new SoundInfo();
			using (var ms = new MemoryStream())
			{
				((IRawSerializable)this).SerializeAsync(ms);

				ms.Position = 0;

				((IRawSerializable)clone).DeserializeAsync(ms).GetAwaiter().GetResult();
			}

			return clone;
		}

		public bool Equals(SoundInfo other)
		{
			if (ReferenceEquals(null, other))
			{
				return false;
			}

			if (ReferenceEquals(this, other))
			{
				return true;
			}

			return string.Equals(_name, other._name) && string.Equals(_waveName, other._waveName) && Is3D == other.Is3D && Play == other.Play && Loop == other.Loop && IsFolder == other.IsFolder && Delay.Equals(other.Delay) && MaxRadius.Equals(other.MaxRadius) && MinRadius.Equals(other.MinRadius) && DopplerFactor.Equals(other.DopplerFactor) && PitchVar.Equals(other.PitchVar) && Pitch.Equals(other.Pitch) && VolumeVar.Equals(other.VolumeVar) && Volume.Equals(other.Volume) && Priority == other.Priority && Category == other.Category;
		}

		/// <inheritdoc />
		public override bool Equals(object obj)
		{
			return ReferenceEquals(this, obj) || obj is SoundInfo other && Equals(other);
		}

		/// <inheritdoc />
		public override int GetHashCode()
		{
			unchecked
			{
				int hashCode = (_name != null ? _name.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (_waveName != null ? _waveName.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ Is3D.GetHashCode();
				hashCode = (hashCode * 397) ^ Play.GetHashCode();
				hashCode = (hashCode * 397) ^ Loop.GetHashCode();
				hashCode = (hashCode * 397) ^ IsFolder.GetHashCode();
				hashCode = (hashCode * 397) ^ Delay.GetHashCode();
				hashCode = (hashCode * 397) ^ MaxRadius.GetHashCode();
				hashCode = (hashCode * 397) ^ MinRadius.GetHashCode();
				hashCode = (hashCode * 397) ^ DopplerFactor.GetHashCode();
				hashCode = (hashCode * 397) ^ PitchVar.GetHashCode();
				hashCode = (hashCode * 397) ^ Pitch.GetHashCode();
				hashCode = (hashCode * 397) ^ VolumeVar.GetHashCode();
				hashCode = (hashCode * 397) ^ Volume.GetHashCode();
				hashCode = (hashCode * 397) ^ Priority;
				hashCode = (hashCode * 397) ^ (int)Category;
				return hashCode;
			}
		}

		public static bool operator ==(SoundInfo left, SoundInfo right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(SoundInfo left, SoundInfo right)
		{
			return !Equals(left, right);
		}
	}
}