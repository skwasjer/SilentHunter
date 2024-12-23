using System;
using System.Linq;
using SilentHunter.Controllers;
using SilentHunter.FileFormats.Extensions;

namespace SilentHunter.FileFormats.Dat.Controllers;

internal class ControllerFactory : IControllerFactory, IItemFactory
{
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

    /// <inheritdoc />
    public bool CanCreate(Type controllerType)
    {
        return _controllerAssembly.Controllers
            .SelectMany(c => c.Value.Values)
            .Any(t => t == controllerType) && controllerType.IsController();
    }

    /// <inheritdoc />
    public Controller CreateController(string controllerName, ControllerProfile profile, bool initializeFields)
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

    /// <inheritdoc />
    public Controller CreateController(Type controllerType, bool initializeFields)
    {
        if (controllerType == null)
        {
            throw new ArgumentNullException(nameof(controllerType));
        }

        if (CanCreate(controllerType) && _controllerAssembly.Controllers.Any(profile => profile.Value.Any(ctrl => ctrl.Value == controllerType)))
        {
            return initializeFields
                ? (Controller)CreateNewItem(controllerType)
                : (Controller)Activator.CreateInstance(controllerType);
        }

        throw new ArgumentException("Unknown controller type.");
    }

    /// <inheritdoc />
    public object CreateNewItem(Type type)
    {
        return _itemFactory.CreateNewItem(type);
    }

    /// <inheritdoc />
    public object CreateNewItem(object original)
    {
        return _itemFactory.CreateNewItem(original);
    }
}
