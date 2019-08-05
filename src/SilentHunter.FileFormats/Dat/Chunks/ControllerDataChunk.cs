using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using SilentHunter.FileFormats.Dat.Controllers;
using SilentHunter.FileFormats.IO;

namespace SilentHunter.FileFormats.Dat.Chunks
{
	/// <summary>
	/// Represents a controller data chunk.
	/// </summary>
	public sealed class ControllerDataChunk : DatChunk
	{
		private readonly IControllerReader _controllerReader;
		private readonly IControllerWriter _controllerWriter;
		private readonly object _lockObject = new object();
		private byte[] _unparsedControllerData;
		private long _absolutePosition, _relativePosition;
		private object _parsedController;

		/// <summary>
		/// Initializes a new instance of the <see cref="ControllerDataChunk"/> class.
		/// </summary>
		/// <param name="controllerReader">The controller reader.</param>
		/// <param name="controllerWriter">The controller writer.</param>
		public ControllerDataChunk(IControllerReader controllerReader, IControllerWriter controllerWriter)
			: base(DatFile.Magics.ControllerData)
		{
			_controllerReader = controllerReader ?? throw new ArgumentNullException(nameof(controllerReader));
			_controllerWriter = controllerWriter ?? throw new ArgumentNullException(nameof(controllerWriter));
		}

		/// <inheritdoc />
		public override bool SupportsParentId => true;

		/// <summary>
		/// Gets the controller name. Returns <see cref="string.Empty"/> if the controller has not yet been read/parsed (by accessing <see cref="ControllerData"/> property).
		/// </summary>
		public string ControllerName => _parsedController?.GetType().Name ?? string.Empty;

		/// <summary>
		/// Gets or sets the controller.
		/// </summary>
		public object ControllerData
		{
			get
			{
				if (_parsedController != null)
				{
					return _parsedController;
				}

				lock (_lockObject)
				{
					if (_parsedController == null && _unparsedControllerData != null)
					{
						string controllerName = GetControllerName();

						using (var ms = new MemoryStream(_unparsedControllerData))
						{
							// Attempt to deserialize.
							_parsedController = _controllerReader.Read(ms, controllerName);
						}

						_unparsedControllerData = null;
					}
				}

				// If controller data is a byte array, the controller was not deserialized. Either it's not implemented, or the data or the implementation contains a bug.
				if (_parsedController is byte[])
				{
					UnknownData.Add(new UnknownChunkData(_absolutePosition,
						_relativePosition,
						_parsedController,
						"Failed to read controller data. Either the data contains a bug, or S3D's controller definition is missing or incorrect."));
				}

				return _parsedController;
			}
			set
			{
				lock (_lockObject)
				{
					_parsedController = value;
				}
			}
		}

		/// <inheritdoc />
		protected override Task DeserializeAsync(Stream stream)
		{
			lock (_lockObject)
			{
				_parsedController = null;
				UnknownData.Clear();
			}

			var regionStream = stream as RegionStream;

			using (var reader = new BinaryReader(stream, FileEncoding.Default, true))
			{
				// Read parent id.
				ParentId = reader.ReadUInt64();

				ulong alwaysZero = reader.ReadUInt64();
				Debug.Assert(alwaysZero == 0, "Expected 0.");

				// Cache position.
				_relativePosition = stream.Position;
				_absolutePosition = regionStream?.BaseStream.Position ?? _relativePosition;

				// Read the raw unparsed controller data (we defer deserializing until property access).
				_unparsedControllerData = reader.ReadBytes((int)(stream.Length - _relativePosition));
			}

			return Task.CompletedTask;
		}

		/// <inheritdoc />
		protected override Task SerializeAsync(Stream stream)
		{
			using (var writer = new BinaryWriter(stream, FileEncoding.Default, true))
			{
				writer.Write(ParentId);

				// Always zero.
				writer.Write((ulong)0);

				// The controller has not been parsed yet, so we can simply write the unparsed data.
				if (_unparsedControllerData != null && _parsedController == null)
				{
					writer.Write(_unparsedControllerData, 0, _unparsedControllerData.Length);
				}
				else if (_parsedController != null)
				{
					_controllerWriter.Write(stream, _parsedController);
				}
			}

			return Task.CompletedTask;
		}

		/// <summary>
		/// Get the controller name from previous chunk, which should be a Controller chunk. If the previous chunk is not a Controller chunk, null is returned.
		/// </summary>
		/// <returns></returns>
		private string GetControllerName()
		{
			ControllerChunk prevControllerChunk = null;
			if (ParentFile?.Chunks.Count > 0)
			{
				// Find parent by searching up (reverse).
				prevControllerChunk = ParentFile.Chunks
					.OfType<ControllerChunk>()
					// TODO: LastOrDefault? Not efficient either, rather I want to have a more efficient iteration.
					.Reverse()
					.FirstOrDefault(c => c.Id == ParentId);
			}

			string controllerName = prevControllerChunk?.Name;

			// Code needed to detect StateMachineClass.
			if (string.IsNullOrEmpty(controllerName) && SubType == -1)
			{
				controllerName = "StateMachineClass";
			}

			return controllerName;
		}
	}
}