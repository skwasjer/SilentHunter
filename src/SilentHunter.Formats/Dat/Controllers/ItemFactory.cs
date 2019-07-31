using System;
using System.Collections.Generic;
using System.Reflection;
using SilentHunter.Extensions;

namespace SilentHunter.Dat.Controllers
{
	public class ItemFactory : IItemFactory
	{
		/// <summary>
		/// Creates an new instance of the specified <paramref name="type" />. All (child) fields that are reference types are also instantiated using the default constructor.
		/// </summary>
		/// <param name="type">The type to create.</param>
		/// <returns>Returns the new instance.  All (child) fields that are reference types are also instantiated using the default constructor.</returns>
		public object CreateNewItem(Type type)
		{
			if (type == typeof(string))
			{
				return "";
			}

			// Handle nullable.
			type = Nullable.GetUnderlyingType(type) ?? type;

			object newObject = Activator.CreateInstance(type);
			foreach (FieldInfo fi in type.GetFields(BindingFlags.Public | BindingFlags.Instance))
			{
				Type fieldType = fi.FieldType;
				if (fieldType.IsEnum)
				{
					fi.SetValue(newObject, Enum.GetValues(fieldType).GetValue(0));
					continue;
				}

				if (fieldType.Name.StartsWith("SHUnion"))
				{
				}
				else if (fieldType.IsGenericType)
				{
					// Lists are supported, other generics are not
					if (!fieldType.IsClosedTypeOf(typeof(IList<>)))
					{
						continue;
					}
				}
				else if (fieldType == typeof(DateTime))
				{
					fi.SetValue(newObject, new DateTime(1939, 1, 1));
					continue;
				}
				else if (!fieldType.IsClass)
				{
					continue;
				}

				fi.SetValue(newObject, CreateNewItem(fi.FieldType));
			}

			return newObject;
		}

		/// <summary>
		/// Creates an new instance of the same type as the object in <paramref name="original" />. All (child) fields that are reference types are also instantiated using the default constructor if the original object has this property set.
		/// </summary>
		/// <remarks>This method is similar to <see cref="M:CreateNewItem(type)" /> but differs in that it checks the original object to see which fields are non-null. This is generally only used in arrays, and is there to ensure each array item is exactly the same.</remarks>
		/// <param name="original">The object to create a new type of.</param>
		/// <returns>Returns the new instance. All (child) fields that are reference types are also instantiated using the default constructor if the original object has this property set.</returns>
		public object CreateNewItem(object original)
		{
			if (original == null)
			{
				return null;
			}

			Type t = original.GetType();

			if (t == typeof(string))
			{
				return "";
			}

			object newObject = t.Assembly.CreateInstance(t.FullName);
			foreach (FieldInfo fi in t.GetFields(BindingFlags.Public | BindingFlags.Instance))
			{
				Type fieldType = fi.FieldType;
				if (fieldType.IsGenericType)
				{
					Type[] typeArgs = fieldType.GetGenericArguments();
					if (typeArgs.Length != 1)
					{
						if (fieldType.Name.StartsWith("SHUnion"))
						{
							object union = fi.GetValue(original);
							object newUnion = ((ICloneable)union).Clone();
							foreach (PropertyInfo unionFi in fieldType.GetProperties())
							{
								if (unionFi.Name == "Type")
								{
									var unionType = (Type)unionFi.GetValue(union, null);
									unionFi.SetValue(newUnion, unionType, null);
									fi.SetValue(newObject, newUnion);
									//break;
								}
								else if (unionFi.Name == "Value")
								{
									// Reset value to default.
									Type objectType = unionFi.GetValue(union, null).GetType();
									unionFi.SetValue(newUnion, objectType.Assembly.CreateInstance(objectType.FullName), null);
								}
							}
						}

						continue;
					}
				}
				else if (fieldType == typeof(DateTime))
				{
				}
				else if (!fieldType.IsClass && !fieldType.IsEnum)
				{
					continue;
				}

				object orgFieldValue = fi.GetValue(original);
				if (orgFieldValue != null)
				{
					if (fieldType == typeof(DateTime))
					{
						fi.SetValue(newObject, new DateTime(1939, 1, 1));
					}
					else if (fieldType.IsEnum)
					{
						fi.SetValue(newObject, Enum.GetValues(fieldType).GetValue(0));
					}
					else
					{
						fi.SetValue(newObject, CreateNewItem(orgFieldValue));
					}
				}
			}

			return newObject;
		}
	}
}