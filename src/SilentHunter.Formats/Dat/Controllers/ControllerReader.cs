using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using SilentHunter.Controllers;
using SilentHunter.Controllers.Decoration;
using SilentHunter.Dat.Controllers.Serialization;
using SilentHunter.Extensions;
using skwas.IO;

namespace SilentHunter.Dat.Controllers
{
	public class ControllerReader : IControllerReader
	{
		private readonly ControllerAssembly _controllerAssembly;
		private readonly IControllerFactory _controllerFactory;
		private readonly ControllerSerializerResolver _controllerSerializerResolver;

		public ControllerReader(ControllerAssembly controllerAssembly, IControllerFactory controllerFactory)
		{
			_controllerAssembly = controllerAssembly ?? throw new ArgumentNullException(nameof(controllerAssembly));
			_controllerFactory = controllerFactory ?? throw new ArgumentNullException(nameof(controllerFactory));
			_controllerSerializerResolver = new ControllerSerializerResolver();
		}

		/// <summary>
		/// Reads a controller from a stream. If the controller is not implemented, the raw data is returned as byte array.
		/// </summary>
		/// <param name="stream"></param>
		/// <param name="controllerName">The name of the controller or null if the controller must be auto-detected (not optimal, some controllers cannot be detected).</param>
		/// <returns></returns>
		public object Read(Stream stream, string controllerName)
		{
			long currentPos = stream.Position;
			int availableData = (int)(stream.Length - stream.Position);

			var reader = new BinaryReader(stream, Encoding.ParseEncoding);

			// In case of a normal controller (10/-1) subtype the next Int32 contains the size of the following structure. In case of animation controllers (sub type 0/1/2/4/5/6), see below.
			int controllerSize = reader.ReadInt32();

			// The size read from the stream matches the available data (incl. the 4 bytes of the Int32 already read), then we have a normal controller. Otherwise, we have to do some more logic to determine correct type of controller.
			// PROBLEM: In the odd unlucky case the size of a raw controller may match exactly, and it is then assumed to be non-raw.
			bool isRawController = controllerSize + 4 != availableData;

			var profile = ControllerProfile.Unknown;
			// Raw controller? (byte stream)
			if (isRawController)
			{
				// First 4 bytes means something else.
				ushort animationControllerType = unchecked((ushort)(controllerSize & 0xFFFF));
				ushort unknown = unchecked((ushort)((controllerSize & 0xFFFF0000) >> 16));
				if (string.IsNullOrEmpty(controllerName))
				{
					// If no name provided, check if sub type matches that of an animation controller.
					if (Enum.IsDefined(typeof(AnimationType), animationControllerType))
					{
						controllerName = ((AnimationType)animationControllerType).ToString();
					}
				}

				// Only continue reading if we have a controller name.
				if (!string.IsNullOrEmpty(controllerName))
				{
					profile = _controllerAssembly.GetControllerProfilesByName(controllerName).FirstOrDefault();

					if (profile != ControllerProfile.Unknown && _controllerAssembly.TryGetControllerType(controllerName, profile, out Type controllerType))
					{
						// Check if the controller is indeed a raw controller. If it isn't, the size descriptor may have contained an invalid size for controller data. We just attempt normal deserialization then.
						if (controllerType.IsRawController())
						{
							ControllerAttribute controllerAttribute = controllerType.GetCustomAttribute<ControllerAttribute>() ?? new ControllerAttribute();
							// Check that the detected controller matches subtype.
							if (controllerAttribute.SubType.HasValue && controllerAttribute.SubType.Value == animationControllerType)
							{
								// For animation controllers, the 2 bytes are part of the data and identify an 'ushort' count field for n number of frames.
								if (typeof(AnimationController).IsAssignableFrom(controllerType))
								{
									stream.Position -= 2;
								}
							}
							else
								// The subtype/count is part of the controller data, move back.
							{
								stream.Position -= 4;
							}
						}
					} // exits
				} // exits
			}
			else // Non-raw controller, including names and size specifiers.
			{
				// Check if the stream contains a controller name.
				string readControllerName = PeekName(reader);

				// Substitute name of controller with the one we read from stream, if it does not match.
				if (controllerName == null || controllerName != readControllerName)
				{
					Debug.WriteLine("Controller name mismatch between '{0}' and '{1}', using '{1}'.",
						controllerName ?? "<unspecified>",
						readControllerName);

					controllerName = readControllerName;
				}

				profile = _controllerAssembly.GetControllerProfilesByName(controllerName).FirstOrDefault();
			}

			// If we have found a profile to use, try to read the controller.
			if (profile != ControllerProfile.Unknown && !string.IsNullOrEmpty(controllerName))
			{
				long controllerStartPosition = stream.Position;

				// Attempt to parse controller, starting with newest implementation.
				Type previousControllerType = null;
				Type controllerType = null;
				while (true)
				{
					try
					{
						if (_controllerAssembly.TryGetControllerType(controllerName, profile, out controllerType) && previousControllerType != controllerType)
						{
							RawController controller = _controllerFactory.CreateController(controllerType, false);
							IControllerSerializer cs = _controllerSerializerResolver.GetSerializer(controllerType);
							cs.Deserialize(stream, controller);
							return controller;
						}

						previousControllerType = controllerType;
					}
					catch (Exception ex)
					{
						// Reset stream to beginning.
						stream.Position = controllerStartPosition;
						Debug.WriteLine(ex.Message);

						previousControllerType = controllerType;
					}

					// If we reach the oldest version of SH, we are finished.
					if (profile >= ControllerProfile.SH3)
					{
						break;
					}

					// Move to next profile.
					profile++;
				}
			}

			// If not implemented, return the raw bytes.
			stream.Position = currentPos;
			return reader.ReadBytes((int)(stream.Length - currentPos));
		}

		private static string PeekName(BinaryReader reader)
		{
			long currentPos = reader.BaseStream.Position;
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
	}
}