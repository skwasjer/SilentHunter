using System;
using System.Linq;
using System.Reflection;
using SilentHunter.Dat;

namespace SilentHunter.Formats
{
	public static class ConvertExtensions
	{
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
			return typeof(IRawController).IsAssignableFrom(type);// memberInfo.HasAttribute<ControllerAttribute>();
		}

		/// <summary>
		/// Indicates if specified type is a raw controller only.
		/// </summary>
		/// <param name="type">The type to check.</param>
		public static bool IsRawController(this Type type)
		{
			return typeof(IRawController).IsAssignableFrom(type) && !typeof(IController).IsAssignableFrom(type);
		}

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
		/// Indicates if one or more instances of attribute <typeparamref name="T"/> is defined on this member.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="attributeProvider"></param>
		/// <param name="inherit"></param>
		/// <returns></returns>
		public static bool HasAttribute<T>(this ICustomAttributeProvider attributeProvider, bool inherit = false) where T : Attribute
		{
			return attributeProvider.IsDefined(typeof(T), inherit);
		}

		/// <summary>
		/// Return the first custom attribute of type T or null otherwise.
		/// </summary>
		/// <typeparam name="T">The attribute type.</typeparam>
		/// <param name="attributeProvider">The provider.</param>
		/// <param name="inherit">When true, look up the hierarchy chain for the inherited custom attribute.</param>
		/// <returns>Returns the first attribute if found or null otherwise.</returns>
		public static T GetAttribute<T>(this ICustomAttributeProvider attributeProvider, bool inherit = false) where T : Attribute
		{
			return attributeProvider.GetCustomAttributes(typeof(T), inherit).Cast<T>().FirstOrDefault();
		}
	}
}
