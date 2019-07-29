using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Xml;
using SilentHunter.Formats;

namespace SilentHunter.Dat.Controllers
{
	public class ControllerAssemblyHelpText
	{
		private IDictionary<string, string> _dict = new Dictionary<string, string>(); 

		/// <summary>
		/// Loads help text into a dictionary.
		/// </summary>
		/// <param name="docFile">The doc file to load from.</param>
		public void Load(string docFile)
		{
			using (var xmlReader = XmlReader.Create(docFile))
				Load(xmlReader);
		}

		/// <summary>
		/// Loads help text into a dictionary.
		/// </summary>
		/// <param name="stream">The stream to load the doc file from.</param>
		public void Load(Stream stream)
		{
			using (var xmlReader = XmlReader.Create(stream))
				Load(xmlReader);
		}

		/// <summary>
		/// Loads help text into a dictionary.
		/// </summary>
		/// <param name="xmlReader">The reader to load the doc file from.</param>
		public void Load(XmlReader xmlReader)
		{
			_dict.Clear();
			
			do
			{
				if (!xmlReader.ReadToFollowing("member")) break;
				var name = xmlReader.GetAttribute("name");
				if (string.IsNullOrEmpty(name) || name.StartsWith("M:")) continue;
				if (!xmlReader.ReadToDescendant("summary")) break;
				_dict.Add(name, xmlReader.ReadInnerXml().Trim());
			} while (!xmlReader.EOF);
		}

		public string this[string key]
		{
			get { return _dict.ContainsKey(key) ? _dict[key] : null; }
		}

		public string this[Type type]
		{
			get
			{
				return type.GetAttribute<DescriptionAttribute>()?.Description ?? this["T:" + type.FullName];
			}
		}

		public string this[MemberInfo member]
		{
			get
			{
				return member.GetAttribute<DescriptionAttribute>()?.Description 
					?? this[string.Format("{0}:{1}.{2}", member.MemberType.ToString()[0], member.DeclaringType.FullName, member.Name)];
			} 
		}
	}
}
