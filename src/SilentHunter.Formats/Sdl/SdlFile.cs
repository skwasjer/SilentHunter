using System;
using System.Reflection;
using System.IO;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using skwas.IO;
using SilentHunter.Formats;

namespace SilentHunter.Sdl
{
	public class SdlFile
		: KeyedCollection<string, SoundInfo>, ISilentHunterFile
	{
		private static readonly string AssemblyPath = "Sdl.dll";

		public SdlFile()
			: base(EqualityComparer<string>.Default, -1)
		{
		}

		protected override string GetKeyForItem(SoundInfo item)
		{
			return item.Name;
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

		#region Implementation of ISilentHunterFile

		/// <summary>
		/// Loads the file from specified stream.
		/// </summary>
		/// <param name="stream">The stream to load from.</param>
		public void Load(Stream stream)
		{
			Clear();

			var bs = new BufferedStream(stream, 1024);
			while (bs.Position < bs.Length)
			{
				var sndInfo = new SoundInfo();
				((IRawSerializable)sndInfo).Deserialize(bs);

				if (string.Compare(sndInfo.Name, AssemblyPath, StringComparison.OrdinalIgnoreCase) == 0)
					continue;

				Add(sndInfo);
			}
		}

		/// <summary>
		/// Saves the file to specified stream.
		/// </summary>
		/// <param name="stream">The stream to write to.</param>
		public void Save(Stream stream)
		{
			var copyright = new SoundInfo
			{
				Name = AssemblyPath,
				WaveName = GetSignature(),
				IsFolder = true
			};
			((IRawSerializable)copyright).Serialize(stream);

			foreach (IRawSerializable sndInfo in this)
				sndInfo.Serialize(stream);
		}

		#endregion

		#region Implementation of IRawSerializable

		/// <summary>
		/// When implemented, deserializes the implemented class from specified <paramref name="stream"/>.
		/// </summary>
		/// <param name="stream">The stream.</param>
		void IRawSerializable.Deserialize(Stream stream)
		{
			Load(stream);
		}

		/// <summary>
		/// When implemented, serializes the implemented class to specified <paramref name="stream"/>.
		/// </summary>
		/// <param name="stream">The stream.</param>
		void IRawSerializable.Serialize(Stream stream)
		{
			Save(stream);
		}

		#endregion
	}
}
