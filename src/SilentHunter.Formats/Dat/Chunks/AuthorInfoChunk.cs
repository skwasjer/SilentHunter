using System.Diagnostics;
using System.IO;
using System.Reflection;
using SilentHunter.Extensions;
using skwas.IO;

namespace SilentHunter.Dat.Chunks
{
	/// <summary>
	/// Represents the AuthorInfo chunk of a Silent Hunter game file.
	/// </summary>
	public sealed class AuthorInfoChunk : DatChunk
	{
		public AuthorInfoChunk()
			: base(DatFile.Magics.AuthorInfo)
		{
		}

		public long Unknown { get; set; }

		/// <summary>
		/// Gets or sets the author that created or last modified the file.
		/// </summary>
		public string Author { get; set; }

		/// <summary>
		/// Gets or sets the description of the file. In case this is an original, unmodified file, this property will most likely hold the value "Created/modified with Kashmir".
		/// </summary>
		public string Description { get; set; }

		/// <summary>
		/// Gets a value indicating this is an original SH-file. At least, it isn't saved by this library. This is just an indicator, because when a file is modified with a hex editor, this property would still return 0.
		/// </summary>
		public bool IsOriginalFile { get; private set; }

		protected override void Deserialize(Stream stream)
		{
#if DEBUG
			// For debugging purposes, it is useful to know the filename.
			Debug.WriteLine(stream.GetBaseStreamName());
#endif

			var regionStream = stream as RegionStream;

			using (var reader = new BinaryReader(stream, FileEncoding.Default, true))
			{
				Unknown = reader.ReadInt64();
				UnknownData.Add(new UnknownChunkData(regionStream?.BaseStream.Position - 8 ?? stream.Position - 8,
					stream.Position - 8,
					Unknown,
					"No idea"));

				Author = reader.ReadNullTerminatedString();
				if (stream.Position < stream.Length)
				{
					Description = reader.ReadNullTerminatedString();
				}
				else
				{
					Description = Author;
					Author = null;
				}

				if (stream.Position == stream.Length)
				{
					return;
				}

				// S3D adds a signature. Ignore.
				string s3dSignature = reader.ReadString((int)(stream.Length - stream.Position - 1))?.TrimEnd(" \0".ToCharArray());
				Debug.WriteLine(s3dSignature);

				// Make sure we are EOS.
				stream.Position = stream.Length;
			}
		}

		protected override void Serialize(Stream stream)
		{
			using (var writer = new BinaryWriter(stream, FileEncoding.Default, true))
			{
				writer.WriteStruct(Unknown);

				writer.Write(Author, '\0');
				writer.Write(Description, '\0');
			}
		}
	}
}