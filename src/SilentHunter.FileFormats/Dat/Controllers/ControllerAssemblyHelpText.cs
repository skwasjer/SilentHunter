using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Xml;

namespace SilentHunter.FileFormats.Dat.Controllers
{
	/// <summary>
	/// Encapsulates XML documentation file that is generated for a <see cref="ControllerAssembly"/>, and allows querying for this documentation by key, type or member (field).
	/// </summary>
	/// <remarks>
	/// Uses lazy loading, so file will only be read when key is requested. Thus, if a stream or reader is provided, do not dispose it after creating an instance of this class.
	/// </remarks>
	public class ControllerAssemblyHelpText : IReadOnlyDictionary<string, string>, IReadOnlyDictionary<Type, string>, IReadOnlyDictionary<MemberInfo, string>
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		private readonly Lazy<IDictionary<string, string>> _loadHelpText;

		/// <summary>
		/// Initializes a new instance of the <see cref="ControllerAssemblyHelpText"/> using specified <paramref name="documentationFilePath"/>.
		/// </summary>
		/// <param name="documentationFilePath">The path of the XML document file.</param>
		public ControllerAssemblyHelpText(string documentationFilePath)
			: this(XmlReader.Create(documentationFilePath))
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ControllerAssemblyHelpText"/> using specified <paramref name="stream"/>.
		/// </summary>
		/// <param name="stream">The stream to read the XML documentation from.</param>
		/// <param name="disposeStream">true to dispose the stream when loading completed</param>
		public ControllerAssemblyHelpText(Stream stream, bool disposeStream = true)
			: this(XmlReader.Create(stream), disposeStream)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ControllerAssemblyHelpText"/> using specified <paramref name="xmlReader"/>.
		/// </summary>
		/// <param name="xmlReader">The XML reader to read the XML documentation from.</param>
		/// <param name="disposeReader">true to dispose the stream when loading completed</param>
		public ControllerAssemblyHelpText(XmlReader xmlReader, bool disposeReader = true)
		{
			_loadHelpText = new Lazy<IDictionary<string, string>>(
				() => LoadHelpText(xmlReader, disposeReader),
				LazyThreadSafetyMode.ExecutionAndPublication
			);
		}

		/// <inheritdoc />
		public string this[string key]
		{
			get
			{
				if (key == null)
				{
					throw new ArgumentNullException(nameof(key));
				}

				if (TryGetValue(key, out string value))
				{
					return value;
				}

				throw new KeyNotFoundException($"Key '{key}' not found.");

			}
		}

		/// <inheritdoc />
		public string this[Type type]
		{
			get
			{
				if (type == null)
				{
					throw new ArgumentNullException(nameof(type));
				}

				if (TryGetValue(type, out string value))
				{
					return value;
				}

				throw new KeyNotFoundException($"Key '{type.FullName}' not found.");
			}
		}

		/// <inheritdoc />
		public string this[MemberInfo member]
		{
			get
			{
				if (member == null)
				{
					throw new ArgumentNullException(nameof(member));
				}

				if (member.DeclaringType == null)
				{
					throw new InvalidOperationException("Member must be declared by a type.");
				}

				if (TryGetValue(member, out string value))
				{
					return value;
				}

				throw new KeyNotFoundException($"Key '{member.Name}' not found.");
			}
		}

		/// <inheritdoc />
		public bool ContainsKey(string key)
		{
			return key != null && _loadHelpText.Value.ContainsKey(key);
		}

		/// <inheritdoc />
		public bool ContainsKey(Type key)
		{
			return key != null && ContainsKey(GetKey(key));
		}

		/// <inheritdoc />
		public bool ContainsKey(MemberInfo key)
		{
			return key != null 
			 && key.DeclaringType != null
			 && ContainsKey(GetKey(key));
		}

		/// <inheritdoc />
		public bool TryGetValue(string key, out string value)
		{
			return _loadHelpText.Value.TryGetValue(key, out value);
		}

		/// <inheritdoc />
		public bool TryGetValue(Type key, out string value)
		{
			value = null;
			// ReSharper disable once ConditionIsAlwaysTrueOrFalse
			return key != null && TryGetValue(GetKey(key), out value);
		}

		/// <inheritdoc />
		public bool TryGetValue(MemberInfo key, out string value)
		{
			value = null;
			// ReSharper disable once ConditionIsAlwaysTrueOrFalse
			return key != null
			 && key.DeclaringType != null
			 && TryGetValue(GetKey(key), out value);
		}

		/// <inheritdoc />
		public IEnumerable<string> Keys => _loadHelpText.Value.Keys;

		/// <summary>Gets an enumerable collection that contains the values in the read-only dictionary.</summary>
		/// <returns>An enumerable collection that contains the values in the read-only dictionary.</returns>
		public IEnumerable<string> Values => _loadHelpText.Value.Values;

		/// <inheritdoc />
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		IEnumerable<Type> IReadOnlyDictionary<Type, string>.Keys => throw new NotImplementedException();

		/// <inheritdoc />
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		IEnumerable<MemberInfo> IReadOnlyDictionary<MemberInfo, string>.Keys => throw new NotImplementedException();

		/// <inheritdoc />
		public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
		{
			return _loadHelpText.Value.GetEnumerator();
		}

		/// <inheritdoc />
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		IEnumerator<KeyValuePair<Type, string>> IEnumerable<KeyValuePair<Type, string>>.GetEnumerator()
		{
			throw new NotImplementedException();
		}

		IEnumerator<KeyValuePair<MemberInfo, string>> IEnumerable<KeyValuePair<MemberInfo, string>>.GetEnumerator()
		{
			throw new NotImplementedException();
		}

		/// <summary>Gets the number of elements in the collection.</summary>
		/// <returns>The number of elements in the collection.</returns>
		public int Count => _loadHelpText.Value.Count;

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

		private static string GetKey(MemberInfo key)
		{
			return $"{key.MemberType.ToString()[0]}:{key.DeclaringType.FullName}.{key.Name}";
		}

		private static string GetKey(Type key)
		{
			return "T:" + key.FullName;
		}
	}
}