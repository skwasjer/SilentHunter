using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using SilentHunter.Formats;
using skwas.IO;

namespace SilentHunter.Dat.Controllers
{
	public static class ControllerAssembly
	{
		public static IItemFactory ItemFactory { get; set; }
		public static IControllerFactory ControllerFactory { get; set; }

		public static IDictionary<ControllerProfile, Dictionary<string, Type>> Controllers => ((ControllerFactory)ControllerFactory).Controllers;

		public static ControllerAssemblyHelpText HelpText { get; set; }

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
				if (!ControllerFactory.CanCreate(controllerType))
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
	}
}
