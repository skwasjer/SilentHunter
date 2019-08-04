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
		/// Decorator for animation controllers, indicating the subType.
		/// </summary>
		/// <param name="subType"></param>
		public ControllerAttribute(ushort subType)
		{
			SubType = subType;
		}

		public ushort? SubType { get; }
	}
}