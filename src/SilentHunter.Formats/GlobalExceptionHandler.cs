using System;
using System.IO;
using System.Threading.Tasks;

namespace SilentHunter
{
	internal static class GlobalExceptionHandler
	{
		public static void HandleException(Action action, string errorMessage)
		{
			try
			{
				action();
			}
			catch (Exception ex) when (ShouldWrapException(ex))
			{
				throw new SilentHunterParserException(errorMessage, ex);
			}
		}

		public static async Task HandleException(Func<Task> action, string errorMessage)
		{
			try
			{
				await action().ConfigureAwait(false);
			}
			catch (Exception ex) when (ShouldWrapException(ex))
			{
				throw new SilentHunterParserException(errorMessage, ex);
			}
		}

		private static bool ShouldWrapException(Exception ex)
		{
			return !(ex is SilentHunterParserException || ex is IOException);
		}
	}
}
