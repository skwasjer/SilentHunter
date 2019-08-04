using System;
using System.Reflection;
using SilentHunter.Controllers;

namespace SilentHunter.FileFormats.Dat.Controllers.Serialization
{
	public class ControllerSerializationContext
	{
		public ControllerSerializationContext(MemberInfo member, Controller controller)
		{
			// Member info can only be a Type or a FieldInfo.
			if (!(member is Type || member is FieldInfo))
			{
				throw new ArgumentException("Expected a Type or FieldInfo.", nameof(member));
			}

			Member = member;
			Controller = controller ?? throw new ArgumentNullException(nameof(controller));
			var fieldInfo = Member as FieldInfo;
			Type = fieldInfo?.FieldType ?? (Type)Member;
			Name = fieldInfo?.Name ?? Type.FullName;
		}

		public Controller Controller { get; }
		public MemberInfo Member { get; }
		public Type Type { get; }
		public string Name { get; }
	}
}
