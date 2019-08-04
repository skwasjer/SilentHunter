#if DEBUG
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using SilentHunter.FileFormats.IO;

namespace SilentHunter.FileFormats.Extensions
{
	[ExcludeFromCodeCoverage]
	internal static class DebugExtensions
	{
		internal static string GetBaseStreamName(this Stream s)
		{
			Stream baseStream = s;
			if (baseStream is RegionStream)
			{
				baseStream = ((RegionStream)s).BaseStream;
			}

			// TODO: can we remove reflection to get base stream?? Even though we only use this in DEBUG..
			if (baseStream is BufferedStream)
			{
				// Get the private field _s.
				baseStream = (Stream)typeof(BufferedStream).GetField("_stream", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(baseStream);
			}

			Type t = baseStream.GetType();
			if (t.Name == "SyncStream")
			{
				baseStream = (Stream)t.GetField("_stream", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(baseStream);
			}

			if (baseStream is BufferedStream)
			{
				// Get the private field _s.
				baseStream = (Stream)typeof(BufferedStream).GetField("_stream", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(baseStream);
			}

			if (baseStream is FileStream fileStream)
			{
				return fileStream.Name;
			}

			return null;
		}
	}
}
#endif