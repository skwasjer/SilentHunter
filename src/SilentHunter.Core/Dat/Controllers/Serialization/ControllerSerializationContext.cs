using System;
using System.Reflection;

namespace SilentHunter.Dat.Controllers.Serialization
{
	public interface IControllerSerializationContext
	{
		MemberInfo Member { get; }
		Type Type { get; }
	}

	public class ControllerSerializationContext : IControllerSerializationContext
	{
		public ControllerSerializationContext(MemberInfo member, object value)
		{
			Member = member ?? throw new ArgumentNullException(nameof(member));
			var fieldInfo = Member as FieldInfo;
			Type = fieldInfo?.FieldType ?? (Type)Member;
			Name = fieldInfo?.Name ?? Type.FullName;
			Value = value;
		}

		public MemberInfo Member { get; }
		public Type Type { get; }
		public object Value { get; }
		public string Name { get; }
	}

	public class ControllerDeserializationContext : IControllerSerializationContext
	{
		public ControllerDeserializationContext(MemberInfo member)
		{
			Member = member ?? throw new ArgumentNullException(nameof(member));
			var fieldInfo = Member as FieldInfo;
			Type = fieldInfo?.FieldType ?? (Type)Member;
			Name = fieldInfo?.Name ?? Type.FullName;
		}

		public MemberInfo Member { get; }
		public Type Type { get; }
		public string Name { get; }
	}
}
