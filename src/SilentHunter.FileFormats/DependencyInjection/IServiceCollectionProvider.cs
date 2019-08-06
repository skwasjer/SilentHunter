using System.ComponentModel;
using Microsoft.Extensions.DependencyInjection;

namespace SilentHunter.FileFormats.DependencyInjection
{
	/// <summary>
	/// In order to keep the fluent configuration API clean, this interface is used to provide the <see cref="IServiceCollection"/> without exposing it to Intellisense.
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Never)]
	public interface IServiceCollectionProvider : IFluentInterface
	{
		/// <summary>
		/// </summary>
		[EditorBrowsable(EditorBrowsableState.Never)]
		IServiceCollection ServiceCollection { get; }
	}
}