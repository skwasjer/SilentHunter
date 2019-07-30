using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using skwas.IO;

namespace SilentHunter.Dat.Controllers.Serialization
{
	public class DefaultObjectSerializer : IControllerValueSerializer
	{
		private static readonly ConcurrentDictionary<Type, FieldInfo[]> FieldInfoCache = new ConcurrentDictionary<Type, FieldInfo[]>();

		public bool IsSupported(IControllerSerializationContext context)
		{
			return true;
		}

		public void Serialize(BinaryWriter writer, ControllerSerializationContext context)
		{
			if (context.Type.IsClass)
			{
				IEnumerable<FieldInfo> fields = GetCachedFieldInfos(context.Type);
				foreach (FieldInfo f in fields)
				{
					if (f.FieldType.IsClass)
					{
						// TODO: if field type is class, drill down, to support nesting.
					}

					writer.WriteStruct(f.GetValue(context.Value));
				}
			}
			else
			{
				writer.WriteStruct(context.Value);
			}
		}

		public object Deserialize(BinaryReader reader, ControllerDeserializationContext context)
		{
			if (!context.Type.IsClass)
			{
				return reader.ReadStruct(context.Type);
			}

			IEnumerable<FieldInfo> fields = GetCachedFieldInfos(context.Type);
			object instance = Activator.CreateInstance(context.Type);
			foreach (FieldInfo f in fields)
			{
				if (f.FieldType.IsClass)
				{
					// TODO: if field type is class, drill down, to support nesting.
				}

				f.SetValue(instance, reader.ReadStruct(f.FieldType));
			}

			return instance;

		}

		private static IEnumerable<FieldInfo> GetCachedFieldInfos(Type type)
		{
			return FieldInfoCache.GetOrAdd(
				type,
				t => t.GetFields(BindingFlags.Public | BindingFlags.Instance)
			);
		}
	}
}
