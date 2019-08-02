using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using skwas.IO;

namespace SilentHunter.Sdl
{
	public class SdlFile : KeyedCollection<string, SoundInfo>, ISilentHunterFile
	{
		private const string S3DAssemblyPath = "Sdl.dll";

		public SdlFile()
			: base(EqualityComparer<string>.Default, -1)
		{
		}

		protected override string GetKeyForItem(SoundInfo item)
		{
			return item.Name;
		}

		/// <summary>
		/// Loads the file from specified stream.
		/// </summary>
		/// <param name="stream">The stream to load from.</param>
		public async Task LoadAsync(Stream stream)
		{
			Clear();

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
		}

		/// <summary>
		/// Saves the file to specified stream.
		/// </summary>
		/// <param name="stream">The stream to write to.</param>
		public async Task SaveAsync(Stream stream)
		{
			foreach (IRawSerializable sndInfo in this)
			{
				await sndInfo.SerializeAsync(stream).ConfigureAwait(false);
			}
		}

		/// <summary>
		/// When implemented, deserializes the implemented class from specified <paramref name="stream" />.
		/// </summary>
		/// <param name="stream">The stream.</param>
		Task IRawSerializable.DeserializeAsync(Stream stream)
		{
			return LoadAsync(stream);
		}

		/// <summary>
		/// When implemented, serializes the implemented class to specified <paramref name="stream" />.
		/// </summary>
		/// <param name="stream">The stream.</param>
		Task IRawSerializable.SerializeAsync(Stream stream)
		{
			return SaveAsync(stream);
		}
	}
}