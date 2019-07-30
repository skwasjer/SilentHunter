using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using SilentHunter.Extensions;
using SilentHunter.Formats;
using skwas.IO;

namespace SilentHunter.Dat.Controllers.Serialization
{
	public class SHUnionValueSerializer : IControllerValueSerializer
	{
		private class SHUnionPropertyCache
		{
			public PropertyInfo TypeProperty { get; set; }
			public PropertyInfo ValueProperty { get; set; }
		}

		private static readonly IDictionary<Type, SHUnionPropertyCache> PropertyCache = new ConcurrentDictionary<Type, SHUnionPropertyCache>();

		public bool IsSupported(IControllerSerializationContext context)
		{
			return context.Type.IsClosedTypeOf(typeof(SHUnion<,>));
		}

		public void Serialize(BinaryWriter writer, ControllerSerializationContext context)
		{
			SHUnionPropertyCache propertyCache = GetCachedProperties(context.Type);
			writer.WriteStruct(propertyCache.ValueProperty.GetValue(context.Value, null));
		}

		public object Deserialize(BinaryReader reader, ControllerDeserializationContext context)
		{
			long dataSize = reader.BaseStream.Length - reader.BaseStream.Position;

			SHUnionPropertyCache propertyCache = GetCachedProperties(context.Type);

			Type[] typeArgs = context.Type.GetGenericArguments();
			// Using size of data to determine which of the SHUnion generic type arguments to use.
			Type valueType = typeArgs.FirstOrDefault(type => Marshal.SizeOf(type) == dataSize);
			if (valueType == null)
			{
				throw new IOException($"The available stream data does not match the size one of the two union types for property '{context.Name}'.");
			}

			object union = Activator.CreateInstance(context.Type);
			propertyCache.TypeProperty.SetValue(union, valueType, null);
			propertyCache.ValueProperty.SetValue(union, reader.ReadStruct(valueType), null);
			return union;

		}

		private static SHUnionPropertyCache GetCachedProperties(Type type)
		{
			if (PropertyCache.ContainsKey(type))
			{
				return PropertyCache[type];
			}

			var x = new SHUnionPropertyCache
			{
				TypeProperty = type.GetProperty("Type"),
				ValueProperty = type.GetProperty("Value")
			};
			PropertyCache.Add(type, x);
			return x;
		}
	}
}
