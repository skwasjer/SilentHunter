using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;
using SilentHunter.Extensions;
using skwas.IO;

namespace SilentHunter.Sdl
{
	public class SdlFile : KeyedCollection<string, SoundInfo>, ISilentHunterFile
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
			Assembly asm = Assembly.GetEntryAssembly();
			string title = asm.GetAttribute<AssemblyTitleAttribute>().Title;
			string product = asm.GetAttribute<AssemblyProductAttribute>().Product;
			string version = asm.GetAttribute<AssemblyFileVersionAttribute>().Version;
			string cw = asm.GetAttribute<AssemblyCopyrightAttribute>().Copyright;

			return $"Modified with {product} - {title} (version {version}). {cw}";
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
		public void Save(Stream stream)
		{
#if !DEBUG
			var copyright = new SoundInfo
			{
				Name = AssemblyPath,
				WaveName = GetSignature(),
				IsFolder = true
			};
			((IRawSerializable)copyright).Serialize(stream);
#endif

			foreach (IRawSerializable sndInfo in this)
			{
				sndInfo.Serialize(stream);
			}
		}

		#endregion

		#region Implementation of IRawSerializable

		/// <summary>
		/// When implemented, deserializes the implemented class from specified <paramref name="stream" />.
		/// </summary>
		/// <param name="stream">The stream.</param>
		void IRawSerializable.Deserialize(Stream stream)
		{
			Load(stream);
		}

		/// <summary>
		/// When implemented, serializes the implemented class to specified <paramref name="stream" />.
		/// </summary>
		/// <param name="stream">The stream.</param>
		void IRawSerializable.Serialize(Stream stream)
		{
			Save(stream);
		}

		#endregion
	}
}