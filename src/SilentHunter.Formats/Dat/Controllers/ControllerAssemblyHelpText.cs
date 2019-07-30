using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Xml;
using SilentHunter.Formats;

namespace SilentHunter.Dat.Controllers
{
	public class ControllerAssemblyHelpText
	{
		private readonly Lazy<IDictionary<string, string>> _loadHelpText;

		public ControllerAssemblyHelpText(string docFile)
			: this(XmlReader.Create(docFile), true)
		{
		}

		public ControllerAssemblyHelpText(Stream stream, bool disposeReader = false)
			: this(XmlReader.Create(stream), disposeReader)
		{
		}

		public ControllerAssemblyHelpText(XmlReader xmlReader, bool disposeReader = false)
		{
			_loadHelpText = new Lazy<IDictionary<string, string>>(() => LoadHelpText(xmlReader, disposeReader), LazyThreadSafetyMode.ExecutionAndPublication);
		}

		private static IDictionary<string, string> LoadHelpText(XmlReader xmlReader, bool disposeReader)
		{
			var dict = new Dictionary<string, string>();
			do
			{
				if (!xmlReader.ReadToFollowing("member"))
				{
					break;
				}

				string name = xmlReader.GetAttribute("name");
				if (string.IsNullOrEmpty(name) || name.StartsWith("M:"))
				{
					continue;
				}

				if (!xmlReader.ReadToDescendant("summary"))
				{
					break;
				}

				dict.Add(name, xmlReader.ReadInnerXml().Trim());
			} while (!xmlReader.EOF);

			if (disposeReader)
			{
				xmlReader.Dispose();
			}

			return dict;
		}

		public string this[string key] => _loadHelpText.Value.ContainsKey(key) ? _loadHelpText.Value[key] : null;

		public string this[Type type] => type.GetAttribute<DescriptionAttribute>()?.Description ?? this["T:" + type.FullName];

		public string this[MemberInfo member] =>
			member.GetAttribute<DescriptionAttribute>()?.Description
			?? this[$"{member.MemberType.ToString()[0]}:{member.DeclaringType.FullName}.{member.Name}"];
	}
}