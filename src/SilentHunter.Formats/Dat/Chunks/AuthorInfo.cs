using System.IO;
using System.Diagnostics;
using System.Reflection;
using skwas.IO;
using SilentHunter.Formats;

namespace SilentHunter.Dat
{
	/// <summary>
	/// Represents the AuthorInfo chunk of a Silent Hunter game file.
	/// </summary>
	public sealed class AuthorInfo : DatChunk
	{
		public AuthorInfo()
			: base(DatFile.Magics.AuthorInfo)
		{
		}

		private long _unknown;

		public long Unknown
		{
			get { return _unknown; }
			set { _unknown = value; }
		}
		
		/// <summary>
		/// Gets or sets the author that created or last modified the file.
		/// </summary>
		public string Author { get; set; }

		/// <summary>
		/// Gets or sets the description of the file. In case this is an original, unmodified file, this property will most likely hold the value "Created/modified with Kashmir".
		/// </summary>
		public string Description { get; set; }

		public string LastSavedString { get; private set; }

		/// <summary>
		/// Gets a value indicating this is an original SH-file. At least, it isn't saved by this library. This is just an indicator, because when a file is modified with a hex editor, this property would still return 0.
		/// </summary>
		public bool IsOriginalFile { get; private set; }

		protected override void Deserialize(Stream stream)
		{
			var regionStream = stream as RegionStream;

			using (var reader = new BinaryReader(stream, Encoding.ParseEncoding, true))
			{
				_unknown = reader.ReadInt64();
				UnknownData.Add(new UnknownChunkData(regionStream?.BaseStream.Position - 8 ?? stream.Position - 8,
					stream.Position - 8, _unknown, "No idea"));

				Author = reader.ReadNullTerminatedString();
				if (stream.Position < stream.Length)
					Description = reader.ReadNullTerminatedString();
				else
				{
					Description = Author;
					Author = null;
				}

				// Discard remaining data. Original files don't have more data here, but our OnSerialize implementation writes an extra line with copyright info.
				IsOriginalFile = stream.Position == stream.Length;
				if (IsOriginalFile) return;
				LastSavedString = reader.ReadString((int) (stream.Length - stream.Position - 1))?.TrimEnd(" \0".ToCharArray());
				stream.Position = stream.Length;
			}

#if DEBUG
			Debug.WriteLine(LastSavedString + " : " + GetBaseStreamName(stream));
#endif
		}

		protected override void Serialize(Stream stream)
		{
			using (var writer = new BinaryWriter(stream, Encoding.ParseEncoding, true))
			{
				writer.WriteStruct(_unknown);

				writer.Write(Author, '\0');
				writer.Write(Description, '\0');

#if !DEBUG
				writer.Write(LastSavedString = GetSignature(), '\0');
#endif
			}
		}

		/// <summary>
		/// Generates a S3D signature.
		/// </summary>
		/// <returns>Returns the S3D signature.</returns>
		private static string GetSignature()
		{
			var asm = Assembly.GetEntryAssembly();
			var title = asm.GetAttribute<AssemblyTitleAttribute>().Title;
			var product = asm.GetAttribute<AssemblyProductAttribute>().Product;
			var version = asm.GetAttribute<AssemblyFileVersionAttribute>().Version;
			var cw = asm.GetAttribute<AssemblyCopyrightAttribute>().Copyright;

			return string.Format("Modified with {0} - {1} (version {2}). {3}", product, title, version, cw);
		}
	}
}
