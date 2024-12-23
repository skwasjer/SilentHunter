using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SilentHunter.Controllers;

namespace SilentHunter.FileFormats.Extensions;

internal static class TypeExtensions
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
    public static bool IsControllerOrObject(this Type type)
    {
        return type.IsClass && !typeof(IEnumerable).IsAssignableFrom(type);
    }

    /// <summary>
    /// Indicates if specified type is a controller of any type.
    /// </summary>
    /// <param name="type">The type to check.</param>
    public static bool IsController(this Type type)
    {
        return typeof(Controller).IsAssignableFrom(type);
    }

    /// <summary>
    /// Indicates if specified type is a behavior controller.
    /// </summary>
    /// <param name="type">The type to check.</param>
    public static bool IsBehaviorController(this Type type)
    {
        return typeof(BehaviorController).IsAssignableFrom(type);
    }

    /// <summary>
    /// Indicates if specified type is an animation controller.
    /// </summary>
    /// <param name="type">The type to check.</param>
    public static bool IsAnimationController(this Type type)
    {
        return typeof(AnimationController).IsAssignableFrom(type);
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
