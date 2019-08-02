using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SilentHunter.Controllers;
using SilentHunter.Controllers.Decoration;

namespace SilentHunter.Extensions
{
	public static class TypeExtensions
	{
		public static bool IsClosedTypeOf(this Type @this, Type openGeneric)
		{
			return TypesAssignableFrom(@this).Any(t => t.GetTypeInfo().IsGenericType && !@this.GetTypeInfo().ContainsGenericParameters && t.GetGenericTypeDefinition() == openGeneric);
		}

		private static IEnumerable<Type> TypesAssignableFrom(Type candidateType)
		{
			return candidateType.GetTypeInfo()
				.ImplementedInterfaces.Concat(
					Traverse.Across(candidateType, t => t.GetTypeInfo().BaseType));
		}

		// ReSharper disable once InconsistentNaming
		public static bool IsControllerOrSHType(this Type type)
		{
			return type.IsController() || type.IsSHType();
		}

		/// <summary>
		/// Indicates if specified type is a controller or raw controller.
		/// </summary>
		/// <param name="type">The type to check.</param>
		public static bool IsController(this Type type)
		{
			return typeof(RawController).IsAssignableFrom(type); // memberInfo.HasAttribute<ControllerAttribute>();
		}

		/// <summary>
		/// Indicates if specified type is a raw controller only.
		/// </summary>
		/// <param name="type">The type to check.</param>
		public static bool IsRawController(this Type type)
		{
			return typeof(RawController).IsAssignableFrom(type) && !typeof(Controller).IsAssignableFrom(type);
		}

		// ReSharper disable once InconsistentNaming
		public static bool IsSHType(this Type type)
		{
			return type.HasAttribute<SHTypeAttribute>();
		}

		/// <summary>
		/// Returns true if specified type if a nullable type.
		/// </summary>
		/// <param name="type">The type to check.</param>
		public static bool IsNullable(this Type type)
		{
			return Nullable.GetUnderlyingType(type) != null;
		}

		/// <summary>
		/// Indicates if one or more instances of attribute <typeparamref name="T" /> is defined on this member.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="attributeProvider"></param>
		/// <param name="inherit"></param>
		/// <returns></returns>
		public static bool HasAttribute<T>(this ICustomAttributeProvider attributeProvider, bool inherit = false)
			where T : Attribute
		{
			return attributeProvider.IsDefined(typeof(T), inherit);
		}
	}
}