using System;
using System.Collections.Generic;

namespace SilentHunter.Extensions
{
	internal static class Traverse
	{
		public static IEnumerable<T> Across<T>(T first, Func<T, T> next)
			where T : class
		{
			T item = first;
			while (item != null)
			{
				yield return item;

				item = next(item);
			}
		}
	}
}