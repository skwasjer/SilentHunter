using System;
using System.IO;
using SilentHunter.Dat.Controllers.Serialization;

namespace SilentHunter.Dat
{
	public abstract class RawController : IRawController
	{
		public virtual Type ControllerSerializerType { get; } = typeof(RawControllerSerializer);

		public void Deserialize(Stream stream)
		{
			IControllerSerializer serializer = (IControllerSerializer)Activator.CreateInstance(ControllerSerializerType);
			serializer.Deserialize(stream, this);
		}

		public void Serialize(Stream stream)
		{
			IControllerSerializer serializer = (IControllerSerializer)Activator.CreateInstance(ControllerSerializerType);
			serializer.Serialize(stream, this);
		}
	}
}