using System;
using SilentHunter.Controllers;

namespace SilentHunter.FileFormats.Dat.Controllers;

/// <summary>
/// Creates a controller by name/profile and optionally initializes its fields.
/// </summary>
public interface IControllerFactory
{
    /// <summary>
    /// Creates the specified controller name for the requested profile.
    /// </summary>
    /// <param name="controllerName"></param>
    /// <param name="profile">The profile to search for the controller.</param>
    /// <param name="initializeFields">True to initialize fields.</param>
    /// <returns>Returns the newly created controller. All (child) fields that are reference types are also pre-instantiated.</returns>
    /// <exception cref="ArgumentException">Thrown when the controller name is empty or cannot be found for the <paramref name="profile" />.</exception>
    public Controller CreateController(string controllerName, ControllerProfile profile, bool initializeFields);

    /// <summary>
    /// Creates the controller for specified <paramref name="controllerType" />.
    /// </summary>
    /// <param name="controllerType">The controller type.</param>
    /// <param name="initializeFields">True to initialize fields.</param>
    /// <returns>Returns the newly created controller. All (child) fields that are reference types are also instantiated using the default constructor.</returns>
    /// <exception cref="ArgumentException">Thrown when the controller name is empty or cannot be found.</exception>
    public Controller CreateController(Type controllerType, bool initializeFields);

    /// <summary>
    /// Tests if the specified type is a controller type and can be created using this factory.
    /// </summary>
    /// <param name="controllerType">The controller type.</param>
    /// <returns><see langword="true" /> if <paramref name="controllerType" /> is a valid type that can be created, or <see langword="false" /> otherwise.</returns>
    public bool CanCreate(Type controllerType);
}
