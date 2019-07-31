using System;
using System.Reflection;

namespace SilentHunter.Dat.Controllers.Serialization
{
	public interface IControllerSerializationContext
	{
		IControllerSerializer Serializer { get; }
		MemberInfo Member { get; }
		Type Type { get; }
	}

	public class ControllerSerializationContext : IControllerSerializationContext
	{
		public ControllerSerializationContext(IControllerSerializer controllerSerializer, MemberInfo member, object value)
		{
			Serializer = controllerSerializer ?? throw new ArgumentNullException(nameof(controllerSerializer));
			Member = member ?? throw new ArgumentNullException(nameof(member));
			var fieldInfo = Member as FieldInfo;
			Type = fieldInfo?.FieldType ?? (Type)Member;
			Name = fieldInfo?.Name ?? Type.FullName;
			Value = value;
		}

		public IControllerSerializer Serializer { get; }
		public MemberInfo Member { get; }
		public Type Type { get; }
		public object Value { get; }
		public string Name { get; }
	}

	public class ControllerDeserializationContext : IControllerSerializationContext
	{
		public ControllerDeserializationContext(IControllerSerializer controllerSerializer, MemberInfo member)
		{
			Serializer = controllerSerializer ?? throw new ArgumentNullException(nameof(controllerSerializer));
			Member = member ?? throw new ArgumentNullException(nameof(member));
			var fieldInfo = Member as FieldInfo;
			Type = fieldInfo?.FieldType ?? (Type)Member;
			Name = fieldInfo?.Name ?? Type.FullName;
		}

		public IControllerSerializer Serializer { get; }
		public MemberInfo Member { get; }
		public Type Type { get; }
		public string Name { get; }
	}
}
