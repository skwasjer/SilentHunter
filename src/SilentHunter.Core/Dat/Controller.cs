using System;

namespace SilentHunter.Dat
{
	/// <summary>
	/// Represents controller data.
	/// </summary>
	public abstract class Controller : RawController, IController
	{
		public override Type ControllerSerializerType { get; } = typeof(ControllerSerializer);
	}
}