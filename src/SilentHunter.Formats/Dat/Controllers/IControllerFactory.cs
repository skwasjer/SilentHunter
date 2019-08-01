using System;
using System.Collections.Generic;

namespace SilentHunter.Dat.Controllers
{
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
		RawController CreateController(string controllerName, ControllerProfile profile, bool initializeFields);

		/// <summary>
		/// Creates the controller for specified <paramref name="controllerType" />.
		/// </summary>
		/// <param name="controllerType">The controller type.</param>
		/// <param name="initializeFields">True to initialize fields.</param>
		/// <returns>Returns the newly created controller. All (child) fields that are reference types are also instantiated using the default constructor.</returns>
		/// <exception cref="ArgumentException">Thrown when the controller name is empty or cannot be found for the <paramref name="profile" />.</exception>
		RawController CreateController(Type controllerType, bool initializeFields);

		bool CanCreate(Type controllerType);
	}
}