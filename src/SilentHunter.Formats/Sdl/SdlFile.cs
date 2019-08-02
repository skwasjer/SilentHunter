using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using skwas.IO;

namespace SilentHunter.Sdl
{
	/// <summary>
	/// Represents an SDL file parser.
	/// </summary>
	public sealed class SdlFile : KeyedCollection<string, SoundInfo>, ISilentHunterFile
	{
		private const string S3DAssemblyPath = "Sdl.dll";

		/// <summary>
		/// Initializes a new instance of the <see cref="SdlFile"/> class.
		/// </summary>
		public SdlFile()
			: base(EqualityComparer<string>.Default, -1)
		{
		}

		/// <inheritdoc />
		protected override string GetKeyForItem(SoundInfo item)
		{
			return item.Name;
		}

		/// <inheritdoc />
		public Task LoadAsync(Stream stream)
		{
			if (stream == null)
			{
				throw new ArgumentNullException(nameof(stream));
			}

			Clear();

			return GlobalExceptionHandler.HandleException(async () =>
				{
					while (stream.Position < stream.Length)
					{
						var sndInfo = new SoundInfo();
						await ((IRawSerializable)sndInfo).DeserializeAsync(stream).ConfigureAwait(false);

						// S3D adds this, so ignore.
						if (string.Compare(sndInfo.Name, S3DAssemblyPath, StringComparison.OrdinalIgnoreCase) == 0)
						{
							continue;
						}

						Add(sndInfo);
					}
				},
				"Failed to load file.");
		}

		/// <inheritdoc />
		public Task SaveAsync(Stream stream)
		{
			if (stream == null)
			{
				throw new ArgumentNullException(nameof(stream));
			}

			return GlobalExceptionHandler.HandleException(async () =>
				{
					foreach (IRawSerializable sndInfo in this)
					{
						await sndInfo.SerializeAsync(stream).ConfigureAwait(false);
					}
				},
				"Failed to save file.");
		}
	}
}