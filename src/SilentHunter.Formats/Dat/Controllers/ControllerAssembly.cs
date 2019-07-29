using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using SilentHunter.Formats;
using skwas.IO;

namespace SilentHunter.Dat.Controllers
{
	public static class ControllerAssembly
	{
		public static Dictionary<ControllerProfile, Dictionary<string, Type>> Controllers { get; private set; }

		public static ControllerAssemblyHelpText HelpText;

		public static List<Assembly> Assemblies { get; private set; }

		public static void LoadFrom(Assembly assembly)
		{
			if (Assemblies != null)
				throw new InvalidOperationException("A controller assembly was previously (attempted) loaded.");

			Controllers = new Dictionary<ControllerProfile, Dictionary<string, Type>>
			{
				{ControllerProfile.SH5, new Dictionary<string, Type>()},
				{ControllerProfile.SH4, new Dictionary<string, Type>()},
				{ControllerProfile.SH3, new Dictionary<string, Type>()}
			};

			Assemblies = new List<Assembly>
			{
				assembly
			};

			// Find all types with a ControllerAttribute applied.
			EnumControllers(assembly);

			string docFile = Path.Combine(Path.GetDirectoryName(assembly.Location), Path.GetFileNameWithoutExtension(assembly.Location) + ".xml");
			HelpText = new ControllerAssemblyHelpText();
			HelpText.Load(docFile);
		}

		/// <summary>
		/// Loads controllers from the remote assembly. On order to qualify, the type must have ControllerAttribute applied.
		/// </summary>
		/// <param name="remoteAsm"></param>
		private static void EnumControllers(Assembly remoteAsm)
		{
			const BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly |					 BindingFlags.Static;

			void NotSupportedMember(IList list, Type type, string memberName)
			{
				if (list.Count > 0)
					throw new NotSupportedException($"The type '{type.Name}' defines one or more {memberName} which is not supported");
			}

			foreach (var remoteType in remoteAsm.GetTypes())
			{
				// No interfaces allowed.
				if (remoteType.IsInterface)
					throw new NotSupportedException($"The type '{remoteType.Name}' is an interface which is not supported.");

				// No properties allowed.
				NotSupportedMember(remoteType.GetProperties(bindingFlags), remoteType, "properties");
				// No methods allowed.
				//notSupportedMember(remoteType.GetMethods(BindingFlags), remoteType, "methods");

				if (remoteType.IsValueType && !remoteType.IsDefined(typeof (SerializableAttribute), false))
				{
					throw new NotSupportedException(
						$"The type '{remoteType.Name}' is a value type, which requires a SerializableAttribute."
					);
				}

				// Check if type is a controller.
				if (!remoteType.IsController()) continue;

				string controllerProfileStr = remoteType.FullName?.Substring(0, 3) ?? string.Empty;
				if (Enum.TryParse(controllerProfileStr, out ControllerProfile profile))
				{
					if (Controllers[profile].ContainsKey(remoteType.Name))
						Controllers[profile][remoteType.Name] = remoteType;
					else
						Controllers[profile].Add(remoteType.Name, remoteType);
				}
				else
				{
					// If not explicit, add to all.
					Controllers[ControllerProfile.SH3].Add(remoteType.Name, remoteType);
					Controllers[ControllerProfile.SH4].Add(remoteType.Name, remoteType);
					Controllers[ControllerProfile.SH5].Add(remoteType.Name, remoteType);
				}
			}
		}

		/// <summary>
		/// Reads a controller from a stream. If the controller is not implemented, the raw data is returned as byte array.
		/// </summary>
		/// <param name="stream"></param>
		/// <param name="controllerName">The name of the controller or null if the controller must be auto-detected (not optimal, some controllers cannot be detected).</param>
		/// <returns></returns>
		public static object ReadController(Stream stream, string controllerName)
		{
			if (Controllers == null)
				throw new ApplicationException("No controllers are loaded yet. Call LoadFrom(..) first.");

			var currentPos = stream.Position;
			var availableData = (int)(stream.Length - stream.Position);

			var reader = new BinaryReader(stream, Encoding.ParseEncoding);

			// In case of a normal controller (0) subtype the next Int32 contains the size of the following structure. In case of animation controllers (sub type 2/3/4/5), this Int32 indicates the subtype (same as previous chunk).
			var controllerSize = reader.ReadInt32();

