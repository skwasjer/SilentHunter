using System;

namespace SilentHunter.FileFormats.Dat.Controllers;

/// <summary>
/// Represents a factory to create Silent Hunter game controller objects.
/// </summary>
public interface IItemFactory
{
    /// <summary>
    /// Creates an new instance of the specified <paramref name="type" />. All (child) fields that are reference types are also instantiated using the default constructor.
    /// </summary>
    /// <param name="type">The type to create.</param>
    /// <returns>Returns the new instance.  All (child) fields that are reference types are also instantiated using the default constructor.</returns>
    object CreateNewItem(Type type);

    /// <summary>
    /// Creates an new instance of the same type as the object in <paramref name="original" />. All (child) fields that are reference types are also instantiated using the default constructor if the original object has this property set.
    /// </summary>
    /// <remarks>This method is similar to <see cref="CreateNewItem(Type)" /> but differs in that it checks the original object to see which fields are non-null. This is generally only used in arrays, and is there to ensure each array item is exactly the same.</remarks>
    /// <param name="original">The object to create a new type of.</param>
    /// <returns>Returns the new instance. All (child) fields that are reference types are also instantiated using the default constructor if the original object has this property set.</returns>
    object CreateNewItem(object original);
}