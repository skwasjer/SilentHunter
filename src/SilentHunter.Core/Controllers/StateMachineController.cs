﻿using System.Collections.Generic;
using System.ComponentModel;

namespace SilentHunter.Controllers
{
	public class StateMachineController : RawController
	{
		private static class Magics
		{
			public const int Entry = 0x6cf46e74;
			public const int Condition = 0x596344fb;
			public const int Action = 0x7e03767b;
		}

		// NOTE: some unknown fields, but they always seem to be the same. So mark them advanced, so they don't show up in simple editor views.

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public int Unknown0 = 0x423B410F;

		/// <summary>
		/// The name that this StateMachineClass controller can be referenced by.
		/// </summary>
		public string GraphName;

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public int Unknown1 = 0x73A2500A;

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public int Unknown2 = 0x24CE7F70;

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public int Unknown3;

		/// <summary>
		/// A list of state entries that make up the state (behavior) of a character or object.
		/// </summary>
		public List<StateMachineEntry> StateEntries;
	}
}