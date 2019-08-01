using System;
using System.Collections.Generic;

namespace SilentHunter.Dat.Controllers.Serialization
{
	public class ControllerSerializerResolver
	{
		private readonly List<ControllerMapping> _serializerMappings;

		private class ControllerMapping
		{
			public Type ControllerType { get; set; }
			public IControllerSerializer Serializer { get; set; }
		}

		public ControllerSerializerResolver()
		{
			_serializerMappings = new List<ControllerMapping>
			{
				new ControllerMapping
				{
					ControllerType = typeof(StateMachineController),
					Serializer = new StateMachineControllerSerializer()
				},
				new ControllerMapping
				{
					ControllerType = typeof(MeshAnimationController),
					Serializer = new MeshAnimationControllerSerializer()
				},
				new ControllerMapping
				{
					ControllerType = typeof(Controller),
					Serializer = new ControllerSerializer()
				},
				new ControllerMapping
				{
					ControllerType = typeof(RawController),
					Serializer = new RawControllerSerializer()
				}
			};
		}

		public IControllerSerializer GetSerializer(Type controllerType)
		{
			foreach (ControllerMapping mapping in _serializerMappings)
			{
				if (mapping.ControllerType.IsAssignableFrom(controllerType))
				{
					return mapping.Serializer;
				}
			}

			return null;
		}
	}
}