			// The size read from the stream matches the available data (incl. the 4 bytes of the Int32 already read), then we have a property collection.			
			// PROBLEM: this assumption is incorrect. In the odd unlucky case the size of a raw controller may match exactly, and it is then assumed to be non-raw.
			var isRawController = controllerSize + 4 != availableData;

			var profile = ControllerProfile.Unknown;
			// Raw controller? (byte stream)
			if (isRawController)
			{
				// Get sub type and count.
				var subType = (ushort)((long)controllerSize & 0xFFFF);
				//ushort count = (ushort)(((long)propSize & 0xFFFF0000) >> 16);
				if (string.IsNullOrEmpty(controllerName))
				{
					// If no name provided, check if sub type matches that of an animation controller.
					if (Enum.IsDefined(typeof(AnimationType), subType))
						controllerName = ((AnimationType)subType).ToString();
				}

				// Only continue reading if we have a controller name.
				if (!string.IsNullOrEmpty(controllerName))
				{
					profile = GetFirstMatchingControllerProfile(controllerName);

					if (profile != ControllerProfile.Unknown)
					{
						var controllerType = Controllers[profile][controllerName];

						// Check if the controller is indeed a raw controller. If it isn't, the size descriptor may have contained an invalid size for controller data. We just attempt normal deserialization then.
						if (controllerType.IsRawController())
						{
							var controllerAttribute = controllerType.GetAttribute<ControllerAttribute>() ?? new ControllerAttribute();
							// Check that the detected controller matches subtype.
							if (controllerAttribute.SubType.HasValue && controllerAttribute.SubType.Value == subType)
							{
								// If the controller writes its own count field, move the position in stream back by 2 bytes.
								// These are typically controllers that behave differently and are related to animations.
								if (controllerAttribute.HasCountField)
									stream.Position -= 2;
							}
							else
							// The subtype/count is part of the controller data, move back.
								stream.Position -= 4;
						}
					} // exits
				} // exits
			}
			else // Non-raw controller, including names and size specifiers.
			{
				// Check if the stream contains a controller name.
				var readControllerName = PeekName(reader);

				// Substitute name of controller with the one we read from stream, if it does not match.
				if (controllerName == null || controllerName != readControllerName)
				{
					Debug.WriteLine("Controller name mismatch between '{0}' and '{1}', using '{1}'.", 
						controllerName ?? "<unspecified>", 
						readControllerName);

					controllerName = readControllerName;
				}

				profile = GetFirstMatchingControllerProfile(controllerName);
			}

			// If we have found a profile to use, try to read the controller.
			if ((profile != ControllerProfile.Unknown) && !string.IsNullOrEmpty(controllerName))
			{
				var controllerStartPosition = stream.Position;

				// Attempt to parse controller, starting with newest implementation.
				while (true)
				{
					try
					{
						if (IsControllerImplemented(controllerName, profile))
						{
							var controllerType = Controllers[profile][controllerName];

							if (typeof (IRawController).IsAssignableFrom(controllerType))
							{
								var serializable = (IRawController)Activator.CreateInstance(controllerType);
								serializable.Deserialize(stream);
								return serializable;
							}
							throw new NotImplementedException();
							//return isRawController 
							//	? ReadRawValue(reader, controllerType, null) 
							//	: ReadMember(reader, controllerType, controllerName);
						}
					}
					catch (Exception ex)
					{
						// Reset stream to beginning.
						stream.Position = controllerStartPosition;
						Debug.WriteLine(ex.Message);
					}

					// If we reach the oldest version of SH, we are finished.
					if (profile >= ControllerProfile.SH3) break;

					// Move to next profile.
					profile++;	
				}
			}

			// If not implemented, return the raw bytes.
			stream.Position = currentPos;
			return reader.ReadBytes((int)(stream.Length - currentPos));
		}

