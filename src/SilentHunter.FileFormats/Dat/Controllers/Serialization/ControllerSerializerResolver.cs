using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace SilentHunter.FileFormats.Dat.Controllers.Serialization;

internal sealed class ControllerSerializerResolver
{
    [DebuggerDisplay("{ControllerType.Name,nq}, HasInstance = {_instance != null,nq}")]
    public sealed class Mapping
    {
        private IControllerSerializer _instance;
        private Func<IControllerSerializer> _implementationFactory;

        public Type ControllerType { get; set; }
        public Func<IControllerSerializer> ImplementationFactory
        {
            set => _implementationFactory = value;
        }

        public IControllerSerializer GetOrCreate()
        {
            return _instance ?? (_instance = _implementationFactory());
        }
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

        Mapping mapping = _serializerMappings.FirstOrDefault(m => m.ControllerType.IsAssignableFrom(controllerType));
        if (mapping != null)
        {
            return mapping.GetOrCreate();
        }

        throw new InvalidOperationException($"No serializer registered for controller type {controllerType.Name}.");
    }
}