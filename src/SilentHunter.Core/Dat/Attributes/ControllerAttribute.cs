using System;

namespace SilentHunter.Dat
{
	[AttributeUsage(AttributeTargets.Class)]
	public sealed class ControllerAttribute : Attribute
	{
		public ControllerAttribute()
		{
		}

		/// <summary>
		/// Indicates a normal controller, or a raw controller. In case of a raw controller, the subType and countField indicate what the first 4 bytes are composed off. In case of a normal controller, the extra fields are not used.
		/// </summary>
		/// <param name="raw"></param>
		/// <param name="subType"></param>
		/// <param name="hasCountField"></param>
		public ControllerAttribute(ushort subType, bool hasCountField)
		{
			SubType = subType;
			HasCountField = hasCountField;
		}

		public ushort? SubType { get; }

		public bool HasCountField { get; }
	}
}