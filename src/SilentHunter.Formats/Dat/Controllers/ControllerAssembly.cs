using System;
using System.Collections.Generic;
using System.IO;
using SilentHunter.Formats;

namespace SilentHunter.Dat.Controllers
{
	public static class ControllerAssembly
	{
		public static IItemFactory ItemFactory { get; set; }
		public static IControllerFactory ControllerFactory { get; set; }

		public static IReadOnlyDictionary<ControllerProfile, Dictionary<string, Type>> Controllers => ((ControllerFactory)ControllerFactory).Controllers;

		public static ControllerAssemblyHelpText HelpText { get; set; }

		public static IControllerReader Reader => new ControllerReader(ControllerFactory);

		public static IControllerWriter Writer => new ControllerWriter(ControllerFactory);

	}
}
