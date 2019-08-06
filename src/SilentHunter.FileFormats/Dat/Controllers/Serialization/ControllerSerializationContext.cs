using System;
using System.Reflection;
using SilentHunter.Controllers;

namespace SilentHunter.FileFormats.Dat.Controllers.Serialization
{
	/// <summary>
	/// Controller serialization context.
	/// </summary>
	public class ControllerSerializationContext
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ControllerSerializationContext"/> class.
		/// </summary>
		/// <param name="member">The member info.</param>
		/// <param name="controller">The controller.</param>
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

		/// <summary>
		/// Gets the controller which is being (de)serialized.
		/// </summary>
		public Controller Controller { get; }

		/// <summary>
		/// Gets the member or type which is being (de)serialized.
		/// </summary>
		public MemberInfo Member { get; }

		/// <summary>
		/// Gets the declaring type.
		/// </summary>
		public Type Type { get; }

		/// <summary>
		/// Gets the field or type name.
		/// </summary>
		public string Name { get; }
	}
}