		/// <summary>
		/// Writes a controller to a stream.
		/// </summary>
		/// <param name="stream"></param>
		/// <param name="controller"></param>
		public static void WriteController(Stream stream, object controller)
		{
			if (Controllers == null)
				throw new InvalidOperationException("No controllers are loaded yet. Call LoadFrom(..) first.");
			if (controller == null)
				throw new ArgumentNullException(nameof(controller));

			if (controller is byte[] byteBuffer)
			{
				// This is a raw byte controller. Simply write it.
				stream.Write(byteBuffer, 0, byteBuffer.Length);
				return;
			}

			using (var writer = new BinaryWriter(stream, Encoding.ParseEncoding, true))
			{
				var controllerType = controller.GetType();
				if (controllerType.IsRawController())
				{
					var controllerAttribute = controllerType.GetAttribute<ControllerAttribute>() ?? new ControllerAttribute();
					if (controllerAttribute.SubType.HasValue)
						writer.Write(controllerAttribute.SubType.Value);
					if (controllerAttribute.HasCountField || !controllerAttribute.SubType.HasValue)
					{
						// Skip writing the field. The count value will be written by the RawList type.					
					}
					else
						writer.Write((ushort) 0);

					((IRawController)controller).Serialize(writer.BaseStream);
					return;
				}

				// Test if the type is from our controller assembly.
				if (!Assemblies.Contains(controllerType.Assembly) || !controllerType.IsController())
					throw new NotSupportedException("Invalid controller.");

				// We don't know the size yet, so just write 0 for now.
				writer.Write(0);

				var startPos = stream.Position;

				var serializable = controller as IRawController;
				serializable?.Serialize(writer.BaseStream);

				// After the controller is written, determine and write the size.
				var currentPos = stream.Position;
				stream.Position = startPos - 4;
				writer.Write((int) (currentPos - startPos));

				// Restore position to the end of the controller.
				stream.Position = currentPos;
			}
		}

		/// <summary>
		/// Get the controller type for the first profile that implements the controller.
		/// </summary>
		/// <param name="controllerName">The controller name.</param>
		/// <returns>The first found profile.</returns>
		public static Type GetFirstMatchingController(string controllerName)
		{
			var profile = GetFirstMatchingControllerProfile(controllerName);

			return profile != ControllerProfile.Unknown ? Controllers[profile][controllerName] : null;
		}

		/// <summary>
		/// Get the first profile that implements the controller.
		/// </summary>
		/// <param name="controllerName">The controller name.</param>
		/// <returns>The first found profile.</returns>
		public static ControllerProfile GetFirstMatchingControllerProfile(string controllerName)
		{
			return Enum
				.GetValues(typeof (ControllerProfile))
				.Cast<ControllerProfile>()
				.FirstOrDefault(cp => cp != ControllerProfile.Unknown && IsControllerImplemented(controllerName, cp));
		}

		private static string PeekName(BinaryReader reader)
		{
			var currentPos = reader.BaseStream.Position;
			try
			{
				// Skip size
				reader.ReadInt32();
				return reader.ReadNullTerminatedString();
			}
			finally
			{
				reader.BaseStream.Position = currentPos;
			}
		}

		public static bool IsControllerImplemented(string controllerName, ControllerProfile profile)
		{
			if (Controllers == null)
				throw new ApplicationException("No controllers are loaded yet. Call Initialize() first.");

			return Controllers[profile].ContainsKey(controllerName);
		}

		/// <summary>
		/// Creates an new instance of the specified <paramref name="type"/>. All (child) fields that are reference types are also instantiated using the default constructor.
		/// </summary>
		/// <param name="type">The type to create.</param>
		/// <returns>Returns the new instance.  All (child) fields that are reference types are also instantiated using the default constructor.</returns>
		public static object CreateNewItem(Type type)
		{
			if (type == typeof(string)) return "";

			// Handle nullable.
			type = Nullable.GetUnderlyingType(type) ?? type;

			var newObject = Activator.CreateInstance(type);
			foreach (var fi in type.GetFields(BindingFlags.Public | BindingFlags.Instance))
			{
				var fieldType = fi.FieldType;
				if (fieldType.IsEnum)
				{
					fi.SetValue(newObject, Enum.GetValues(fieldType).GetValue(0));
					continue;
				}
				if (fieldType.Name.StartsWith("SHUnion"))
				{

				}
				else if (fieldType.IsGenericType)
				{
					var typeArgs = fieldType.GetGenericArguments();					
					if (typeof(IRawList).IsAssignableFrom(fieldType))
					{
					}
					else
						if (typeArgs.Length != 1)
							continue;
				}
				else if (fieldType == typeof(DateTime))
				{
					fi.SetValue(newObject, new DateTime(1939, 1, 1));
					continue;
				}				
				else if (!fieldType.IsClass)
					continue;

				fi.SetValue(newObject, CreateNewItem(fi.FieldType));
			}
			return newObject;
		}

