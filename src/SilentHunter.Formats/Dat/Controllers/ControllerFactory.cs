using System;
using System.Linq;
using System.Reflection;
using SilentHunter.Formats;

namespace SilentHunter.Dat.Controllers
{
	public class ControllerFactory : IControllerFactory, IItemFactory
	{
		private const BindingFlags ResolveBindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly | BindingFlags.Static;

		private readonly ControllerAssembly _controllerAssembly;
		private readonly IItemFactory _itemFactory;

		public ControllerFactory(ControllerAssembly controllerAssembly)
			: this(controllerAssembly, new ItemFactory())
		{
		}

		public ControllerFactory(ControllerAssembly controllerAssembly, IItemFactory itemFactory)
		{
			_controllerAssembly = controllerAssembly ?? throw new ArgumentNullException(nameof(controllerAssembly));
			_itemFactory = itemFactory ?? throw new ArgumentNullException(nameof(itemFactory));
		}

		public bool CanCreate(Type controllerType)
		{
			return _controllerAssembly.Controllers
				       .SelectMany(c => c.Value.Values)
				       .Any(t => t == controllerType) && controllerType.IsController();
		}

		/// <summary>
		/// Creates the specified controller name for the requested profile.
		/// </summary>
		/// <param name="controllerName"></param>
		/// <param name="profile">The profile to search for the controller.</param>
		/// <param name="initializeFields">True to initialize fields.</param>
		/// <returns>Returns the newly created controller. All (child) fields that are reference types are also pre-instantiated.</returns>
		/// <exception cref="ArgumentException">Thrown when the controller name is empty or cannot be found for the <paramref name="profile" />.</exception>
		public IRawController CreateController(string controllerName, ControllerProfile profile, bool initializeFields)
		{
			if (string.IsNullOrEmpty(controllerName))
			{
				throw new ArgumentNullException(nameof(controllerName));
			}

			if (_controllerAssembly.TryGetControllerType(controllerName, profile, out Type controllerType))
			{
				return CreateController(controllerType, initializeFields);
			}

			throw new ArgumentException("Unknown controller type.");
		}

		/// <summary>
		/// Creates the controller for specified <paramref name="controllerType" />.
		/// </summary>
		/// <param name="controllerType">The controller type.</param>
		/// <param name="initializeFields">True to initialize fields.</param>
		/// <returns>Returns the newly created controller. All (child) fields that are reference types are also instantiated using the default constructor.</returns>
		/// <exception cref="ArgumentException">Thrown when the controller name is empty or cannot be found for the <paramref name="profile" />.</exception>
		public IRawController CreateController(Type controllerType, bool initializeFields)
		{
			if (controllerType == null)
			{
				throw new ArgumentNullException(nameof(controllerType));
			}

			if (CanCreate(controllerType) && _controllerAssembly.Controllers.Any(profile => profile.Value.Any(ctrl => ctrl.Value == controllerType)))
			{
				return initializeFields ? (IRawController)CreateNewItem(controllerType) : (IRawController)Activator.CreateInstance(controllerType);
			}

			throw new ArgumentException("Unknown controller type.");
		}

		public object CreateNewItem(Type type)
		{
			return _itemFactory.CreateNewItem(type);
		}

		public object CreateNewItem(object original)
		{
			return _itemFactory.CreateNewItem(original);
		}
	}
}