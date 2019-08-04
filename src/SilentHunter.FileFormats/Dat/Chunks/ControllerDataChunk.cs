using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using SilentHunter.FileFormats.Dat.Controllers;
using SilentHunter.FileFormats.IO;

namespace SilentHunter.FileFormats.Dat.Chunks
{
	public sealed class ControllerDataChunk : DatChunk
	{
		private readonly IControllerReader _controllerReader;
		private readonly IControllerWriter _controllerWriter;
		private readonly object _lockObject = new object();
		private MemoryStream _unparsedControllerStream;
		private long _origin, _localOrigin;
		private object _parsedController;

		public ControllerDataChunk(IControllerReader controllerReader, IControllerWriter controllerWriter)
			: base(DatFile.Magics.ControllerData)
		{
			_controllerReader = controllerReader ?? throw new ArgumentNullException(nameof(controllerReader));
			_controllerWriter = controllerWriter ?? throw new ArgumentNullException(nameof(controllerWriter));
		}

		protected override void Dispose(bool disposing)
		{
			try
			{
				base.Dispose(disposing);
			}
			finally
			{
				_unparsedControllerStream?.Dispose();
				_unparsedControllerStream = null;
			}
		}

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
					if (_parsedController == null && _unparsedControllerStream != null)
					{
						string controllerName = GetControllerName();

						using (_unparsedControllerStream)
						{
							// Attempt to deserialize.
							_parsedController = _controllerReader.Read(_unparsedControllerStream, controllerName);
						}

						_unparsedControllerStream = null;
					}
				}

				// If controller data is a byte array, the controller was not deserialized. Either it's not implemented, or the data or the implementation contains a bug.
				if (_parsedController is byte[])
				{
					UnknownData.Add(new UnknownChunkData(_origin,
						_localOrigin,
						_parsedController,
						"Failed to read controller data. Either the data contains a bug, or S3D's controller definition is missing or incorrect."));
				}

				return _parsedController;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException();
				}

				lock (_lockObject)
				{
					_parsedController = value;
					UnknownData.Clear();
				}
			}
		}

		/// <summary>
		/// Gets the controller name. Returns <see cref="string.Empty"/> if the controller has not yet been read/parsed (by accessing <see cref="ControllerData"/> property).
		/// </summary>
		public string ControllerName => _parsedController?.GetType().Name ?? string.Empty;

		public override bool SupportsParentId => true;

		protected override Task SerializeAsync(Stream stream)
		{
			using (var writer = new BinaryWriter(stream, FileEncoding.Default, true))
			{
				writer.Write(ParentId);

				// Always zero.
				writer.Write((ulong)0);

				_controllerWriter.Write(stream, ControllerData);
			}

			return Task.CompletedTask;
		}

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
				_localOrigin = stream.Position;
				_origin = regionStream?.BaseStream.Position ?? _localOrigin;

				// Read the raw unparsed controller data (we defer deserializing until property access).
				_unparsedControllerStream = new MemoryStream(reader.ReadBytes((int)(stream.Length - _localOrigin)));
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
			if (ParentFile.Chunks.Count > 0)
			{
				if (ParentFile is DatFile f)
				{
					// Find it by searching up.
					prevControllerChunk = (ControllerChunk)f.Chunks
						.Reverse()
						.FirstOrDefault(c =>
							c.Magic == DatFile.Magics.Controller && c.Id == ParentId
						);
				}
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