		/// <summary>
		/// Creates an new instance of the same type as the object in <paramref name="original"/>. All (child) fields that are reference types are also instantiated using the default constructor if the original object has this property set.
		/// </summary>
		/// <remarks>This method is similar to <see cref="M:CreateNewItem(type)"/> but differs in that it checks the original object to see which fields are non-null. This is generally only used in arrays, and is there to ensure each array item is exactly the same.</remarks>
		/// <param name="original">The object to create a new type of.</param>
		/// <returns>Returns the new instance. All (child) fields that are reference types are also instantiated using the default constructor if the original object has this property set.</returns>
		public static object CreateNewItem(object original)
		{
			if (original == null) return null;
			var t = original.GetType();

			if (t == typeof(string)) return "";

			var newObject = t.Assembly.CreateInstance(t.FullName);
			foreach (var fi in t.GetFields(BindingFlags.Public | BindingFlags.Instance))
			{
				var fieldType = fi.FieldType;
				if (fieldType.IsGenericType)
				{
					var typeArgs = fieldType.GetGenericArguments();
					if (typeArgs.Length != 1)
					{
						if (fieldType.Name.StartsWith("SHUnion"))
						{
							object union = fi.GetValue(original);
							object newUnion = ((ICloneable)union).Clone();
							foreach (PropertyInfo unionFi in fieldType.GetProperties())
							{
								if (unionFi.Name == "Type")
								{
									var unionType = (Type)unionFi.GetValue(union, null);
									unionFi.SetValue(newUnion, unionType, null);
									fi.SetValue(newObject, newUnion);
									//break;
								}
								else if (unionFi.Name == "Value")
								{
									// Reset value to default.
									var objectType = unionFi.GetValue(union, null).GetType();
									unionFi.SetValue(newUnion, objectType.Assembly.CreateInstance(objectType.FullName), null);
								}
							}
						}
						continue;
					}
				}
				else if (fieldType == typeof(DateTime))
				{
				}
				else if (!fieldType.IsClass && !fieldType.IsEnum)
					continue;

				var orgFieldValue = fi.GetValue(original);
				if (orgFieldValue != null)
				{
					if (fieldType == typeof(DateTime))
						fi.SetValue(newObject, new DateTime(1939, 1, 1));
					else if (fieldType.IsEnum)
						fi.SetValue(newObject, Enum.GetValues(fieldType).GetValue(0));
					else
						fi.SetValue(newObject, CreateNewItem(orgFieldValue));
				}
			}
			return newObject;
		}

/*		/// <summary>
		/// Clones specified value.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="value"></param>
		/// <param name="typeOfValue"></param>
		/// <returns></returns>
		public static object Clone(object value, Type typeOfValue)
		{
			using (var ms = new MemoryStream())
			{
				var wr = new BinaryWriter(ms, Encoding.ParseEncoding);
				WriteValue(wr, "clone", value, null);
				ms.Position = 0;

				var rd = new BinaryReader(ms, Encoding.ParseEncoding);
				return ReadMember(rd, typeOfValue, "clone");
			}
		}*/

		/// <summary>
		/// Creates the specified controller name for the requested profile.
		/// </summary>
		/// <param name="controllerName"></param>
		/// <param name="profile">The profile to search for the controller.</param>
		/// <returns>Returns the newly created controller. All (child) fields that are reference types are also pre-instantiated.</returns>
		/// <exception cref="ArgumentException">Thrown when the controller name is empty or cannot be found for the <paramref name="profile"/>.</exception>
		public static object CreateController(string controllerName, ControllerProfile profile)
		{
			if (string.IsNullOrEmpty(controllerName))
				throw new ArgumentNullException(nameof(controllerName));

			if (Controllers[profile].ContainsKey(controllerName))
				return CreateController(Controllers[profile][controllerName]);

			throw new ArgumentException("Unknown controller type.");
		}

		/// <summary>
		/// Creates the controller for specified <paramref name="type"/>.
		/// </summary>
		/// <param name="type">The controller type.</param>
		/// <returns>Returns the newly created controller. All (child) fields that are reference types are also instantiated using the default constructor.</returns>
		/// <exception cref="ArgumentException">Thrown when the controller name is empty or cannot be found for the <paramref name="profile"/>.</exception>
		public static object CreateController(Type type)
		{		
			if (type == null)
				throw new ArgumentNullException(nameof(type));

			if (((ControllerProfile[]) Enum.GetValues(typeof (ControllerProfile)))
				.Where(profile => profile != ControllerProfile.Unknown)
				.SelectMany(profile => Controllers[profile].Values)
				.Any(t => t == type))
				return CreateNewItem(type);

			throw new ArgumentException("Unknown controller type.");
		}
	}
}
