using System;
using System.Diagnostics;
using System.IO;
using skwas.IO;

namespace SilentHunter.Dat
{
	/// <summary>
	/// Represents unknown chunk data and the exact location in the file.
	/// </summary>
	[DebuggerDisplay("Offset = {Offset}, RelOffset = {RelativeOffset}, Data = {Data}, MyGuess = {MyGuess}")]
	public class UnknownChunkData
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly string _myGuess;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly object _data;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly long _offset;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly long _relativeOffset;

		/// <summary>
		/// Initializes a new instance of <see cref="UnknownChunkData"/>.
		/// </summary>
		/// <param name="offset">The absolute file offset.</param>
		/// <param name="relativeOffset">The relative offset in the chunk.</param>
		/// <param name="data">The unknown data.</param>
		/// <param name="myGuess">A guess of what the data may be.</param>
		public UnknownChunkData(long offset, long relativeOffset, object data, string myGuess)
		{
			if (data == null)
				throw new ArgumentNullException(nameof(data));

			_data = data;
			_offset = offset;
			_relativeOffset = relativeOffset + DatChunk.ChunkHeaderSize;	// Add header size.
			_myGuess = myGuess;
		}

		/// <summary>
		/// Gets the type of the data.
		/// </summary>
		public Type Type => _data.GetType();

		public string MyGuess => _myGuess;

		/// <summary>
		/// Gets the data.
		/// </summary>
		public object Data => _data;

		public byte[] GetDataAsByteArray()
		{
			var data = Data as byte[];
			if (data != null) return data;
			// Convert data to byte array.
			using (var ms = new MemoryStream())
			{
				using (var writer = new BinaryWriter(ms, Encoding.ParseEncoding))
				{
					writer.WriteStruct(Data);
					return ms.ToArray();
				}
			}
		}

		/// <summary>
		/// Gets the absolute file offset.
		/// </summary>
		public long Offset => _offset;

		/// <summary>
		/// Gets the relative offset in the chunk.
		/// </summary>
		public long RelativeOffset => _relativeOffset;

		/// <summary>
		/// Returns the size of the data.
		/// </summary>
		/// <returns></returns>
		private long GetDataSize()
		{
			try
			{
				return System.Runtime.InteropServices.Marshal.SizeOf(_data);
			}
			catch
			{
				try
				{
					return GetDataAsByteArray().Length;
				}
				catch
				{
					return -1;
				}
			}
		}

		/// <summary>
		/// Returns a string that represents the current object.
		/// </summary>
		/// <param name="offsetFormat">The format to write the offset in.</param>
		/// <returns>
		/// A string that represents the current object.
		/// </returns>
		public string ToString(string offsetFormat)
		{
			string leadHex = null;
			if (offsetFormat.ToUpper() == "X")
				leadHex = "0x";

			return string.Format("RelOffset = " + leadHex + "{0:" + offsetFormat + "}, Size = {1}", RelativeOffset, GetDataSize());
		}

		/// <summary>
		/// Returns a string that represents the current object.
		/// </summary>
		/// <returns>
		/// A string that represents the current object.
		/// </returns>
		public override string ToString()
		{
			return ToString("d");
		}
	}
}