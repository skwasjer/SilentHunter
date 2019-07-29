using System;
using System.Collections.Generic;
using System.IO;
using SilentHunter.Formats;

namespace SilentHunter.Dat.Controllers
{
	public static class ControllerAssembly
	{
		public static IItemFactory ItemFactory { get; set; }
		public static IControllerFactory ControllerFactory { get; set; }

		public static IReadOnlyDictionary<ControllerProfile, Dictionary<string, Type>> Controllers => ((ControllerFactory)ControllerFactory).Controllers;

		public static ControllerAssemblyHelpText HelpText { get; set; }

		public static IControllerReader Reader => new ControllerReader(ControllerFactory);

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
	}
}
