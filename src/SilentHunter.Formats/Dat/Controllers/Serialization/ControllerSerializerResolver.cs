using System;
using System.Collections.Generic;
using System.Linq;

namespace SilentHunter.Dat.Controllers.Serialization
{
	internal sealed class ControllerSerializerResolver
	{
		public sealed class Mapping
		{
			public Type ControllerType { get; set; }
			public Func<IControllerSerializer> ImplementationFactory { get; set; }
		}

		private readonly List<Mapping> _serializerMappings;

		public ControllerSerializerResolver(IEnumerable<Mapping> mappings)
		{
			if (mappings == null)
			{
				throw new ArgumentNullException(nameof(mappings));
			}

			_serializerMappings = mappings.ToList();
		}

		public IControllerSerializer GetSerializer(Type controllerType)
		{
			if (controllerType == null)
			{
				throw new ArgumentNullException(nameof(controllerType));
			}

			foreach (Mapping mapping in _serializerMappings)
			{
				if (mapping.ControllerType.IsAssignableFrom(controllerType))
				{
					return mapping.ImplementationFactory();
				}
			}

			throw new InvalidOperationException($"No serializer registered for controller type {controllerType.Name}.");
		}
	}
}
