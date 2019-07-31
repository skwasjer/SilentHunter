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
			// Member info can only be a Type or a FieldInfo.
			if (!(member is Type || member is FieldInfo))
			{
				throw new ArgumentException("Expected a Type or FieldInfo.", nameof(member));
			}

			Member = member;
			var fieldInfo = Member as FieldInfo;
			Type = fieldInfo?.FieldType ?? (Type)Member;
			Name = fieldInfo?.Name ?? Type.FullName;
			Value = value;
		}

		public MemberInfo Member { get; }
		public Type Type { get; }
		public string Name { get; }
		public object Value { get; }
	}

	public class ControllerDeserializationContext : IControllerSerializationContext
	{
		public ControllerDeserializationContext(MemberInfo member)
		{
			// Member info can only be a Type or a FieldInfo.
			if (!(member is Type || member is FieldInfo))
			{
				throw new ArgumentException("Expected a Type or FieldInfo.", nameof(member));
			}

			Member = member;
			var fieldInfo = Member as FieldInfo;
			Type = fieldInfo?.FieldType ?? (Type)Member;
			Name = fieldInfo?.Name ?? Type.FullName;
		}

		public MemberInfo Member { get; }
		public Type Type { get; }
		public string Name { get; }
	}
}
