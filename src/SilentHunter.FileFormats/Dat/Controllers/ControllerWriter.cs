using System;
using System.IO;
using System.Reflection;
using SilentHunter.Controllers;
using SilentHunter.Controllers.Decoration;
using SilentHunter.FileFormats.Dat.Controllers.Serialization;
using SilentHunter.FileFormats.Extensions;

namespace SilentHunter.FileFormats.Dat.Controllers
{
	internal class ControllerWriter : IControllerWriter
	{
		private readonly IControllerFactory _controllerFactory;
		private readonly ControllerSerializerResolver _controllerSerializerResolver;

		public ControllerWriter(IControllerFactory controllerFactory, ControllerSerializerResolver controllerSerializerResolver)
		{
			_controllerFactory = controllerFactory ?? throw new ArgumentNullException(nameof(controllerFactory));
			_controllerSerializerResolver = controllerSerializerResolver ?? throw new ArgumentNullException(nameof(controllerSerializerResolver));
		}

		/// <summary>
		/// Writes a controller to a stream.
		/// </summary>
		/// <param name="stream"></param>
		/// <param name="controller"></param>
		public void Write(Stream stream, object controller)
		{
			if (controller == null)
			{
				throw new ArgumentNullException(nameof(controller));
			}

			// For fallback support, allow byte arrays/streams to be passed.
			// We simply write it as is.
			if (controller is byte[] byteBuffer)
			{
				stream.Write(byteBuffer, 0, byteBuffer.Length);
				return;
			}

			if (controller is Stream inputStream)
			{
				inputStream.CopyTo(stream);
				return;
			}

			using (var writer = new BinaryWriter(stream, FileEncoding.Default, true))
			{
				Type controllerType = controller.GetType();
				IControllerSerializer cs = _controllerSerializerResolver.GetSerializer(controllerType);

				if (!controllerType.IsBehaviorController())
				{
					ControllerAttribute controllerAttribute = controllerType.GetCustomAttribute<ControllerAttribute>() ?? new ControllerAttribute();
					if (controllerAttribute.SubType.HasValue)
					{
						writer.Write(controllerAttribute.SubType.Value);
					}

					if (controllerType.IsAnimationController() || !controllerAttribute.SubType.HasValue)
					{
						// Skip writing the count field, the serializer will take care of it.
					}
					else
					{
						writer.Write((ushort)0);
					}

					cs.Serialize(stream, (Controller)controller);

					return;
				}

				// Test if the type is from our controller assembly.
				if (!_controllerFactory.CanCreate(controllerType))
				{
					throw new NotSupportedException("Invalid controller.");
				}

				// We don't know the size yet, so just write 0 for now.
				writer.Write(0);

				long startPos = stream.Position;

				cs.Serialize(stream, (Controller)controller);

				// After the controller is written, determine and write the size.
				long currentPos = stream.Position;
				stream.Position = startPos - 4;
				writer.Write((int)(currentPos - startPos));

				// Restore position to the end of the controller.
				stream.Position = currentPos;
			}
		}
	}
}