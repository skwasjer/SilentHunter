using System;

namespace SilentHunter.Dat
{
	[AttributeUsage(AttributeTargets.Field)]
	public class CountTypeAttribute : Attribute
	{
		public CountTypeAttribute(Type serializationType)
		{
			SerializationType = serializationType ?? throw new ArgumentNullException(nameof(serializationType));
		}

		/// <summary>
		/// Gets the type used when (de)serializing the number of items in the list.
		/// </summary>
		public Type SerializationType { get; }
	}
}