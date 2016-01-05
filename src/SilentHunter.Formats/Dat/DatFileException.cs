using System;
using System.IO;
using System.Runtime.Serialization;

namespace SilentHunter.Dat
{
	/// <summary>
	/// The exception that is thrown when a parsing error occurs.
	/// </summary>
	public class DatFileException
		: IOException
	{
		/// <summary>
		/// Initializes a new instance of <see cref="DatFileException"/>.
		/// </summary>
		/// <param name="chunkIndex">The index of the chunk at which the exception occurred.</param>
		/// <param name="chunkOffset">The index of the chunk at which the exception occurred.</param>
		/// <param name="fileOffset">The file offset where the exception occurred.</param>
		/// <param name="innerException">The inner exception if any.</param>
		public DatFileException(int chunkIndex, long chunkOffset, long fileOffset, Exception innerException)
			: base("The file could not be read due to a parse error.", innerException)
		{
			ChunkIndex = chunkIndex;
			ChunkOffset = chunkOffset;
			FileOffset = fileOffset;
		}

		protected DatFileException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		/// <summary>
		/// Gets the index of the chunk at which the exception occurred.
		/// </summary>
		public int ChunkIndex { get; }

		/// <summary>
		/// Gets the offset inside the chunk where the exception occurred.
		/// </summary>
		public long ChunkOffset { get; }

		/// <summary>
		/// Gets the file offset where the exception occurred.
		/// </summary>
		public long FileOffset { get; }

		/// <summary>
		/// Gets a message that describes the current exception.
		/// </summary>
		/// <returns>
		/// The error message that explains the reason for the exception, or an empty string ("").
		/// </returns>
		public override string Message
		{
			get
			{
				return base.Message + "\r\n   Chunk index: " + ChunkIndex + 
						"\r\n   Chunk offset: 0x" + ChunkOffset.ToString("x8") + 
						"\r\n   Parsing stopped at: 0x" + FileOffset.ToString("x8") + "\r\n\r\n";
			}
		}
	}
}