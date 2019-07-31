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
				Unknown = reader.ReadInt64();
				UnknownData.Add(new UnknownChunkData(regionStream?.BaseStream.Position - 8 ?? stream.Position - 8,
					stream.Position - 8, Unknown, "No idea"));

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

				// Discard remaining data. Original files don't have more data here, but our OnSerialize implementation writes an extra line with copyright info.
				IsOriginalFile = stream.Position == stream.Length;
				if (IsOriginalFile)
				{
					return;
				}

				LastSavedString = reader.ReadString((int)(stream.Length - stream.Position - 1))?.TrimEnd(" \0".ToCharArray());
				stream.Position = stream.Length;
			}

#if DEBUG
			Debug.WriteLine(LastSavedString + " : " + stream.GetBaseStreamName());
#endif
		}

		protected override void Serialize(Stream stream)
		{
			using (var writer = new BinaryWriter(stream, Encoding.ParseEncoding, true))
			{
				writer.WriteStruct(Unknown);

				writer.Write(Author, '\0');
				writer.Write(Description, '\0');

				if (((DatFile)ParentFile).SaveSignature)
				{
					writer.Write(LastSavedString = GetSignature(), '\0');
				}
			}
		}

		/// <summary>
		/// Generates a S3D signature.
		/// </summary>
		/// <returns>Returns the S3D signature.</returns>
		private static string GetSignature()
		{
			Assembly asm = Assembly.GetEntryAssembly();
			string title = asm.GetAttribute<AssemblyTitleAttribute>().Title;
			string product = asm.GetAttribute<AssemblyProductAttribute>().Product;
			string version = asm.GetAttribute<AssemblyFileVersionAttribute>().Version;
			string cw = asm.GetAttribute<AssemblyCopyrightAttribute>().Copyright;

			return $"Modified with {product} - {title} (version {version}). {cw}";
		}
	}
}