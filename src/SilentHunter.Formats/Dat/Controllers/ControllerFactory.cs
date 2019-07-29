using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using SilentHunter.Formats;

namespace SilentHunter.Dat.Controllers
{
	public class ControllerFactory : IControllerFactory, IItemFactory
	{
		private const BindingFlags ResolveBindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly | BindingFlags.Static;

		private readonly IItemFactory _itemFactory;
		private readonly Assembly _controllerAssembly;

		public ControllerFactory(Assembly controllerAssembly)
			: this(controllerAssembly, new ItemFactory())
		{
		}

		public ControllerFactory(Assembly controllerAssembly, IItemFactory itemFactory)
		{
			_itemFactory = itemFactory ?? throw new ArgumentNullException(nameof(itemFactory));
			_controllerAssembly = controllerAssembly ?? throw new ArgumentNullException(nameof(controllerAssembly));
			Controllers = new ReadOnlyDictionary<ControllerProfile, Dictionary<string, Type>>(
				EnumControllers(_controllerAssembly)
			);
		}

		public IReadOnlyDictionary<ControllerProfile, Dictionary<string, Type>> Controllers { get; }

		public bool CanCreate(Type controllerType)
		{
			return Controllers
				.SelectMany(c => c.Value.Values)
				.Any(t => t == controllerType) && controllerType.IsController();
		}

		public bool GetControllerType(string controllerName, ControllerProfile profile, out Type controllerType)
		{
			if (Controllers.TryGetValue(profile, out Dictionary<string, Type> ctrls) && ctrls.TryGetValue(controllerName, out controllerType))
			{
				return true;
			}

			controllerType = null;
			return false;
		}

		/// <summary>
		/// Creates the specified controller name for the requested profile.
		/// </summary>
		/// <param name="controllerName"></param>
		/// <param name="profile">The profile to search for the controller.</param>
		/// <returns>Returns the newly created controller. All (child) fields that are reference types are also pre-instantiated.</returns>
		/// <exception cref="ArgumentException">Thrown when the controller name is empty or cannot be found for the <paramref name="profile" />.</exception>
		public IRawController CreateController(string controllerName, ControllerProfile profile)
		{
			if (string.IsNullOrEmpty(controllerName))
			{
				throw new ArgumentNullException(nameof(controllerName));
			}

			if (GetControllerType(controllerName, profile, out Type controllerType))
			{
				return CreateController(controllerType);
			}

			throw new ArgumentException("Unknown controller type.");
		}

		/// <summary>
		/// Creates the controller for specified <paramref name="type" />.
		/// </summary>
		/// <param name="type">The controller type.</param>
		/// <returns>Returns the newly created controller. All (child) fields that are reference types are also instantiated using the default constructor.</returns>
		/// <exception cref="ArgumentException">Thrown when the controller name is empty or cannot be found for the <paramref name="profile" />.</exception>
		public IRawController CreateController(Type type)
		{
			if (type == null)
			{
				throw new ArgumentNullException(nameof(type));
			}

			if (typeof(IRawController).IsAssignableFrom(type))
			{
				
				if (Controllers.Any(profile => profile.Value.Any(ctrl => ctrl.Value == type)))
				{
					return (IRawController)CreateNewItem(type);
				}
			}

			throw new ArgumentException("Unknown controller type.");
		}

		public object CreateNewItem(Type type) => _itemFactory.CreateNewItem(type);

		public object CreateNewItem(object original) => _itemFactory.CreateNewItem(original);

		/// <summary>
		/// Loads controllers from the remote assembly. On order to qualify, the type must have ControllerAttribute applied.
		/// </summary>
		/// <param name="controllerAssembly"></param>
		private static IDictionary<ControllerProfile, Dictionary<string, Type>> EnumControllers(Assembly controllerAssembly)
		{
			var controllerTypes = new Dictionary<ControllerProfile, Dictionary<string, Type>>();
			foreach (ControllerProfile cp in Enum.GetValues(typeof(ControllerProfile)))
			{
				controllerTypes.Add(cp, new Dictionary<string, Type>());
			}

			void NotSupportedMember(IList list, Type type, string memberName)
			{
				if (list.Count > 0)
				{
					throw new NotSupportedException($"The type '{type.Name}' defines one or more {memberName} which is not supported");
				}
			}

			foreach (Type remoteType in controllerAssembly.GetTypes())
			{
				// No interfaces allowed.
				if (remoteType.IsInterface)
				{
					throw new NotSupportedException($"The type '{remoteType.Name}' is an interface which is not supported.");
				}

				// No properties allowed.
				NotSupportedMember(remoteType.GetProperties(ResolveBindingFlags), remoteType, "properties");

				if (remoteType.IsValueType && !remoteType.IsDefined(typeof(SerializableAttribute), false))
				{
					throw new NotSupportedException(
						$"The type '{remoteType.Name}' is a value type, which requires a SerializableAttribute."
					);
				}

				// Check if type is a controller.
				if (!remoteType.IsController())
				{
					continue;
				}

				string controllerProfileStr = remoteType.FullName?.Substring(0, 3) ?? string.Empty;
				if (Enum.TryParse(controllerProfileStr, out ControllerProfile profile))
				{
					// Last wins.
					AddOrUpdate(controllerTypes, profile, remoteType);
				}
				else
				{
					// If not explicit, add to all.
					controllerTypes[ControllerProfile.SH3].Add(remoteType.Name, remoteType);
					controllerTypes[ControllerProfile.SH4].Add(remoteType.Name, remoteType);
					controllerTypes[ControllerProfile.SH5].Add(remoteType.Name, remoteType);
				}
			}

			return controllerTypes;
		}

		private static void AddOrUpdate(IDictionary<ControllerProfile, Dictionary<string, Type>> controllerTypes, ControllerProfile profile, Type remoteType)
		{
			if (controllerTypes[profile].ContainsKey(remoteType.Name))
			{
				controllerTypes[profile][remoteType.Name] = remoteType;
			}
			else
			{
				controllerTypes[profile].Add(remoteType.Name, remoteType);
			}
		}
	}
}