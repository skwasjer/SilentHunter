using System;

namespace SilentHunter.Controllers.Decoration
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
		/// <param name="subType"></param>
		public ControllerAttribute(ushort subType)
		{
			SubType = subType;
		}

		public ushort? SubType { get; }
	}
}